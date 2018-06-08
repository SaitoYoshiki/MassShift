using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	[SerializeField]
	bool canWalk = true;    // 左右移動可能フラグ
	public bool CanWalk {
		get {
			return canWalk;
		}
		set {
			canWalk = value;
		}
	}
	[SerializeField]
	bool canJump = true;    // ジャンプ可能フラグ
	public bool CanJump {
		get {
			return canJump;
		}
		set {
			canJump = value;
		}
	}
	[SerializeField]
	bool canShift = true;   // 重さ移し可能フラグ
	public bool CanShift {
		get {
			return canShift;
		}
		set {
			canShift = value;
		}
	}
	[SerializeField]
	bool canRotation = true;
	public bool CanRotation {
		get {
			return canRotation;
		}
		set {
			canRotation = value;
		}
	}
	[SerializeField]
	bool isShift = true;    // 重さ移し中フラグ
	public bool IsShift {
		get {
			return isShift;
		}
		set {
			isShift = value;
		}
	}
	public bool IsLanding {
		get {
			if (!Land) return false;
			return Land.IsLanding;
		}
	}
	[SerializeField]
	bool isRotation = false;
	bool IsRotation {
		get {
			return isRotation;
		}
		set {
			prevIsRotation = isRotation;
			isRotation = value;
		}
	}

	bool IsRotationChange {
		get {
			return (isRotation != prevIsRotation);
		}
	}
	bool prevIsRotation = false;
		
	[SerializeField]
	bool useManualJump = true;      // ボタン入力での通常ジャンプを使用する
	[SerializeField]
	bool useAutoClimbJump = true;   // 左右移動のみでの自動小ジャンプを使用する

	[SerializeField]
	List<float> walkWeightLvSpd = new List<float>(3);		// 左右移動速度
	float WalkSpd {
		get {
			return walkWeightLvSpd[(int)WeightMng.WeightLv];
		}
	}
	[SerializeField]
	List<float> walkWeightLvStop = new List<float>(3);	// 左右移動速度から停止
	float WalkStop {
		get {
			return walkWeightLvStop[(int)WeightMng.WeightLv];
		}
	}
	[SerializeField]
	List<float> jumpWeightLvSpd = new List<float>(3);		// ジャンプ中左右移動速度
	float JumpSpd {
		get {
			return jumpWeightLvSpd[(int)WeightMng.WeightLv];
		}
	}
	[SerializeField]
	List<float> jumpWeightLvStop = new List<float>(3);	// ジャンプ中左右移動から停止
	float JumpStop {
		get {
			return jumpWeightLvStop[(int)WeightMng.WeightLv];
		}
	}

	[SerializeField]
	List<float> jumpWeightLvHeight = new List<float>(3);    // ジャンプ力
	float JumpHeight {
		get {
			return jumpWeightLvHeight[(int)WeightMng.WeightLv];
		}
	}

	//	[SerializeField]
	//	List<float> jumpWeightLvDis;       // 最大ジャンプ距離
	//	float JumpDis {
	//		get {
	//			return jumpWeightLvDis[(int)WeightMng.WeightLv];
	//		}
	//	}

	//	[SerializeField]
	//	List<float> jumpWeightLvTime;      // 最大ジャンプ滞空時間
	//	float JumpTime {
	//		get {
	//			return jumpWeightLvTime[(int)WeightMng.WeightLv];
	//		}
	//	}

	[SerializeField]
	float walkStandbyVec = 0.0f;    // 移動しようとしている方向
	[SerializeField]
	bool jumpStandbyFlg = false;   // ジャンプしようとしているフラグ
	bool prevJumpStandbyFlg = false;
	//	float jumpLimitTime = 0.0f;						// 次回ジャンプ可能時間

	[SerializeField]
	float remainJumpTime = 0.0f;
	[SerializeField]
	bool prevIsInWater = false;

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

	Landing land = null;
	Landing Land {
		get {
			if (land == null) {
				land = GetComponent<Landing>();
				if (land == null) {
					Debug.LogError("Landingが見つかりませんでした。");
				}
			}
			return land;
		}
	}

	Lifting lift = null;
	Lifting Lift {
		get {
			if (!lift) {
				lift = GetComponent<Lifting>();
				if (!lift) {
					Debug.LogError("Liftingが見つかりませんでした。");
				}
			}
			return lift;
		}
	}
	bool liftTrg = false;

	WaterState waterStt = null;
	WaterState WaterStt {
		get {
			if (waterStt == null) {
				waterStt = GetComponent<WaterState>();
			}
			return waterStt;
		}
	}

	PlayerAnimation plAnim = null;
	PlayerAnimation PlAnim {
		get {
			if (!plAnim) {
				plAnim = GetComponent<PlayerAnimation>();
				if (!plAnim) {
					Debug.LogError("PlayerAnimationが見つかりませんでした。");
				}
			}
			return plAnim;
		}
	}

	[SerializeField]
	Transform modelRotTransform = null;
	[SerializeField]
	Transform colRotTransform = null;
	[SerializeField]
	Vector3 rotVec = new Vector3(1.0f, 0.0f, 0.0f); // 左右向きと非接地面
	public Vector3 RotVec {
		get {
			return rotVec;
		}
		set {
			rotVec = value;
		}
	}
	[SerializeField]
	float rotSpd = 0.2f;
	[SerializeField]
	float turnRotBorderSpd = 1.0f;
	[SerializeField]
	float correctionaAngle = 1.0f;
	[SerializeField]
	float jumpStartOneTimeLimitSpd = 1.0f;

	[SerializeField]
	List<Transform> ClimbJumpWeightLvCollider = new List<Transform>(3);   // 自動ジャンプ当たり判定
	[SerializeField]
	List<Transform> ClimbJumpWeightLvInWaterCollider = new List<Transform>(3);   // 水中での自動ジャンプ当たり判定
	[SerializeField]
	LayerMask climbJumpMask;
	[SerializeField]
	bool autoClimbJumpMask = true;
	[SerializeField]
	List<float> ClimbJumpWeightLvHeight = new List<float>(3);
	[SerializeField]
	List<float> ClimbJumpWeightLvHeightInWater = new List<float>(3);

	// purfabを編集しないでもいい様に
	[SerializeField]
	bool testAutoSetParam = true;

	void Awake() {
		if (autoClimbJumpMask) climbJumpMask = LayerMask.GetMask(new string[] { "Stage", "Box", "Fence" });

		//test
		if (walkWeightLvSpd == null || walkWeightLvSpd.Count == 0) {
			Debug.LogWarning(
				"TestAutoSetParamがtrueです。\n" +
				"walkWeightLvSpd,, walkWeightLvStopTime, jumpWeightLvSpdを仮設定します。");

			walkWeightLvSpd = new List<float>();
			walkWeightLvSpd.Add(0.1f);
			walkWeightLvSpd.Add(0.1f);
			walkWeightLvSpd.Add(0.1f);

			walkWeightLvStop = new List<float>();
			walkWeightLvStop.Add(1.0f);
			walkWeightLvStop.Add(1.0f);
			walkWeightLvStop.Add(1.0f);

			jumpWeightLvSpd = new List<float>();
			jumpWeightLvSpd.Add(0.1f);
			jumpWeightLvSpd.Add(0.1f);
			jumpWeightLvSpd.Add(0.1f);

			jumpWeightLvStop = new List<float>();
			jumpWeightLvStop.Add(1.0f);
			jumpWeightLvStop.Add(1.0f);
			jumpWeightLvStop.Add(1.0f);
		}
		//test
	}

	void Update() {
		// 左右移動入力
		walkStandbyVec = Input.GetAxis("Horizontal");

		// ジャンプ入力
		jumpStandbyFlg |= (Input.GetAxis("Jump") != 0.0f);

		// ジャンプ滞空時間
		remainJumpTime = (!Land.IsLanding ? remainJumpTime + Time.deltaTime : 0.0f);

		// 持ち上げ/下げ
		if ((Land.IsLanding || WaterStt.IsWaterSurface) && !IsRotation) {
			if ((Input.GetAxis("Lift") != 0.0f)) {
				//if (!liftTrg) {
				Lift.Lift();

				//}
				//	liftTrg = true;
				//} else {
				//	liftTrg = false;
			}
		}
	}

	void FixedUpdate() {
		// 持ち下ろしアニメーション中以外なら
		if (!Lift.IsLiftStop) {
			// 左右移動
			Walk();
		}

		// ジャンプ
		bool isJump = Jump();
		prevJumpStandbyFlg = jumpStandbyFlg;
		jumpStandbyFlg = false;

		// 立ち止まり
		WalkDown();

		// 左右上下回転
		Rotate();

		// 自動ジャンプ
		ClimbJump();

		// 上下回転
		if (Land.IsLandingChange || Land.IsWaterFloatLandingChange ||
			((WaterStt.IsInWater != prevIsInWater) && WeightMng.WeightLv == WeightManager.Weight.light)) {
			prevIsInWater = WaterStt.IsInWater;

			// 必要なら回転アニメーション
			float nowRotVec = RotVec.y;
			float landRotVec = 0.0f;

			if (MoveMng.PrevMove.y > 0.0f) {
				if ((WeightMng.WeightLv == WeightManager.Weight.flying) ||
				((WeightMng.WeightLv == WeightManager.Weight.light) && (WaterStt.IsInWater))) {
					landRotVec = 1.0f;
				}
			}

			// 回転アニメーション
			if (nowRotVec != landRotVec) {
				RotVec = new Vector3(RotVec.x, landRotVec, RotVec.z);
			}
		}

		// 着地アニメーション
		if ((Land.IsLanding && Land.IsLandingChange) ||
			(Land.IsWaterFloatLanding && Land.IsWaterFloatLandingChange)) {
			Land.IsLandingChange = false;
			Land.IsWaterFloatLandingChange = false;
			if (!Lift.IsLifting) {
				PlAnim.StartLand();
			} else {
				PlAnim.StartHoldLand();
			}
		}

//		// 着水アニメーション
//		if (WaterStt.IsWaterSurfaceChange) {
//			WaterStt.IsWaterSurfaceChange = false;
//
//			if (WaterStt.IsWaterSurface) {
//				if (!Lift.IsLifting) {
//					PlAnim.StartSwim();
//				} else {
//					PlAnim.StartHoldSwim();
//				}
//			}
//		}

		// 落下アニメーション
		if (!Land.IsLanding && Land.IsLandingChange) {
			Land.IsLandingChange = false;
			if (!isJump) {
				if (!Lift.IsLifting) {
					PlAnim.StartFall();
				} else {
					PlAnim.StartHoldFall();
				}
			}
		}
	}

	void Walk() {
		// 歩行アニメーション
		if ((walkStandbyVec != 0.0f) && CanWalk) {
			if (!WaterStt.IsWaterSurface) {
				if (!Lift.IsLifting) {
					PlAnim.StartWalk();
				} else {
					PlAnim.StartHoldWalk();
				}
				PlAnim.SetSpeed(Mathf.Abs(walkStandbyVec));
			}
			// 泳ぎアニメーション
			else {
				if (!Lift.IsLifting) {
					PlAnim.StartSwim();
				} else {
					PlAnim.StartHoldSwim();
				}
			}
		}
		// 待機アニメーション
		else {
			if (!Lift.IsLifting) {
				PlAnim.StartStandBy();
			} else {
				PlAnim.StartHoldStandBy();
			}
		}

		// 左右移動入力があれば
		if (walkStandbyVec == 0.0f) return;

		// 左右移動可能でなければ
		if (!canWalk) return;

		// 地上なら
		if (Land.IsLanding) {
			// 左右方向へ加速
			MoveMng.AddMove(new Vector3(walkStandbyVec * WalkSpd, 0.0f, 0.0f));
		}
		// 空中なら
		else {
			// 左右方向へ加速
			MoveMng.AddMove(new Vector3(walkStandbyVec * JumpSpd, 0.0f, 0.0f));
		}
	}
	bool Jump() {
		if (!useManualJump) return false;

		// ジャンプ入力(トリガー)がなければ
		if (!jumpStandbyFlg || prevJumpStandbyFlg) return false;

		// ジャンプ可能でなければ
		if (!canJump) return false;

		// ステージに接地、又は水面で安定していなければ
		//		Debug.LogWarning("IsLanding:" + Land.IsLanding);
		//if (!Land.IsLanding && !WaterStt.IsWaterSurface) {
		if (!(Land.IsLanding || WaterStt.IsWaterSurface)) {
			PileWeight pile = GetComponent<PileWeight>();
			// 接地、又は安定しているオブジェクトにも接地していなければ
			List<Transform> pileObjs = pile.GetPileBoxList(new Vector3(0.0f, MoveMng.GravityForce, 0.0f));
			bool stagePile = false;
			foreach (var pileObj in pileObjs) {
				Landing pileLand = pileObj.GetComponent<Landing>();
				WaterState pileWaterStt = pileObj.GetComponent<WaterState>();
				//				WaterState pileWaterStt = pileObj.GetComponent<WaterState>();
				//				if ((pileLand && (pileLand.IsLanding || pileLand.IsExtrusionLanding)) || (pileWaterStt && (pileWaterStt.IsWaterSurface))) {
				if ((pileLand && (pileLand.IsLanding || pileLand.IsExtrusionLanding || pileWaterStt.IsWaterSurface))) {
					stagePile = true;
					break;
				}
			}
			if ((pileObjs.Count == 0) || !stagePile) {
				// ジャンプ不可
				return false;
			}
		}

		// ジャンプ直後であれば
		//		if (jumpLimitTime > Time.time) return;

		Debug.Log("Jump");

		// ジャンプアニメーション
		if (!Lift.IsLifting) {
			PlAnim.StartJump();
		} else {
			PlAnim.StartHoldJump();
		}

		// 前回までの上下方向の加速度を削除
		MoveMng.StopMoveVirtical(MoveManager.MoveType.prevMove);

		// 左右方向の加速度を削除
		//		MoveMng.StopMoveHorizontalAll();

		// 左右方向の移動量も一更新だけ制限
//		MoveMng.OneTimeMaxSpd = jumpStartOneTimeLimitSpd;

		// 上方向へ加速
		//float jumpGravityForce = (0.5f * Mathf.Pow(jumpTime * 0.5f, 2) + jumpHeight);	// ジャンプ中の重力加速度
		//		float jumpGravityForce = -100;   // ジャンプ中の重力加速度

		//		MoveMng.AddMove(new Vector3(0.0f, (-jumpGravityForce * JumpTime * 0.5f), 0.0f));
		//		Debug.Log(jumpGravityForce);

		MoveMng.AddMove(new Vector3(0.0f, (JumpHeight), 0.0f));

		// 離地
		Land.IsLanding = false;
		WaterStt.IsWaterSurface = false;
		WaterStt.BeginWaterStopIgnore();

		// ジャンプ入力を無効化
		jumpStandbyFlg = false;

		// 通常の重力加速度を一時的に無効
		//MoveMng.GravityCustomTime = (Time.time + JumpTime);
		//MoveMng.GravityForce = jumpGravityForce;

		// 次回ジャンプ可能時間を設定
		//		jumpLimitTime = Time.time + jumpTime * 0.5f;	// ジャンプしてからジャンプ滞空時間の半分の時間まではジャンプ不可

		return true;
	}
	void WalkDown() {
		// 進行方向側への左右入力があれば
		if ((walkStandbyVec != 0.0f) && (Mathf.Sign(MoveMng.PrevMove.x) == Mathf.Sign(walkStandbyVec))) return;

		// 接地中、または水上での安定状態、安定状態オブジェクトへの接地であれば
		if (Land.IsLanding || WaterStt.IsWaterSurface || Land.IsWaterFloatLanding) {
			// 減速
			float moveX = (Mathf.Min((WalkSpd / WalkStop), Mathf.Abs(MoveMng.PrevMove.x))) * -Mathf.Sign(MoveMng.PrevMove.x);
			MoveMng.AddMove(new Vector3(moveX, 0.0f, 0.0f));
		}
		// 空中であれば
		else if (!WaterStt.IsInWater && !WaterStt.IsWaterSurface) {
			// 空中での減速
			float moveX = (Mathf.Min((JumpSpd / JumpStop), Mathf.Abs(MoveMng.PrevMove.x))) * -Mathf.Sign(MoveMng.PrevMove.x);
			MoveMng.AddMove(new Vector3(moveX, 0.0f, 0.0f));
		}
	}

	void Rotate() {
		if (!CanRotation) {
			IsRotation = false;
			return;
		}

		//		// 持ち上げモーション中は処理しない
		//		if ((Lift.St == Lifting.LiftState.invalid) ||
		//			(Lift.St == Lifting.LiftState.standby)) {
		//		// 接地中なら
		//		if (Land.IsLanding || WaterStt.IsWaterSurface) {
		// 左右入力中なら
		if (walkStandbyVec != 0.0f) {
			// 一定の移動がある方向に向きを設定
			if (MoveMng.PrevMove.x > turnRotBorderSpd) {
				RotVec = new Vector3(1.0f, RotVec.y, RotVec.z);
			} else if (MoveMng.PrevMove.x < -turnRotBorderSpd) {
				RotVec = new Vector3(-1.0f, RotVec.y, RotVec.z);
			} else {
				// 移動量が一定以下なら入力方向に向く
				if (walkStandbyVec > 0.0f) {
					RotVec = new Vector3(1.0f, RotVec.y, RotVec.z);
				} else if (walkStandbyVec < 0.0f) {
					RotVec = new Vector3(-1.0f, RotVec.y, RotVec.z);
				}
			}
		}
		//		}

//		RotVec = new Vector3(RotVec.x, 0.0f, RotVec.z);
//		// 接地方向によって向きを設定
//		if (WeightMng.WeightLv == WeightManager.Weight.flying) {
//			RotVec = new Vector3(RotVec.x, 1.0f, RotVec.z);
//		}
//		if ((WeightMng.WeightLv == WeightManager.Weight.light) &&
//			(WaterStt.IsInWater && false)) {
//			RotVec = new Vector3(RotVec.x, 1.0f, RotVec.z); ;
//		}


//		// 自身が重さ0であり、重さ2のブロックを持ち上げている場合
//		if (WeightMng.WeightLv == WeightManager.Weight.flying) {
//			if (Lift && Lift.LiftObj) {
//				WeightManager liftWeightMng = Lift.LiftObj.GetComponent<WeightManager>();
//				if (liftWeightMng && (liftWeightMng.WeightLv == WeightManager.Weight.heavy)) {
//					// 接地変更を指定
//					RotVec.y = 0.0f;
//				}
//			}
//		}

		// 結果の姿勢を求める
		Quaternion qt = Quaternion.Euler(RotVec.y * 180.0f, -90.0f + RotVec.x * 90.0f, 0.0f);

		// 現在の向きと結果の向きとの角度が一定以内なら
		float angle = Quaternion.Angle(modelRotTransform.rotation, qt);
		if (angle < correctionaAngle) {
			// 向きを合わせる
			modelRotTransform.rotation = Quaternion.Lerp(modelRotTransform.rotation, qt, 1);
			IsRotation = false;

			// 自身が重さ0であり、重さ2のブロックを持ち上げている場合
			if (WeightMng.WeightLv == WeightManager.Weight.flying) {
				if (Lift && Lift.LiftObj) {
					WeightManager liftWeightMng = Lift.LiftObj.GetComponent<WeightManager>();
					if (liftWeightMng && (liftWeightMng.WeightLv == WeightManager.Weight.heavy)) {
						// 強制的に持っているブロックを離す
						Lift.LiftDownFailed();
						WeightMng.LiftWeightMng = null;
					}
				}
			}
		}
		// 角度が一定以上なら
		else {
			// 設定された向きにスラープ
			modelRotTransform.rotation = Quaternion.Slerp(modelRotTransform.rotation, qt, rotSpd);
			IsRotation = true;
		}

		colRotTransform.rotation = modelRotTransform.rotation;

		// 重さに合わせてモデルと当たり判定位置を補正
		Lift.CorrectFourSideCollider(Lift.IsLifting);
	}
	//	}

	void ClimbJump() {
		if (!useAutoClimbJump) return;

		// 持ち上げ中や持ち上げ入力があれば処理しない
		if (Lift.IsLifting || (Input.GetAxis("Lift") != 0.0f)) return;

		// ジャンプ可能でなければ処理しない
		if (!canJump) return;

		// 接地中でも水上安定中でもなく、水上安定中ブロックに乗ってもいなければ処理しない
		if (!Land.IsLanding && !WaterStt.IsWaterSurface && !Land.IsWaterFloatLanding) return;

		// 左右への移動入力がなければ処理しない
		if (walkStandbyVec == 0.0f) return;

		// 自動ジャンプ判定がなければ処理しない
		if ((ClimbJumpWeightLvCollider == null) || (ClimbJumpWeightLvCollider.Count <= (int)WeightMng.WeightLv) || (ClimbJumpWeightLvCollider[(int)WeightMng.WeightLv] == null) ||
			(ClimbJumpWeightLvInWaterCollider == null) || (ClimbJumpWeightLvInWaterCollider.Count <= (int)WeightMng.WeightLv) || (ClimbJumpWeightLvInWaterCollider[(int)WeightMng.WeightLv] == null)) return;

		Transform climbJumpCol;
		if (!WaterStt.IsInWater) {
			climbJumpCol = ClimbJumpWeightLvCollider[(int)WeightMng.WeightLv];
		} else {
			climbJumpCol = ClimbJumpWeightLvInWaterCollider[(int)WeightMng.WeightLv];
		}

		// 自動ジャンプ判定内に対象オブジェクトがなければ処理しない
		if (Physics.OverlapBox(climbJumpCol.transform.position,
			climbJumpCol.lossyScale * 0.5f,
			climbJumpCol.rotation, climbJumpMask).Length <= 0) return;

		// 現在の向きと移動入力方向が異なれば処理しない
		if (Vector3.Dot((climbJumpCol.position - transform.position), (Vector3.right * walkStandbyVec)) <= 0.0f) return;

		// ジャンプアニメーション
		if (!Lift.IsLifting) {
			PlAnim.StartJump();
		} else {
			PlAnim.StartHoldJump();
		}

		// 前回までの上下方向の加速度を削除
		MoveMng.StopMoveVirtical(MoveManager.MoveType.prevMove);

		// 左右方向の移動量も一更新だけ制限
		MoveMng.OneTimeMaxSpd = jumpStartOneTimeLimitSpd;

		// 上方向へ加速
		if (!WaterStt.IsInWater) {
			MoveMng.AddMove(new Vector3(0.0f, ClimbJumpWeightLvHeight[(int)WeightMng.WeightLv], 0.0f));
		}
		// 水中
		else {
			MoveMng.AddMove(new Vector3(0.0f, ClimbJumpWeightLvHeightInWater[(int)WeightMng.WeightLv], 0.0f));
		}

		// 離地
		Land.IsLanding = false;
		WaterStt.IsWaterSurface = false;
		WaterStt.BeginWaterStopIgnore();
	}
}
