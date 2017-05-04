using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ZombieAttack : NetworkBehaviour {

	// 次の攻撃までの間隔（秒）
	private float attackRate = 3;
	// 次に攻撃する時間
	private float nextAttack;
	// ダメージ量
	private int damage = 10;
	// 攻撃の射程距離
	private float minDistance = 2;
	// 現在のゾンビとPlayerとの距離
	private float currentDistance;
	// 現在位置
	private Transform myTransform;
	// ターゲットの位置
	private ZombieTarget targetScript;

	// 通常時の色
	[SerializeField]
	private Material zombieGreen;
	// 攻撃時の色
	[SerializeField]
	private Material zombieRed;

	// Use this for initialization
	void Start () {
		// 現在位置とZombieTargetスクリプトをキャッシュ
		myTransform = transform;
		targetScript = GetComponent<ZombieTarget> ();
		// 自分がサーバーだったらコルーチン実行
		if(isServer){
			StartCoroutine (Attack ());
		}
	}

	void CheckIfTargetInRange(){
		// ターゲットが決まっていたら
		if(targetScript.targetTransform != null){
			// ターゲットお自分の距離を計算
			currentDistance = Vector3.Distance(targetScript.targetTransform.position,myTransform.position);
			// ターゲットが射程距離内かつゲーム開始からの経過時間がnextAttackより大きかった時
			if(currentDistance < minDistance && Time.time > nextAttack){
				// nextAttackに3秒足す
				nextAttack = Time.time + attackRate;

				// Playerにダメージを与える
				targetScript.targetTransform.GetComponent<PlayerHealth>().DeductHealth(damage);
				// ホストサーバー向け
				StartCoroutine(ChangeZombieMat());
				// クライアントサーバー向け
				RpcChangeZombieAppearance();
			}
		}
	}

	IEnumerator ChangeZombieMat(){
		// 攻撃を実行したら1.5秒間赤くなり、緑に戻る
		GetComponent<Renderer> ().material = zombieRed;
		yield return new WaitForSeconds (attackRate / 2);
		GetComponent<Renderer> ().material = zombieGreen;
	}

	[ClientRpc]
	void RpcChangeZombieAppearance(){
		// クライアントサーバー向け
		StartCoroutine(ChangeZombieMat());
	}

	IEnumerator Attack(){
		// 無限ループ
		for(;;){
			yield return new WaitForSeconds (0.2f);
			CheckIfTargetInRange ();
		}
	}
}