using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningCameraMove : MonoBehaviour {

	//目標地点
	public Vector3 mTarget;

	//速度
	public float mSpeed;

	//移動するかどうか
	public bool mIsMove = false;

	//目標地点に到達したかどうか
	public bool IsReach {
		get {
			return mTarget == transform.position;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (mIsMove == false) return;

		Vector3 lDirection = mTarget - transform.position;
		float lMoveDelta = mSpeed * Time.deltaTime;

		//目標地点にたどり着かないなら
		if(lMoveDelta < lDirection.magnitude) {
			transform.position += lDirection.normalized * lMoveDelta;
		}
		else {
			transform.position = mTarget;
		}
	}
}
