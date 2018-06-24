using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterState : MonoBehaviour {
	const float DefaultWaterIgnoreTime = 0.1f;

	[SerializeField]
	bool canFloat = true;
	public bool CanFloat {
		get {
			return canFloat;
		}
		set {
			canFloat = value;
		}
	}

	[SerializeField]
	bool isInWater = false;
	public bool IsInWater {
		get {
			return isInWater;
		}
		set {
			// 変更がなかった
			if (isInWater == value) return;

			// 値を変更
			isInWater = value;

			// 入水時
			if (isInWater) {
				Debug.Log("InWater OneTimeMaxSpd:" + weightLvEnterWaterMoveMax[(int)WeightMng.WeightLv] + " StayMaxSpd:" + weightLvStayWaterMoveMax[(int)WeightMng.WeightLv] + Support.ObjectInfoToString(gameObject));
				SetWaterMaxSpeed(weightLvEnterWaterMoveMax, weightLvStayWaterMoveMax);
			}
			// 出水時
			else {
				Debug.Log("OutWater OneTimeMaxSpd:" + weightLvExitWaterMoveMax[(int)WeightMng.WeightLv] + " StayMaxSpd:null" + Support.ObjectInfoToString(gameObject));
				SetWaterMaxSpeed(weightLvExitWaterMoveMax, null);

				// 水面状態を解除
				//				IsWaterSurface = false;
			}
		}
	}

	// 水面で安定状態にあるフラグ
	[SerializeField]
	bool isWaterSurface = false;
	public bool IsWaterSurface {
		get {
			return isWaterSurface;
		}
		set {
			// 変化時
			if (IsWaterSurface != value) {
				IsWaterSurfaceChange = true;

				// trueへの変化時
				if (value) {
					//bool flg = false;
					//if (flg = MoveManager.MoveTo(new Vector3(transform.position.x, ((inWaterCol.bounds.center.y + inWaterCol.bounds.size.y * 0.5f + 0.5f) - 0.5f), transform.position.z), waterCol, LayerMask.GetMask(new string[] { "Stage", "Player", "Box", "Fance" }), false, true)) {
					// 高さを補正
					MoveManager.MoveTo(new Vector3(transform.position.x, ((inWaterCol.bounds.center.y + inWaterCol.bounds.size.y * 0.5f + 0.5f) - 0.5f) + 0.499f, transform.position.z), gameObject, LayerMask.GetMask(new string[] { "Stage", "Player", "Box", "Fance" }));
					//transform.position = new Vector3(transform.position.x, prevHeight, transform.position.z);
					//}
					//Debug.LogError("水上補正 " + name + " " + transform.position.y + " " + flg);

					// 安定時の高さを保持
					prevHeight = transform.position.y;
				}
			}
			//			else {
			//				IsWaterSurfaceChange = false;
			//			}

			// 値の変更
			isWaterSurface = value;
		}
	}
	[SerializeField]
	bool isWaterSurfaceChange = false;
	public bool IsWaterSurfaceChange {
		get {
			return isWaterSurfaceChange;
		}
		set {
			isWaterSurfaceChange = value;
		}
	}

	[SerializeField]
	bool isSubmerge = false;    // 上に乗っているオブジェクトによって水面から沈められている
	bool IsSubmerge {
		get {
			return isSubmerge;
		}
		set {
			isSubmerge = value;
		}
	}

	//	[SerializeField] bool isWaterSurfaceChange = false;
	//	public bool IsWaterSurfaceChange {
	//		get {
	//			return isWaterSurfaceChange;
	//		}
	//		set {
	//			isWaterSurfaceChange = value;
	//		}
	//	}

	[SerializeField]
	List<float> waterFloatSpd = new List<float>(); // 重さ毎の上昇量

	WeightManager weightMng = null;
	WeightManager WeightMng {
		get {
			if (weightMng == null) {
				weightMng = GetComponent<WeightManager>();
				if (weightMng == null) {
					Debug.LogError("WeightManagerが見つかりませんでした。");
				}
			}
			return weightMng;
		}
	}

	MoveManager moveMng = null;
	MoveManager MoveMng {
		get {
			if (moveMng == null) {
				moveMng = GetComponent<MoveManager>();
				if (moveMng == null) {
					Debug.LogError("MoveManagerが見つかりませんでした。");
				}
			}
			return moveMng;
		}
	}

	[SerializeField]
	List<float> weightLvEnterWaterMoveMax = new List<float>(3);    // 各重さレベルの入水時の最高移動量
	[SerializeField]
	List<float> weightLvStayWaterMoveMax = new List<float>(3);     // 各重さレベルの入水中の最高移動量
	[SerializeField]
	List<float> weightLvExitWaterMoveMax = new List<float>(3);     // 各重さレベルの出水時の最高移動量
	[SerializeField]
	float cutOutSpd = 1.0f;                                        // 水面に浮く重さレベルでの入出水時に完全に移動を停止する移動量基準
	[SerializeField]
	float waterStopIgnoreRemainTime = 0.0f;
	float WaterStopIgnoreRemainTime {
		get {
			return waterStopIgnoreRemainTime;
		}
		set {
			waterStopIgnoreRemainTime = value;
		}
	}
	[SerializeField]
	float prevHeight = 0.0f;
	[SerializeField]
	BoxCollider waterCol = null;
	[SerializeField]
	BoxCollider inWaterCol = null;
	bool prevIsWaterSurface = false;

	Landing land = null;
	Landing Land {
		get {
			if (!land) {
				land = GetComponent<Landing>();
			}
			return land;
		}
	}

	void Start() {
		if (!waterCol) {
			waterCol = GetComponent<BoxCollider>();
		}
		List<RaycastHit> waterAreaList = Support.GetColliderHitInfoList(waterCol, Vector3.zero, LayerMask.GetMask("WaterArea"));
		if (waterAreaList.Count > 0) {
			// 入った水エリアを保持する
			float nearDis = float.MaxValue;
			foreach (var waterArea in waterAreaList) {
				BoxCollider waterBox = waterArea.transform.GetComponent<BoxCollider>();
				float cmpDis = Mathf.Abs((waterBox.bounds.center.y + waterBox.bounds.size.y * 0.5f) - (waterCol.bounds.center.y));
				if (cmpDis < nearDis) {
					nearDis = cmpDis;
					inWaterCol = waterBox;
				}
			}
			isInWater = true;   // プロパティを使わずに直接変更
		}
	}

	void FixedUpdate() {
		List<RaycastHit> waterAreaList = Support.GetColliderHitInfoList(waterCol, Vector3.zero, LayerMask.GetMask("WaterArea"));
		if (waterAreaList.Count > 0) {
			// 入った水エリアを保持する
			float nearDis = float.MaxValue;
			foreach (var waterArea in waterAreaList) {
				BoxCollider waterBox = waterArea.transform.GetComponent<BoxCollider>();
				float cmpDis = Mathf.Abs((waterBox.bounds.center.y + waterBox.bounds.size.y * 0.5f) - (waterCol.bounds.center.y));
				if (cmpDis < nearDis) {
					nearDis = cmpDis;
					inWaterCol = waterBox;
				}
			}
			IsInWater = true;
		}
		else {
			IsInWater = false;
		}

		// 水中なら
		if (isInWater && !IsWaterSurface) {
			if (CanFloat) {
				// 押し付けられていない場合
				if (!(Land && Land.IsExtrusionLanding)) {
					// 水による浮上
					//Debug.LogWarning("waterfloat");
					MoveMng.AddMove(new Vector3(0.0f, waterFloatSpd[(int)WeightMng.WeightLv], 0.0f), MoveManager.MoveType.waterFloat);
				}
				// 自身が宙に浮く重さであり、水上に浮く以上の重さのオブジェクトに抑えられている場合
				if ((WeightMng.WeightLv == WeightManager.Weight.flying) && (WeightMng.PileMaxWeightLv >= WeightManager.Weight.light)) {
					// 重力による移動と前回の移動量をなくす
					MoveMng.StopMoveVirtical(MoveManager.MoveType.gravity);
					MoveMng.StopMoveVirtical(MoveManager.MoveType.prevMove);
				}
			}
		}
		// 水上なら
		else if (IsWaterSurface) {
			// 重さや位置に変化が無ければ
			if ((WeightMng.WeightLv == WeightManager.Weight.light) && (transform.position.y == prevHeight) && (WeightMng.PileMaxWeightLv != WeightManager.Weight.heavy)) {
				// 落下しない
				MoveMng.StopMoveVirtical(MoveManager.MoveType.gravity);
				MoveMng.StopMoveVirtical(MoveManager.MoveType.prevMove);
				MoveMng.StopMoveVirtical(MoveManager.MoveType.waterFloat);
			}
			// 重さや位置が変化していれば
			else {
				// 水面状態を解除
				IsWaterSurface = false;

				// 上に乗っているオブジェクトに沈められた場合
				if (WeightMng.PileMaxWeightLv == WeightManager.Weight.heavy) {
					// 下方向への移動が続く限り、水面状態にならない
					IsSubmerge = true;
				}
			}
		}

		// 上に乗っているオブジェクトに沈められた後、上方向に移動していれば
		if (IsSubmerge && MoveMng.PrevMove.y > 0.0f) {
			// 沈められていない
			IsSubmerge = false;
		}
	}

	void SetWaterMaxSpeed(List<float> _oneTimeWeightLvMaxSpd, List<float> _stayWeightLvMaxSpd) {
		// 水面に浮かぶ重さレベルでの入出水時に入出水速度が一定以下であり、上に乗っているオブジェクトに沈められていなければ
		//		Debug.LogError("(" + MoveMng.TotalMove.magnitude + " <= " + cutOutSpd + ")");
		if ((WeightMng.WeightLv == WeightManager.Weight.light) && (MoveMng.PrevMove.magnitude <= cutOutSpd) && !IsSubmerge) {
			// 停止
			Debug.Log("WaterState CutOut" + MoveMng.PrevMove.magnitude);
			MoveMng.OneTimeMaxSpd = 0.0f;
			IsWaterSurface = true;
		}
		else {
			// 一度の更新に限り最大速度を制限
			MoveMng.OneTimeMaxSpd = _oneTimeWeightLvMaxSpd[(int)WeightMng.WeightLv];
		}

		// 継続的に最大速度を制限
		if (_stayWeightLvMaxSpd != null) {
			MoveMng.CustomWeightLvMaxSpd.AddRange(_stayWeightLvMaxSpd);
		}
		else {
			moveMng.CustomWeightLvMaxSpd.Clear();
		}
	}

	public void BeginWaterStopIgnore(float _time = DefaultWaterIgnoreTime) {
		WaterStopIgnoreRemainTime = _time;
	}
}
