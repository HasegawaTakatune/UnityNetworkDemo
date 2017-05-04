using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ZombieTarget : NetworkBehaviour {

	private UnityEngine.AI.NavMeshAgent agent;
	private Transform myTransform;
	public Transform targetTransform;
	// ゾンビが探知するレイヤー
	private LayerMask raycastLayer;
	// ゾンビがPlayerを探知する半径
	private float radius = 100;

	// Use this for initialization
	void Start () {
		agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		myTransform = transform;
		raycastLayer = 1 << LayerMask.NameToLayer ("Player");

		// コルーチンの実行
		if(isServer){
			StartCoroutine ("DoCheck");
		}
	}
		
	void SearchForTarget(){
		// サーバーじゃなければメソッド終了
		if(!isServer){
			return;
		}

		// Playerをまだ取得していない時
		if(targetTransform == null){
			// Physics.OverlapSphere: ある地点を中心に球を作り、衝突したオブジェクトを取得する
			// 第一引数: 中心点  第二引数: 半径  第三引数: 対象のレイヤー
			Collider[] hitColliders = Physics.OverlapSphere(myTransform.position,radius,raycastLayer);
			if (hitColliders.Length > 0) {
				int randomInt = Random.Range (0, hitColliders.Length);
				targetTransform = hitColliders [randomInt].transform;
			}
		}
		// Playerは取得しているがBoxColliderが非アクティブの時 = isDeadがTrueの時
		if (targetTransform != null && targetTransform.GetComponent<BoxCollider> ().enabled == false) {
			targetTransform = null;
		}
	}

	void MoveForTarget(){
		// Playerオブジェクト取得済みで、自分がサーバーの時
		if (targetTransform != null && isServer) {
			SetNavDestination (targetTransform);
		}
	}

	void SetNavDestination(Transform dest){
		// ゾンビAIの目的地設定
		agent.SetDestination(dest.position);
	}

	IEnumerator DoCheck(){
		// 無限ループ
		for (;;) {
			SearchForTarget ();
			MoveForTarget ();
			yield return new WaitForSeconds (0.2f);
		}
	}

}
