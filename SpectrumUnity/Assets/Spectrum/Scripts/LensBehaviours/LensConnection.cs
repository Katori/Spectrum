using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace Spectrum.Lens
{
	public delegate void LensMessageDelegate(LensMessage netMsg);
	public class LensConnection
	{

		public Dictionary<short, LensMessageDelegate> ClientMessageHandlers;

		public int hostId = -1;
		public int connectionId = -1;
		public bool isReady;
		public string address;
		public float lastMessageTime;

		public bool logNetworkMessages;
		public bool isConnected { get { return hostId != -1; } }

		public virtual void Initialize(string networkAddress, int networkHostId, int networkConnectionId)
		{
			address = networkAddress;
			hostId = networkHostId;
			connectionId = networkConnectionId;
			ClientMessageHandlers = new Dictionary<short, LensMessageDelegate>();
		}

		public void Disconnect()
		{
			isReady = false;
		}

		public override string ToString()
		{
			return string.Format("hostId: {0} connectionId: {1} isReady: {2}", hostId, connectionId, isReady);
		}

		public void HandleBytes(byte[] buffer)
		{
			// unpack message
			ushort msgType;
			byte[] content;
			if (Protocol.UnpackMessage(buffer, out msgType, out content))
			{
				if (logNetworkMessages) { Spectrum.LogInformation("ConnectionRecv con:" + connectionId + " msgType:" + msgType + " content:" + System.BitConverter.ToString(content)); }

				LensMessageDelegate msgDelegate;
				if (ClientMessageHandlers.TryGetValue((short)msgType, out msgDelegate))
				{
					// create message here instead of caching it. so we can add it to queue more easily.
					LensMessage msg = new LensMessage();
					msg.msgType = (short)msgType;
					msg.reader = new NetworkReader(content);
					msg.conn = this;

					msgDelegate(msg);
					lastMessageTime = Time.time;
				}
				else
				{
					//NOTE: this throws away the rest of the buffer. Need moar error codes
					Spectrum.LogError("Unknown message ID " + msgType + " connId:" + connectionId);
				}
			}
			else
			{
				Spectrum.LogError("HandleBytes UnpackMessage failed for: " + System.BitConverter.ToString(buffer));
			}
		}

		internal void SetHandlers(Dictionary<short, LensMessageDelegate> serverMessageHandlers)
		{
			ClientMessageHandlers = serverMessageHandlers;
		}
	}

	public class LensMessage
	{
		public short msgType;
		public LensConnection conn;
		public NetworkReader reader;

		public TMsg ReadMessage<TMsg>() where TMsg : MessageBase, new()
		{
			var msg = new TMsg();
			msg.Deserialize(reader);
			return msg;
		}

		public void ReadMessage<TMsg>(TMsg msg) where TMsg : MessageBase
		{
			msg.Deserialize(reader);
		}
	}
}
