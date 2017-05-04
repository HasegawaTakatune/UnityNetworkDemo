using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerStatus : NetworkBehaviour {
	public const byte
	ALIVE = 0,
	DEAD = 1;
}