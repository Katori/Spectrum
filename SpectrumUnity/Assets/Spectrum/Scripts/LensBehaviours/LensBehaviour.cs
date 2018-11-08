using System.Collections.Generic;
using UnityEngine;
using Telepathy;
using Mirror;
using UnityEngine.Serialization;
using System;

namespace Spectrum
{
	/// <summary>
	/// Lens is a lightweight alternative to NetworkManager, designed primarily for Client connections between the GameServer or GameClient and Master Server,
	/// but extensible and also offering a lightweight server component. Lens and NetworkManager can be used simultaneously--that's the whole point.
	/// </summary>
	public class LensBehaviour : MonoBehaviour
	{
		[FormerlySerializedAs("NetworkAddress")]
		public string ConnectionAddress;
		[FormerlySerializedAs("Port")]
		public int ConnectionPort;

		public bool IsServer;

		public static readonly string AuthCode = "CHANGEMEPLEASE";

		Server server = new Server();
		Client client = new Client();

		Dictionary<short, NetworkMessageDelegate> m_MessageHandlers;

		static Dictionary<short, NetworkMessageDelegate> s_MessageHandlers = new Dictionary<short, NetworkMessageDelegate>();

		public System.Action<Message> Connected { get; internal set; }
		public System.Action<Message> Disconnected { get; internal set; }

		public delegate void OnConnect(string msg);
		public delegate void OnDisconnect(string msg);

		Dictionary<int, NetworkConnection> s_Connections = new Dictionary<int, NetworkConnection>();

		int s_ServerHostId = 0;

		static Type s_NetworkConnectionClass = typeof(NetworkConnection);

		public void StartClient()
		{
			StartCommon();
			RegisterClientHandlers();
			Spectrum.LogInformation("Connecting to Spectrum Master Server: " + ConnectionAddress + ":" + ConnectionPort);
			client.Connect(ConnectionAddress, ConnectionPort);
		}

		public void StopClient()
		{
			Spectrum.LogInformation("Disconnecting from Spectrum Master Server");
			client.Disconnect();
		}

		public void StartServer()
		{
			StartCommon();
			RegisterServerHandlers();
			server.Start(ConnectionPort);
		}

		public virtual void RegisterServerHandlers()
		{

		}

		public virtual void RegisterClientHandlers()
		{

		}

		public void StopServer()
		{
			server.Stop();
		}

		public void StartCommon()
		{
			Application.runInBackground = true;
			Telepathy.Logger.LogMethod = Debug.Log;
			Telepathy.Logger.LogWarningMethod = Debug.LogWarning;
			Telepathy.Logger.LogErrorMethod = Debug.LogError;
			m_MessageHandlers = new Dictionary<short, NetworkMessageDelegate>();
			//Transport.layer = new TelepathyWebsocketsMultiplexTransport();
		}

		private void Update()
		{
			HandleNewMessages();
		}

		public void HandleNewMessages()
		{
			Message msg;
			if (IsServer)
			{
				while (server.GetNextMessage(out msg))
				{
					switch (msg.eventType)
					{
						case Telepathy.EventType.Connected:
							Connected(msg);
							HandleConnect(msg.connectionId, 0);
							break;
						case Telepathy.EventType.Data:
							HandleBytes(msg.data);
							break;
						case Telepathy.EventType.Disconnected:
							Disconnected(msg);
							OnDisconnectedInternal(msg.connectionId);
							break;
						default:
							break;
					}
				}
			}
			else
			{
				if (!client.Connected)
				{
					return;
				}
				while (client.GetNextMessage(out msg))
				{
					switch (msg.eventType)
					{
						case Telepathy.EventType.Connected:
							Spectrum.LogInformation("Connected to Spectrum Master Server");
							ClientSendMsg((short)Spectrum.MsgTypes.AuthCode, new SpectrumAuthCode() { AuthCode = AuthCode });
							HandleConnect(msg.connectionId, 0);
							break;
						case Telepathy.EventType.Data:
							//Debug.Log("Data: " + BitConverter.ToString(msg.data));
							HandleBytes(msg.data);
							break;
						case Telepathy.EventType.Disconnected:
							Debug.Log("Disconnected");
							break;
					}
				}
			}
			
		}

		private void OnDisconnectedInternal(int connectionId)
		{
			NetworkConnection conn;
			if (s_Connections.TryGetValue(connectionId, out conn))
			{
				conn.Disconnect();
				RemoveConnection(connectionId);
				if (LogFilter.Debug) { Debug.Log("Server lost client:" + connectionId); }

				OnDisconnected(conn);
			}
		}

		private bool RemoveConnection(int connectionId)
		{
			return s_Connections.Remove(connectionId);
		}

