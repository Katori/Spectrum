using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace SpectrumFPSExampleGame.Sources
{
	public class NetworkController : NetworkManager
	{
		internal static NetworkController Instance { get; private set; }


		[SerializeField]
		private GameObject SpawnCamera;

		internal UnityEvent EventConnectedOrStarted = new UnityEvent();

		// Use this for initialization
		void Start()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
				Destroy(gameObject);
			}
		}

		public override void OnStartServer()
		{
			base.OnStartServer();
			EventConnectedOrStarted.Invoke();
		}

		public override void OnClientConnect(NetworkConnection conn)
		{
			base.OnClientConnect(conn);
			//ClientScene.Ready(conn);
			//ClientScene.AddPlayer();
			EventConnectedOrStarted.Invoke();
		}

		public override void OnServerAddPlayer(NetworkConnection conn)
		{
			var c = GetStartPosition();
			GameObject Player = Instantiate(playerPrefab, c.position, c.rotation);
			NetworkServer.AddPlayerForConnection(conn, Player);
			//GameManager.Instance.AddPlayerToListOfPlayers(conn.connectionId, Player);
		}

		public override void OnStartHost()
		{
			base.OnStartHost();
			EventConnectedOrStarted.Invoke();
			SpawnCamera.SetActive(false);
		}

		public override void OnStartClient(NetworkClient client)
		{
			base.OnStartClient(client);
			SpawnCamera.SetActive(false);
		}

		public void StartServerProcess(string NetworkAddress)
		{
			Debug.LogError("This message will make the console appear in Development Builds");
			networkAddress = NetworkAddress;
			serverBindAddress = NetworkAddress;
			serverBindToIP = true;
			StartServer();
			Debug.Log(networkAddress);
		}

		public void StartClientProcess(string NetworkAddress)
		{
			networkAddress = NetworkAddress;
			StartClient(7777);
		}
	}
}