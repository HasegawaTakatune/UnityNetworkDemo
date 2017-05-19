using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson{
	public class FirstPersonControllerCustom : FirstPersonControllerSandbox {
		// プレイヤのカメラ取得
		[SerializeField]
		private Camera FPSCharacterCamera;
		// ジェット速度
		private float jetSpeed = 5;
		// 実行制限
		private bool DoOnceJet = false;
		// 角度
		private float angle;
		// 移動結果
		private Vector3 movementValue = Vector3.zero;
		// 衝突判定
		private bool conflicted = false;


	// Use this for initialization
		void Start () {
			base.Start ();
		}
	
	// Update is called once per frame
	void Update () {
			base.Update ();
			// ジェット走行
			JetFlightKey ();
		}

		// ジェット動作
		void UseJetFlight(){
			// 初期化
			float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
			float vertical = CrossPlatformInputManager.GetAxis("Vertical");
			// キーステータス
			int KeyStatus = 0;
			// キー判定
			KeyStatus += (horizontal > 0) ? MyKeyCode.RIGHT : (horizontal < 0) ? MyKeyCode.LEFT : MyKeyCode.WAIT;
			KeyStatus += (vertical > 0) ? MyKeyCode.UP : (vertical < 0) ? MyKeyCode.DOWN : MyKeyCode.WAIT;
			// 移動量保存
			AmountOfMovement (KeyStatus);
			// 速度初期化
			jetSpeed = 0.7f;
		}
		// ジェットキー
		void JetFlightKey(){
			// ジェット移動の準備
			if (base.m_Jumping && !DoOnceJet) {
				if (CrossPlatformInputManager.GetButtonDown ("Jump")) {
					UseJetFlight ();
					DoOnceJet = true;
				}
			}
			// ジェット移動使用後処理
			if (jetSpeed != 0) {
				transform.position += movementValue * jetSpeed;
				jetSpeed -= Time.deltaTime * 0.8f;
				if (jetSpeed <= 0 || conflicted)
					jetSpeed = 0;
				DoOnceJet = false;
				conflicted = false;
			}
		}

		// 方向と角度をもらい移動量を算出する
		public void AmountOfMovement(int type){
			// 実行
			bool Execution = true;
			// 移動方向の確定
			switch (type) {
			case MyKeyCode.UP:			angle = 0;	break;
			case MyKeyCode.DOWN:		angle = 180;break;
			case MyKeyCode.RIGHT:		angle = 90;	break;
			case MyKeyCode.LEFT:		angle = 270;break;
			case MyKeyCode.RIGHT_UP:	angle = 45;	break;
			case MyKeyCode.RIGHT_DOWN:	angle = 135;break;
			case MyKeyCode.LEFT_UP:		angle = 315;break;
			case MyKeyCode.LEFT_DOWN:	angle = 225;break;
			case MyKeyCode.WAIT:		angle = 0; 	break;//Execution = false;break;
			default:Debug.Log ("Error :: Movement type value of Move class has nothing." + type);Execution = false;break;
			}
			// 移動実行
			if (Execution) {
				movementValue = new Vector3 (
					Mathf.Sin ((transform.localEulerAngles.y + angle) * 3.14f / 180),
					-(Mathf.Tan ((FPSCharacterCamera.transform.localRotation.x))) * 4,
					Mathf.Cos ((transform.localEulerAngles.y + angle) * 3.14f / 180));
				Debug.Log ("rotation = " + FPSCharacterCamera.transform.localRotation);
			}
		}

		void OnTriggerEnter(Collider other){
			if (other.gameObject.tag == "Floor") {
				conflicted = true;
			}
		}
	}
}