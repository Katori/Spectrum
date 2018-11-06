using Mirror;
using UnityEngine;

namespace Spectrum
{
	public class GameServerLens : LensBehaviour
	{
		public static GameServerLens Instance { get; private set; }

		public GameNetworkManager GameNetMan;

		public bool AutoStartServer = false;
		public string AutoStartIP;
		public int AutoStartPort;

		private void Start()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
				Destroy(gameObject);
			}
			Spectrum.LogLevel = Spectrum.SpectrumLogLevel.Information;
			Telepathy.Logger.LogMethod = Debug.Log;
			Telepathy.Logger.LogErrorMethod = Debug.Log;
			Telepathy.Logger.LogWarningMethod = Debug.Log;
			StartClient();

		}

		public override void OnConnected()
		{
			base.OnConnected();
			if (AutoStartServer)
			{
				GameNetMan.OpenGameServer(AutoStartIP, AutoStartPort);
			}
			else if (Spectrum.Args.StartGameServer)
			{
				GameNetMan.OpenGameServer(Spectrum.Args.MachineIp, Spectrum.Args.AssignedPort);
			}
		}

		public void GameServerAvailable()
		{
			var c = new IntegerMessage(GameNetMan.networkPort);
			ClientSendMsg(Spectrum.MsgTypes.AddGameServerToList, c);
		}

		public void ClientConnectedToGameServer()
		{
			var c = new EmptyMessage();
			ClientSendMsg(Spectrum.MsgTypes.IncrementPlayerCountOfServer, c);
		}

		public void ClientDisconnectedFromGameServer()
		{
			var c = new EmptyMessage();
			ClientSendMsg(Spectrum.MsgTypes.DecrementPlayerCountOfServer, c);
		}
	}
}
