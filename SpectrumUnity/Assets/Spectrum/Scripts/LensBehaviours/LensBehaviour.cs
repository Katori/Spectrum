using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Telepathy;
using Mirror;

namespace Spectrum.Lens
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

		Server server = new Server();
		Client client = new Client();

		public Dictionary<short, LensMessageDelegate> ServerMessageHandlers;

		Dictionary<int, LensConnection> s_Connections = new Dictionary<int, LensConnection>();

		int s_ServerHostId = 0;

		static System.Type s_NetworkConnectionClass = typeof(LensConnection);

		private LensConnection ClientConnection;
		private LensConnection ServerConnection;

		public void StartClient()
		{
			StartCommon();
			Spectrum.LogInformation("Connecting to Spectrum Master Server: " + ConnectionAddress + ":" + ConnectionPort);
			ClientConnection = new LensConnection();
			ClientConnection.Initialize(ConnectionAddress, 0, 0);
			RegisterClientHandlers();
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
			ServerMessageHandlers = new Dictionary<short, LensMessageDelegate>();
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
							HandleConnect(msg.connectionId, 0);
							break;
						case Telepathy.EventType.Data:
							if (s_Connections.TryGetValue(msg.connectionId, out LensConnection conn))
							{
								conn.HandleBytes(msg.data);
							}
							else
							{
								Spectrum.LogError("HandleData Unknown connectionId:" + msg.connectionId);
							}
							break;
						case Telepathy.EventType.Disconnected:
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
							ClientSendMsg((short)Spectrum.MsgTypes.AuthCode, new SpectrumAuthCode() { AuthCode = Spectrum.AuthCode });
							OnClientConnected();
							break;
						case Telepathy.EventType.Data:
							//Debug.Log("Data: " + BitConverter.ToString(msg.data));
							ClientConnection.HandleBytes(msg.data);
							break;
						case Telepathy.EventType.Disconnected:
							OnDisconnectedInternal(msg.connectionId);
							break;
					}
				}
			}
			
		}

		public virtual void OnClientConnected()
		{
			
		}

		private void OnDisconnectedInternal(int connectionId)
		{
			if (s_Connections.TryGetValue(connectionId, out LensConnection conn))
			{
				conn.Disconnect();
				RemoveConnection(connectionId);
				Spectrum.LogInformation("Server lost client:" + connectionId);

				OnDisconnected(conn);
			}
		}

		private bool RemoveConnection(int connectionId)
		{
			return s_Connections.Remove(connectionId);
		}

		public virtual void OnDisconnected(LensConnection conn)
		{
			
		}

		public virtual void OnConnected(LensConnection conn)
		{

		}

		private bool AddConnection(LensConnection conn)
		{
			if (!s_Connections.ContainsKey(conn.connectionId))
			{
				// connection cannot be null here or conn.connectionId
				// would throw NRE
				s_Connections[conn.connectionId] = conn;
				conn.SetHandlers(ServerMessageHandlers);
				return true;
			}
			// already a connection with this id
			return false;
		}

		private void HandleConnect(int connectionId, byte error)
		{
			Spectrum.LogInformation("Server accepted client:" + connectionId);

			if (error != 0)
			{
				Debug.Log("error: " + error);
				return;
			}

			// get ip address from connection
			server.GetConnectionInfo(connectionId, out string address);
			//Transport.layer.GetConnectionInfo(connectionId, out string address);

			// add player info
			LensConnection conn = (LensConnection)System.Activator.CreateInstance(s_NetworkConnectionClass);
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

		public void RegisterServerHandler(short msgType, LensMessageDelegate handler)
		{
			if (ServerMessageHandlers.ContainsKey(msgType))
			{
				//if (LogFilter.Debug) { Debug.Log("NetworkConnection.RegisterHandler replacing " + msgType); }
				Debug.Log("NetworkConnection.RegisterHandler replacing " + msgType);
			}
			ServerMessageHandlers[msgType] = handler;
		}

		public void RegisterClientHandler(short msgType, LensMessageDelegate handler)
		{
			if (ClientConnection.ClientMessageHandlers.ContainsKey(msgType))
			{
				Spectrum.LogInformation("Replacing client handler: " + msgType);
			}
			ClientConnection.ClientMessageHandlers[msgType] = handler;
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
