using System.Diagnostics;
using Mirror;

namespace Spectrum
{
	public class SpawnerLens : LensBehaviour
	{
		public bool AutoStartSpawner;

		private void Start()
		{
			Spectrum.LogLevel = Spectrum.SpectrumLogLevel.Information;
			if(Spectrum.Args.StartSpawner)
			{
				ConnectionAddress = Spectrum.Args.MasterIp;
				ConnectionPort = Spectrum.Args.MasterPort;
				StartClient();
			}
			else if (AutoStartSpawner)
			{
				StartClient();
			}
		}

		public override void RegisterClientHandlers()
		{
			base.RegisterClientHandlers();
			RegisterHandler((short)Spectrum.MsgTypes.PortOfGameServerToOpen, StartNewServer);
		}

		private void StartNewServer(NetworkMessage netMsg)
		{
			Spectrum.LogInformation("Starting new game server");
			var d = new ProcessStartInfo(System.Environment.CurrentDirectory+"StartGameServer.bat")
			{
				CreateNoWindow = true
			};
			try
			{
				Process.Start(d);
			}
			catch (System.Exception e)
			{
				Spectrum.LogError("Exception occurred while spawning Game Server process: " + e.Message + " " + e.StackTrace);
			}
		}
	}
}
