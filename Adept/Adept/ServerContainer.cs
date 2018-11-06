using System.Collections.Generic;
using System.Diagnostics;

namespace Adept
{
	internal class ServerContainer
	{
		int s_ServerHostId = 0;

		static System.Type s_NetworkConnectionClass = typeof(NetworkConnection);

		static Dictionary<short, NetworkMessageDelegate> s_MessageHandlers = new Dictionary<short, NetworkMessageDelegate>();

		static Dictionary<int, NetworkConnection> s_Connections = new Dictionary<int, NetworkConnection>();

		public void HandleDisconnect(int connectionId, int v)
		{
			Debug.WriteLine("Server disconnect client:" + connectionId);

			NetworkConnection conn;
			if (s_Connections.TryGetValue(connectionId, out conn))
			{
				conn.Disconnect();
				RemoveConnection(connectionId);
				Debug.WriteLine("Server lost client:" + connectionId);

				OnDisconnected(conn);
			}
		}

		private static bool RemoveConnection(int connectionId)
		{
			return s_Connections.Remove(connectionId);
		}

		private void OnDisconnected(NetworkConnection conn)
		{
			conn.InvokeHandlerNoData((short)MsgType.Disconnect);

			Debug.WriteLine("Server lost client:" + conn.connectionId);
			conn.Dispose();
		}

		public void HandleData(int connectionId, byte[] data, int v)
		{
			NetworkConnection conn;
			if (s_Connections.TryGetValue(connectionId, out conn))
			{
				OnData(conn, data);
			}
			else
			{
				Debug.WriteLine("HandleData Unknown connectionId:" + connectionId);
			}
		}

		static void OnData(NetworkConnection conn, byte[] data)
		{
			conn.TransportReceive(data);
		}

		public void HandleConnect(int connectionId, byte error)
		{
			System.Diagnostics.Debug.WriteLine("Server accepted client:" + connectionId);

			if (error != 0)
			{
				GenerateConnectError(error);
				return;
			}

			// get ip address from connection
			Transport.layer.GetConnectionInfo(connectionId, out string address);

			// add player info
			NetworkConnection conn = (NetworkConnection)System.Activator.CreateInstance(s_NetworkConnectionClass);
			conn.Initialize(address, s_ServerHostId, connectionId);
			AddConnection(conn);
			OnConnected(conn);
		}

		private void OnConnected(NetworkConnection conn)
		{
			System.Diagnostics.Debug.WriteLine("Server accepted client:" + conn.connectionId);
			//conn.InvokeHandlerNoData((short)MsgType.Connect);
			SetClientReadyInternal(conn);
		}

		private static bool AddConnection(NetworkConnection conn)
		{
			if (!s_Connections.ContainsKey(conn.connectionId))
			{
				// connection cannot be null here or conn.connectionId
				// would throw NRE
				s_Connections[conn.connectionId] = conn;
				conn.SetHandlers(s_MessageHandlers);
				return true;
			}
			// already a connection with this id
			return false;
		}

		private void GenerateConnectError(byte error)
		{
			Debug.WriteLine("connected error" + error);
		}

		internal static void SetClientReadyInternal(NetworkConnection conn)
		{
			Debug.WriteLine("SetClientReadyInternal for conn:" + conn.connectionId);

			if (conn.isReady)
			{
				Debug.WriteLine("SetClientReady conn " + conn.connectionId + " already ready");
				return;
			}

			conn.isReady = true;
		}

		public static void RegisterHandler(short msgType, NetworkMessageDelegate handler)
		{
			if (s_MessageHandlers.ContainsKey(msgType))
			{
				Debug.WriteLine("NetworkServer.RegisterHandler replacing " + msgType);
			}
			s_MessageHandlers[msgType] = handler;
		}

		static public void RegisterHandler(MsgType msgType, NetworkMessageDelegate handler)
		{
			RegisterHandler((short)msgType, handler);
		}
	}
}