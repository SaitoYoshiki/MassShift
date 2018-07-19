using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableFromTitle : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (cameraMove.fromTitle) {
            cameraMove.fromTitle = false;
        }
	}
}
