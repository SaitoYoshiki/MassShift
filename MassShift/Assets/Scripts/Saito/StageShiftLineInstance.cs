using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageShiftLineInstance : MonoBehaviour {

	[SerializeField]
	Color mUpColor;

	[SerializeField]
	float mUpColorPower;

	[SerializeField]
	Color mDownColor;

	[SerializeField]
	float mDownColorPower;


	[SerializeField]
	GameObject mLinePrefab;
	
	GameObject mLine;

	[SerializeField]
	GameObject mFail;

	bool mIsUp = false;

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
		mIsUp = false;
	}
	public void ShowUp() {
		mFail.SetActive(false);
		mLine.SetActive(true);
		mIsUp = true;
	}
	public void ShowFail() {
		mLine.SetActive(false);
		mFail.SetActive(true);
	}

	public void NotShow() {
		mLine.SetActive(false);
		mFail.SetActive(false);
	}


	public void SetPosition(Vector3 aWorldPosition) {

		Vector3 lStart = aWorldPosition;
		Vector3 lEnd = lStart + Vector3.up * 2.0f;

		//下に行くなら、線の開始位置と終了位置を入れ替える
		if(mIsUp == false) {
			Vector3 t = lStart;
			lStart = lEnd;
			lEnd = t;
		}

		mLine.GetComponent<MassShiftLine>().SetLinePosition(lStart, lEnd);
		

		Color lColor = mUpColor * mUpColorPower;
		if (mIsUp == false) {
			lColor = mDownColor * mDownColorPower;
		}

		mLine.GetComponent<MassShiftLine>().ChangeColor(lColor);


		mFail.transform.position = aWorldPosition + Vector3.up * 1.0f;
	}
}
