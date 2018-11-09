﻿using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Spectrum.Lens
{
	public delegate void LensMessageDelegate(LensMessage netMsg);
	public class LensConnection
	{

		Dictionary<short, LensMessageDelegate> m_MessageHandlers;

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
		}

		public void Disconnect()
		{
			isReady = false;
		}

		public override string ToString()
		{
			return string.Format("hostId: {0} connectionId: {1} isReady: {2}", hostId, connectionId, isReady);
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