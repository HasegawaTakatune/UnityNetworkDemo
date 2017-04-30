using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSyncRotation : NetworkBehaviour {

	//SyncVar: ホストサーバーからクライアントへ送られる
	// プレイヤーの角度
	[SyncVar]
	private Quaternion syncPlayerRotation;
	// FirstPersonCharacterのカメラ角度
	[SyncVar]
	private Quaternion syncCameraRotation;

	[SerializeField]
	private Transform playerTransform;
	[SerializeField]
	private Transform cameraTransform;
	[SerializeField]
	private float lerpRate = 15;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void FixedUpdate(){
		// クライアント側のPlayerの角度を取得
		TransmitRotations();
		// 現在角度と取得した角度を補間する
		LerpRotations();
	}

	// 角度を補間するメソッド
	void LerpRotations(){
		// 自分プレイヤー以外のPlayerの時
		if(!isLocalPlayer){
			// プレイヤー角度とカメラ角度を補間
			playerTransform.rotation = Quaternion.Lerp (playerTransform.rotation, syncPlayerRotation,Time.deltaTime * lerpRate);
			cameraTransform.rotation = Quaternion.Lerp (cameraTransform.rotation, syncCameraRotation, Time.deltaTime * lerpRate);
		}
	}

	// クライアントあらホストへ送られる
	[Command]
	void CmdProvideRotationsToSever(Quaternion playerRota,Quaternion cameraRota){
		syncPlayerRotation = playerRota;
		syncCameraRotation = cameraRota;
	}

	// クライアント側だけが実行できるメソッド
	[Client]
	void TransmitRotations(){
		if (isLocalPlayer) {
			CmdProvideRotationsToSever (playerTransform.rotation, cameraTransform.rotation);
		}
	}

}
