using Mirror;
using UnityEngine;

namespace Spectrum.Lens
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

		public override void OnClientConnected()
		{
			base.OnClientConnected();
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
			ClientSendMsg((short)Spectrum.MsgTypes.AddGameServerToList, c);
		}

		public void ClientConnectedToGameServer()
		{
			var c = new EmptyMessage();
			ClientSendMsg((short)Spectrum.MsgTypes.IncrementPlayerCountOfServer, c);
		}

		public void ClientDisconnectedFromGameServer()
		{
			var c = new EmptyMessage();
			ClientSendMsg((short)Spectrum.MsgTypes.DecrementPlayerCountOfServer, c);
		}
	}
}
