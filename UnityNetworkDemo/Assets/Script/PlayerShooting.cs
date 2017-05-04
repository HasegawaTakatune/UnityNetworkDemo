using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShooting : NetworkBehaviour {

	// ダメージ量
	private int damage = 25;

	// Raycastの距離
	private float range = 200;

	// FirstPersonCharacterを指定
	[SerializeField]
	private Transform cameraTransform;
	private RaycastHit hit;



	// Update is called once per frame
	void Update () {
		CheckIfShooting ();
	}

	void CheckIfShooting(){
		if (!isLocalPlayer) {
			return;
		}
		if (Input.GetMouseButtonDown(0)) {
			Shooting ();
		}
	}

	void Shooting(){
		// カメラの前方にRaycastを飛ばす
		// TransformPoint: 指定した分だけ座標をずらす
		if (Physics.Raycast (cameraTransform.TransformPoint (0, 0, 0.5f), cameraTransform.forward, out hit, range)) {
			
			// RaycastがPlayerと衝突した時
			if (hit.transform.tag == "Player") {
				// 名前を取得
				string uniqueIdentity = hit.transform.name;
				// 名前とダメージ量を引数にメソッド実行
				CmdTellServerWhoWasShot (uniqueIdentity, damage);
			} 
			// Zombieと衝突した時
			else if (hit.transform.tag == "Zombie") {
				// 名前を取得する
				string uniqueIdentity = hit.transform.name;
				// 名前とダメージ量を引数にメソッド呼び出し
				CmdTellServerWhickZombieWasShot (uniqueIdentity, damage);
			}

		}
	}

	// Command: SyncVar変数の変更結果を、全クライアントへ送信
	[Command]
	void CmdTellServerWhoWasShot(string uniqueID,int dmg){
		// 敵プレイヤーの名前でGameObjectを取得
		GameObject go = GameObject.Find (uniqueID);
		// PlayerHealth->DeductHealthメソッド呼び出し
		go.GetComponent<PlayerHealth> ().DeductHealth (dmg);
	}

	[Command]
	void CmdTellServerWhickZombieWasShot(string uniqueID,int dmg){
		// IDのオブジェクトを取得する
		GameObject go = GameObject.Find (uniqueID);
		// オブジェクトおHPをダメージ量分減らす
		go.GetComponent<ZombieHealth> ().DeductHealth (dmg);
	}
}
