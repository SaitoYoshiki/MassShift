using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTransform : MonoBehaviour {
	public Vector3 mStartPosition;
	public Vector3 mEndPosition;

	[SerializeField, Tooltip("移動にかける時間")]
	public float mTakeTime;

	float mDeltaTime = 0.0f;
	bool mIsMove = false;

	public bool IsMove {
		get {
			return mIsMove;
		}
	}

	private void Awake() {
		mEndPosition = transform.position;
		
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if(mIsMove == true) {
			mDeltaTime += Time.deltaTime;
			mDeltaTime = Mathf.Clamp(mDeltaTime, 0.0f, mTakeTime);

			float t = mDeltaTime / mTakeTime;
			//t = (-Mathf.Cos(t * 2 * Mathf.PI / 2.0f) + 1.0f) / 2.0f;
			t = t * t;

			transform.position = Vector3.Lerp(mStartPosition, mEndPosition, t);

			if(IsMoveEnd) {
				mIsMove = false;
			}
		}
	}

	public void MoveStart() {
		mIsMove = true;
		mDeltaTime = 0.0f;
	}

	public bool IsMoveEnd {
		get {
			return mDeltaTime >= mTakeTime;
		}
		set {
			mDeltaTime = 0.0f;
		}
	}

	public void MoveStartPosition() {
		transform.position = mStartPosition;
	}
	
}
