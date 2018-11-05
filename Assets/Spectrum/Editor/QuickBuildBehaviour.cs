using UnityEditor;

namespace Spectrum
{
	public class SpectrumQuickBuild
	{
		public static string SpectrumRoot = "Assets/Spectrum";

		public static BuildTarget TargetPlatform = BuildTarget.StandaloneWindows64;

		public static BuildOptions BuildOptions = BuildOptions.Development;

		public static string PrevPath = null;

		[MenuItem("Tools/Spectrum/Build All", false, 0)]
		public static void BuildGame()
		{
			var path = GetPath();
			if (string.IsNullOrEmpty(path))
				return;

			BuildMaster(path);
			BuildSpawner(path);
			BuildClient(path);
			BuildGameServer(path);
		}

		/// <summary>
		/// Creates a build for master server and spawner
		/// </summary>
		/// <param name="path"></param>
		public static void BuildMaster(string path)
		{
			var masterScenes = new[]
			{
				SpectrumRoot+ "/Scenes/MasterServer.unity"
			};

			BuildPipeline.BuildPlayer(masterScenes, path + "/Master/MasterServer.exe", TargetPlatform, BuildOptions);
		}

		public static void BuildSpawner(string path)
		{
			var masterScenes = new[]
			{
				SpectrumRoot+ "/Scenes/SpawnerServer.unity"
			};
			BuildPipeline.BuildPlayer(masterScenes, path + "/Spawner/SpawnerServer.exe", TargetPlatform, BuildOptions);
		}

		/// <summary>
		/// Creates a build for client
		/// </summary>
		/// <param name="path"></param>
		public static void BuildClient(string path)
		{
			var clientScenes = new[]
			{
				SpectrumRoot+"/Scenes/GameClient.unity",
				"Assets/FPSExampleGame/Scenes/Game.unity"
			};
			BuildPipeline.BuildPlayer(clientScenes, path + "/Client/Client.exe", TargetPlatform, BuildOptions);
		}

		/// <summary>
		/// Creates a build for game server
		/// </summary>
		/// <param name="path"></param>
		public static void BuildGameServer(string path)
		{
			var gameServerScenes = new[]
			{
				SpectrumRoot+"/Scenes/GameServer.unity",
				"Assets/FPSExampleGame/Scenes/Game.unity"
			};
			BuildPipeline.BuildPlayer(gameServerScenes, path + "/GameServer/GameServer.exe", TargetPlatform, BuildOptions);
		}

		#region Editor Menu

		[MenuItem("Tools/Spectrum/Build Master", false, 11)]
		public static void BuildMasterMenu()
		{
			var path = GetPath();
			if (!string.IsNullOrEmpty(path))
			{
				BuildMaster(path);
			}
		}

		[MenuItem("Tools/Spectrum/Build Spawner", false, 11)]
		public static void BuildSpawnerMenu()
		{
			var path = GetPath();
			if (!string.IsNullOrEmpty(path))
			{
				BuildSpawner(path);
			}
		}

		[MenuItem("Tools/Spectrum/Build Client", false, 11)]
		public static void BuildClientMenu()
		{
			var path = GetPath();
			if (!string.IsNullOrEmpty(path))
			{
				BuildClient(path);
			}
		}

		[MenuItem("Tools/Spectrum/Build Game Server", false, 11)]
		public static void BuildGameServerMenu()
		{
			var path = GetPath();
			if (!string.IsNullOrEmpty(path))
			{
				BuildGameServer(path);
			}
		}

		#endregion

		public static string GetPath()
		{
			var prevPath = EditorPrefs.GetString("spectrum.buildPath", "");
			string path = EditorUtility.SaveFolderPanel("Choose Location for binaries", prevPath, "");

			if (!string.IsNullOrEmpty(path))
			{
				EditorPrefs.SetString("spectrum.buildPath", path);
			}
			return path;
		}
	}
}