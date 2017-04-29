using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour {

	[SerializeField]
	GameObject obj;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private IEnumerator Spawner(float delay){
		Network.Instantiate (obj, transform.position, Quaternion.identity, 0);
		yield return new WaitForSeconds (delay);
	}
}
