using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageShiftLine : MonoBehaviour {

	[SerializeField]
	GameObject mLinePrefab;

	GameObject mSourceLine;
	GameObject mDestLine;


	// Use this for initialization
	void Start() {
		mSourceLine = Instantiate(mLinePrefab, transform);
		mDestLine = Instantiate(mLinePrefab, transform);
	}

	// Update is called once per frame
	void Update() {

	}

	public StageShiftLineInstance GetSource() {
		return mSourceLine.GetComponent<StageShiftLineInstance>();
	}
	public StageShiftLineInstance GetDest() {
		return mDestLine.GetComponent<StageShiftLineInstance>();
	}
}
