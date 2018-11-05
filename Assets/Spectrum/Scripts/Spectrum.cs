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

		public static class MsgTypes
		{
			public static readonly short AddSpawnerToList = 5000;
			public static readonly short PortOfGameServerToOpen = 5001;
			public static readonly short PortOfGameServerThatDisconnected = 5002;
			public static readonly short AddGameServerToList = 5005;
			public static readonly short SendGameServerIPToClient = 5010;
			public static readonly short IPAndPortOfGameServerForClient = 5011;
			public static readonly short IncrementPlayerCountOfServer = 5020;
			public static readonly short DecrementPlayerCountOfServer = 5021;
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
}