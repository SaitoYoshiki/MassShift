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
        cameraMove.fromTitle = true;
		mEndPosition = transform.position;

		Player p = GetPlayer();

        mStartPosition = p.transform.position;

        // チュートリアル以外
        if (Area.GetAreaNumber() != 0) {
            mStartPosition.y -= 1.0f;
            mStartPosition.z = 40.0f;
        }
        else {
            // タイトルからの遷移なら
            if (cameraMove.fromTitle) {
                //mStartPosition = new Vector3(-17.0f, -4.5f, 45.0f);
                mStartPosition.y += 10.0f;
                mStartPosition.z = 50.0f;
            }
            // それ以外なら
            else {
                mStartPosition.z = 35.0f;
            }
        }
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
