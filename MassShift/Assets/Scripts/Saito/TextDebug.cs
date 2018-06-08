using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextDebug : MonoBehaviour {

	public void SetText(string aString) {
		GetComponent<UnityEngine.UI.Text>().text = aString;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
