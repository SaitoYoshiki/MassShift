using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTransform : MonoBehaviour {
	public Vector3 mStartPosition;
	Vector3 mEndPosition;

	[SerializeField, Tooltip("移動にかける時間")]
	float mTakeTime;

	float mDeltaTime = 0.0f;
	bool mIsMove = false;

	private void Awake() {
		mEndPosition = transform.position;

		Player p = GetPlayer();

		mStartPosition = p.transform.position;
		mStartPosition.z = 40.0f;
		mStartPosition.y -= 1.0f;
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

	Player GetPlayer() {
		foreach(var p in FindObjectsOfType<Player>()) {
			if(p.gameObject.activeSelf == true) {
				return p;
			}
		}
		return null;
	}
}
