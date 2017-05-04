using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerDeath : NetworkBehaviour {

	// PlayerHealthスクリプトの変数
	private PlayerHealth healthScript;
	// 照準のImage
	private Image crossHairImage;

	// Use this for initialization
	void Start () {
		// キャッシュしておく
		crossHairImage = GameObject.Find("Crosshair Image").GetComponent<Image>();
		healthScript = GetComponent<PlayerHealth> ();
		// Eventを登録
		healthScript.EventDie += DisablePlayer;
	}

	// メモリーリークした時用の、安全のためのメソッド
	// OnDisable: 消滅する時に呼ばれる
	void OnDisable(){
		// EventからDisablePlayerメソッドを削除
		healthScript.EventDie -= DisablePlayer;
	}

	// Eventで登録されるメソッド CheckConditionメソッド内で使われる
	// 各コンポーネントを非アクティブ状態にする
	void DisablePlayer(){
		GetComponent<CharacterController> ().enabled = false;
		GetComponent<PlayerShooting> ().enabled = false;
		GetComponent<BoxCollider> ().enabled = false;

		// 子オブジェクトのRendererを全て格納
		Renderer[] renderers = GetComponentsInChildren<Renderer>();
		// 格納したRenderer全てを非アクティブ化
		foreach(Renderer ren in renderers){
			ren.enabled = false;
		}
		// isDeadをtrueにすることで、CheckConditionのif文内に入らないようにする
		healthScript.isDead=true;

		if (isLocalPlayer) {
			GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ().enabled = false;
			crossHairImage.enabled = false;
			// RespawnButtonをアクティブ化
			GameObject.Find("GameManager").GetComponent<GameManagerReferences>().respawnButton.SetActive(true);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
