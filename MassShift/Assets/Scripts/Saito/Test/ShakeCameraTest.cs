﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCameraTest : MonoBehaviour {

	[SerializeField]
	float mTime = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Input.GetKeyDown(KeyCode.T)) {
			//Shake(mTime);
			ShakeCamera.ShakeAll(mTime, 0.2f);
		}
	}

}
