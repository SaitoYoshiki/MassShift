using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Landing)), RequireComponent(typeof(WeightManager))]
public class LandImpact : MonoBehaviour {

	//落ちた場所の環境
	public enum CEnviroment {
		cInvalid,	//無効な値
		cGround,	//地上
		cWaterSurface,	//水面
		cWater,	//水中
	}

	public delegate void OnLandEvent(WeightManager.Weight aWeight, CEnviroment aEnviroment, float aFallDistance);
	public event OnLandEvent OnLand;

	[SerializeField, Tooltip("この距離以上を落下すると、落下演出が起きる"), EditOnPrefab]
	float mImpactDistanceFlying = 1.0f;

	[SerializeField, Tooltip("この距離以上を落下すると、落下演出が起きる"), EditOnPrefab]
	float mImpactDistanceLight = 1.0f;

	[SerializeField, Tooltip("この距離以上を落下すると、落下演出が起きる"), EditOnPrefab]
	float mImpactDistanceHeavy = 1.0f;

	Vector3 mBeforePosition;    //前回の位置を保存

	Vector3 mHighestPosition;	//落下時の、最高地点を保存
	
	bool mBeforeInWater = false;	//前のフレームで水の中にいたか

	//コンポーネントのキャッシュ
	Landing mLanding;
	WeightManager mWeightManager;
	WaterState mWaterState;
	MoveManager mMoveManager;


	// Use this for initialization
	void Start () {
		mLanding = GetComponent<Landing>();
		mWeightManager = GetComponent<WeightManager>();
		mWaterState = GetComponent<WaterState>();
		mMoveManager = GetComponent<MoveManager>();

		mHighestPosition = transform.position;  //最高地点を更新
		mBeforePosition = transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		//移動量が大きすぎる場合は、ワープ扱いをして落下距離をリセット
		if((mBeforePosition - transform.position).magnitude >= 2.0f) {
			mHighestPosition = transform.position;
			mBeforePosition = transform.position;
		}

		float lFallDistance = Mathf.Abs(mHighestPosition.y - transform.position.y);


		//接地し始めた瞬間で
		if (mLanding.noticeLandEffect == true) {

			mLanding.noticeLandEffect = false;	//falseにしておく

			//一定距離以上落ちていたら
			if (lFallDistance >= GetImpactDistance()) {

				//水中なら
				if (IsInWater()) {
					OnLand(mWeightManager.WeightLv, CEnviroment.cWater, lFallDistance);    //水中に落下
				}
				else {
					OnLand(mWeightManager.WeightLv, CEnviroment.cGround, lFallDistance);    //地上に落下
				}
				mHighestPosition = transform.position;	//最高地点を更新
			}
		}


		//着水判定
		//
		bool lInWater = IsInWater();

		//水の中に入った瞬間で
		//if (mBeforeInWater == false && lInWater == true) {
		//水の中に入った瞬間、または出た瞬間で
		if (mBeforeInWater != lInWater) {

			//一定距離以上落ちていたら
			if (lFallDistance >= GetImpactDistance()) {
				OnLand(mWeightManager.WeightLv, CEnviroment.cWaterSurface, lFallDistance);    //インパクトのイベントを呼び出す
				mHighestPosition = transform.position;  //最高地点を更新
			}
		}
		mBeforeInWater = lInWater;
		

		//最高地点の更新
		//

		//下向きに落ちるなら
		if(mMoveManager.GetFallVec() < 0.0f) {
			//上向きに進んでいたら
			if (transform.position.y >= mBeforePosition.y) {
				mHighestPosition = transform.position;  //最高地点を更新
			}
		}
		//上向きに落ちるなら
		else {
			//下向きに進んでいたら
			if (transform.position.y <= mBeforePosition.y) {
				mHighestPosition = transform.position;  //最高地点を更新
			}
		}

		
		//前回位置を更新
		mBeforePosition = transform.position;
	}

	//水の中にいるかどうかを取得する
	bool IsInWater() {
		return mWaterState.IsInWater;
	}

	float GetImpactDistance() {
		switch(mWeightManager.WeightLv) {
			case WeightManager.Weight.flying:
				return mImpactDistanceFlying;
			case WeightManager.Weight.light:
				return mImpactDistanceLight;
			case WeightManager.Weight.heavy:
				return mImpactDistanceHeavy;
		}
		return float.MaxValue;
	}
}
