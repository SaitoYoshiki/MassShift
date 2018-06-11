using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_StageSelectEnterUI : MonoBehaviour {

	[SerializeField]
	StageSelectEnterUI mUI;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Input.GetKeyDown(KeyCode.Y)) {
			mUI.StartAnimation();
		}
		if (Input.GetKeyDown(KeyCode.U)) {
			mUI.StopAnimation();
		}

		mUI.SetPosition(FindObjectOfType<Player>().transform.position);
	}
}
