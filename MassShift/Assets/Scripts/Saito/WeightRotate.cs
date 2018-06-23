using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WeightRotate : MonoBehaviour {

	private void Awake() {
		mConditionX = Camera.main.transform.position.x;
	}

	// Use this for initialization
	void Start () {
		mWeightManagers = FindObjectsOfType<WeightManager>();
		mRotateRate = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {

		mStateTime += Time.deltaTime;
		mCalculatedTargetRotateRate = false;

		switch(mState) {
			case CState.cNormal:
				UpdateNormal();
				break;
			case CState.cShake:
				UpdateShake();
				break;
			case CState.cRotate:
				UpdateRotate();
				break;
		}
	}

	void UpdateNormal() {
		//初期化
		if (mNeedInitState) {
			mNeedInitState = false;
		}

		//回転する必要があるなら
		if (ReachRotate() == false) {
			mState = CState.cShake;    //まずは揺れる状態へ
		}
	}

	void UpdateShake() {
		//初期化
		if (mNeedInitState) {
			mNeedInitState = false;
			
		}

		//回転する必要がないなら
		if(ReachRotate() == true) {
			mState = CState.cNormal;    //通常状態へ
		}

		//一定秒数経過したら
		if(mStateTime >= mShakeSecond) {
			mState = CState.cRotate;	//回転状態へ
		}
	}

	void UpdateRotate() {
		//初期化
		if (mNeedInitState) {
			mNeedInitState = false;
		}

		//回転割合の更新
		mRotateRate = RotateRate(mRotateRate, TargetRotateRate);

		//トランスフォームの回転
		transform.rotation = Quaternion.Euler(0.0f, 0.0f, mRotateRate * mMaxRotateDegree);

		//回転する必要がないなら
		if (ReachRotate() == true) {
			mState = CState.cNormal;    //通常状態へ
		}
	}

	//目標とする回転に到達したか
	bool ReachRotate() {
		return Mathf.Approximately(TargetRotateRate, mRotateRate);
	}

	//目標とする割合に向けて回転した、角度の割合を返す
	float RotateRate(float aNowRate, float aTargetRate) {
		float lRes = aNowRate;

		//回転方向を求める
		float lDir = aTargetRate - aNowRate;

		//もし回転しないなら
		if(lDir == 0.0f) {
			return aNowRate;	//元の回転を返す
		}

		lRes += Mathf.Sign(lDir) * Time.deltaTime / mMaxRotateSecond;

		//-方向に回転するなら
		if(lDir < 0.0f) {
			lRes = Mathf.Clamp(lRes, aTargetRate, aNowRate);	//目標とする割合を超えないようにする
		}
		//+方向に回転するなら
		else {
			lRes = Mathf.Clamp(lRes, aNowRate, aTargetRate);
		}

		return lRes;
	}


	enum CState {
		cNormal,
		cShake,
		cRotate,
	}
	CState _State;
	CState mState {
		get {
			return _State;
		}
		set {
			_State = value;
			mStateTime = 0.0f;
			mNeedInitState = true;	//初期化する必要がある
		}
	}

	float mStateTime;
	bool mNeedInitState;	//

	float mRotateRate;	//回転している割合（-1.0f~1.0f）

	WeightManager[] mWeightManagers;    //重さを持つ全てのオブジェクト
	float mTargetRotateRate;	//傾くべき角度
	bool mCalculatedTargetRotateRate;	//このフレームで、傾くべき角度をすでに計算しているかどうか

	//どの角度まで回転させるか
	float TargetRotateRate {
		get {
			//このフレームで計算されていないなら、計算する
			if(mCalculatedTargetRotateRate == false) {
				mTargetRotateRate = CalculateTargetRotateRate();
				mCalculatedTargetRotateRate = true;
			}
			return mTargetRotateRate;	//計算済みの値を返す
		}
	}
	float CalculateTargetRotateRate() {

		System.Func<WeightManager, float> f = (WeightManager w) => { return w.transform.position.x; };

		//左右の重さの合計を求める
		//
		var lLeft = mWeightManagers.Where(x => f(x) < mConditionX);
		var lRight = mWeightManagers.Where(x => f(x) >= mConditionX);

		int lLeftTotal = lLeft.Sum(x => (int)(x.WeightLv));
		int lRightTotal = lRight.Sum(x => (int)(x.WeightLv));

		//左のほうが4以上重かったら
		if (lLeftTotal - lRightTotal >= 2) {
			return -1.0f;	//左に傾く
		}

		//右のほうが4以上重かったら
		if (lRightTotal - lLeftTotal >= 2) {
			return 1.0f;   //右に傾く
		}

		return mTargetRotateRate;	//依然と同じ傾き
	}

	float mConditionX = 0.0f;	//左側にあるか右側にあるかを判断するときに使用する、ワールド座標のx

	
	[SerializeField, Tooltip("最大回転角度"), EditOnPrefab]
	float mMaxRotateDegree = 1.0f;

	[SerializeField, Tooltip("最大回転角度までの秒数"), EditOnPrefab]
	float mMaxRotateSecond = 1.0f;

	[SerializeField, Tooltip("揺れる大きさ"), EditOnPrefab]
	float mShakeMagnitude = 0.1f;

	[SerializeField, Tooltip("揺れる秒数"), EditOnPrefab]
	float mShakeSecond = 0.2f;
}
