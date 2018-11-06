namespace Spectrum
{
	public class GameServerLens : LensBehaviour
	{
		public GameNetworkManager GameNetMan;

		private void Start()
		{
			GameNetMan.networkAddress = "10.1.10.65";
			GameNetMan.serverBindAddress = "10.1.10.65";
			GameNetMan.serverBindToIP = true;
			GameNetMan.networkPort = 7790;
			GameNetMan.StartServer();
		}
	}
}
