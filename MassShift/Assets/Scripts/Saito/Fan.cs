using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Fan : MonoBehaviour {

	// Use this for initialization
	void Start () {
		mWindStopEffect = Instantiate(mWindStopEffectPrefab, mWindEffect.transform);
		
		mWindStopEffect.transform.localRotation = Quaternion.identity;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		UpdateRotate();
		UpdateWindHitList();
		ApplyWindMove();

		//風のエフェクトを止めるコライダーの位置を更新
		mWindStop.transform.localPosition = GetDirectionVector(mDirection) * (mWindHitDistance + 0.5f + mWindStop.transform.lossyScale.x / 2.0f);

		//風が止まった位置に出すエフェクトの、位置を更新
		mWindStopEffect.transform.localPosition = Vector3.left * (mWindHitDistance + 0.5f);

		//風がステージに当たっているなら
		if(mIsWindHitStage) {
			mWindStopEffect.transform.localRotation = Quaternion.identity;
		}
		else {
			mWindStopEffect.transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
		}
	}

	//モデルの回転処理
	void UpdateRotate() {
		float lRotateDegree = Time.fixedDeltaTime * 360.0f * mRotateSpeed;
		//mRotateFanModel.transform.localRotation *= Quaternion.Euler(0.0f, 0.0f, lRotateDegree);
		mRotateFanModel.transform.localRotation *= Quaternion.Euler(lRotateDegree, 0.0f, 0.0f);	//元のモデルが回転しているため、こういう訳の分からない回転に
	}

	//風に当たっているオブジェクトのリストを取得
	void UpdateWindHitList() {
		mBeforeWindHitList = mWindHitList;
		mWindHitList = GetWindHitList();

		//前のフレームで当たっていたが今のフレームで当たっていないなら
		foreach(var g in mBeforeWindHitList) {
			if(!mWindHitList.Contains(g)) {
				//風に飛ばされているフラグをfalseにする
				SetPlayerWindMove(g, false);
			}
		}
	}

	//風に当たっているオブジェクトを動かす
	void ApplyWindMove() {
		//TODO
		foreach (var windHit in mWindHitList) {
			MoveManager hitMoveMng = windHit.GetComponent<MoveManager>();
			WeightManager hitWeightMng = windHit.GetComponent<WeightManager>();
			if (hitMoveMng && hitWeightMng &&
				(hitWeightMng.WeightLv < WeightManager.Weight.heavy)) {

				Vector3 lBeforePosition = hitMoveMng.transform.position;

				// 左右移動を加える
				MoveManager.Move(GetDirectionVector(mDirection) * mWindMoveSpeed, (BoxCollider)hitMoveMng.UseCol, LayerMask.GetMask(new string[] { "Stage", "Player", "Box", "Fence" }));


				//風で少しでも移動出来ていたら、オブジェクトの重力の動きを無くす
				Vector3 lAfterPosition = hitMoveMng.transform.position;
				if (Mathf.Approximately(lAfterPosition.x, lBeforePosition.x) == false) {
					// 上下の移動量を削除
					hitMoveMng.StopMoveVirticalAll();

					// 左右の移動量を削除
					hitMoveMng.StopMoveHorizontalAll();

					//風に飛ばされているフラグをtrueにする
					SetPlayerWindMove(windHit, true);
				}
				else {
					//風に飛ばされているフラグをfalseにする
					SetPlayerWindMove(windHit, false);
				}
			}
		}

	}

	void SetPlayerWindMove(GameObject aObject, bool aIsMove) {
		var p = aObject.GetComponent<Player>();
		if (p == null) return;  //プレイヤーでないなら処理しない

		p.IsMoveByWind = aIsMove;
	}


	List<GameObject> GetWindHitList() {

		mIsWindHitStage = false;

		var lBase = new List<HitData>();
		foreach(var c in mHitColliderList) {
			IEnumerable<HitData> tHit = GetHitListEachCollider(c);
			MergeHitDataList(lBase, tHit);
		}

		var lHit = lBase.OrderBy(x => x.mHitDistance);

		var lRes = new List<GameObject>();

		mWindHitDistance = 100.0f;

		foreach (var h in lHit) {
			//どのオブジェクトも、3つの風判定のうち1つしか当たっていないと効果範囲外
			if(h.mHitTimes < mHitConditionCount) {
				continue;
			}
			//ステージなら、障害物扱いなので風を止める
			if (h.mGameObject.layer == LayerMask.NameToLayer("Stage")) {
				mWindHitDistance = h.mHitDistance;
				mIsWindHitStage = true;
				break;
			}
			//静的な箱なら、障害物扱いで風を止める
			if (h.mGameObject.GetComponent<MoveManager>() == null) {
				mWindHitDistance = h.mHitDistance;
				break;
			}

			//動く床でも、障害物扱いなので風を止める
			if (h.mGameObject.CompareTag("MoveFloor")) {
				mWindHitDistance = h.mHitDistance;
				break;
			}
			//重いオブジェクトなら、風を止める
			if (h.mGameObject.GetComponent<WeightManager>().WeightLv == WeightManager.Weight.heavy) {
				mWindHitDistance = h.mHitDistance;
				break;
			}

			//風で動かせないフラグが立っていたら、風を止める
			MoveManager lMoveMng = h.mGameObject.GetComponent<MoveManager>();
			if (lMoveMng && lMoveMng.CanMoveByWind == false) {
				mWindHitDistance = h.mHitDistance;
				break;
			}

			//風が適用されるオブジェクト
			lRes.Add(h.mGameObject);
			{
				//風下にある、隣接するオブジェクトにも風を適用
				foreach(var t in h.mGameObject.GetComponent<PileWeight>().GetPileBoxList(GetDirectionVector(mDirection), false)) {

					//重さが2ならそこで止める
					if(t.GetComponent<WeightManager>().WeightLv == WeightManager.Weight.heavy) {
						break;
					}
					lRes.Add(t.gameObject);
				}
			}
			mWindHitDistance = h.mHitDistance;
			break;	//1つで風が適用されるオブジェクトは1つだけ
		}

		return lRes;
	}

	IEnumerable<HitData> GetHitListEachCollider(GameObject aCollider) {
		LayerMask l = LayerMask.GetMask(new string[] { "Player", "Box", "Stage" });
		var rc = Physics.BoxCastAll(aCollider.transform.position, aCollider.transform.lossyScale / 2.0f, GetDirectionVector(mDirection), aCollider.transform.rotation, 100.0f, l);
		return rc.Select(x => new HitData() { mGameObject = x.collider.gameObject, mHitTimes = 1, mHitDistance = x.distance });
	}

	void MergeHitDataList(List<HitData> aBase, IEnumerable<HitData> aAdd) {
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

		float lAngle = GetDirectionVector(mDirection).x < 0.0f ? 0.0f : 1.0f;
		mWindEffect.transform.rotation = Quaternion.Euler(0.0f, lAngle * 180.0f, 0.0f);

		foreach (var c in mHitColliderList) {
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
	List<GameObject> mWindHitList;  //風にヒットしたオブジェクト

	[SerializeField, Disable]
	List<GameObject> mBeforeWindHitList;  //以前に風にヒットしたオブジェクト


	[SerializeField, Tooltip("風が吹く方向")]
	CDirection mDirection;

	[SerializeField, EditOnPrefab, Tooltip("オブジェクトを飛ばす速度")]
	float mWindMoveSpeed = 0.1f;

	[SerializeField, EditOnPrefab, Tooltip("ファンの回転する速度")]
	float mRotateSpeed;

	[SerializeField, EditOnPrefab, Tooltip("モデル")]
	GameObject mFanModel;

	[SerializeField, Tooltip("回転するファンのモデル")]
	GameObject mRotateFanModel;

	[SerializeField, EditOnPrefab, Tooltip("風のエフェクト")]
	GameObject mWindEffect;

	[SerializeField, EditOnPrefab, Tooltip("風が障害物に当たっているところのエフェクト")]
	GameObject mWindStopEffectPrefab;

	GameObject mWindStopEffect;

	[SerializeField, EditOnPrefab, Tooltip("モデルを回転させる角度")]
	float mModelRotate = 10.0f;

	[SerializeField, EditOnPrefab, Tooltip("風のエフェクトを止めるモデル")]
	GameObject mWindStop;

	[SerializeField, Tooltip("風の当たり判定のコライダー")]
	List<GameObject> mHitColliderList;

	[SerializeField, EditOnPrefab, Tooltip("風の当たり判定のコライダーがいくつ当たっていたら風に当たっている判定か")]
	int mHitConditionCount = 3;

	float mWindHitDistance = float.MaxValue;

	bool mIsWindHitStage;
}
