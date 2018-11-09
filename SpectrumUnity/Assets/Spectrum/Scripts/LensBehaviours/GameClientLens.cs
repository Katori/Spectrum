﻿using UnityEngine;
using Mirror;

namespace Spectrum.Lens
{
	class GameClientLens : LensBehaviour
	{

		public GameNetworkManager GameNetMan;

		private void Start()
		{
			Spectrum.LogLevel = Spectrum.SpectrumLogLevel.Information;
			Telepathy.Logger.LogMethod = Debug.Log;
			Telepathy.Logger.LogErrorMethod = Debug.Log;
			Telepathy.Logger.LogWarningMethod = Debug.Log;
			StartClient();
		}

		public override void RegisterClientHandlers()
		{
			base.RegisterClientHandlers();
			RegisterHandler((short)Spectrum.MsgTypes.IPAndPortOfGameServerForClient, ReceivedServerToConnect);
			RegisterHandler((short)MsgType.SpawnFinished, ReceivedEmptySpawnMessage);
		}

		private void ReceivedEmptySpawnMessage(LensMessage netMsg)
		{
			// don't need to do anything here
		}

		public override void OnClientConnected()
		{
			base.OnClientConnected();
			Spectrum.LogInformation("Connected to master received by GameClientLens");
			ClientSendMsg((short)Spectrum.MsgTypes.SendGameServerIPToClient, new EmptyMessage());
		}

		private void ReceivedServerToConnect(LensMessage netMsg)
		{
			var c = netMsg.ReadMessage<StringMessage>();
			Spectrum.LogInformation("Received server info: " + c);
			var d = c.value.Split(')');
			var address = d[0];
			var port = int.Parse(d[1]);
			GameNetMan.networkAddress = address;
			GameNetMan.networkPort = port;
			GameNetMan.StartClient();
		}
	}
}
