﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFloorTest : MonoBehaviour {

	[SerializeField]
	MoveFloor mMoveFloor;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.U)) {
			Debug.Log("ToFlying", mMoveFloor);
			mMoveFloor.mWeightManager.WeightLv = WeightManager.Weight.flying;
		}
		if (Input.GetKeyDown(KeyCode.I)) {
			Debug.Log("ToLight", mMoveFloor);
			mMoveFloor.mWeightManager.WeightLv = WeightManager.Weight.light;
		}
		if (Input.GetKeyDown(KeyCode.O)) {
			Debug.Log("ToHeavy", mMoveFloor);
			mMoveFloor.mWeightManager.WeightLv = WeightManager.Weight.heavy;
		}
	}
}
