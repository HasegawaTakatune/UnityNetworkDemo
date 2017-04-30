using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Shot :  NetworkBehaviour{
	// 弾丸
	[SerializeField]
	GameObject[] bullet;
	// 銃口
	[SerializeField]
	Transform muzzle;
	//
	[SerializeField]
	float interval = 3.0f;

	// Use this for initialization
	public void Start () {
	}
	
	// Update is called once per frame
	public void Update () {
		if (isLocalPlayer) {
			KeyShot ();
		}
	}
		
	// 射出
	void KeyShot(){
		// 撃ち出し
		if (Input.GetMouseButtonDown (0)) {
			// 弾の生成
			GameObject bullets = GameObject.Instantiate (bullet [bullet.Length - 1])as GameObject;
			// 弾の移動方向設定
			bullets.GetComponent<Bullet> ().SetAngle (gameObject.transform.localEulerAngles);
			// 発射位置修正
			bullets.transform.position = muzzle.position;
			NetworkServer.Spawn (bullet [bullet.Length - 1]);
			StartCoroutine (DestroyBullet (bullet [bullet.Length - 1]));
		}
	}
		
	[Server]
	IEnumerator DestroyBullet(GameObject bullet){
		yield return new WaitForSeconds (interval);
		NetworkServer.Destroy (bullet);
		Destroy (bullet);
	}
}
