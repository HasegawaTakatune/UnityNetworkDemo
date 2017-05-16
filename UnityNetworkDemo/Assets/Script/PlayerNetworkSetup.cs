using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerNetworkSetup : NetworkBehaviour {

	[SerializeField]
	Camera FPSCharacterCamera;
	[SerializeField]
	AudioListener audioListener;

	// Use this for initialization
	public override void OnStartLocalPlayer () {
		// SceneCameraを非アクティブ化
		GameObject.Find ("SceneCamera").SetActive (false);
		GetComponent<CharacterController> ().enabled = true;
		// FirstPersonControllerをアクティブ化
		//GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ().enabled = true;
		GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonControllerCustom> ().enabled = true;
		// FirstPersonCharacterの各コンポーネントをアクティブ化
		FPSCharacterCamera.enabled = true;
		audioListener.enabled = true;
		// LocalPlayerのRendererｗｐ非表示にする
		Renderer[] rens = GetComponentsInChildren<Renderer>();
		foreach (Renderer ren in rens) {
			ren.enabled = false;
		}
		// LocalPlayerのAnimatorパラメータを自動的に送る
		GetComponent<NetworkAnimator>().SetParameterAutoSend(0,true);
	}

	public override void PreStartClient(){
		// ClientのAnimatorパラメータを自動的に送る
		GetComponent<NetworkAnimator>().SetParameterAutoSend(0,true);
	}
}
