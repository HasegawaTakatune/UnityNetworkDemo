using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSyncPosition : NetworkBehaviour {

	// ホストから全クライアントへ送られる
	[SyncVar]
	private Vector3 syncPos;
	// playerの現在位置
	[SerializeField]
	Transform myTransform;
	// Lerp: 2ベクトル間を補間する
	[SerializeField]
	float lerpRate = 15;

	void FixedUpdate(){
		TransmitPosition ();
		LerpPosition ();// 2点間を補足する
	}

	// ポジション補間用メソッド
	void LerpPosition(){
		// 補間対象は相手プレイヤーのみ
		if(!isLocalPlayer){
			// Lerp(from, to, 割合) from～toのベクトル間を補間する
			myTransform.position = Vector3.Lerp(myTransform.position,syncPos,Time.deltaTime*lerpRate);
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
		if(isLocalPlayer){
			CmdProvidePositionToServer (myTransform.position);
		}
	}
}
