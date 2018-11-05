using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpectrumFPSExampleGame.Sources
{
	public class UIController : MonoBehaviour
	{
		[SerializeField]
		private GameObject ConnectionPanel;
		
		[SerializeField]
		private InputField IPInput;

		private void Start()
		{
			NetworkController.Instance.EventConnectedOrStarted.AddListener(HideConnectionUI);
		}

		public void StartServer()
		{
			NetworkController.Instance.StartServerProcess(IPInput.text);
		}

		public void ConnectToServer()
		{
			NetworkController.Instance.StartClientProcess(IPInput.text);
		}

		private void HideConnectionUI()
		{
			ConnectionPanel.SetActive(false);
		}
	}
}