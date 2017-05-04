using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ZombieID : NetworkBehaviour {

	[SyncVar]
	public string zombieID;
	private Transform myTransform;

	// Use this for initialization
	void Start () {
		myTransform = transform;
	}
	
	// Update is called once per frame
	void Update () {
		SetIdentity ();
	}

	void SetIdentity(){
		if (myTransform.name == "" || myTransform.name == "Zombie(Clone)") {
			// zombieIDは、GameManagerZombieSpawnerクラスに設定されている
			myTransform.name = zombieID;
		}
	}
}
