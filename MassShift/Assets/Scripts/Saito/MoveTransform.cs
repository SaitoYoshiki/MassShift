using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTransform : MonoBehaviour {

	[SerializeField, Tooltip("開始位置")]
	Vector3 mStartPosition;

	[SerializeField, Tooltip("終了位置")]
	Vector3 mEndPosition;

	[SerializeField, Tooltip("移動にかける時間")]
	float mTakeTime;

	float mDeltaTime = 0.0f;
	bool mIsMove = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if(mIsMove == true) {
			mDeltaTime += Time.deltaTime;
			mDeltaTime = Mathf.Clamp(mDeltaTime, 0.0f, mTakeTime);

			transform.position = Vector3.Lerp(mStartPosition, mEndPosition, mDeltaTime / mTakeTime);

			if(IsMoveEnd()) {
				mIsMove = false;
			}
		}
	}

	public void MoveStart() {
		mIsMove = true;
		mDeltaTime = 0.0f;
	}

	public bool IsMoveEnd() {
		return mDeltaTime >= mTakeTime;
	}

	public void MoveStartPoisition() {
		transform.position = mStartPosition;
	}
}
