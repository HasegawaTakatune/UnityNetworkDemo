using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[NetworkSettings(channel=0,sendInterval=0.033f)]

public class PlayerSyncPosition : NetworkBehaviour {

	// hook: SyncVar変数が変更された時、指定メソッドを実行するようサーバーから全クライアントへ命令を出す
	//[SyncVar(hook="SyncPositionValues")]
	//private Vector3 syncPos;
	// ホストから全クライアントへ送られる
	[SyncVar]
	private Vector3 syncPos;

	// playerの現在位置
	[SerializeField]
	Transform myTransform;

	// Lerp: 2ベクトル間を補間する
	float lerpRate;
	float normalLerpRate = 15;
	float fasterLerpRate = 25;

	// 前フレームの最終位置
	private Vector3 lastPosition;

	// threshold: しきい値、境目となる値の事
	// 0.5unitを超えなければ移動していない事とする
	private float threshold = 0.5f;

	private NetworkClient networkClient;

	// 遅延時間
	private int latency;

	// 遅延時間表示用テキスト
	private Text latencyText;

	// Position同期用のList
	private List<Vector3> syncPositionList = new List<Vector3>();

	// HistoricalLerpingメソッドを使う時はtrueにする
	[SerializeField]
	private bool useHistoricalLerping = false;

	// 2点間の距離を判定する時に使う
	private float closeEnough = 0.1f;

	void Start(){
		// NetworkClientとTextをキャッシュする
		networkClient = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().client;
		latencyText = GameObject.Find ("Latency Text").GetComponent<Text> ();
		lerpRate = normalLerpRate;
	}

	void Update(){
		// 強制処理移行
		if(useHistoricalLerping)useHistoricalLerping = false;

		// 2点間を補足する
		LerpPosition ();

		ShowLatency ();
	}

	void FixedUpdate(){
		TransmitPosition ();
	}

	// ポジション補間用メソッド
	void LerpPosition(){
		// 補間対象は相手プレイヤーのみ
		if(!isLocalPlayer){
			if (useHistoricalLerping) {
				// 前時代の補間メソッド
				HistoricalLerping ();
			} else {
				// 通常の補間メソッド
				OrdinaryLerping();
			}
		}
	}

	// クライアントからホストへ、Position情報を送る
	[Command]
	void CmdProvidePositionToServer(Vector3 pos){
		// サーバー側が受け取る値
		syncPos = pos;
		//Debug.Log ("Command");
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

	// クライアントのみ有効
	[Client]
	// hookで指定されたメソッド　全クライアントが実行
	void SyncPositionValues(Vector3 latesPos){
		syncPos = latesPos;
		// ListにPosition追加
		syncPositionList.Add(syncPos);
	}

	// 通信遅延の表示
	void ShowLatency(){
		if (isLocalPlayer) {
			// latencyを取得
			latency = networkClient.GetRTT();
			//latencyを表示
			latencyText.text = latency.ToString();
		}
	}

	// 通常使われる補間メソッド
	void OrdinaryLerping(){
		// Lerp(from, to, 割合) from～toのベクトル間を補間する
		myTransform.position = Vector3.Lerp (myTransform.position, syncPos, Time.deltaTime * lerpRate);
	}

	// 過去使用されていた補間メソッド
	void HistoricalLerping(){
		// Listが1以上あったら
		if(syncPositionList.Count >0){
			// 現在位置とListの0番目の位置との中間値を補間
			myTransform.position = Vector3.Lerp(myTransform.position,syncPositionList[0],Time.deltaTime*lerpRate);

			// 2点間がcloseEnoughより小さくなった時
			if(Vector3.Distance(myTransform.position,syncPositionList[0])<closeEnough){
				// Listの0番目を削除
				syncPositionList.RemoveAt(0);
			}

			if (syncPositionList.Count > 10) {
				lerpRate = fasterLerpRate;
			} else {
				lerpRate = normalLerpRate;
			}

			// syncPositionList.Countが0に戻った時、同期が追いついたことを意味する
			Debug.Log(syncPositionList.Count.ToString());
		}
	}
}
