using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerLatency : NetworkBehaviour {

	// 遅延時間
	private int latency;
	// 遅延時間表示用テキスト
	private Text latencyText;

	private NetworkClient networkClient;

	public override void OnStartLocalPlayer(){
		// NetworkClientとTextをキャッシュする
		networkClient = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().client;
		latencyText = GameObject.Find ("Latency Text").GetComponent<Text> ();
	}

	// Update is called once per frame
	void Update () {
		ShowLatency ();
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
}
