using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageShiftLineInstance : MonoBehaviour {

	[SerializeField]
	GameObject mLinePrefab;
	
	GameObject mLine;

	[SerializeField]
	GameObject mFail;

	// Use this for initialization
	void Awake() {
		mLine = Instantiate(mLinePrefab, transform);
		NotShow();
	}

	// Update is called once per frame
	void Update() {
		mLine.GetComponent<MassShiftLine>().UpdatePosition();
	}

	public void ShowDown() {
		mFail.SetActive(false);
		mLine.SetActive(true);
	}
	public void ShowUp() {
		mFail.SetActive(false);
		mLine.SetActive(true);
	}
	public void ShowFail() {
		mLine.SetActive(true);
		mFail.SetActive(true);
	}

	public void NotShow() {
		mLine.SetActive(false);
		mFail.SetActive(false);
	}


	public void SetPosition(Vector3 aWorldPosition) {

		Vector3 lStart = aWorldPosition;
		Vector3 lEnd = lStart + Vector3.up * 1.5f;

		mLine.GetComponent<MassShiftLine>().SetLinePosition(lStart, lEnd);

		mFail.transform.position = lStart + Vector3.up * 1.0f;
	}
}
