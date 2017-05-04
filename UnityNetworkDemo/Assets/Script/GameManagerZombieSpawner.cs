using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManagerZombieSpawner : NetworkBehaviour {

	[SerializeField]
	GameObject zombiePrefab;
	[SerializeField]
	GameObject zombieSpawn;

	private int counter;
	private int numberOfZombie = 10;

	public override void OnStartServer(){
		for (int i = 0; i < numberOfZombie; i++) {
			SpawnZombies ();
		}
	}

	void SpawnZombies(){
		// counter

		GameObject go = GameObject.Instantiate (zombiePrefab, zombieSpawn.transform.position, Quaternion.identity)as GameObject;
		NetworkServer.Spawn (go);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
