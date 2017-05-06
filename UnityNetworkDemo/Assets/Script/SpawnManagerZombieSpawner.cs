using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnManagerZombieSpawner : NetworkBehaviour {

	[SerializeField]
	GameObject zombiePrefab;

	private GameObject[] zombieSpawns;

	private int counter;
	private int numberOfZombies = 10;
	// ゾンビの最大数
	private int maxNumberOfZombies = 80;
	// ゾンビが出現する間隔（秒）
	private float waveRate = 10;
	// 出現フラグ
	private bool isSpawnActivated = true;


	public override void OnStartServer(){
		// ゾンビ発生地点を取得
		zombieSpawns = GameObject.FindGameObjectsWithTag("ZombieSpawn");
		// コルーチン
		StartCoroutine(ZombieSpawner());
	}

	IEnumerator ZombieSpawner(){
		// 無限ループ
		for(;;){
			yield return new WaitForSeconds (waveRate);
			// ゲーム上の全ゾンビ取得
			GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");
			// ゾンビの数が最大値より少なかったらメソッド実行
			if(zombies.Length < maxNumberOfZombies){
				CommenceSpawn ();
			}
		}
	}

	void CommenceSpawn(){
		if (isSpawnActivated) {
			for (int i = 0; i < numberOfZombies; i++) {
				// 0～3のランダムで選んだ発生地点の位置情報を引数にメソッド実行
				int randomIndex = Random.Range(0,zombieSpawns.Length);
				SpawnZombies (zombieSpawns [randomIndex].transform.position);
			}
		}
	}

	void SpawnZombies(Vector3 spawnPos){
		counter++;

		GameObject go = GameObject.Instantiate (zombiePrefab, spawnPos, Quaternion.identity)as GameObject;
		//NetworkServer.Spawn (go);
		// ZombieにIDを付けていく
		go.GetComponent<ZombieID>().zombieID="Zombie" + counter;
		NetworkServer.Spawn (go);
	}
}
