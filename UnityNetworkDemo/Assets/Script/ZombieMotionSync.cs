using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ZombieMotionSync : NetworkBehaviour {

	// 全クライアントへ送信するTransform情報
	[SyncVar]
	private Vector3 syncPos;
	[SyncVar]
	private float syncYRot;

	// 1つ前のTransform情報
	private Vector3 lastPos;
	private Quaternion lastRot;

	// 現在位置
	private Transform myTransform;

	// 補間率
	private float lerpRate = 10;

	// 補間する際のしきい値
	private float posThreshold = 0.5f;
	private float rotThreshold = 5;

	// Use this for initialization
	void Start () {
		myTransform = transform;
	}
	
	// Update is called once per frame
	void Update () {
		TransmitMotion ();
		LerpMotion ();
	}

	void TransmitMotion(){
		if (!isServer)
			return;

		// Transform情報が１つ前と比べてThreshold（しきい値）より大きい時
		if (Vector3.Distance (myTransform.position, lastPos) > posThreshold || Quaternion.Angle (myTransform.rotation, lastRot) > rotThreshold) {
			lastPos = myTransform.position;
			lastRot = myTransform.rotation;

			// SyncVar変数を変更し、全クライアントと同期を図る
			syncPos = myTransform.position;
			//localEulerAngles: Quaternion→オイラー角（360度表記）
			syncYRot = myTransform.localEulerAngles.y;
		}
	}

	// 現在のTransform情報とSyncVar情報とを補間する
	void LerpMotion(){
		if (isServer)
			return;

		// 位置情報の補間
		myTransform.position = Vector3.Lerp (myTransform.position, syncPos, Time.deltaTime * lerpRate);
		// Y軸のみ変える
		Vector3 newRota = new Vector3 (0, syncYRot, 0);

		// 角度の補間
		// Euler: オイラー角→Quaternion
		myTransform.rotation = Quaternion.Lerp (myTransform.rotation, Quaternion.Euler (newRota), Time.deltaTime * lerpRate);
	}
}
