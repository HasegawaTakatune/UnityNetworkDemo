using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour {

	// SyncVar: Clientの変数変更情報をサーバーが受け釣り、変更結果を全クライアントへ送る
	// [Command]メソッド内でhealthが変更されたら指定メソッドを呼び出す *1
	[SyncVar (hook = "OnHealthChanged")]
	private int health = 100;

	private Text healthText;

	private bool shouldDie = false;
	public bool isDead = false;

	// 死んだ時に機能するDelegateとEvent
	// Event: メソッドを登録しておき、任意のタイミングで呼び出す 関数を変数に格納して扱う
	public delegate void DieDelegate();
	public event DieDelegate EventDie;

	// Player再生成のためのイベント
	public delegate void RespawnDelegate();
	public event RespawnDelegate EventRespawn;

	// Use this for initialization
	public override void OnStartLocalPlayer () {
		// Textオブジェクトをキャッシュ
		healthText = GameObject.Find("Health Text").GetComponent<Text>();
		SetHealthText ();
	}
	
	// Update is called once per frame
	void Update () {
		CheckCondition ();
	}

	void CheckCondition(){
		// HPが0になった時
		if(health <= 0 && !shouldDie && !isDead){
			shouldDie = true;
		}

		// HPが0で、shouldDieがtrueの時
		if(health <= 0 && shouldDie){
			// Eventが登録されている時
			if(EventDie != null){
				// Event実行
				EventDie();
			}
			shouldDie = false;
		}

		// HPが1以上あるのにisDead = trueの時
		if (health > 0 && isDead) {
			// EventRespawnに何か登録されている時
			if (EventRespawn != null) {
				// EventRespawn実行
				EventRespawn ();
			}
			isDead = false;
		}
	}

	void SetHealthText(){
		if (isLocalPlayer) {
			// 現在HPを表示
			healthText.text = "Health " + health.ToString();
		}
	}


	// ダメージを受けた時のメソッド
	// [Commmand]のついているCmdTellServerWhoWasShotメソッドから呼ばれるため
	// hookが作動するらしい *1
	public void DeductHealth(int dmg){
		health -= dmg;
	}

	// health変数に更新があると実行 *1
	void OnHealthChanged(int hlth){
		// 全クライアントへ変更結果を送信
		health = hlth;
		// HPの表示を変更
		SetHealthText();
	}

	public void ResetHealth(){
		// PlayerRespawnスクリプトのCmdRespawnOnServerメソッドが[Command]のためSyncVarが機能する
		health = 100;
	}

}
