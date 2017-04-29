﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerNetworkSetup : NetworkBehaviour {

	[SerializeField]
	Camera FPSCharacterCamera;
	[SerializeField]
	AudioListener audioListener;

	// Use this for initialization
	void Start () {
		if (isLocalPlayer) {
			// SceneCameraを非アクティブ化
			GameObject.Find ("SceneCamera").SetActive (false);
			GetComponent<CharacterController> ().enabled = true;
			// FirstPersonControllerをアクティブ化
			GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ().enabled = true;
			// FirstPersonCharacterの各コンポーネントをおアクティブ化
			FPSCharacterCamera.enabled = true;
			audioListener.enabled = true;
		}
	}
}