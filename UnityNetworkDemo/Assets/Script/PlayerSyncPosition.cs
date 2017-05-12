using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(channel=0,sendInterval=0.033f)]

public class PlayerSyncPosition : NetworkBehaviour {

	// ホストから全クライアントへ送られる
	[SyncVar]
	private Vector3 syncPos;

	// playerの現在位置
	[SerializeField]
	Transform myTransform;

	// Lerp: 2ベクトル間を補間する
	private float lerpRate = 15;

	// 前フレームの最終位置
	private Vector3 lastPosition;

	// threshold: しきい値、境目となる値の事
	// 0.5unitを超えなければ移動していない事とする
	private float threshold = 0.5f;

	void Update(){
		// 2点間を補足する
		LerpPosition ();
	}

	void FixedUpdate(){
		TransmitPosition ();
	}

	// ポジション補間用メソッド
	void LerpPosition(){
		// 補間対象は相手プレイヤーのみ
		if (!isLocalPlayer) {
			// 通常の補間メソッド
			OrdinaryLerping ();
		}
	}

	// クライアントからホストへ、Position情報を送る
	[Command]
	void CmdProvidePositionToServer(Vector3 pos){
		// サーバー側が受け取る値
		syncPos = pos;
	}

	// クライアントのみ実行される
	[ClientCallback]
	void TransmitPosition(){
		// 位置座標を送るメソッド
		// 自分プレイヤー且つ、現在位置と前フレームの最終位置との距離がthresholdより大きい時
		if (isLocalPlayer) {
			if (Vector3.Distance (myTransform.position, lastPosition) > threshold) {
				CmdProvidePositionToServer (myTransform.position);
				// 現在位置を最終位置として保存
				lastPosition = myTransform.position;
			}
		}
	}
				
	// 通常使われる補間メソッド
	void OrdinaryLerping(){
		// Lerp(from, to, 割合) from～toのベクトル間を補間する
		myTransform.position = Vector3.Lerp (myTransform.position, syncPos, Time.deltaTime * lerpRate);
	}
}
