using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

namespace Spectrum
{
	public class GameNetworkManager : NetworkManager {

		//public static GameNetworkManager Instance { get; private set; }

		public bool UseSpawnCamera;
		public string SpawnCameraName = "SpawnCamera";

		//public bool AutoStartServer = false;

		//public GameServerConnectionToMasterBehaviour Connect;

		//public void Start()
		//{
		//	if (Instance == null)
		//	{
		//		Instance = this;
		//	}
		//	else
		//	{
		//		Destroy(gameObject);
		//	}
		//	if(AutoStartServer || Spectrum.Args.StartGameServer)
		//	{
		//		Connect = GameServerConnectionToMasterBehaviour.Instance as GameServerConnectionToMasterBehaviour;
		//	}
		//}

		//public void OpenGameServer()
		//{
		//	if (Connect.AutoStartServer)
		//	{
		//		Spectrum.LogInformation("Using Auto Server (Inspector Configured)");
		//		networkAddress = Connect.AutoMasterIP;
		//		serverBindAddress = Connect.AutoMasterIP;
		//	}
		//	else
		//	{
		//		networkAddress = Spectrum.MachineIP;
		//		serverBindAddress = Spectrum.MachineIP;
		//	}
			
		//	if (Spectrum.Args.StartGameServer)
		//	{
		//		networkPort = Spectrum.Args.AssignedPort;
		//	}
		//	else
		//	{
		//		networkPort = 7778;
		//	}
		//	serverBindToIP = true;
		//	StartServer();
		//}

		private void GameSceneLoaded(Scene arg0, LoadSceneMode arg1)
		{
			if (UseSpawnCamera)
			{
				GameObject.Find(SpawnCameraName).SetActive(false);
			}
			SceneManager.sceneLoaded -= GameSceneLoaded;
		}

		//public override void OnStartServer()
		//{
		//	base.OnStartServer();
		//	Connect.GameServerAvailable();
		//}

		//public override void OnServerConnect(NetworkConnection conn)
		//{
		//	base.OnServerConnect(conn);
		//	Connect.ClientConnectedToGameServer();
		//}

		//public override void OnServerDisconnect(NetworkConnection conn)
		//{
		//	base.OnServerDisconnect(conn);
		//	Connect.ClientDisconnectedFromGameServer();
		//}
	}
}