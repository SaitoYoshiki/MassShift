using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class titleCameraMove : MonoBehaviour {
    [SerializeField]
    Vector3 startCameraPos;

	// Use this for initialization
	void Awake () {
        FindObjectOfType<MoveTransform>().mStartPosition = startCameraPos;
	}
}
