﻿using Mirror;
using UnityEngine;

namespace Spectrum
{
	class MasterServerLens : LensBehaviour
	{
		private void Start()
		{
			StartServer();
		}

		public override void RegisterServerHandlers()
		{
			base.RegisterServerHandlers();
			RegisterHandler(Spectrum.MsgTypes.SendGameServerIPToClient, SendGameServerIPToClient);
		}

		private void SendGameServerIPToClient(NetworkMessage netMsg)
		{
			var d = new StringMessage("10.1.10.65" + ")" + "7790");
			ServerSendMsg(netMsg.conn.connectionId, Spectrum.MsgTypes.IPAndPortOfGameServerForClient, d);
		}
	}
}