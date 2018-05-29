﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Fan : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		UpdateRotate();
		UpdateWindHitList();
		ApplyWindMove();
	}

	//モデルの回転処理
	void UpdateRotate() {
		float lRotateDegree = Time.fixedDeltaTime * 360.0f * mRotateSpeed;
		//mRotateFanModel.transform.localRotation *= Quaternion.Euler(0.0f, 0.0f, lRotateDegree);
		mRotateFanModel.transform.localRotation *= Quaternion.Euler(lRotateDegree, 0.0f, 0.0f);
	}

	//風に当たっているオブジェクトのリストを取得
	void UpdateWindHitList() {
		mWindHitList = GetWindHitList();
	}

	//風に当たっているオブジェクトを動かす
	void ApplyWindMove() {
		//TODO
		foreach (var windHit in mWindHitList) {
			MoveManager hitMoveMng = windHit.GetComponent<MoveManager>();
			WeightManager hitWeightMng = windHit.GetComponent<WeightManager>();
			if (hitMoveMng && hitWeightMng &&
				(hitWeightMng.WeightLv < WeightManager.Weight.heavy)) {
				// 左右移動を加える
				if (MoveManager.Move(GetDirectionVector(mDirection) * mWindMoveSpeed, windHit.GetComponent<BoxCollider>(), LayerMask.GetMask(new string[] { "Stage", "Player", "Box" }))) {
					// 上下の移動量を削除
					hitMoveMng.StopMoveVirticalAll();

					// 左右の移動量を削除
					hitMoveMng.StopMoveHorizontalAll();
				}
			}
		}

	}

	List<GameObject> GetWindHitList() {

		var lBase = new List<HitData>();
		foreach(var c in mHitColliderList) {
			List<HitData> tHit = GetHitListEachCollider(c);
			MergeHitDataList(lBase, tHit);
		}

		var lHit = lBase.OrderBy(x => x.mHitDistance);

		var lRes = new List<GameObject>();

		foreach(var h in lHit) {
			//どのオブジェクトも、3つの風判定のうち1つしか当たっていないと効果範囲外
			if(h.mHitTimes < 2) {
				continue;
			}
			//ステージなら、障害物扱いなので風を止める
			if (h.mGameObject.layer == LayerMask.NameToLayer("Stage")) {
				break;
			}
			//重いオブジェクトなら、風を止める
			if (h.mGameObject.GetComponent<WeightManager>().WeightLv == WeightManager.Weight.heavy) {
				break;
			}
			//風が適用されるオブジェクト
			lRes.Add(h.mGameObject);
		}

		return lRes;
	}

	List<HitData> GetHitListEachCollider(GameObject aCollider) {
		LayerMask l = LayerMask.GetMask(new string[] { "Player", "Box", "Stage" });
		var rc = Physics.BoxCastAll(aCollider.transform.position, aCollider.transform.lossyScale / 2.0f, GetDirectionVector(mDirection), aCollider.transform.rotation, 100.0f, l);
		return rc.Select(x => new HitData() { mGameObject = x.collider.gameObject, mHitTimes = 1, mHitDistance = x.distance }).ToList();
	}

	void MergeHitDataList(List<HitData> aBase, List<HitData> aAdd) {
		foreach(var a in aAdd) {
			bool lAlreadyExist = false;
			foreach (var b in aBase) {
				//追加するオブジェクトが、既に別のコライダーでヒット済みなら、ヒット数を増やす
				if(a.mGameObject == b.mGameObject) {
					lAlreadyExist = true;
					b.mHitTimes += 1;
					b.mHitDistance = Mathf.Min(a.mHitDistance, b.mHitDistance);
					break;
				}
			}
			//別のコライダーで当たっていなかったら、新規追加
			if(lAlreadyExist == false) {
				aBase.Add(a);
			}
		}
	}

	class HitData {
		public GameObject mGameObject;
		public int mHitTimes;
		public float mHitDistance;
	}

	enum CDirection {
		cNone,
		cLeft,
		cRight
	}

	Vector3 GetDirectionVector(CDirection aDirection) {
		switch (aDirection) {
			case CDirection.cLeft:
				return Vector3.left;
			case CDirection.cRight:
				return Vector3.right;
		}
		Debug.LogError("DirectionがNoneです", this);
		return Vector3.zero;
	}


#if UNITY_EDITOR

	//風の向きに応じてモデルを再配置
	void ReplaceModel() {

		//初期値から変わっていないのでエラー
		if (mDirection == CDirection.cNone) {
			Debug.LogError("DirectionがNoneです", this);
			return;
		}

		mFanModel.transform.rotation = Quaternion.Euler(0.0f, GetDirectionVector(mDirection).x * -mModelRotate, 0.0f);

		foreach(var c in mHitColliderList) {
			Vector3 lNewPos = c.gameObject.transform.localPosition;
			lNewPos.x = Mathf.Abs(lNewPos.x) * GetDirectionVector(mDirection).x;
			c.gameObject.transform.localPosition = lNewPos;
		}
	}

	[ContextMenu("Replace")]
	void Replace() {
		if (this == null) return;
		if (EditorUtility.IsPrefab(gameObject)) return;
		ReplaceModel();
	}

	private void OnValidate() {
		UnityEditor.EditorApplication.delayCall += Replace;
	}

#endif

	[SerializeField,Disable]
	List<GameObject> mWindHitList;	//風にヒットしたオブジェクト


	[SerializeField, Tooltip("風が吹く方向")]
	CDirection mDirection;

	[SerializeField, EditOnPrefab, Tooltip("オブジェクトを飛ばす速度")]
	float mWindMoveSpeed = 0.1f;

	[SerializeField, EditOnPrefab, Tooltip("ファンの回転する速度")]
	float mRotateSpeed;

	[SerializeField, EditOnPrefab, Tooltip("モデル")]
	GameObject mFanModel;

	[SerializeField, EditOnPrefab, Tooltip("回転するファンのモデル")]
	GameObject mRotateFanModel;

	[SerializeField, EditOnPrefab, Tooltip("モデルを回転させる角度")]
	float mModelRotate = 10.0f;

	[SerializeField, EditOnPrefab, Tooltip("風の当たり判定のコライダー")]
	List<GameObject> mHitColliderList;
}
