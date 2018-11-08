using System.Collections.Generic;
using UnityEngine;

namespace Spectrum
{
	public class Spectrum : MonoBehaviour
	{

		public static SpectrumArgs Args;

		public static string MasterServerIP;
		public static int MasterServerPort;
		public static string MachineIP;

		public static List<string> SpawnerServerIPs;
		public static SpectrumLogLevel LogLevel = SpectrumLogLevel.None;

		public static readonly string SpectrumConnectionInfoFileName = "SpectrumConnectionInfo.json";

		public static readonly string AuthCode = "CHANGEMEPLEASE";


		static Spectrum()
		{
			Args = new SpectrumArgs();
			MasterServerIP = Args.MasterIp;
			MasterServerPort = Args.MasterPort;
			MachineIP = Args.MachineIp;
		}

		internal static void LogError(string Info)
		{
			var c = System.DateTime.Now;
			Debug.LogError("Spectrum Info - " + c.ToShortDateString() + "@" + c.ToShortTimeString() + ": " + Info);
		}

		public enum MsgTypes : short
		{
			AddSpawnerToList = 5000,
			 PortOfGameServerToOpen = 5001,
		PortOfGameServerThatDisconnected = 5002,
			AddGameServerToList = 5005,
			 SendGameServerIPToClient = 5010,
			 IPAndPortOfGameServerForClient = 5011,
			 ClientWaitForSpawnedServer = 5012,
			IncrementPlayerCountOfServer = 5020,
			DecrementPlayerCountOfServer = 5021,
			AuthCode = 10000
		}

		public enum SpectrumLogLevel
		{
			Information,
			Warning,
			None
		}

		public static void LogInformation(string Info)
		{
			if(LogLevel == SpectrumLogLevel.Information)
			{
				var c = System.DateTime.Now;
				Debug.Log("Spectrum Info - " + c.ToShortDateString() + "@" + c.ToShortTimeString() + ": " + Info);
			}
		}


	}

	[System.Serializable]
	public class SpectrumConnectionInfo
	{
		public string MasterServerIP;
		public int MasterServerPort;
	}
}