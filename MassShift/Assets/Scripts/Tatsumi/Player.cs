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
	public bool IsRotation {
		get {
			return isRotation;
		}
		private set {
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
	bool isHandSpring = false;
	bool IsHandSpring {
		get {
			return isHandSpring;
		}
		set {
			isHandSpring = value;
		}
	}
		
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

	[SerializeField]
	List<float> jumpWeightLvHeightInWater = new List<float>(3);	// 水中でのジャンプ力
	float JumpHeightInWater {
		get {
			return jumpWeightLvHeightInWater[(int)WeightMng.WeightLv];
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
	[SerializeField]
	bool prevIsLanding = false;
	[SerializeField]
	bool prevIsWaterFloatLanding = false;
	[SerializeField]
	bool prevFallFlg = false;

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
	Transform rotTransform = null;
	[SerializeField]
	Transform colRotTransform = null;
	[SerializeField]
	Transform modelTransform = null;

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

	[SerializeField]
	float handSpringWeitTime = 0.2f;
	float handSpringWeitEndTime = 0.0f; 
	float HandSpringEndTime {
		get {
			return handSpringWeitEndTime;
		}
		set {
			handSpringWeitEndTime = value;
		}
	}
	[SerializeField]
	bool HandSpringWeitFlg {
		get {
			return (HandSpringEndTime > Time.time);
		}
	}

	[SerializeField]
	float cameraLookRatio = 0.0f;			// カメラの方を向いている比率
	[SerializeField]
	float cameraLookRatioSpd = 0.01f;		// 待機時のカメラの方を向く割合の変化量
	[SerializeField]
	float cameraLookCancelRatioSpd = 0.01f;	// 待機時のカメラの方を向く状態を解除する速さ
	[SerializeField]
	float cameraLookMaxRatio = 0.3f;		// 待機時のカメラの方を向く最大比率
	[SerializeField]
	float cameraLookBorderSpd = 0.1f;       // カメラの方を向くようになる移動速度

	[SerializeField]
	float walkAnimRatio = 0.5f;		// 歩きアニメーションの再生速度倍率
	[SerializeField]
	float walkAnimMinSpd = 0.3f;	// 歩きアニメーションの最低再生速度

	void Awake() {
		if (autoClimbJumpMask) climbJumpMask = LayerMask.GetMask(new string[] { "Stage", "Box", "Fence" });
	}

	void Update() {
		// 左右移動入力
//		walkStandbyVec = Input.GetAxis("Horizontal");
		walkStandbyVec = VirtualController.GetAxis(VirtualController.CtrlCode.Horizontal);

		// ジャンプ入力
//		jumpStandbyFlg |= (Input.GetAxis("Jump") != 0.0f);
		jumpStandbyFlg |= (VirtualController.GetAxis(VirtualController.CtrlCode.Jump) != 0.0f);

		// ジャンプ滞空時間
		remainJumpTime = (!Land.IsLanding ? remainJumpTime + Time.deltaTime : 0.0f);

		// 持ち上げ/下げ
		if ((Land.IsLanding || WaterStt.IsWaterSurface) && !IsRotation) {
//			if ((Input.GetAxis("Lift") != 0.0f)) {
			if ((VirtualController.GetAxis(VirtualController.CtrlCode.Lift) != 0.0f)) {
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
		// 落下アニメーション
		bool fallFlg = ((!Land.IsLanding && !Land.IsWaterFloatLanding && !WaterStt.IsWaterSurface) && (MoveMng.PrevMove.y < 0.0f));
		if (fallFlg && !prevFallFlg) {
			Debug.Log("Fall");
			if (!Lift.IsLifting) {
				PlAnim.StartFall();
			}
			else {
				PlAnim.StartHoldFall();
			}
		}
		prevFallFlg = fallFlg;

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

		bool landTrueChangeFlg = ((land.IsLanding && (land.IsLanding != prevIsLanding)) ||
			(land.IsWaterFloatLanding && (land.IsWaterFloatLanding != prevIsWaterFloatLanding)));
		prevIsLanding = land.IsLanding;
		prevIsWaterFloatLanding = land.IsWaterFloatLanding;

		// 着地時、または入/出水時の戻り回転時
		//		if ((Land.IsLandingTrueChange || Land.IsWaterFloatLandingTrueChange) ||   // 着地時の判定
		if (landTrueChangeFlg || ((WaterStt.IsInWater != prevIsInWater) && (WeightMng.WeightLv == WeightManager.Weight.light) && (RotVec.y != 0.0f))) {

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
				PlAnim.StartHandSpring();
				RotVec = new Vector3(RotVec.x, landRotVec, RotVec.z);
				HandSpringEndTime = (Time.time + handSpringWeitTime);
				IsHandSpring = true;
			}
		}

		// 着地アニメーション
		//		if ((Land.IsLanding && Land.IsLandingTrueChange) ||
		//			(Land.IsWaterFloatLanding && Land.IsWaterFloatLandingTrueChange)) {
		if (landTrueChangeFlg) {
//			Land.IsLandingTrueChange = false;
//			Land.IsWaterFloatLandingTrueChange = false;
			if (!Lift.IsLifting) {
				PlAnim.StartLand();
			} else {
				PlAnim.StartHoldLand();
			}
		}

		// 着水アニメーション
		if (WaterStt.IsWaterSurfaceChange) {
			WaterStt.IsWaterSurfaceChange = false;

			if (WaterStt.IsWaterSurface) {
				if (!Lift.IsLifting) {
//					PlAnim.StartLand();
				} else {
//					PlAnim.StartHoldLand();
				}
			}
		}

//		if ((!Land.IsLanding && Land.IsLandingTrueChange) && (!Land.IsWaterFloatLanding && Land.IsWaterFloatLandingTrueChange)) {
//			Land.IsLandingTrueChange = false;
//			if (!isJump) {
//				if (!Lift.IsLifting) {
//					PlAnim.StartFall();
//				} else {
//					PlAnim.StartHoldFall();
//				}
//			}
//		}

		// 待機時に少しカメラ方向を向く
		LookCamera();
	}

	void Walk() {
		// 歩行アニメーション
		if ((walkStandbyVec != 0.0f) && CanWalk) {
			if (!WaterStt.IsWaterSurface) {
				if (Land.IsLanding || Land.IsExtrusionLanding) {
					if (!Lift.IsLifting) {
						PlAnim.StartWalk();
					} else {
						PlAnim.StartHoldWalk();
					}
				}
				float walkAnimSpd = Mathf.Max((Mathf.Abs(MoveMng.PrevMove.x) * walkAnimRatio), walkAnimMinSpd);
				PlAnim.SetSpeed(walkAnimSpd);
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
			if (!WaterStt.IsWaterSurface) {
				if (!Lift.IsLifting) {
					PlAnim.StartStandBy();
				} else {
					PlAnim.StartHoldStandBy();
				}
			} else {
				if (!Lift.IsLifting) {
					PlAnim.StartWaterStandBy();
				} else {
					PlAnim.StartHoldWaterStandBy();
				}
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
		if (!useManualJump) {
			return false;
		}
		// ジャンプ入力(トリガー)がなければ
		if (!jumpStandbyFlg || prevJumpStandbyFlg) {
			return false;
		}
		// ジャンプ可能でなければ
		if (!canJump) {
			return false;
		}
		// ステージに接地、又は水面で安定していなければ
		//		Debug.LogWarning("IsLanding:" + Land.IsLanding);
		//if (!Land.IsLanding && !WaterStt.IsWaterSurface) {
		if (!(Land.IsLanding || WaterStt.IsWaterSurface)) {
			PileWeight pile = GetComponent<PileWeight>();
			// 接地、又は安定しているオブジェクトにも接地していなけ	れば
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

		// 左右方向の移動量をジャンプ中速度まで下げる
		MoveMng.PrevMove = new Vector3(Mathf.Sign(MoveMng.PrevMove.x) * Mathf.Clamp(MoveMng.PrevMove.x, -JumpSpd, JumpSpd), MoveMng.PrevMove.y, MoveMng.PrevMove.z);

		// 左右方向の加速度を削除
		//		MoveMng.StopMoveHorizontalAll();

		// 左右方向の移動量も一更新だけ制限
		//		MoveMng.OneTimeMaxSpd = jumpStartOneTimeLimitSpd;

		// 上方向へ加速
		//float jumpGravityForce = (0.5f * Mathf.Pow(jumpTime * 0.5f, 2) + jumpHeight);	// ジャンプ中の重力加速度
		//		float jumpGravityForce = -100;   // ジャンプ中の重力加速度

		//		MoveMng.AddMove(new Vector3(0.0f, (-jumpGravityForce * JumpTime * 0.5f), 0.0f));
		//		Debug.Log(jumpGravityForce);

		// 離地方向に移動
		if (!WaterStt.IsInWater || WaterStt.IsWaterSurface) {
//			Debug.LogWarning("landJump");
			MoveMng.AddMove(new Vector3(0.0f, (JumpHeight), 0.0f));
		} else {
//			Debug.LogWarning("inWaterJump");
			MoveMng.AddMove(new Vector3(0.0f, (JumpHeightInWater), 0.0f));
		}

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

		// 回転待ち
		if (HandSpringEndTime > Time.time) {
			IsRotation = true;
			IsHandSpring = true;
			return;
		}else {
			IsHandSpring = false;
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
		float angle = Quaternion.Angle(rotTransform.rotation, qt);
		if (angle < correctionaAngle) {
			// 向きを合わせる
			rotTransform.rotation = Quaternion.Lerp(rotTransform.rotation, qt, 1);

			// 回転終了
			IsRotation = false;

			// 自身が重さ0であり、重さ2のブロックを持ち上げている場合
			if (WeightMng.WeightLv == WeightManager.Weight.flying) {
				if (Lift && Lift.LiftObj) {
					WeightManager liftWeightMng = Lift.LiftObj.GetComponent<WeightManager>();
					if (liftWeightMng && (liftWeightMng.WeightLv == WeightManager.Weight.heavy)) {
						// 持っているブロックを離す
						//						Lift.ReleaseLiftObject();
						Lift.LiftDownEnd();

						// アニメーション遷移
						PlAnim.ExitRelease();
					}
				}
			}
		}
		// 角度が一定以上なら
		else {
			// 設定された向きにスラープ
			rotTransform.rotation = Quaternion.Slerp(rotTransform.rotation, qt, rotSpd);
			IsRotation = true;
		}

		colRotTransform.rotation = rotTransform.rotation;

		// 重さに合わせてモデルと当たり判定位置を補正
		Lift.CorrectFourSideCollider(Lift.IsLifting);
	}
	//	}

	void ClimbJump() {
		if (!useAutoClimbJump) return;

		// 持ち上げ中や持ち上げ入力があれば処理しない
		//		if (Lift.IsLifting || (Input.GetAxis("Lift") != 0.0f)) return;
		if (Lift.IsLifting || (VirtualController.GetAxis(VirtualController.CtrlCode.Lift) != 0.0f)) return;

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


	void LookCamera() {
		// 接地状態で移動がなければ少しカメラ方向を向く
		if ((Land.IsLanding || land.IsWaterFloatLanding || WaterStt.IsWaterSurface) &&
			(Mathf.Abs(MoveMng.TotalMove.magnitude) <= cameraLookBorderSpd)) {
			cameraLookRatio += cameraLookRatioSpd;
		}
		// 移動があればキャラクター進行方向を向く
		else {
			cameraLookRatio -= cameraLookCancelRatioSpd;
		}
		cameraLookRatio = Mathf.Clamp(cameraLookRatio, 0.0f, cameraLookMaxRatio);

		// モデルの向きを設定
//		modelTransform.rotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(0.0f, 180.0f, 0.0f), cameraLookRatio);
//		Debug.LogWarning(modelTransform.rotation.eulerAngles);

		//test
//		modelTransform.rotation = modelTransform.rotation * Quaternion.Euler(new Vector3(0.0f, 100.0f, 0.0f));
	}
}
