using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerNetworkEntity : NetworkBehaviour {

	[SerializeField]
	private List<Behaviour> ComponentsToDisableOnRemote;

	// Use this for initialization
	void Start () {
		if (!isLocalPlayer)
		{
			foreach (var item in ComponentsToDisableOnRemote)
			{
				item.enabled = false;
			}
		}
	}
	
}
