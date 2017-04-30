using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Bullet : NetworkBehaviour {

	// 角度
	Vector3 angle;
	// 速度
	float speed = 1;
	// 移動量
	Vector3 movement;

	// Use this for initialization
	public void Start () {
		movement = new Vector3 (
			Mathf.Sin ((angle.y) * 3.14f / 180) * speed,
			-(Mathf.Tan ((angle.x) * 3.14f / 180) * speed),
			Mathf.Cos ((angle.y) * 3.14f / 180) * speed);
	}
	
	// Update is called once per frame
	public void Update () {
		transform.position += movement;
	}
	// 弾の移動方向セット
	public void SetAngle(Vector3 input){
		angle = input;
	}

}