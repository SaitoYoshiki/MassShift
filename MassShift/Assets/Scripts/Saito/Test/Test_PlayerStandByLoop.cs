using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_PlayerStandByLoop : MonoBehaviour {

	[SerializeField]
	PlayerAnimation pl;

	// Use this for initialization
	void Start () {
		if(pl == null) {
			pl = FindObjectOfType<PlayerAnimation>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log("StandByFinish:" + pl.IsStandByAnimationFinish());
	}
}
