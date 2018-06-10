using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTest : MonoBehaviour {

	[SerializeField]
	PlayerAnimation mPlayerAnimation;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.X)) {
			mPlayerAnimation.StartHandSpring();
		}
		if (Input.GetKeyDown(KeyCode.C)) {
			mPlayerAnimation.StartHoldHandSpring();
		}
	}
}
