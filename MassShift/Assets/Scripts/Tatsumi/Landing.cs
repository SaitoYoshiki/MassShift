﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landing : MonoBehaviour {
	[SerializeField] Transform landingCol = null;    // 接地判定用オブジェクト
	public Transform LandingCol {
		get {
			return landingCol;
		}
		set {
			landingCol = value;
		}
	}

	[SerializeField] bool isLanding = false;
	public bool IsLanding {
		get {
			return isLanding;
		}
		set {
			if (value == true) {
				// 縦方向の移動を停止
				MoveMng.StopMoveVirtical(MoveManager.MoveType.prevMove);
				MoveMng.StopMoveVirtical(MoveManager.MoveType.gravity);
			}

			// 値に変化がない
			if (isLanding == value) return;

			Debug.Log("isLanding " + value + " " + name);

			// 値を変更
			isLanding = value;

			// 接地時
			if (value == true) {
				// 着地エフェクト出現タイミング
				noticeLandEffect = true;

				// ジャンプによる通常の重力加速度停止を解除
				MoveMng.GravityCustomTime = 0.0f;

				//				// 有効になった瞬間
				//				IsLandingTrueChange = true;
			}
			// 離地時
			else {
				LandColList.Clear();
			}
		}
	}

	public bool noticeLandEffect = false;

//	[SerializeField] bool isLandingTrueChange = false;
//	public bool IsLandingTrueChange {
//		get {
//			return isLandingTrueChange;
//		}
//		set {
//			isLandingTrueChange = value;
//		}
//	}
		
	[SerializeField] bool isExtrusionLanding;
	public bool IsExtrusionLanding {
		get {
			return isExtrusionLanding;
		}
		set {
			isExtrusionLanding = value;

			//// 値の変更時
			//if(isExtrusionLanding != value) {
			//	isExtrusionLandingChange = true;
			//}

			// 押し出し接地時
			if (value == true) {
				isExtrusionLandingChange = true;
				// 縦方向の移動を停止
				MoveMng.StopMoveVirtical(MoveManager.MoveType.prevMove);
				MoveMng.StopMoveVirtical(MoveManager.MoveType.gravity);
			}
			// 押し出し離地時
			else {
				landExtrusionColList.Clear();
			}
		}
	}
	[SerializeField]
	bool isExtrusionLandingChange = false;

	[SerializeField]
	bool isWaterFloatLanding = false;
	public bool IsWaterFloatLanding {
		get {
			return isWaterFloatLanding;
		}
		set {
			if (value == true) {
				// 縦方向の移動を停止
				MoveMng.StopMoveVirtical(MoveManager.MoveType.prevMove);
				MoveMng.StopMoveVirtical(MoveManager.MoveType.gravity);
			}

			// 値に変更が無ければ処理しない
			if (isWaterFloatLanding == value) return;

			// 値の変更
			isWaterFloatLanding = value;

			Debug.Log("IsWaterFloatLanding =" + value);

//			// 有効化になった瞬間
//			if (value == true) {
//				IsWaterFloatLandingTrueChange = true;
//			}
		}
	}

//	[SerializeField]
//	bool isWaterFloatLandingTrueChange = false;
//	public bool IsWaterFloatLandingTrueChange {
//		get {
//			return isWaterFloatLandingTrueChange;
//		}
//		set {
//			isWaterFloatLandingTrueChange = value;
//		}
//	}

	[SerializeField] List<Collider> landColList = new List<Collider>();				// 接地しているオブジェクト
	public List<Collider> LandColList {
		get {
			return landColList;
		}
	}
	[SerializeField] List<Collider> landExtrusionColList = new List<Collider>();	// 押し出しによって接地しているオブジェクト

//	[SerializeField] bool upCollide = false;
//	[SerializeField] bool downCollide = false;
//	[SerializeField] bool leftCollide = false;
//	[SerializeField] bool rightCollide = false;

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

	FourSideCollider fourSideCol = null;
	FourSideCollider FourSideCol {
		get {
			if (fourSideCol == null) {
				fourSideCol = GetComponent<FourSideCollider>();
				if (fourSideCol == null) {
					Debug.LogError("FourSideColliderが見つかりませんでした。");
				}
			}
			return fourSideCol;
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

	WaterState waterStt = null;
	WaterState WaterStt {
		get {
			if (waterStt == null) {
				waterStt = GetComponent<WaterState>();
				if (waterStt == null) {
					Debug.LogError("WaterStateが見つかりませんでした。");
				}
			}
			return waterStt;
		}
	}

	PileWeight pile = null;
	PileWeight Pile {
		get {
			if (pile == null) {
				pile = GetComponent<PileWeight>();
				if (pile == null) {
					Debug.LogError("PileWeightが見つかりませんでした。");
				}
			}
			return pile;
		}
	}

	// 当たり判定を行うレイヤーマスク
	[SerializeField] LayerMask mask;
	[SerializeField] bool autoMask = true;

	void Awake() {
		if (autoMask) {
			mask = LayerMask.GetMask(new string[] { "Stage", "Player", "Box", "Fence" });
		}
	}

	void FixedUpdate() {
		if (IsLanding) {
			CheckLandingFalse();
			if (IsLanding) {
				MoveMng.StopMoveVirtical(MoveManager.MoveType.gravity);
				MoveMng.StopMoveVirtical(MoveManager.MoveType.prevMove);
			}
		}
//		if (IsExtrusionLanding) {
//			CheckExtrusionLandingFalse();
////			if (IsExtrusionLanding) {
////				MoveMng.StopMoveVirtical(MoveManager.MoveType.gravity);
////				MoveMng.StopMoveVirtical(MoveManager.MoveType.prevMove);
////			}
//		}

		UpdateWaterFloatLanding();
		if (!isExtrusionLandingChange) {
			IsExtrusionLanding = false;
		}
		isExtrusionLandingChange = false;
	}

	// 接触時にその接触が指定方向への接触かを判定
	public bool GetIsLanding(Vector3 _move) {
		if (_move == Vector3.zero) return false;

		//		// 接地方向
		//		float landVec = WeightMng.WeightForce;
		//		// 水中であり水に浮く重さなら
		//		if (WaterStt && WeightMng && WaterStt.IsInWater && !WaterStt.IsWaterSurface && (WeightMng.WeightLv <= WeightManager.Weight.light)) {
		//			landVec = 1.0f;
		//		}

		//		float dot = Vector3.Dot((Vector3.up * landVec).normalized, _move.normalized);
		if (!MoveMng) {
			return false;
		}
		float dot = Vector3.Dot((Vector3.up * MoveMng.GetFallVec()).normalized, _move.normalized);

		//		Debug.LogError(landingCol.localPosition + " " + _move + " " + (dot < 0.0f));

		// 指定方向の反対方向への接触
		if (dot < 0.0f) return false;

		// 指定方向への接触
		return true;
	}

	void CheckLandingFalse() {
		LandColList.Clear();

		//		// 接地方向を求める
		//		float landVec = -1.0f;
		//		// 宙に浮かぶ重さ、又は水中での水面に浮かぶ重さなら
		//		if ((WeightMng.WeightLv == WeightManager.Weight.flying) ||
		//			(WaterStt.IsInWater && WeightMng.WeightLv <= WeightManager.Weight.light)) {
		//			// 上方向に接地
		//			landVec = 1.0f;
		//		}

		if (!MoveMng) {
			return;
		}

		float landVec = MoveMng.GetFallVec();

		// 接地方向の逆方向に移動していれば接地していない
		if (landVec == -1.0f) {
			if (MoveMng.PrevMove.y > 0.0f) {
				IsLanding = false;
				return;
			}
		}
		else {
			if (MoveMng.PrevMove.y < 0.0f) {
				IsLanding = false;
				return;
			}
		}

		// 接地側の判定オブジェクトを取得
		if (landVec < 0.0f) {
			LandingCol = FourSideCol.BottomCol;
		}
		else {
			LandingCol = FourSideCol.TopCol;
		}

		// 離地判定
		LandColList.AddRange(Physics.OverlapBox(LandingCol.position, LandingCol.localScale * 0.5f, LandingCol.rotation, mask));

		// 自身は接地対象から除く
		for (int idx = LandColList.Count - 1; idx >= 0; idx--) {
			if (LandColList[idx].gameObject == gameObject) {
				LandColList.RemoveAt(idx);
			}
		}

		// 一致方向のすり抜け床の除外
		for (int idx = LandColList.Count - 1; idx >= 0; idx--) {
			OnewayFloor oneway = LandColList[idx].GetComponent<OnewayFloor>();
			if (MoveMng.PrevMove.y != 0.0f) {
				if (oneway && oneway.IsThrough(Vector3.up * MoveMng.PrevMove.y, gameObject)) {
					LandColList.RemoveAt(idx);
				}
			}
			else {
				if (oneway && oneway.IsThrough(Vector3.up * MoveMng.GetFallVec(), gameObject)) {
					LandColList.RemoveAt(idx);
				}
			}
		}

//		// 自身が上方向に移動する重さの際に、下方向に移動する重さオブジェクトを除外（MoveFloorは除外しない）
//		if (MoveMng.GetFallVec() == 1.0f) {
//			for (int idx = LandColList.Count - 1; idx >= 0; idx--) {
//				if (LandColList[idx].tag != "MoveFloor") {	// MoveFloorは除外しない
//					MoveManager landMoveMng = LandColList[idx].GetComponent<MoveManager>();
//					if (landMoveMng && (landMoveMng.GetFallVec() == -1.0f))
//						LandColList.RemoveAt(idx);
//				}
//			}
//		}

		//		// 自身が重さ1で水中にない時、水中の重さ0には着地できない
		//		if ((WeightMng.WeightLv == WeightManager.Weight.light) && !WaterStt.IsInWater) {
		//			for (int idx = LandColList.Count - 1; idx >= 0; idx--) {
		//				WeightManager colWeightMng = LandColList[idx].GetComponent<WeightManager>();
		//				WaterState colWaterStt = LandColList[idx].GetComponent<WaterState>();
		//				if (colWeightMng && colWaterStt && colWaterStt.IsInWater && (colWeightMng.WeightLv == WeightManager.Weight.flying)) {
		//					LandColList.RemoveAt(idx);
		//				}
		//			}
		//		}
		//
		//		// 自身が重さ1以上で水中にない時、水中の自身の重さ未満のオブジェクトには着地できない
		//		if ((WeightMng.WeightLv == WeightManager.Weight.heavy) && !WaterStt.IsInWater) {
		//			for (int idx = LandColList.Count - 1; idx >= 0; idx--) {
		//				WeightManager colWeightMng = LandColList[idx].GetComponent<WeightManager>();
		//				WaterState colWaterStt = LandColList[idx].GetComponent<WaterState>();
		//				if (colWeightMng && colWaterStt && colWaterStt.IsInWater && (colWeightMng.WeightLv < WeightMng.WeightLv)) {
		//					LandColList.RemoveAt(idx);
		//				}
		//			}
		//		}

		// 自身の重さ未満の浮いているオブジェクトには着地できない
		for (int idx = LandColList.Count -1;idx >= 0; idx--) {
			WeightManager colWeightMng = LandColList[idx].GetComponent<WeightManager>();
			Landing colLand = LandColList[idx].GetComponent<Landing>();
			if (WeightMng && colWeightMng && colLand &&
				(WeightMng.WeightLv > colWeightMng.WeightLv) && !colLand.IsLanding && !colLand.IsExtrusionLanding) {
				LandColList.RemoveAt(idx);
			}
		}

		// 自身にしか着地していない自身より軽いオブジェクトを除外
		List<Collider> thisOnlyLandList = new List<Collider>();
		for (int idx = LandColList.Count - 1; idx >= 0; idx--) {
			WeightManager colWeightMng = LandColList[idx].GetComponent<WeightManager>();
			Landing colLand = LandColList[idx].GetComponent<Landing>();
			if (colLand && WeightMng && colWeightMng && ((colLand.LandColList.Count == 1) && colLand.LandColList.Contains(MoveMng.UseCol)) && (WeightMng.WeightLv > colWeightMng.WeightLv)) {
				thisOnlyLandList.Add(LandColList[idx]);
			}
		}

		// 接地しているオブジェクトが存在しなければ離地
		if ((LandColList.Count - thisOnlyLandList.Count) == 0) {
			IsLanding = false;
			Debug.Log("離地 " + Support.ObjectInfoToString(gameObject));
		}
	}

	void CheckExtrusionLandingFalse() {
		//		// 接地方向の反対方向に移動していなければ接地していない
		//		if (WeightMng.WeightLv != WeightManager.Weight.flying) {
		//			if (MoveMng.PrevMove.y < 0.0f) {
		//				IsExtrusionLanding = false;
		//				return;
		//			}
		//		} else {
		//			if (MoveMng.PrevMove.y > 0.0f) {
		//				IsExtrusionLanding = false;
		//				return;
		//			}
		//		}

		// 接地方向に移動していれば反接地していない
		if (MoveMng && (MoveMng.GetFallVec() == Mathf.Sign(MoveMng.PrevMove.y)) && (MoveMng.PrevMove.y != 0.0f)) {
			IsExtrusionLanding = false;
			return;
		}

		// 反接地側の判定オブジェクトを取得
		Transform extLandingCol = null;
		if (MoveMng.GetFallVec() > 0.0f) {
			extLandingCol = FourSideCol.BottomCol;
		} else {
			extLandingCol = FourSideCol.TopCol;
		}

		// 離地判定
		landExtrusionColList.Clear();
		landExtrusionColList.AddRange(Physics.OverlapBox(extLandingCol.position, extLandingCol.localScale * 0.5f, extLandingCol.rotation, mask));

		// 自身は反接地対象から除く
		for (int idx = landExtrusionColList.Count - 1; idx >= 0; idx--) {
			if (landExtrusionColList[idx].gameObject == gameObject) {
				landExtrusionColList.RemoveAt(idx);
			}
		}

		// 反接地しているオブジェクトが存在しなければ離地
		if (landExtrusionColList.Count <= 0) {
			IsExtrusionLanding = false;
			Debug.Log("Ext離地");
		}
	}

	void UpdateWaterFloatLanding() {
		bool prevIsWaterFloatLanding = IsWaterFloatLanding;
		bool waterFloat = false;
		// 水中で無く、水面に浮かぶ重さである場合
		if (!WaterStt.IsInWater && (WeightMng.WeightLv == WeightManager.Weight.light)) {
			// 水面に浮かぶオブジェクトの上に積まれていればtrue
			List<Transform> underPileObjs = Pile.GetPileBoxList(Vector3.down);  // 自身が積まれているオブジェクト
//			Debug.LogWarning(name + " " + underPileObjs.Count);
			foreach (var underPileObj in underPileObjs) {
				// 水面に浮かぶオブジェクトが見つかればtrue
				WaterState underPileWaterStt = underPileObj.GetComponent<WaterState>();
				if (underPileWaterStt && underPileWaterStt.IsWaterSurface) {
					waterFloat = true;
					break;
				}
			}
		}
		IsWaterFloatLanding = waterFloat;

//		// 値変化時
//		if (prevIsWaterFloatLanding != IsWaterFloatLanding) {
//			IsWaterFloatLandingTrueChange = true;
//		}

//		Debug.LogWarning("UpdateWaterFloatLanding:" + IsWaterFloatLanding);
	}
}
