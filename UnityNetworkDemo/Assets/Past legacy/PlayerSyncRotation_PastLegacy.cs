using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSyncRotation_PastLegacy : NetworkBehaviour {

	// Quaternion型からfloat型に修正
	// hook: SyncVar変数が変更された時に、SyncVar変数を引数にして指定したメソッドを実行
	//[SyncVar (hook = "OnPlayerRotaSynced")]
	[SyncVar]
	private float syncPlayerRotation;
	//[SyncVar (hook = "OnCameraRotaSynced")]
	[SyncVar]
	private float syncCameraRotation;

	private float lerpRate = 17;
	// float型（オイラー角、360度）
	private float lastPlayerRota;
	private float lastCameraRota;
	//
	private float threshold = 1;
	// 角度保存用List
	private List<float> syncPlayerRotaList = new List<float>();
	private List<float> syncCameraRotaList = new List<float>();
	// HistoricalInterpolationで角度の判定に使用
	private float closeEnough = 0.4f;
	// 前時代の角度同期メソッドを使うか
	[SerializeField]
	private bool useHistoricalInterpolation;

	/*//SyncVar: ホストサーバーからクライアントへ送られる
	// プレイヤーの角度
	[SyncVar]
	private Quaternion syncPlayerRotation;
	// FirstPersonCharacterのカメラ角度
	[SyncVar]
	private Quaternion syncCameraRotation;*/

	[SerializeField]
	private Transform playerTransform;
	[SerializeField]
	private Transform cameraTransform;
	/*[SerializeField]
	private float lerpRate = 15;

	// 前フレームの最終角度
	private Quaternion lastPlayerRota;
	private Quaternion lastCameraRota;
	// threshold: しきい値、境目となる値の事
	// 5unitを超えなければ移動していない事とする
	private float threshold = 5;*/


	void Update(){
		// 処理強制移行
		if (useHistoricalInterpolation)useHistoricalInterpolation = false;
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
		if(!isLocalPlayer){
			// プレイヤー角度とカメラ角度を補間
			//playerTransform.rotation = Quaternion.Lerp (playerTransform.rotation, syncPlayerRotation,Time.deltaTime * lerpRate);
			//cameraTransform.rotation = Quaternion.Lerp (cameraTransform.rotation, syncCameraRotation, Time.deltaTime * lerpRate);
			if (useHistoricalInterpolation) {
				// 前時代の角度同期判定
				HistoricalInterpolation ();
			} else {
				// UNETを使用した角度同期判定
				OrdinaryLerping();
			}
		}
	}

	// 前時代の角度同期判定
	void HistoricalInterpolation(){
		// Listが１つでもあったら
		if(syncPlayerRotaList.Count > 0){
			LerpPlayerRotation (syncPlayerRotaList [0]);
			if (Mathf.Abs (playerTransform.localEulerAngles.y - syncPlayerRotaList [0]) < closeEnough) {
				syncPlayerRotaList.RemoveAt (0);
			}
			Debug.Log (syncPlayerRotaList.Count.ToString () + "syncPlayerRotaList Count");
		}

		if (syncCameraRotaList.Count > 0) {
			LerpCameraRotation (syncCameraRotaList [0]);
			if (Mathf.Abs (cameraTransform.localEulerAngles.x - syncCameraRotaList [0]) < closeEnough) {
				syncCameraRotaList.RemoveAt (0);
			}
			Debug.Log (syncCameraRotaList.Count.ToString () + "syncCameraRotaList Count");
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

	// クライアントあらホストへ送られる
	[Command]
	void CmdProvideRotationsToSever(float playerRota,float cameraRota){//Quaternion playerRota,Quaternion cameraRota){
		syncPlayerRotation = playerRota;
		syncCameraRotation = cameraRota;
		//Debug.Log ("Command for angle");
	}

	// クライアント側だけが実行できるメソッド
	[Client]
	void TransmitRotations(){
		if (isLocalPlayer) {
			//if (Quaternion.Angle (playerTransform.rotation, lastPlayerRota) > threshold ||
			//    Quaternion.Angle (cameraTransform.rotation, lastCameraRota) > threshold) {
			// localEularAngles: Quaternion角をオイラー角（360度）で回転量を表す
			if (CheckIfBeyondThreshold (playerTransform.localEulerAngles.y, lastPlayerRota) ||
			   CheckIfBeyondThreshold (cameraTransform.localEulerAngles.y, lastCameraRota)) {
				// lastPlayerRotaとlastCameraRotaを現在角度に更新
				lastPlayerRota = playerTransform.localEulerAngles.y;
				lastCameraRota = cameraTransform.localEulerAngles.x;
				// 現在角度に更新したlastPlayerRotaとlastCameraRotaでメソッド実行
				CmdProvideRotationsToSever (lastPlayerRota, lastCameraRota);

				/*CmdProvideRotationsToSever (playerTransform.rotation, cameraTransform.rotation);
				// 最終角度の更新
				lastPlayerRota = playerTransform.rotation;
				lastCameraRota = cameraTransform.rotation;*/
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

	// syncPlayerRotation変数が変更された時に実行（hook）
	// Clientのみ実行
	[Client]
	void OnPlayerRotaSynced(float latestPlayerRota){
		// hookは自分で同期する必要がある
		syncPlayerRotation = latestPlayerRota;
		// Listに登録
		syncPlayerRotaList.Add(syncPlayerRotation);
	}

	// syncCameraRotation変数が変更された時に実行（hook）
	// Clientのみ実行
	[Client]
	void OnCameraRotaSynced(float latestCameraRota){
		// hookは自分で同期する必要がある
		syncCameraRotation = latestCameraRota;
		// Listに登録
		syncCameraRotaList.Add(syncCameraRotation);
	}

}
