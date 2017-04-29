using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : Shot {
	[SerializeField]
	GameObject myObj;
	MeshRenderer myRenderer;
	Image myImage;

	byte status = PlayerStatus.ALIVE;

	// Use this for initialization
	void Start () {
		Init ();
	}
	
	// Update is called once per frame
	void Update () {
		base.Update ();

		switch (status) {
		case PlayerStatus.ALIVE:
			break;
		case PlayerStatus.DEAD:
			DamageUI ();
			CmdPlayerStatusToServer (PlayerStatus.ALIVE);
			break;
		}
	}

	// 
	void CmdPlayerStatusToServer(byte input){
		status = input;
	}

	// ダメージ処理
	[ClientCallback]
	void DamageUI(){
		if (isLocalPlayer) {
			myImage.color = new Color (255, 0, 0, 255);
			StartCoroutine (Delay (0.1f, () => {
				myImage.color = new Color (0, 0, 0, 0);
			}));
		}
	}

	// ヒット処理
	void OnCollisionEnter(){
		myRenderer.material.color = new Color (255, 0, 0, 255);
		StartCoroutine (Delay (0.1f, () => {
			myRenderer.material.color = new Color (40, 40, 40, 0);
		}));
		CmdPlayerStatusToServer (PlayerStatus.DEAD);
	}

	// 遅延処理
	IEnumerator Delay(float waitTime,Action action){
		yield return new WaitForSeconds (waitTime);
		action ();
		yield break;
	}

	// 初期化
	void Init(){
		myRenderer = myObj.GetComponent<MeshRenderer> ();
		myRenderer.material.color = new Color (40, 40, 40, 0);
		myImage = GameObject.Find ("Image").GetComponent<Image> ();
	}
}