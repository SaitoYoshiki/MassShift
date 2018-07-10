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
	bool canRotation = true;	// 設定された方向に合わせるように回転する処理が行えるか
	public bool CanRotation {
		get {
			return canRotation;
		}
		set {
			canRotation = value;
		}
	}
	[SerializeField]
	bool canRotationTurn = true;	// 向く方向を設定する処理が行えるか
	public bool CanRotationTurn {
		get {
			return canRotationTurn;
		}
		set {
			canRotationTurn = value;
		}
	}
	[SerializeField]
	bool isShiftLastRead = true;    // 重さ移し中フラグ
	public bool IsShift {
		get {
			isShiftLastRead = Mass.IsShift;
			return isShiftLastRead;
		}
		//set {
		//	isShiftLastRead = value;
		//}
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

	[SerializeField]
	MassShift mass = null;
	MassShift Mass {
		get {
			if (!mass) {
				mass = (MassShift)FindObjectOfType(typeof(MassShift));
			}
			return mass;
		}
	}

	bool IsRotationChange {
		get {
			return (isRotation != prevIsRotation);
		}
	}
	bool prevIsRotation = false;
	bool PrevIsRotation {
		get {
			return prevIsRotation;
		}
	}

	[SerializeField]
	bool isHandSpring = false;
	public bool IsHandSpring {
		get {
			return isHandSpring;
		}
		set {
			isHandSpring = value;
		}
	}

//	[SerializeField]
//	bool isLiftMove = false;
//	public bool IsLiftMove {
//		get {
//			return (Lift && Lift.IsLifting && Lift.St == Lifting.LiftState.lifting);
//		}
//	}

	[SerializeField]
	bool isMoveByWind = false;
	public bool IsMoveByWind {
		get {
			return isMoveByWind;
		}
		set {
			isMoveByWind = value;
		}
	}

	[SerializeField]
	bool isWaterSurfaceStandby = false;
	bool IsWaterSurfaceStandby {
		get {
			return isWaterSurfaceStandby;
		}
		set {
			isWaterSurfaceStandby = value;
		}
	}
	bool prevIsWaterSurfaceStandby = false;
	[SerializeField]
	bool isWaterSurfaceSwiming = false;
	public bool IsWaterSurfaceSwiming {
		get {
			return isWaterSurfaceSwiming;
		}
		private set {
			isWaterSurfaceSwiming = value;
		}
	}
	bool prevIsWaterSurfaceSwiming = false;

	GameObject swimSoundInstance = null;
	GameObject SwimSoundInstance {
		get {
			if (!swimSoundInstance) {
				swimSoundInstance = SoundManager.SPlay(swimSE);
			}
			return swimSoundInstance;
		}
	}
	AudioSource swimAudioSource = null;
	AudioSource SwimAudioSource {
		get {
			if (!swimAudioSource) {
				swimAudioSource = SwimSoundInstance.GetComponent<AudioSource>();
				SwimAudioSource.volume = 0.0f;
			}
			return swimAudioSource;
		}
	}
	[SerializeField]
	float swimSoundNowVol = 0.0f;
	[SerializeField]
	float swimSoundMinVol = 0.0f;
	[SerializeField]
	float swimSoundMaxVol = 0.5f;
	[SerializeField]
	float swimSoundVolUpSpd = 0.05f;
	[SerializeField]
	float swimSoundVolDownSpd = 0.05f;

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
	[SerializeField]
	bool prevJumpStandbyFlg = false;
	//	float jumpLimitTime = 0.0f;						// 次回ジャンプ可能時間

	[SerializeField]
	float remainJumpTime = 0.0f;
	[SerializeField]
	bool prevIsInWater = false;
	[SerializeField]
	bool prevIsLanding = false;
	[SerializeField]
	bool prevIsExtrusionLanding = false;
	[SerializeField]
	bool prevIsWaterFloatLanding = false;
	[SerializeField]
	bool prevFallFlg = false;
	[SerializeField]
	bool prevFlyFlg = false;

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
	PileWeight pile = null;
	PileWeight Pile {
		get {
			if (!pile) {
				pile = GetComponent<PileWeight>();
			}
			return pile;
		}
	}

	[SerializeField]
	Transform rotTransform = null;
	[SerializeField]
	Transform colRotTransform = null;
	[SerializeField]
	Transform modelTransform = null;
	[SerializeField]
	Transform cameraLookTransform = null;

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

	#region 自動ジャンプ
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
	#endregion

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
	public bool IsHandSpringWeit {
		get {
			return (HandSpringEndTime > Time.time);
		}
	}
	[SerializeField]
	List<float> handSpringJumpWeightLvHeight = new List<float>(3);    // ジャンプ天井回転力
	float HandSpringJumpHeight {
		get {
			return handSpringJumpWeightLvHeight[(int)WeightMng.WeightLv];
		}
	}
	[SerializeField]
	List<float> handSpringJumpWeightLvHeightInWater = new List<float>(3); // 水中での天井回転ジャンプ力
	float HandSpringJumpHeightInWater {
		get {
			return handSpringJumpWeightLvHeightInWater[(int)WeightMng.WeightLv];
		}
	}

	[SerializeField]
	float cameraLookRatio = 1.0f;			// カメラの方を向いている比率
	public float CameraLookRatio {
		get {
			return cameraLookRatio;
		}
		set {
			cameraLookRatio = value;
		}
	}
	[SerializeField]
	float cameraLookRatioSpd = 0.05f;		// 待機時のカメラの方を向く割合の変化量
	[SerializeField]
	float cameraLookCancelRatioSpd = 0.05f;	// 待機時のカメラの方を向く状態を解除する速さ
	[SerializeField]
	float cameraLookMaxAngle = 35.0f;		// 待機時のカメラの方を向く最大角度
	[SerializeField]
	float cameraLookBorderSpd = 0.1f;		// カメラの方を向くようになる移動速度

	[SerializeField]
	float walkAnimRatio = 0.5f;		// 歩きアニメーションの再生速度倍率
	[SerializeField]
	float walkAnimMinSpd = 0.3f;    // 歩きアニメーションの最低再生速度

	[SerializeField]
	GameObject swimSE;
	[SerializeField]
	GameObject jumpSE;
	[SerializeField]
	GameObject handSpringJumpSE;
	[SerializeField]
	float jumpSEDeray = 0.2f;
	[SerializeField]
	float handSpringJumpSEDeray = 0.2f;
	[SerializeField]
	float lightLandAnimSpd = 3.0f;
	[SerializeField]
	float heavyLandAnimSpd = 1.0f;
	[SerializeField]
	bool isHeavyReleaseRotate = false;
	[SerializeField]
	bool liftInputFlg = false;
	[SerializeField]
	bool jumpReserveInput = true;
	[SerializeField]
	bool isSandwitch = false;
	public bool IsSandwitch {
		get {
			return isSandwitch;
		}
		private set {
			isSandwitch = value;
		}
	}
	[SerializeField]
	bool prevIsSandwitch = false;
	[SerializeField]
	List<Collider> sandwitchCols = null;
	public List<Collider> SandwitchCols {
		get {
			return sandwitchCols;
		}
		private set {
			sandwitchCols = value;
		}
	}

	FourSideCollider fourSideCol = null;
	FourSideCollider FourSideCol {
		get {
			if (!fourSideCol) {
				fourSideCol = GetComponent<FourSideCollider>();
			}
			return fourSideCol;
		}
	}
	Transform TopCol {
		get {
			if (!FourSideCol) return null;
			return FourSideCol.TopCol;
		}
	}
	Transform BottomCol {
		get {
			if (!FourSideCol) return null;
			return FourSideCol.BottomCol;
		}
	}
	[SerializeField]
	bool autoSandwitchMask = true;
	LayerMask sandwitchMask;

	[SerializeField]
	Transform noFlyAnimCol = null;
	[SerializeField]
	LayerMask noFlyAnimColMask;
	[SerializeField]
	Transform WaterCol = null;
	LayerMask waterAreaMask;

	void Start() {
		if (autoClimbJumpMask) climbJumpMask = LayerMask.GetMask(new string[] { "Stage", "Box", "Fence" });
		cameraLookTransform.localRotation = Quaternion.Euler(new Vector3(cameraLookTransform.localRotation.eulerAngles.x, (cameraLookMaxAngle * CameraLookRatio), cameraLookTransform.localRotation.eulerAngles.z));
		if (autoSandwitchMask) sandwitchMask = LayerMask.GetMask(new string[] { "Stage", "Box", "Fence" });
		waterAreaMask = LayerMask.GetMask("WaterArea");
	}

	void Update() {
		// 左右移動入力
		//		walkStandbyVec = Input.GetAxis("Horizontal");
		walkStandbyVec = VirtualController.GetAxis(VirtualController.CtrlCode.Horizontal);

		// ジャンプ入力
		//		jumpStandbyFlg |= (Input.GetAxis("Jump") != 0.0f);
		//		jumpStandbyFlg |= (VirtualController.GetAxis(VirtualController.CtrlCode.Jump) != 0.0f);
		jumpStandbyFlg = (VirtualController.GetAxis(VirtualController.CtrlCode.Jump) != 0.0f);

		// ジャンプ滞空時間
		remainJumpTime = (!Land.IsLanding ? remainJumpTime + Time.deltaTime : 0.0f);

		// 持ち上げ/下げ入力
		if (VirtualController.GetAxis(VirtualController.CtrlCode.Lift) != 0.0f) {
			liftInputFlg = true;
		}
	}

	void FixedUpdate() {
		// 持ち上げ/下げ
		if (liftInputFlg) {
			liftInputFlg = false;
			if ((Land.IsLanding || WaterStt.IsWaterSurface || land.IsWaterFloatLanding) /*&& !IsRotation*/ && !IsHandSpring) {
				//			if ((Input.GetAxis("Lift") != 0.0f)) {
				//if (!liftTrg) {
				if (!(IsRotation && Lift.St == Lifting.LiftState.standby)) {
					Lift.Lift();
				}
				//}
				//	liftTrg = true;
				//} else {
				//	liftTrg = false;
			}
		}

		// 浮かびアニメーション
		bool flyFlg = false;
		// 水中以外で落下方向と体の上下向きが逆の場合
		if ((!WaterStt.IsInWater && !WaterStt.IsWaterSurface) && (Mathf.Sign(RotVec.y * 2 - 1) != MoveMng.GetFallVec())) {
			// 落下方向に移動していれば
			if (MoveMng.GetFallVec() == Mathf.Sign(MoveMng.PrevMove.y)) {
				// 浮かびアニメーションを行わないコライダーに足場や水場が触れていない
				if (Physics.OverlapBox(noFlyAnimCol.position, noFlyAnimCol.lossyScale * 0.5f, noFlyAnimCol.rotation, noFlyAnimColMask).Length == 0) {
					flyFlg = true;
					if (!prevFlyFlg) {
						Debug.Log("Fly");
						if (!Lift.IsLifting) {
							PlAnim.StartFly();
						} else {
							PlAnim.StartHoldFly();
						}
					}
				}
			}
		}
		prevFlyFlg = flyFlg;

		// 落下アニメーション
		bool fallFlg = ((!Land.IsLanding && !Land.IsWaterFloatLanding && !WaterStt.IsWaterSurface && !Land.IsExtrusionLanding) && (Mathf.Sign(MoveMng.PrevMove.y) == (MoveMng.GetFallVec())));
		// 持ち上げ/下しの最中は落下しない
		if (Lift.IsLiftCantMove) {
			fallFlg = false;
		}
		if (fallFlg && !prevFallFlg) {
			if (!flyFlg) {
				Debug.Log("Fall");
				if (!Lift.IsLifting) {
					PlAnim.StartFall();
				} else {
					PlAnim.StartHoldFall();
				}
			}
		}
		prevFallFlg = fallFlg;

		bool landTrueChangeFlg = ((land.IsLanding && (land.IsLanding != prevIsLanding)) ||
			(land.IsWaterFloatLanding && (land.IsWaterFloatLanding != prevIsWaterFloatLanding)));

		Collider[] topOverlapCols = Physics.OverlapBox(TopCol.position, TopCol.lossyScale * 0.5f, TopCol.rotation, sandwitchMask);
		Collider[] bottomOverlapCols = Physics.OverlapBox(BottomCol.position, TopCol.lossyScale * 0.5f, TopCol.rotation, sandwitchMask);
		bool topColOverlap = (topOverlapCols.Length > 0);
		bool bottomColOverlap = (bottomOverlapCols.Length > 0);
		IsSandwitch = (topColOverlap && bottomColOverlap);
		if (IsSandwitch) {
			SandwitchCols.Clear();
			SandwitchCols.AddRange(topOverlapCols);
			SandwitchCols.AddRange(bottomOverlapCols);
		}
		bool landColOverlap = false;
		if (MoveMng.GetFallVec() <= 0.0f) {
			landColOverlap = bottomColOverlap;
		}
		else {
			landColOverlap = topColOverlap;
		}

		// 着地時、入/出水時の戻り回転時
		//		if ((Land.IsLandingTrueChange || Land.IsWaterFloatLandingTrueChange) ||   // 着地時の判定
		if ((landTrueChangeFlg && !IsSandwitch) ||																					// 通常の着地時
			//(IsLanding && !prevIsExtrusionLanding && Land.IsExtrusionLanding) ||													// 上下を挟まれている時の落下方向変化時
			(prevIsSandwitch && !IsSandwitch && landColOverlap) ||																	// 上下を挟まれている状態から解放された時
			((WaterStt.IsInWater != prevIsInWater) && (WeightMng.WeightLv == WeightManager.Weight.light) && (RotVec.y != 0.0f))) {	// 反転したまま水上に落ちた時

			// 入水時の戻り回転なら天井回転アニメーションは行わない
			bool notHandSpring = (WaterStt.IsInWater != prevIsInWater);

			// 必要なら回転アニメーション
			float nowRotVec = RotVec.y;
			float landRotVec = 0.0f;

			//if (MoveMng.PrevMove.y > 0.0f) {
			if (MoveMng.GetFallVec() > 0.0f) {
				if ((WeightMng.WeightLv == WeightManager.Weight.flying) ||
				((WeightMng.WeightLv == WeightManager.Weight.light) && (WaterStt.IsInWater))) {
					landRotVec = 1.0f;
				}
			}

			if (nowRotVec != landRotVec) {
				RotVec = new Vector3(RotVec.x, landRotVec, RotVec.z);

				if (!notHandSpring) {
					// 天井回転アニメーション
					if (!Lift.IsLifting) {
						PlAnim.StartHandSpring();
					}
					else {
						PlAnim.StartHoldHandSpring();
					}
					HandSpringEndTime = (Time.time + handSpringWeitTime);
					IsHandSpring = true;
				}
			}
		}

		// 左右移動
		Walk();

		// ジャンプ
		if (!Land.noticeLandEffect) {
			if (jumpReserveInput) {
				if (Jump()) {
					prevJumpStandbyFlg = true;
				}
				if (!jumpStandbyFlg) {
					prevJumpStandbyFlg = false;
				}
			} else {
				Jump();
				prevJumpStandbyFlg = jumpStandbyFlg;
			}
		}

		// 立ち止まり
		WalkDown();

		// 左右上下回転
		Rotate();

		// 自動ジャンプ
		ClimbJump();

		// 着地アニメーション
		//		if ((Land.IsLanding && Land.IsLandingTrueChange) ||
		//			(Land.IsWaterFloatLanding && Land.IsWaterFloatLandingTrueChange)) {
		//if (landTrueChangeFlg) {
		if ((landTrueChangeFlg || Land.IsExtrusionLanding) && !PlAnim.IsLandOnlyAnim) {
			//			Land.IsLandingTrueChange = false;
			//			Land.IsWaterFloatLandingTrueChange = false;
			if (!Lift.IsLifting) {
				PlAnim.StartLand();
			} else {
				PlAnim.StartHoldLand();
			}
			if (WeightMng.WeightLv <= WeightManager.Weight.light) {
				PlAnim.SetLandSpeed(lightLandAnimSpd);
			} else {
				PlAnim.SetLandSpeed(heavyLandAnimSpd);
			}
		}

		// 出水時アニメーション
		if (WaterStt.IsWaterSurfaceChange) {
			WaterStt.IsWaterSurfaceChange = false;
			// 着水解除時
			if (!WaterStt.IsWaterSurface) {
				// 落下の場合
				if (WeightMng.WeightLv == WeightManager.Weight.heavy) {
					// 落下アニメーションに遷移
					if (!Lift.IsLifting) {
						PlAnim.StartFall();
					} else {
						PlAnim.StartHoldFall();
					}
				}
				// 浮遊の場合
				else if (WeightMng.WeightLv == WeightManager.Weight.flying) {
					// 浮遊アニメーションに遷移
					if (!Lift.IsLifting) {
						PlAnim.StartFly();
					} else {
						PlAnim.StartHoldFly();
					}
				}
				// ジャンプでの出水の場合
				else {
					// ジャンプアニメーションに遷移
					if (!Lift.IsLifting) {
						PlAnim.StartJump();
					} else {
						PlAnim.StartHoldJump();
					}
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
		UpdateLookCamera();
		LookCamera();

		// 泳ぎサウンドの音量を変更
		if (IsWaterSurfaceSwiming) {
			SwimAudioSource.volume += swimSoundVolUpSpd;
		} else {
			SwimAudioSource.volume -= swimSoundVolDownSpd;
		}
		SwimAudioSource.volume = Mathf.Clamp(SwimAudioSource.volume, swimSoundMinVol, swimSoundMaxVol);

		prevIsLanding = Land.IsLanding;
		prevIsExtrusionLanding = Land.IsExtrusionLanding;
		prevIsWaterFloatLanding = Land.IsWaterFloatLanding;
		prevIsInWater = WaterStt.IsInWater;
		prevIsSandwitch = isSandwitch;
	}

	void Walk() {
		prevIsWaterSurfaceStandby = IsWaterSurfaceStandby;
		prevIsWaterSurfaceSwiming = IsWaterSurfaceSwiming;
		IsWaterSurfaceStandby = false;
		IsWaterSurfaceSwiming = false;

		// 持ち下ろしアニメーション中、または天井回転待ち中であれば処理しない
		if (Lift.IsLiftCantMove || IsHandSpringWeit) return;

		// 重さ0のプレイヤーが重さ2の持ち上げオブジェクトを離す時の回転中も処理しない
		if (isHeavyReleaseRotate) return;

		// 歩行アニメーション
		if ((walkStandbyVec != 0.0f) && CanWalk) {
			if (!WaterStt.IsWaterSurface) {
				if (Land.IsLanding || Land.IsExtrusionLanding || Land.IsWaterFloatLanding) {
					if (!Lift.IsLifting) {
						PlAnim.StartWalk();
					} else {
						PlAnim.StartHoldWalk();
					}
				}
				float walkAnimSpd = Mathf.Max((Mathf.Abs(MoveMng.PrevMove.x) * walkAnimRatio), walkAnimMinSpd);
				//float walkAnimSpd = (walkStandbyVec * walkAnimRatio);
				PlAnim.SetSpeed(walkAnimSpd);
			}
			// 泳ぎアニメーション
			else {
				IsWaterSurfaceSwiming = true;
				if (IsWaterSurfaceSwiming && !prevIsWaterSurfaceSwiming) {
					if (!Lift.IsLifting) {
						PlAnim.StartSwim();
					} else {
						PlAnim.StartHoldSwim();
					}
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
				IsWaterSurfaceStandby = true;
				if (IsWaterSurfaceStandby && !prevIsWaterSurfaceStandby) {
					if (!Lift.IsLifting) {
						PlAnim.StartWaterStandBy();
					} else {
						PlAnim.StartHoldWaterStandBy();
					}
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
		if (IsHandSpringWeit) {
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
		// 天井反転中なら
		if (IsHandSpring) {
			return false;
		}

		// ステージに接地、又は水面で安定していなければ
		//		Debug.LogWarning("IsLanding:" + Land.IsLanding);
		//if (!Land.IsLanding && !WaterStt.IsWaterSurface) {
		if (!(Land.IsLanding || WaterStt.IsWaterSurface)) {
			// 接地、又は安定しているオブジェクトにも接地していなければ
			List<Transform> pileObjs = Pile.GetPileBoxList(new Vector3(0.0f, MoveMng.GravityForce, 0.0f));
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

		// 水面の場合
		if (WaterStt.IsWaterSurface) {
			// 自身の上にオブジェクトが乗っていればジャンプできない
			if (Pile.GetPileBoxList(Vector3.up).Count > 0) {
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

		if (!WaterStt.IsWaterSurface) {
			// サウンド再生
			SoundManager.SPlay(jumpSE, jumpSEDeray);
		}

		// 前回までの上下方向の加速度を削除
		MoveMng.StopMoveVirtical(MoveManager.MoveType.prevMove);

		// 左右方向の移動量をジャンプ中速度まで下げる
		MoveMng.PrevMove = new Vector3(Mathf.Clamp(MoveMng.PrevMove.x, -JumpSpd, JumpSpd), MoveMng.PrevMove.y, MoveMng.PrevMove.z);

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

		//// ジャンプ入力を無効化
		//prevJumpStandbyFlg = jumpStandbyFlg;
		//jumpStandbyFlg = false;

		// 通常の重力加速度を一時的に無効
		//MoveMng.GravityCustomTime = (Time.time + JumpTime);
		//MoveMng.GravityForce = jumpGravityForce;

		// 次回ジャンプ可能時間を設定
		//		jumpLimitTime = Time.time + jumpTime * 0.5f;	// ジャンプしてからジャンプ滞空時間の半分の時間まではジャンプ不可

		return true;
	}
	void WalkDown() {
		// 進行方向側への左右入力があるか、接地した際の天井回転待ち状態なら
		if (((walkStandbyVec != 0.0f) && (Mathf.Sign(MoveMng.PrevMove.x) == Mathf.Sign(walkStandbyVec))) && !IsHandSpringWeit) return;

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
		if (IsHandSpringWeit) {
			IsRotation = true;
			IsHandSpring = true;
			return;
		} else {
			// 回転待ち終了時
			if (IsHandSpring) {
				IsHandSpring = false;

				// 離地方向に跳ねる
				HandSpringJump();
			}
		}

		//		// 持ち上げモーション中は処理しない
		//		if ((Lift.St == Lifting.LiftState.invalid) ||
		//			(Lift.St == Lifting.LiftState.standby)) {
		//		// 接地中なら
		//		if (Land.IsLanding || WaterStt.IsWaterSurface) {
		if (CanRotationTurn) {
			// 左右入力中なら
			if (walkStandbyVec != 0.0f) {
				// 一定の移動がある方向に向きを設定
				if (MoveMng.PrevMove.x > turnRotBorderSpd) {
					RotVec = new Vector3(1.0f, RotVec.y, RotVec.z);
				}
				else if (MoveMng.PrevMove.x < -turnRotBorderSpd) {
					RotVec = new Vector3(-1.0f, RotVec.y, RotVec.z);
				}
				else {
					// 移動量が一定以下なら入力方向に向く
					if (walkStandbyVec > 0.0f) {
						RotVec = new Vector3(1.0f, RotVec.y, RotVec.z);
					}
					else if (walkStandbyVec < 0.0f) {
						RotVec = new Vector3(-1.0f, RotVec.y, RotVec.z);
					}
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


		// 自身が重さ0であり、重さ2のブロックを持ち上げている場合
		if (WeightMng.WeightLv == WeightManager.Weight.flying) {
			if (Lift && Lift.LiftObj) {
				WeightManager liftWeightMng = Lift.LiftObj.GetComponent<WeightManager>();
				if (liftWeightMng && (liftWeightMng.WeightLv == WeightManager.Weight.heavy)) {
					if (!isHeavyReleaseRotate) {
						// 接地変更を指定
						RotVec = new Vector3(RotVec.x, 1.0f, RotVec.z);

						isHeavyReleaseRotate = true;
						MoveMng.StopMoveHorizontal(MoveManager.MoveType.prevMove);
					}
				}
			}
		}

		// 結果の姿勢を求める
		Quaternion qt = Quaternion.Euler(RotVec.y * 180.0f, -90.0f + RotVec.x * 90.0f, 0.0f);

		// 現在の向きと結果の向きとの角度が一定以内なら
		float angle = Quaternion.Angle(rotTransform.rotation, qt);
		if (angle < correctionaAngle) {
			// 向きを合わせる
			rotTransform.rotation = Quaternion.Lerp(rotTransform.rotation, qt, 1);

			// 回転終了
			IsRotation = false;

			isHeavyReleaseRotate = false;

			// 自身が重さ0であり、重さ2のブロックを持ち上げている場合
			if (WeightMng.WeightLv == WeightManager.Weight.flying) {
				if (Lift && Lift.LiftObj && (Lift.St == Lifting.LiftState.lifting)) {
					WeightManager liftWeightMng = Lift.LiftObj.GetComponent<WeightManager>();
					if (liftWeightMng && (liftWeightMng.WeightLv == WeightManager.Weight.heavy)) {
						WeightMng.HeavyRot = true;

						// 持っているブロックを離す
						Debug.Log("強制離し");
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

	void UpdateLookCamera() {
		// 接地状態で待機状態なら少しカメラ方向を向く
		if ((Land.IsLanding || land.IsWaterFloatLanding || WaterStt.IsWaterSurface) &&
			!(!Lift.IsLifting && Lift.LiftObj) &&	// 持ち上げ/下ろしの最中ならfalse
			(Mathf.Abs(MoveMng.TotalMove.magnitude) <= cameraLookBorderSpd)) {
			CameraLookRatio += (cameraLookRatioSpd * RotVec.x * -(RotVec.y * 2 - 1));
		}
		// 移動があればキャラクター進行方向を向く
		else {
			float defSign = Mathf.Sign(CameraLookRatio);
			CameraLookRatio -= (cameraLookCancelRatioSpd * Mathf.Sign(CameraLookRatio));
			if (defSign != Mathf.Sign(CameraLookRatio)) {
				CameraLookRatio = 0.0f;
			}
		}
		CameraLookRatio = Mathf.Clamp(CameraLookRatio, -1.0f, 1.0f);
	}
	public void LookCamera() {
		// モデルの向きを設定
		cameraLookTransform.localRotation = Quaternion.Euler(new Vector3(cameraLookTransform.localRotation.eulerAngles.x, (cameraLookMaxAngle * CameraLookRatio), cameraLookTransform.localRotation.eulerAngles.z));
		//		Debug.LogWarning(modelTransform.rotation.eulerAngles + " " + modelTransform.name);
	}


	void HandSpringJump() {
		// 前回までの上下方向の加速度を削除
		MoveMng.StopMoveVirtical(MoveManager.MoveType.prevMove);

		// 左右方向の移動量をジャンプ中速度まで下げる
		MoveMng.PrevMove = new Vector3(Mathf.Sign(MoveMng.PrevMove.x) * Mathf.Clamp(MoveMng.PrevMove.x, -JumpSpd, JumpSpd), MoveMng.PrevMove.y, MoveMng.PrevMove.z);

		// 離地方向に跳ねる
		if (!WaterStt.IsInWater) {
			MoveMng.AddMove(new Vector3(0.0f, (HandSpringJumpHeight) * -(RotVec.y * 2.0f - 1.0f), 0.0f));
		} else {
			MoveMng.AddMove(new Vector3(0.0f, (HandSpringJumpHeightInWater) * -(RotVec.y * 2.0f - 1.0f), 0.0f));
		}

		// 離地
		Land.IsLanding = false;
		WaterStt.IsWaterSurface = false;
		WaterStt.BeginWaterStopIgnore();

		// サウンド再生
		SoundManager.SPlay(handSpringJumpSE, handSpringJumpSEDeray);
	}

	public void InitRotation() {
		Debug.Log("InitRotation");

		// 重さを設定
		WeightMng.WeightLv = WeightManager.Weight.flying;

		// 接地方向の回転を設定
		RotVec = new Vector3(RotVec.x, 1.0f, RotVec.z);

		// 着地状態
		Land.IsLanding = true;

		// 位置を補正
		transform.position += (Vector3.up * 0.495f); 

		// 姿勢を更新
		rotTransform.rotation = Quaternion.Euler(RotVec.y * 180.0f, -90.0f + RotVec.x * 90.0f, 0.0f);

		// 離地判定コライダーを設定
		Land.LandingCol = GetComponent<FourSideCollider>().TopCol;

		// カメラの反対を向いているので反転
		CameraLookRatio *= -1;

		// 前回更新で少し上に移動していた事にする
		MoveMng.PrevMove = Vector3.up * float.Epsilon;
	}
}
