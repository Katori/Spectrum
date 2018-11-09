using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using Spectrum.Lens;

namespace Spectrum
{
	public class GameNetworkManager : NetworkManager {

		public bool UseSpawnCamera;
		public string SpawnCameraName = "SpawnCamera";

		public void OpenGameServer(string NetAddress, int Port)
		{
			networkAddress = NetAddress;
			serverBindAddress = NetAddress;
			networkPort = Port;
			serverBindToIP = true;
			StartServer();
		}

		private void GameSceneLoaded(Scene arg0, LoadSceneMode arg1)
		{
			if (UseSpawnCamera)
			{
				GameObject.Find(SpawnCameraName).SetActive(false);
			}
			SceneManager.sceneLoaded -= GameSceneLoaded;
		}

		public override void OnStartServer()
		{
			base.OnStartServer();
			GameServerLens.Instance.GameServerAvailable();
		}

		public override void OnServerConnect(NetworkConnection conn)
		{
			base.OnServerConnect(conn);
			GameServerLens.Instance.ClientConnectedToGameServer();
		}

		public override void OnServerDisconnect(NetworkConnection conn)
		{
			base.OnServerDisconnect(conn);
			GameServerLens.Instance.ClientDisconnectedFromGameServer();
		}
	}
}