using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStop : MonoBehaviour {

	[SerializeField]
	float mTime = 1.0f;

	float mNowTime = 0.0f;

	bool mIsHitStop;

	private void Awake() {
		mIsHitStop = false;
	}

	Pause _mPause = null;
	Pause mPause {
		get {
			if(_mPause == null) {
				_mPause = FindObjectOfType<Pause>();
			}
			return _mPause;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		//ポーズ中なら処理しない
		if (mPause) {
			if(mPause.pauseFlg == true) {
				return;
			}
		}

		if(mIsHitStop) {
			mNowTime += Time.unscaledDeltaTime;
			if (mNowTime > mTime) {
				Time.timeScale = 1.0f;  //ゲームを再開する
				mIsHitStop = false;
			}
		}
	}

	public void StartHitStop(float aTime) {
		mNowTime = 0.0f;
		mTime = aTime;
		Time.timeScale = 0.25f;  //ゲームを止める
		mIsHitStop = true;
	}
	public void StartHitStop() {
		StartHitStop(mTime);
	}
}
