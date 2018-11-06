using System.Collections.Generic;
using UnityEngine;

namespace Spectrum
{
	public class HeadlessServerHelperComponent : MonoBehaviour
	{
		private static HeadlessServerHelperComponent Instance;

		[SerializeField]
		private List<GameObject> GameObjectsToDestroyOnHeadless;

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
				Destroy(gameObject);
			}
			if (Spectrum.Args.StartGameServer)
			{
				SetupForHeadless();
			}
			if (GameServerLens.Instance != null)
			{
				SetupForHeadless();
			}
		}

		public static void SetupForHeadless()
		{
			Renderer[] renderers = FindObjectsOfType<Renderer>();
			SkinnedMeshRenderer[] skinnedMeshes = FindObjectsOfType<SkinnedMeshRenderer>();
			foreach (var item in renderers)
			{
				item.enabled = false;
			}
			foreach (var item in skinnedMeshes)
			{
				item.enabled = false;
			}
			foreach (var item in Instance.GameObjectsToDestroyOnHeadless)
			{
				Destroy(item);
			}
		}
	}
}