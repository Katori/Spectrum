using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spectrum.Adept
{
    public class Spectrum
    {
		public enum MsgTypes : short
		{
			AddSpawnerToList = 5000,
			PortOfGameServerToOpen = 5001,
			PortOfGameServerThatDisconnected = 5002,
			AddGameServerToList = 5005,
			SendGameServerIPToClient = 5010,
			IPAndPortOfGameServerForClient = 5011,
			IncrementPlayerCountOfServer = 5020,
			DecrementPlayerCountOfServer = 5021,
			AuthCode = 10000
		}
	}
}
