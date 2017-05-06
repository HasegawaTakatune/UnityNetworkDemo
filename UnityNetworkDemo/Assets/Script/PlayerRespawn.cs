using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerRespawn : NetworkBehaviour {

	// PlayerHealthスクリプト
	private PlayerHealth healthScript;
	// 照準画面
	private Image crossHairImage;
	// Respawnボタン
	private GameObject respawnButton;

	// Use this for initialization
	public override void PreStartClient () {
		// PlayerHealthスクリプトおキャッシュ
		healthScript = GetComponent<PlayerHealth> ();
		// eventを登録
		healthScript.EventRespawn += EnablePlayer;
	}
	public override void OnStartLocalPlayer(){
		//照準画面をキャッシュ
		crossHairImage = GameObject.Find ("Crosshair Image").GetComponent<Image> ();
		SetRespawnButton ();
	}

	void SetRespawnButton(){
		if (isLocalPlayer) {
			// GameManagerを経由してRespawnButtonを取得
			respawnButton = GameObject.Find("GameManager").GetComponent<GameManagerReferences>().respawnButton;
			// onClick.AddListener: ボタンがクリックされた時にメソッドを実行する
			respawnButton.GetComponent<Button>().onClick.AddListener(CommenceRespawn);
			// ボタンを消す
			respawnButton.SetActive(false);
		}
	}

	public override void OnNetworkDestroy(){
		healthScript.EventRespawn -= EnablePlayer;
	}

	// PlayerDeathスクリプトのDisablePlayerメソッドのfalseをtrueに変えたイメージ
	// (health > 0 && isDead)の時に実行される
	void EnablePlayer(){
		GetComponent<CharacterController> ().enabled = true;
		GetComponent<PlayerShooting> ().enabled = true;
		GetComponent<BoxCollider> ().enabled = true;

		Renderer[] renderers = GetComponentsInChildren<Renderer> ();
		foreach (Renderer ren in renderers) {
			ren.enabled = true;
		}

		if (isLocalPlayer) {
			GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ().enabled = true;
			crossHairImage.enabled = true;
			// Respawnボタンを消す
			respawnButton.SetActive(false);
		}
	}

	// Respawnボタンを押した時に実行
	void CommenceRespawn(){
		CmdRespawnOnServer ();
	}

	[Command]
	void CmdRespawnOnServer(){
		// healthを100に戻し、SyncVarを機能させる
		healthScript.ResetHealth();
	}
}
