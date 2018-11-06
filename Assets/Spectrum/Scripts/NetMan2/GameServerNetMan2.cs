namespace Spectrum
{
	public class GameServerNetMan2 : NetMan2
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
