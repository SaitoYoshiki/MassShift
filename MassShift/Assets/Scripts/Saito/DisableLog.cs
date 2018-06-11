using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableLog : MonoBehaviour {

	private void Awake() {
		//ログを非表示にする
		Debug.logger.logEnabled = false;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
