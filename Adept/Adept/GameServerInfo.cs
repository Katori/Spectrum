namespace Adept
{
	[System.Serializable]
	public class GameServerInfo
	{
		public string NetworkAddress;
		public int Port;
		public int JoinedPlayers = 0;
		public readonly int Capacity = 4;
	}
}