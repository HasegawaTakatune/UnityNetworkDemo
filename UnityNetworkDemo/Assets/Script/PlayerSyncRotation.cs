using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSyncRotation : NetworkBehaviour {

	// Quaternion型からfloat型に修正
	[SyncVar]
	private float syncPlayerRotation;
	[SyncVar]
	private float syncCameraRotation;

	private float lerpRate = 17;
	// float型（オイラー角、360度）
	private float lastPlayerRota;
	private float lastCameraRota;
	//
	private float threshold = 1;

	[SerializeField]
	private Transform playerTransform;
	[SerializeField]
	private Transform cameraTransform;

	void Update(){
		// 現在角度と取得した角度を補間する
		LerpRotations();
	}

	// Update is called once per frame
	void FixedUpdate(){
		// クライアント側のPlayerの角度を取得
		TransmitRotations();
	}

	// 角度を補間するメソッド
	void LerpRotations(){
		// 自分プレイヤー以外のPlayerの時
		if (!isLocalPlayer) {
			// プレイヤー角度とカメラ角度を補間
			// UNETを使用した角度同期判定
			OrdinaryLerping ();
		}
	}

	void OrdinaryLerping(){
		LerpPlayerRotation (syncPlayerRotation);
		LerpCameraRotation (syncCameraRotation);
	}

	// プレイヤーの現在角度を補間
	void LerpPlayerRotation(float rotAngle){
		// 引数のオイラー角を、Vector3の形に保存
		Vector3 PlayerNewRota = new Vector3(0,rotAngle,0);
		// Lerp : 現在の角度と受け取った角度の補間値を求める
		// Euler: オイラー角をQuaternion角に変換してくれる
		playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation,Quaternion.Euler(PlayerNewRota),lerpRate * Time.deltaTime);
	}

	// カメラの現在角度を補間
	void LerpCameraRotation(float rotAngle){
		Vector3 cameraNewRota = new Vector3 (rotAngle, 0, 0);
		// カメラは子オブジェクトのため、親オブジェクトの向きを継承するlocalRotationを使う
		cameraTransform.localRotation=Quaternion.Lerp(cameraTransform.localRotation,Quaternion.Euler(cameraNewRota),lerpRate * Time.deltaTime);
	}

	// クライアントからホストへ送られる
	[Command]
	void CmdProvideRotationsToSever(float playerRota,float cameraRota){//Quaternion playerRota,Quaternion cameraRota){
		syncPlayerRotation = playerRota;
		syncCameraRotation = cameraRota;
	}

	// クライアント側だけが実行できるメソッド
	[Client]
	void TransmitRotations(){
		if (isLocalPlayer) {
			// localEularAngles: Quaternion角をオイラー角（360度）で回転量を表す
			if (CheckIfBeyondThreshold (playerTransform.localEulerAngles.y, lastPlayerRota) ||
			    CheckIfBeyondThreshold (cameraTransform.localEulerAngles.y, lastCameraRota)) {
				// lastPlayerRotaとlastCameraRotaを現在角度に更新
				lastPlayerRota = playerTransform.localEulerAngles.y;
				lastCameraRota = cameraTransform.localEulerAngles.x;
				// 現在角度に更新したlastPlayerRotaとlastCameraRotaでメソッド実行
				CmdProvideRotationsToSever (lastPlayerRota, lastCameraRota);
			}
		}
	}

	// 現在角度と前フレームのオイラー角を比較し、threshold(1度)以上開きがあったらtrueを返す
	bool CheckIfBeyondThreshold(float rot1,float rot2){
		// Mathf.Abs: 絶対値取得
		if (Mathf.Abs (rot1 - rot2) > threshold) {
			return true;
		} else {
			return false;
		}
	}

}
