using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageShiftUI : MonoBehaviour {

	[SerializeField]
	GameObject mUIPrefab;

	GameObject mSourceUI;
	GameObject mDestUI;


	// Use this for initialization
	void Start () {
		mSourceUI = Instantiate(mUIPrefab, transform);
		mDestUI = Instantiate(mUIPrefab, transform);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public StageShiftUIInstance GetSourceUI() {
		return mSourceUI.GetComponent<StageShiftUIInstance>();
	}
	public StageShiftUIInstance GetDestUI() {
		return mDestUI.GetComponent<StageShiftUIInstance>();
	}
}
