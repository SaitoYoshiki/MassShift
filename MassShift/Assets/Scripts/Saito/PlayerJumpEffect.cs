using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpEffect : MonoBehaviour {

	//このゲームオブジェクトに追従する
	[HideInInspector]
	public GameObject mFollowTransform;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		Vector3 lPosition = mFollowTransform.transform.position;
		lPosition.z = 0.0f;
		transform.position = lPosition;
		transform.rotation = mFollowTransform.transform.rotation;
	}
}
