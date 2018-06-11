using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingTransform : MonoBehaviour {

	[SerializeField, Tooltip("このゲームオブジェクトに追従する")]
	GameObject mTarget;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if(mTarget) {
			transform.position = mTarget.transform.position;
		}
	}
}
