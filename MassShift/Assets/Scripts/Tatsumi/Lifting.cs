using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifting : MonoBehaviour {
	public enum LiftState {
		invalid,
		standby,
		liftUp,
		liftUpFailed,
		lifting,
		liftDown,
		liftDownFailed,
	}

	[SerializeField] Transform liftPoint = null;    // 持ち上げ位置
	[SerializeField] Transform liftUpCol = null;    // 持ち上げ可能判定
	[SerializeField] GameObject liftObj = null;     // 持ち上げ中オブジェクト
	public GameObject LiftObj {
		get {
			return liftObj;
		}
		set {
			liftObj = value;
			if (liftObj) {
				liftObjModel = liftObj.transform.Find("Model").Find("Rotation");
			} else {
				liftObjModel = null;
			}
		}
	}
	[SerializeField]
	Transform liftObjModel = null;
	Transform LiftObjModel {
		get {
			return liftObjModel;
		}
	}
	MoveManager LiftObjMoveMng {
		get {
			return LiftObj.GetComponent<MoveManager>();
		}
	}

	[SerializeField] Collider standbyCol = null;    // 非持ち上げ中の本体当たり判定
	[SerializeField] Collider liftingCol = null;    // 持ち上げ中の本体当たり判定
	//[SerializeField] float colCenterPoint = 0.75f;		// 本体当たり判定の中心位置
	//[SerializeField] float stdLiftingColPoint = 1.0f;	// 接地方向が通常時の持ち上げ中の本体当たり判定の位置
	//[SerializeField] float revLiftingColPoint = 0.0f;	// 接地方向が逆の時の持ち上げ中の本体当たり判定の位置
	//[SerializeField] float liftObjMaxDisX = 0.9f;       // 持ち上げ時にx軸距離がこれ以上離れないように補正
	[SerializeField] bool afterHoldInput = false;		// 持ち上げ/下ろし後にそのまま入力を続けている
	[SerializeField] LiftState st;	
	public LiftState St {
		get {
			return st;
		}
		set {
			st = value;
		}
	}
	bool heavyFailedFlg = false;

	public bool IsLifting {
		get {
			return (St == LiftState.lifting);
		}
	}
	public bool IsLiftCantMove {
		get {
			return ((St == LiftState.liftUp) || (St == LiftState.liftUpFailed) || (St == LiftState.liftDown) || (St == LiftState.liftDownFailed));
		}
	}

	[SerializeField] Player pl = null;
	public Player Pl {
		get {
			if (!pl) {
				pl = GetComponent<Player>();
				if (!pl) {
					Debug.LogError("Playerが見つかりませんでした。");
				}
			}
			return pl;
		}
	}

	[SerializeField] PlayerAnimation plAnim = null;
	public PlayerAnimation PlAnim {
		get {
			if (plAnim == null) {
				plAnim = GetComponent<PlayerAnimation>();
				if (plAnim == null) {
					Debug.LogError("PlayerAnimationが見つかりませんでした。");
				}
			}
			return plAnim;
		}
		set {
			plAnim = value;
		}
	}

	MoveManager moveMng = null;
	MoveManager MoveMng {
		get {
			if (!moveMng) {
				moveMng = GetComponent<MoveManager>();
				if (!moveMng) {
					Debug.LogError("MoveManagerが見つかりませんでした。");
				}
			}
			return moveMng;
		}
	}

	[SerializeField] bool autoLiftingColMask = true;
	[SerializeField] LayerMask liftingColMask;
	[SerializeField] bool autoBoxMask = true;
	[SerializeField] LayerMask boxMask;

	[SerializeField]
	Transform offsetTransform = null;
	[SerializeField]
	Transform rotationTransform = null;
	[SerializeField]
	Transform modelTransform = null;
	[SerializeField]
	Transform cameraLookTransform = null;
	[SerializeField]
	Transform fourSideTransform = null;
	[SerializeField]
	Transform fourSideTopColTransform = null;

	[SerializeField]
	float downOffsetPos = -0.75f;
	[SerializeField]
	float downRotationPos = 0.5f;
	[SerializeField]
	float downModelPos = -0.25f;
	[SerializeField]
	float downFourSidePos = -0.5f;
	[SerializeField]
	float upStdOffsetPos = 0.375f;
	[SerializeField]
	float upRevOffsetPos = -1.125f;
	[SerializeField]
	float upRotationPos = 0.0f;
	[SerializeField]
	float upModelPos = -0.875f;
	[SerializeField]
	float upStdFourSidePos = -0.5f;
	[SerializeField]
	float upRevFourSidePos = -0.75f;
	[SerializeField]
	float liftingPosOffset = -1.25f;
	[SerializeField]
	float upTopColPos = 0.5f;
	[SerializeField]
	float downTopColPos = 0.5f;

	[SerializeField]
	bool canHeavyLift = true;   // 自身より重いモノでも持てるフラグ、現状(2080609)の仕様では常にtrue

	[SerializeField]
	WaterState liftWaterStt = null;

	[SerializeField]
	GameObject liftSE;
	[SerializeField]
	float liftUpSoundDeray = 0.2f;
	[SerializeField]
	float liftDownSoundDeray = 0.2f;

	[SerializeField]
	bool isPosUp = false;
	bool failedPosUpFlg = false;

	WeightManager weightMng = null;
	WeightManager WeightMng {
		get {
			if (!weightMng) {
				weightMng = GetComponent<WeightManager>();
			}
			return weightMng;
		}
	}

	WaterState waterStt = null;
	WaterState WaterStt {
		get {
			if (!waterStt) {
				waterStt = GetComponent<WaterState>();
			}
			return waterStt;
		}
	}

	[SerializeField]
	bool liftDownWeit = false;

	void Awake() {
		if (autoLiftingColMask) liftingColMask = LayerMask.GetMask(new string[] { "Stage", "Box", "Fence" });
		if (autoBoxMask) boxMask = LayerMask.GetMask(new string[] { "Box" });
	}

	void Start() {
		if (!liftPoint) {
			Debug.LogError("LiftPointが設定されていません。");
			enabled = false;
		}

		if (!offsetTransform) {
			Debug.LogError("OffsetTransformが設定されていません。");
		}
		if (!modelTransform) {
			Debug.LogError("ModelTransformが設定されていません。");
		}
		if (!offsetTransform) {
			Debug.LogError("OffsetTransformが設定されていません。");
		}
		if (!fourSideTransform) {
			Debug.LogError("FourSideTransformが設定されていません。");
		}
	}

	void Update() {
		// 入力がなければ
		if (Input.GetAxis("Lift") == 0.0f) {
			// 処理後状態を解除
			afterHoldInput = false;
		}
	}

	void FixedUpdate() {
		UpdateLifting();
	}

	void UpdateLifting() {
		switch (St) {
		case LiftState.liftUp:
			// 移動不可
			MoveMng.StopMoveVirticalAll();
			MoveMng.StopMoveHorizontalAll();

			// 持ち上げ中オブジェクトを動かさない
			LiftObjMoveMng.StopMoveVirticalAll();
			LiftObjMoveMng.StopMoveHorizontalAll();

			// 持つオブジェクトの補間位置が現在のオブジェクトより高ければ
			bool liftMoveFlg = false;
			float landVec = MoveMng.GravityForce;
			liftWaterStt = LiftObj.GetComponent<WaterState>();
			WeightManager liftWeightMng = LiftObj.GetComponent<WeightManager>();
			// 水中で水に浮く重さなら上方向に接地
			if (liftWaterStt && liftWeightMng && liftWaterStt.IsInWater && !liftWaterStt.IsWaterSurface && liftWeightMng.WeightLv <= WeightManager.Weight.light) {
				landVec = 1.0f;
			}
			if (landVec < 0.0f) {   // 接地方向が下
				if (PlAnim.GetBoxPosition().y > LiftObj.transform.position.y) {
					liftMoveFlg = true;
				}
			} else {                // 接地方向が上
				if (PlAnim.GetBoxPosition().y < LiftObj.transform.position.y) {
					liftMoveFlg = true;
				}
			}

			// オブジェクトの位置を同期
			if (liftMoveFlg) {
				if (heavyFailedFlg || (!MoveManager.Move(GetLiftUpBoxMove(), LiftObj.GetComponent<BoxCollider>(), liftingColMask, false, true))) {
					Debug.Log("持ち上げ失敗");

					//					// 持ち上げ中オブジェクトの強制押し出しフラグを戻す
					//					LiftObjMoveMng.enabled = false;

					// 持ち上げ移動中フラグをfalseに
					LiftObjMoveMng.IsLiftUpMove = false;

					// 対象をすり抜けオブジェクトに追加
					MoveMng.AddThroughCollider(LiftObj);

					// 同期できなければ下ろす
					St = LiftState.liftUpFailed;

					// 失敗アニメーションへの遷移
					PlAnim.FailedCatch();

					return;
				}

				// プレイヤーのモデルと同じ回転をオブジェクトに加える
				LiftObjModel.transform.rotation = cameraLookTransform.rotation;
			}

			// 持ち上げ完了時
			if (PlAnim.CompleteCatch()) {
				LiftUpEnd();
				//				// 持ち上げ中オブジェクトの判定と挙動を無効化
				//				LiftObj.GetComponent<BoxCollider>().enabled = false;
				//				LiftObj.GetComponent<MoveManager>().enabled = false;
				//
				//				// 持ち上げ中のプレイヤー当たり判定を有効化
				//				standbyCol.enabled = false;
				//				liftingCol.enabled = true;

				plAnim.ExitCatch();

				//				// 持ち上げ後処理状態
				//				St = LiftState.liftUpAfterHoldInput;
			}
			break;

		case LiftState.liftDown:
//			Debug.LogWarning(St.ToString() + " " + PlAnim.GetBoxPosition());

			// 移動不可
			MoveMng.StopMoveVirticalAll();
			MoveMng.StopMoveHorizontalAll();

			// 持ち上げ中オブジェクトを動かさない
			LiftObjMoveMng.StopMoveVirticalAll();
			LiftObjMoveMng.StopMoveHorizontalAll();

			// 回転終了待ち
			if (liftDownWeit) {
				if(Pl.CameraLookRatio == 0.0f) {
					liftDownWeit = false;

					// 持ち上げ中オブジェクトの判定を有効化
					LiftObj.GetComponent<BoxCollider>().enabled = true;

					// 下ろしアニメーションへの遷移
					PlAnim.StartRelease();

					// サウンド再生
					SoundManager.SPlay(liftSE, liftDownSoundDeray);
				}
			}
			else {
				// オブジェクトの位置を同期
				if (!MoveManager.MoveTo(GetLiftDownBoxPosition(), LiftObj.GetComponent<BoxCollider>(), liftingColMask)) {
					Debug.Log("下ろし失敗");

					if (Pl.RotVec.y == 1.0f) {
						Debug.LogWarning("下ろし失敗補正");
						failedPosUpFlg = true;
					}

					LiftDownEnd();

					// アニメーション遷移
					PlAnim.ExitRelease();

					return;
				}
			}

			// プレイヤーのモデルと同じ回転をオブジェクトに加える
			LiftObjModel.transform.rotation = cameraLookTransform.rotation;

			// 下ろし完了時
			if (!liftDownWeit && PlAnim.CompleteRelease()) {
				// オブジェクトを離す
				LiftDownEnd();

				//				// 持ち上げ中オブジェクトの判定と挙動を有効化
				//				LiftObj.GetComponent<BoxCollider>().enabled = true;
				//				LiftObj.GetComponent<MoveManager>().enabled = true;
				//
				//				// 持ち上げ中のプレイヤー当たり判定を無効化
				//				standbyCol.enabled = true;
				//				liftingCol.enabled = false;
				//
				//				// 持ち上げ中オブジェクトを下ろし切る
				//				MoveManager.MoveTo(liftPoint.position, LiftObj.GetComponent<BoxCollider>(), LayerMask.GetMask(new string[] { "" }));
				//				LiftObj = null;
				//
				//				// 下ろし処理後状態に
				//				St = LiftState.standby;
				//				afterHoldInput = true;

				// アニメーション遷移
				PlAnim.ExitRelease();
			}
			break;

		case LiftState.liftUpFailed:
			// 移動不可
			MoveMng.StopMoveVirticalAll();
			MoveMng.StopMoveHorizontalAll();

			// 持ち上げ中オブジェクトを動かさない
			LiftObjMoveMng.StopMoveVirticalAll();
			LiftObjMoveMng.StopMoveHorizontalAll();

			// オブジェクトの位置を同期
			if (!MoveManager.MoveTo(GetLiftDownBoxPosition(), LiftObj.GetComponent<BoxCollider>(), liftingColMask)) {
				Debug.Log("持ち上げ失敗に失敗");

				// オブジェクトを離す
				LiftDownEnd();

				// アニメーション遷移
				PlAnim.ExitRelease();

				return;
			}

			// プレイヤーのモデルと同じ回転をオブジェクトに加える
			LiftObjModel.transform.rotation = cameraLookTransform.rotation;

			// 移動不可
			MoveMng.StopMoveVirticalAll();
			MoveMng.StopMoveHorizontalAll();

			// 持ち上げ失敗完了時
			if (PlAnim.CompleteCatchFailed()) {
				// 持ち上げオブジェクトを離す
				LiftDownEnd();

				// アニメーション遷移
				PlAnim.ExitCatchFailed();
			}
			break;


/// 現状の仕様では下ろし失敗状態に遷移する事はない
//		case LiftState.liftDownFailed:
//			// 移動不可
//			MoveMng.StopMoveVirticalAll();
//			MoveMng.StopMoveHorizontalAll();
//
//			// オブジェクトの位置を同期
//			if (!MoveManager.MoveTo(liftPoint.position, LiftObj.GetComponent<BoxCollider>(), LayerMask.GetMask(new string[] { "Stage", "Box" }))) {
//				Debug.Log("下ろし失敗");
//
//				// 同期できなければ持ち上げる
//				LiftUp(LiftObj);
//				return;
//			}
//
//			// 下ろし完了時
//			if (PlAnim.CompleteRelease()) {
//				// 持ち上げ中オブジェクトの判定と挙動を有効化
//				LiftObj.GetComponent<BoxCollider>().enabled = true;
//				LiftObj.GetComponent<MoveManager>().enabled = true;
//
//				// 持ち上げ中のプレイヤー当たり判定を無効化
//				standbyCol.enabled = true;
//				liftingCol.enabled = false;
//
//				// 持ち上げ中オブジェクトを下ろし切る
//				MoveManager.MoveTo(liftPoint.position, LiftObj.GetComponent<BoxCollider>(), LayerMask.GetMask(new string[] { "" }));
//				LiftObj = null;
//			}
//			break;

		case LiftState.lifting:
			// オブジェクトの位置を同期
			//			MoveManager.MoveTo(PlAnim.GetBoxPosition(), LiftObj.GetComponent<BoxCollider>(), liftingColMask);
			LiftObj.transform.position = PlAnim.GetBoxPosition();

			// プレイヤーのモデルと同じ回転をオブジェクトに加える
			LiftObjModel.transform.rotation = cameraLookTransform.rotation;

			break;

		default:
			break;
		}
	}

	public GameObject Lift() {
		// 処理後状態なら入力が解除されるまで処理しない
		if (afterHoldInput) return null;

		Debug.Log("lift");
		switch (St) {
		case LiftState.standby:
			// 重さ変更中は処理しない
			if (Pl.IsShift) return null;

			// 範囲内で最も近い持ち上げられるオブジェクトを取得
			List<RaycastHit> hitInfos = new List<RaycastHit>();
			//			hitInfos.AddRange(Physics.BoxCastAll(transform.position, liftUpCol.localScale * 0.5f, (liftUpCol.position - transform.position),
			//				liftPoint.rotation, Vector3.Distance(transform.position, liftUpCol.position), boxMask));
			hitInfos.AddRange(Physics.BoxCastAll(liftUpCol.transform.position, liftUpCol.localScale * 0.5f, (transform.position - liftUpCol.position), liftPoint.rotation, float.Epsilon, boxMask));

			// 浮いているオブジェクトは持ち上げられない
			for (int idx = hitInfos.Count - 1; idx >= 0; idx--) {
				Landing hitInfoLand = hitInfos[idx].collider.GetComponent<Landing>();
				WaterState hitInfoWaterStt = hitInfos[idx].collider.GetComponent<WaterState>();
				if ((!hitInfoLand || !hitInfoWaterStt) || 
					(!hitInfoLand.IsLanding && !hitInfoLand.IsWaterFloatLanding && !hitInfoWaterStt.IsWaterSurface)) {
					hitInfos.RemoveAt(idx);
				}
			}

			GameObject liftableObj = null;
			float dis = float.MaxValue;
//			Debug.LogWarning(hitInfos.Count);
			foreach (var hitInfo in hitInfos) {
//				Debug.LogWarning(hitInfo.collider.name + " " + hitInfo.collider.tag);
				if ((hitInfo.collider.tag == "LiftableObject") && (hitInfo.distance < dis)) {
					liftableObj = hitInfo.collider.gameObject;
					dis = hitInfo.distance;
				}
			}

			// 持ち上げれるオブジェクトがあれば
			if (liftableObj != null) {
				if (!canHeavyLift) {
					// 重さがプレイヤーより重ければ失敗フラグを立てる
					heavyFailedFlg = (Pl.GetComponent<WeightManager>().WeightLv < liftableObj.GetComponent<WeightManager>().WeightLv);
				}

				// ジャンプ、重さ変更、振り向きを不可に
				Pl.CanJump = false;
				Pl.CanShift = false;
				Pl.CanRotationTurn = false;

				// 移動量を削除
				MoveMng.StopMoveVirticalAll();
				MoveMng.StopMoveHorizontalAll();

				// 持ち上げ中のオブジェクトが水で浮かないように
				WaterState liftWaterStt = liftableObj.GetComponent<WaterState>();
				liftWaterStt.CanFloat = false;

				// 持ち上げ開始
				return LiftUpStart(liftableObj);
			} else {
				// 持ち上げキャンセル
				LiftObj = null;
			}
			break;

		case LiftState.lifting:
			// 重さ変更中は処理しない
			if (Pl.IsShift) return null;

			//// 持ち上げ中オブジェクト以外のすり抜け中のオブジェクトがある場合は処理しない
			//foreach (var throughObj in MoveMng.ThroughObjList) {
			//	if (throughObj != LiftObj) {
			//		return null;
			//	}
			//}

			// ジャンプ、重さ変更、振り向きを不可に
			Pl.CanJump = false;
			Pl.CanShift = false;
			Pl.CanRotationTurn = false;

			// 下ろし始める
			LiftDownStart();

			// 移動量を削除
			MoveMng.StopMoveVirticalAll();
			MoveMng.StopMoveHorizontalAll();

			break;

		default:
			break;
		}
		return null;
	}
	
	GameObject LiftUpStart(GameObject _obj) {
		if (St == LiftState.standby) {
			Debug.Log("LiftUpStart:" + _obj.name);

			// 持ち上げ中オブジェクトの設定
			LiftObj = _obj;

			// 持ち上げアニメーションへの遷移
			PlAnim.StartCatch(_obj);

			// 状態の変更
			St = LiftState.liftUp;

			// 持ち上げ中オブジェクトを動かさない
			MoveManager liftMoveMng = LiftObj.GetComponent<MoveManager>();
			if (liftMoveMng) {
				liftMoveMng.StopMoveVirticalAll();
				liftMoveMng.StopMoveHorizontalAll();
			}

			//			// 持ち上げ中オブジェクトの強制押し出しフラグを一時的に有効化
			//			LiftObjMoveMng.ExtrusionForcible = true;

			// プレイヤーと持ち上げオブジェクトを風で動かされないように変更
			MoveMng.CanMoveByWind = false;
			LiftObjMoveMng.CanMoveByWind = false;

			// 持ち上げ移動中フラグをtrueに
			LiftObjMoveMng.IsLiftUpMove = true;

			// サウンド再生
			SoundManager.SPlay(liftSE, liftUpSoundDeray);

			return LiftObj;
		}

		return null;
	}

	GameObject LiftDownStart() {
		Debug.Log("LiftDownStart:" + LiftObj.name);

		// 状態の変更
		St = LiftState.liftDown;

		// プレイヤーと持ち上げオブジェクトを風で動かされないように変更
		MoveMng.CanMoveByWind = false;
		LiftObjMoveMng.CanMoveByWind = false;

		liftDownWeit = true;

		return LiftObj;
	}

	void LiftUpEnd() {
		Debug.Log("LiftUpEnd");

		//		// 持ち上げ中オブジェクトの強制押し出しフラグを戻す
		//		LiftObjMoveMng.ExtrusionForcible = false;

		// 持ち上げ中オブジェクトの判定と挙動を無効化
		LiftObj.GetComponent<BoxCollider>().enabled = false;
		LiftObj.GetComponent<MoveManager>().enabled = false;

		// プレイヤーと持ち上げオブジェクトを風で動かせるように変更
		MoveMng.CanMoveByWind = true;
		LiftObjMoveMng.CanMoveByWind = true;

		// 持ち上げ移動中フラグをfalseに
		LiftObjMoveMng.IsLiftUpMove = false;

		// プレイヤー当たり判定の設定
		SwitchLiftCollider(true);

		// 持ち上げ処理後状態に
		St = LiftState.lifting;
		afterHoldInput = true;

		// プレイヤーのジャンプ、振り向きを可能に
		Pl.CanJump = true;
		Pl.CanRotationTurn = true;

		// プレイヤーの重さ移しを可能に
		Pl.CanShift = true;
	}

	public void LiftDownEnd() {	// 持っているオブジェクトを強制的に下ろす使い方もできる
		Debug.Log("LiftDownEnd");

		// プレイヤーのモデルに同期していた回転を消去
		LiftObjModel.rotation = Quaternion.identity;

		// 持ち上げ中オブジェクトの判定と挙動を有効化
		LiftObj.GetComponent<BoxCollider>().enabled = true;
		LiftObj.GetComponent<MoveManager>().enabled = true;

		// プレイヤーと持ち上げオブジェクトを風で動かせるように変更
		MoveMng.CanMoveByWind = true;
		LiftObjMoveMng.CanMoveByWind = true;

		#region
		//		// 通常時のプレイヤー当たり判定を無効化/有効化
		//		standbyCol.enabled = !_liftUp;
		//
		//		// 持ち上げ中のプレイヤー当たり判定有効化時に接地方向によって判定位置を移動
		//		if (_liftUp) {
		//			BoxCollider liftingBoxCol = ((BoxCollider)liftingCol);
		//			float dis = Mathf.Abs(liftingBoxCol.center.y - colCenterPoint);
		//
		//			if (Pl.GetComponent<WeightManager>().WeightForce < 0.0f) {
		//				liftingBoxCol.center = new Vector3(liftingBoxCol.center.x, colCenterPoint + dis, liftingBoxCol.center.z);
		////				liftingBoxCol.center = new Vector3(liftingBoxCol.center.x, stdLiftingColPoint, liftingBoxCol.center.z);
		//			}else {
		//				liftingBoxCol.center = new Vector3(liftingBoxCol.center.x, colCenterPoint - dis, liftingBoxCol.center.z);
		//				//				liftingBoxCol.center = new Vector3(liftingBoxCol.center.x, revLiftingColPoint, liftingBoxCol.center.z);
		//			}
		//		}
		//		// 持ち上げ中のプレイヤー当たり判定を有効化/無効化
		//		liftingCol.enabled = _liftUp;
		//
		//		// 有効な当たり判定をMoveManagerに登録
		//		if (standbyCol.enabled) {
		//			MoveMng.UseCol = standbyCol;
		//		}else {
		//			MoveMng.UseCol = liftingCol;
		//		}
		#endregion

		// 対象をすり抜けオブジェクトに追加
		MoveMng.AddThroughCollider(LiftObj);

		// プレイヤー当たり判定の設定
		SwitchLiftCollider(false);

		// 対象の水中浮上可能フラグを戻す
		liftWaterStt.CanFloat = true;
		liftWaterStt = null;

		// 持ち上げオブジェクトをnullに
		LiftObj = null;
		liftWaterStt = null;

		#region
		//			// オブジェクトを離す
		//			ReleaseLiftObject();

		//			// 対象をすり抜けオブジェクトに追加
		//			MoveMng.AddThroughCollider(LiftObj.GetComponent<Collider>());
		//
		//			// 対象の水中浮上可能フラグを戻す
		//			Debug.LogWarning("canFloat = true");
		//			liftWaterStt.CanFloat = true;
		//
		//			// 処理後状態に
		//			St = LiftState.standby;
		//			afterHoldInput = true;
		//
		//			// 持ち上げ中オブジェクトをnullに
		//			Debug.LogWarning("canFloat null");
		//			LiftObj = null;
		//			liftWaterStt = null;
		#endregion

		// 下ろし処理後状態に
		St = LiftState.standby;
		afterHoldInput = true;
		WeightMng.LiftWeightMng = null;

		// プレイヤーの重さ移しを可能に
		Pl.CanShift = true;

		// プレイヤーのジャンプ、振り向きを可能に
		Pl.CanJump = true;
		Pl.CanRotationTurn = true;
	}

	void SwitchLiftCollider(bool _liftUp) {
		// 通常時のプレイヤー当たり判定を無効化/有効化
		standbyCol.enabled = !_liftUp;

		//		// 持ち上げ中のプレイヤー当たり判定有効化時に接地方向によって判定位置を移動
		//		if (_liftUp) {
		//			BoxCollider liftingBoxCol = ((BoxCollider)liftingCol);
		//			float dis = Mathf.Abs(liftingBoxCol.center.y - colCenterPoint);
		//
		//			if (Pl.GetComponent<WeightManager>().WeightForce < 0.0f) {
		//				liftingBoxCol.center = new Vector3(liftingBoxCol.center.x, colCenterPoint + dis, liftingBoxCol.center.z);
		//				//				liftingBoxCol.center = new Vector3(liftingBoxCol.center.x, stdLiftingColPoint, liftingBoxCol.center.z);
		//			} else {
		//				liftingBoxCol.center = new Vector3(liftingBoxCol.center.x, colCenterPoint - dis, liftingBoxCol.center.z);
		//				//				liftingBoxCol.center = new Vector3(liftingBoxCol.center.x, revLiftingColPoint, liftingBoxCol.center.z);
		//			}
		//		}

		WeightManager weightMng = GetComponent<WeightManager>();
		if (!weightMng) {
			Debug.LogError("WeightManagerが見つかりませんでした。");
		}

		Debug.LogWarning("liftUp" + _liftUp + "\nbef isPosUp " + isPosUp);

		// プレイヤー当たり判定とモデルの位置を調整
		if (_liftUp) {
			offsetTransform.localPosition = new Vector3(offsetTransform.localPosition.x, upStdOffsetPos, offsetTransform.localPosition.z);
			rotationTransform.localPosition = new Vector3(rotationTransform.localPosition.x, upRotationPos, rotationTransform.localPosition.z);
			modelTransform.localPosition = new Vector3(modelTransform.localPosition.x, upModelPos, modelTransform.localPosition.z);

			//			if ((weightMng.WeightLv == WeightManager.Weight.flying) || (WaterStt.IsInWater && (WeightMng.WeightLv <= WeightManager.Weight.light))) {
			if ((Pl.RotVec.y == 1.0f && !isPosUp)) {
				MoveManager.Move(new Vector3(0.0f, liftingPosOffset, 0.0f), GetComponent<BoxCollider>(), LayerMask.GetMask(new string[] { "Stage", "Box", "Fance" }));
				//transform.position += new Vector3(0.0f, liftingPosOffset, 0.0f);
				Debug.LogWarning("MoveA");
			}
		}
		else {
			offsetTransform.localPosition = new Vector3(offsetTransform.localPosition.x, downOffsetPos, offsetTransform.localPosition.z);
			rotationTransform.localPosition = new Vector3(rotationTransform.localPosition.x, downRotationPos, rotationTransform.localPosition.z);
			modelTransform.localPosition = new Vector3(modelTransform.localPosition.x, downModelPos, modelTransform.localPosition.z);
			//			if ((weightMng.WeightLv == WeightManager.Weight.flying) || (WaterStt.IsInWater && (WeightMng.WeightLv <= WeightManager.Weight.light))) {
			if ((Pl.RotVec.y == 1.0f && isPosUp) || failedPosUpFlg) {
				List<GameObject> throughObjList = new List<GameObject>();
				foreach (var throughCol in MoveMng.ThroughObjList) {
					throughObjList.Add(throughCol.gameObject);
				}
				MoveManager.Move(new Vector3(0.0f, -liftingPosOffset, 0.0f), GetComponent<BoxCollider>(), LayerMask.GetMask(new string[] { "Stage", "Box", "Fance" }), false, true, throughObjList);
				Debug.LogWarning("MoveB");
				//transform.position -= new Vector3(0.0f, liftingPosOffset, 0.0f);
				isPosUp = false;
			}
			failedPosUpFlg = false;
		}

		Debug.LogWarning("aft isPosUp " + isPosUp);

		// 四辺コライダーの位置を調整
		CorrectFourSideCollider(_liftUp);

		// 持ち上げ中のプレイヤー当たり判定を有効化/無効化
		liftingCol.enabled = _liftUp;

		// 有効な当たり判定をMoveManagerに登録
		if (standbyCol.enabled) {
			MoveMng.UseCol = standbyCol;
		} else {
			MoveMng.UseCol = liftingCol;
		}
	}

	public void CorrectFourSideCollider(bool _liftUp) {
		WeightManager weightMng = GetComponent<WeightManager>();
		if (!weightMng) {
			Debug.LogError("WeightManagerが見つかりませんでした。");
		}

		// 四辺当たり判定の位置を調整
		if (_liftUp) {
//			if (weightMng.WeightLv > WeightManager.Weight.flying) {
			if (Pl.RotVec.y == 0.0f) {
				fourSideTransform.localPosition = new Vector3(fourSideTransform.localPosition.x, upStdFourSidePos, fourSideTransform.localPosition.z);
			} else {
				fourSideTransform.localPosition = new Vector3(fourSideTransform.localPosition.x, upRevFourSidePos, fourSideTransform.localPosition.z);
			}
			fourSideTopColTransform.localPosition = new Vector3(fourSideTopColTransform.localPosition.x, upTopColPos, fourSideTopColTransform.localPosition.z);
		} else {
			fourSideTransform.localPosition = new Vector3(fourSideTransform.localPosition.x, downFourSidePos, fourSideTransform.localPosition.z);
			fourSideTopColTransform.localPosition = new Vector3(fourSideTopColTransform.localPosition.x, downTopColPos, fourSideTopColTransform.localPosition.z);
		}
	}

	Vector3 GetLiftUpBoxMove() {
		if (LiftObj == null) return Vector3.zero;

		Vector3 ret = (PlAnim.GetBoxPosition() - LiftObj.transform.position);
		float vec = Mathf.Sign(LiftObj.transform.position.x - Pl.transform.position.x);
		
		//// x軸が離れようとした場合は離さない
		//float defDis = (LiftObj.transform.position.x - Pl.transform.position.x);
		//float dis = (ret.x - Pl.transform.position.x);
		//if (Mathf.Abs(dis) > Mathf.Abs(defDis)) {
		//	ret = new Vector3((Pl.transform.position.x + defDis), ret.y, ret.z);
		//}
		if(vec == Mathf.Sign(ret.x)) {
			ret = new Vector3(0.0f, ret.y, ret.z);
		}
		return ret;
	}

	Vector3 GetLiftDownBoxPosition() {
		Vector3 pos = PlAnim.GetBoxPosition();

		// x軸がプレイヤーの中心より後ろだったら
		float posVec = Mathf.Sign(LiftObj.transform.position.x - Pl.transform.position.x);
		if(Pl.RotVec.x != posVec) {
			// 中央に補正
			pos = new Vector3(Pl.transform.position.x, pos.y, pos.z);
		}

		return pos;
	}

//	public void ReleaseLiftObject() {
//		// 離す
//		LiftEndObject(LiftObj, false);
//
//
//	}
}
