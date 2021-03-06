﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LightBall : MonoBehaviour {

	Vector3 mFrom;
	Vector3 mTo;

	public Vector3 From {
		get { return mFrom; }
	}
	public Vector3 To {
		get { return mTo; }
	}

	public List<GameObject> mIgnoreList = new List<GameObject>();


	//Toまで到達したか
	bool mIsReached = false;
	public bool IsReached { 
		get { return mIsReached; }
	}

	//オブジェクトに当たったか
	bool mIsHit = false;
	public bool IsHit {
		get { return mIsHit; }
	}

	[EditOnPrefab, Tooltip("光の弾が進む速度")]
	public float mMoveSpeed = 20.0f;

	float mFromDistance;    //Fromから進んだ距離

	Vector3 mBeforePosition;

	public Vector3 mHitPosition;	//光の弾が障害物に当たったときの場所
	public Vector3 mHitDirection;	//光の弾が障害物に当たったときの向き


	[SerializeField, EditOnPrefab]
	GameObject mCollider;

	[SerializeField, EditOnPrefab]
	GameObject mModel;

	[SerializeField]
	List<GameObject> mStopEffectErase;

	public void UpdatePoint() {
		
		mFromDistance += Time.deltaTime * mMoveSpeed;
		ReachCheck();

		mFromDistance = Mathf.Clamp(mFromDistance, 0.0f, (mFrom - mTo).magnitude);
		UpdatePosition();
		HitCheck();
	}
	public void SetPoint(Vector3 aFrom, Vector3 aTo) {
		mFrom = aFrom;
		mTo = aTo;
	}
	public void InitPoint(Vector3 aFrom, Vector3 aTo) {
		mFromDistance = 0.0f;
		mIsHit = false;
		transform.position = aFrom;
		SetPoint(aFrom, aTo);
	}
	
	void ReachCheck() {

		if ((mTo - mFrom).magnitude <= mFromDistance) {
			mIsReached = true;
		}
		else {
			mIsReached = false;
		}
	}

	void UpdatePosition() {

		Vector3 lDir = mTo - mFrom;
		transform.rotation = Quaternion.FromToRotation(Vector3.right, lDir);

		//距離を進む
		mBeforePosition = transform.position;
		transform.position = mFrom + lDir.normalized * mFromDistance;
	}

	//射線が通っているか
	public bool ThroughShotLine(Vector3 aFrom, Vector3 aTo, List<GameObject> aIgnoreList = null) {

		Vector3 lDir = (aTo - aFrom);
		if(lDir.magnitude == 0.0f) {
			lDir = Vector3.up;
		}

		//LayerMask l = LayerMask.GetMask(new string[] { "Box", "Stage" });
		
		//Stageだけが、重さを移すのを遮る
		LayerMask l = LayerMask.GetMask(new string[] { "Stage" });
		List<RaycastHit> rcs = Physics.SphereCastAll(aFrom, mCollider.transform.lossyScale.x / 2.0f, lDir, (aTo - aFrom).magnitude, l).ToList();


		//通り抜けられるオブジェクトを、判定から除外する
		for (int i = rcs.Count - 1; i >= 0; i--) {

			//一方向からすり抜ける床の判定
			OnewayFloor o = rcs[i].collider.GetComponent<OnewayFloor>();
			if (o != null) {
				//もし通り抜けられるなら、判定から除外
				if (o.IsThrough(lDir)) {
					rcs.RemoveAt(i);
					continue;
				}
			}

			//ボタンは通り抜けるので、判定から除外
			if (rcs[i].collider.CompareTag("Button")) {
				rcs.RemoveAt(i);
				continue;
			}

			//ファンは通り抜けるので、判定から除外
			if (rcs[i].collider.CompareTag("Fan")) {
				rcs.RemoveAt(i);
				continue;
			}
		}

		//近い順にソートする
		rcs = rcs.OrderBy(x => x.distance).ToList();

		foreach (var rc in rcs) {
			bool lHit = false;

			if (aIgnoreList == null) {
				lHit = true;
			}
			if (aIgnoreList.Contains(rc.collider.gameObject) == false) {
				lHit = true;
			}

			if(lHit == true) {
				mHitPosition = rc.point;
				mHitDirection = aTo - aFrom;
				return false;
			}
		}
		return true;
	}
	public bool ThroughShotLine(Vector3 aFrom, Vector3 aTo, GameObject aIgnore) {
		return ThroughShotLine(aFrom, aTo, new GameObject[] { aIgnore }.ToList());
	}

	void HitCheck() {
		if(ThroughShotLine(mBeforePosition, transform.position, mIgnoreList)) {
			mIsHit = false;
		}
		else {
			mIsHit = true;
		}
	}


	//エフェクトを再生する
	//
	public void PlayEffect() {
		foreach(var p in mModel.GetComponentsInChildren<ParticleSystem>()) {
			p.Play();
		}
		foreach (var p in mModel.GetComponentsInChildren<MeshRenderer>()) {
			p.enabled = true;
		}
	}

	//エフェクトを停止する
	//
	public void StopEffect() {

		//パーティクルの発生を停止する
		foreach (var p in mModel.GetComponentsInChildren<ParticleSystem>()) {
			p.Stop();
		}
		//モデルを見えなくする
		foreach (var p in mModel.GetComponentsInChildren<MeshRenderer>()) {
			p.enabled = false;
		}

		//長時間動くパーティクルがあり、点が残って変な見た目になるので消しておく
		foreach (var p in mStopEffectErase) {
			p.SetActive(false);
		}
	}

}