		public virtual void OnDisconnected(NetworkConnection conn)
		{
			conn.Dispose();
		}

		public virtual void OnConnected(NetworkConnection conn)
		{

		}

		private bool AddConnection(NetworkConnection conn)
		{
			if (!s_Connections.ContainsKey(conn.connectionId))
			{
				// connection cannot be null here or conn.connectionId
				// would throw NRE
				s_Connections[conn.connectionId] = conn;
				return true;
			}
			// already a connection with this id
			return false;
		}




		private void HandleConnect(int connectionId, byte error)
		{
			if (LogFilter.Debug) { Debug.Log("Server accepted client:" + connectionId); }

			if (error != 0)
			{
				Debug.Log("error: " + error);
				return;
			}

			// get ip address from connection
			string address;
			Transport.layer.GetConnectionInfo(connectionId, out address);

			// add player info
			NetworkConnection conn = (NetworkConnection)Activator.CreateInstance(s_NetworkConnectionClass);
			conn.Initialize(address, s_ServerHostId, connectionId);
			AddConnection(conn);
			OnConnected(conn);
		}

		public bool ServerSendMsg(int connectionId, short msgType, MessageBase msg)
		{
			NetworkWriter writer = new NetworkWriter();
			msg.Serialize(writer);

			// pack message and send
			byte[] message = Protocol.PackMessage((ushort)msgType, writer.ToArray());
			return SendBytes(connectionId, message);
		}

		public bool ClientSendMsg(short msgType, MessageBase msg)
		{
			NetworkWriter writer = new NetworkWriter();
			msg.Serialize(writer);

			// pack message and send
			byte[] message = Protocol.PackMessage((ushort)msgType, writer.ToArray());
			return SendBytes(0, message);
		}

		// protected because no one except NetworkConnection should ever send bytes directly to the client, as they
		// would be detected as some kind of message. send messages instead.
		protected virtual bool SendBytes(int connectionId, byte[] bytes)
		{

			if (bytes.Length > Transport.MaxPacketSize)
			{
				Debug.LogError("NetworkConnection:SendBytes cannot send packet larger than " + Transport.MaxPacketSize + " bytes");
				return false;
			}

			if (bytes.Length == 0)
			{
				// zero length packets getting into the packet queues are bad.
				Debug.LogError("NetworkConnection:SendBytes cannot send zero bytes");
				return false;
			}

			if (IsServer)
			{
				return server.Send(connectionId, bytes);
			}
			else
			{
				return client.Send(bytes);
			}
		}

		// handle this message
		// note: original HLAPI HandleBytes function handled >1 message in a while loop, but this wasn't necessary
		//       anymore because NetworkServer/NetworkClient.Update both use while loops to handle >1 data events per
		//       frame already.
		//       -> in other words, we always receive 1 message per Receive call, never two.
		//       -> can be tested easily with a 1000ms send delay and then logging amount received in while loops here
		//          and in NetworkServer/Client Update. HandleBytes already takes exactly one.
		protected void HandleBytes(byte[] buffer)
		{
			// unpack message
			if (Protocol.UnpackMessage(buffer, out ushort msgType, out byte[] content))
			{

				if (m_MessageHandlers.TryGetValue((short)msgType, out NetworkMessageDelegate msgDelegate))
				{
					// create message here instead of caching it. so we can add it to queue more easily.
					NetworkMessage msg = new NetworkMessage
					{
						msgType = (short)msgType,
						reader = new NetworkReader(content)
					};
					//msg.conn = this;

					msgDelegate(msg);
					//lastMessageTime = Time.time;
				}
				else
				{
					//NOTE: this throws away the rest of the buffer. Need moar error codes
					Debug.LogError("Unknown message ID " + msgType);// + " connId:" + connectionId);
				}
			}
			else
			{
				Debug.LogError("HandleBytes UnpackMessage failed for: " + System.BitConverter.ToString(buffer));
			}
		}

		public void RegisterHandler(short msgType, NetworkMessageDelegate handler)
		{
			if (m_MessageHandlers.ContainsKey(msgType))
			{
				//if (LogFilter.Debug) { Debug.Log("NetworkConnection.RegisterHandler replacing " + msgType); }
				Debug.Log("NetworkConnection.RegisterHandler replacing " + msgType);
			}
			m_MessageHandlers[msgType] = handler;
		}

		public string GetConnectionInfo(int connectionId)
		{
			server.GetConnectionInfo(connectionId, out string address);
			return address;
		}

		private void OnApplicationQuit()
		{
			if (IsServer)
			{
				if (server.Active)
				{
					server.Stop();
				}
			}
			else
			{
				if (client.Connected)
				{
					client.Disconnect();
				}
			}
		}
	}
}
