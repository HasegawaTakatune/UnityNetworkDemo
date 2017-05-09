using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// NetworkManagerを継承*
public class NetworkManagerCustom : NetworkManager {
	bool onLoaded = false;

	void FixedUpdate(){
		// シーンがロードされた時
		SceneManager.sceneLoaded += delegate {
			OnLoaded();
		};
		if (onLoaded) WhenTheSceneIsLoaded ();
	}

	void OnLoaded(){
		onLoaded = true;
	}

	// ButtonStartHostボタンを押した時に実行
	// IPポートを設定し、ホストとして接続
	public void StartupHost(){
		SetPort ();
		NetworkManager.singleton.StartHost ();
	}

	// ButtonJoinGameボタンをした時に実行
	// IPアドレスとポートを設定し、クライアントとして接続
	public void JoinGame(){
		SetIPAddress ();
		SetPort ();
		NetworkManager.singleton.StartClient ();
	}

	void SetIPAddress(){
		// Input Fieldに記入されたIPアドレスを取得し、接続する
		string ipAddress = GameObject.Find("InputFieldIPAddress").transform.FindChild("Placehplder").GetComponent<Text>().text;
		NetworkManager.singleton.networkAddress = ipAddress;
	}

	// ポートの設定
	void SetPort(){
		NetworkManager.singleton.networkPort = 7777;
	}

	// *********** 非推奨 **************
	// UnityデフォルトのAPIシーンをロードした時にLevelを引数に実行
	// 各シーンのLevelはBuild　Settingsにて設定
	/*void OnLevelWasLoaded(int level){
		if (level == 0) {
			// Menuシーンへ移動した時
			SetupMenuSceneButtons ();
		} else {
			// 他のシーン（Mainシーン）へ移動した時
			SetupOtherSceneButton();
		}
	}*/
	// OnLevelWasLoadedが非推奨になったのでその対応策
	void WhenTheSceneIsLoaded(){
		// Scene indexの取得
		int level = SceneManager.GetActiveScene ().buildIndex;
		if (level == 0) {
			// Menuシーンへ移動した時
			SetupMenuSceneButtons ();
		} else {
			// 他のシーン（Mainシーン）へ移動した時
			SetupOtherSceneButton ();
		}
		onLoaded = false;
	}

	void SetupMenuSceneButtons(){
		// RemoveListener: Buttonイベントを削除する
		// AddListener	 : ボタンのベントを登録する
		GameObject.Find ("ButtonStartHost").GetComponent<Button> ().onClick.RemoveAllListeners ();
		GameObject.Find ("ButtonStartHost").GetComponent<Button> ().onClick.AddListener (StartupHost);

		GameObject.Find ("ButtonJoinGame").GetComponent<Button> ().onClick.RemoveAllListeners ();
		GameObject.Find ("ButtonJoinGame").GetComponent<Button> ().onClick.AddListener (JoinGame);
	}

	void SetupOtherSceneButton(){
		// DisconnectボタンにStopHostメソッドを登録する
		GameObject.Find ("ButtonDisconnect").GetComponent<Button> ().onClick.RemoveAllListeners ();
		GameObject.Find ("ButtonDisconnect").GetComponent<Button> ().onClick.AddListener (NetworkManager.singleton.StopHost);
	}
}