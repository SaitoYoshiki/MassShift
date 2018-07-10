using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerAnimation : MonoBehaviour {

	[SerializeField]
	List<GameObject> mAnimationModel;

	[SerializeField]
	Transform mHandTransform;

	[SerializeField]
	Transform mCatchEndHandPosition;

	[SerializeField]
	Transform mCatchEndBoxPosition;

	[SerializeField]
	Transform mReleaseEndHandPosition;

	[SerializeField]
	Transform mReleaseEndBoxPosition;

	[SerializeField]
	Transform mWaterReleaseEndHandPosition;

	[SerializeField]
	Transform mWaterReleaseEndBoxPosition;

	public Vector3 mStartDifference;
	public Vector3 mEndDifference;
	public Vector3 mLiftDifference;

	[SerializeField]
	float mCatchStartTime = 0.0f;

	[SerializeField]
	float mCatchEndTime = 1.0f;

	[SerializeField]
	float mReleaseStartTime = 0.0f;

	[SerializeField]
	float mReleaseEndTime = 1.0f;

	[SerializeField]
	float mWaterReleaseEndTime = 0.5f;

	float mBeforeStandByLoopTime = 1.0f;

	Animator mAnimator;
	
	public enum CState {

		cStandBy,
		cWalk,
		cJumpStart,
		cJumpMid,
		cJumpFall,
		cJumpLand,
		cHoldStandBy,
		cHoldWalk,
		cHoldJumpStart,
		cHoldJumpMid,
		cHoldJumpFall,
		cHoldJumpLand,
		cCatch,
		cCatchFailed,
		cRelease,

		cWaterStandBy,
		cHoldWaterStandBy,

		cSwim,
		cHoldSwim,

		cHandSpring,
		cHoldHandSpring,

		cFly,
		cHoldFly,
	}

	[SerializeField]
	CState mState;

	[SerializeField]
	CState mBeforeState;

	bool mIsInit = true;

	public bool mBeforeCatch = false;	//持ち上げ状態ではあるが、まだボックスを持ち上げていない

	bool mCompleteCatchFailed = false;
	bool mCompleteCatch = false;
	bool mCompleteRelease = false;

	[SerializeField]
	public float mStateTime;

	float mCatchFailedStateTime;

	float mSpeed = 0.0f;
	Vector3 mBeforePosition;

	Player pl = null;
	Player Pl {
		get {
			if (!pl) {
				pl = GetComponent<Player>();
			}
			return pl;
		}
	}
	
	public void ChangeState(CState aNextState) {
		mBeforeState = mState;
		mState = aNextState;
		mIsInit = true;
		mStateTime = 0.0f;
	}

	// Use this for initialization
	void Start () {
		mStateTime = 0.0f;
		mAnimator = GetAnimator(mAnimationModel[0]);
	}

	Animator GetAnimator(GameObject aAnimationModel) {
		return aAnimationModel.GetComponent<Animator>();
	}


	#region UpdateState


	// Update is called once per frame
	void Update () {
		mStateTime += Time.deltaTime;

		UpdateState();

	}


	void UpdateState() {

		switch (mState) {
		case CState.cStandBy:
			UpdateStandBy();
			break;
		case CState.cWalk:
			UpdateWalk();
			break;

		case CState.cWaterStandBy:
			UpdateWaterStandBy();
			break;
		case CState.cSwim:
			UpdateSwim();
			break;
		case CState.cHoldWaterStandBy:
			UpdateHoldStandBy();
			break;
		case CState.cHoldSwim:
			UpdateHoldSwim();
			break;

		case CState.cJumpStart:
			UpdateJumpStart();
			break;
		case CState.cJumpMid:
			UpdateJumpMid();
			break;
		case CState.cJumpFall:
			UpdateJumpFall();
			break;
		case CState.cJumpLand:
			UpdateJumpLand();
			break;
		case CState.cCatch:
			UpdateCatch();
			break;
		case CState.cCatchFailed:
			UpdateCatchFailed();
			break;

		case CState.cHoldStandBy:
			UpdateHoldStandBy();
			break;
		case CState.cHoldWalk:
			UpdateHoldWalk();
			break;
		case CState.cHoldJumpStart:
			UpdateHoldJumpStart();
			break;
		case CState.cHoldJumpMid:
			UpdateHoldJumpMid();
			break;
		case CState.cHoldJumpFall:
			UpdateHoldJumpFall();
			break;
		case CState.cHoldJumpLand:
			UpdateHoldJumpLand();
			break;
		case CState.cRelease:
			UpdateRelease();
			break;

		case CState.cHandSpring:
			UpdateHandSpring();
			break;
		case CState.cHoldHandSpring:
			UpdateHoldHandSpring();
			break;

		case CState.cFly:
			UpdateFly();
			break;
		case CState.cHoldFly:
			UpdateHoldFly();
			break;
		}
	}
	
	void InitStandBy() {
		foreach(var a in mAnimationModel) {
			GetAnimator(a).CrossFadeInFixedTime("StandBy", 0.2f);
		}
	}

	void UpdateStandBy() {
		if(mIsInit) {
			InitStandBy();
			mIsInit = false;
		}
	}

	void InitWaterStandBy() {
		foreach (var a in mAnimationModel) {
			GetAnimator(a).CrossFadeInFixedTime("WaterStandBy", 0.2f);
		}
	}

	void UpdateWaterStandBy() {
		if (mIsInit) {
			InitWaterStandBy();
			mIsInit = false;
		}
	}

	void InitSwim() {
		foreach (var a in mAnimationModel) {
			GetAnimator(a).CrossFadeInFixedTime("Swim", 0.2f);
		}
	}

	void UpdateSwim() {
		if (mIsInit) {
			InitSwim();
			mIsInit = false;
		}
	}


	void InitHoldWaterStandBy() {
		foreach (var a in mAnimationModel) {
			GetAnimator(a).CrossFadeInFixedTime("HoldWaterStandBy", 0.2f);
		}
	}

	void UpdateHoldWaterStandBy() {
		if (mIsInit) {
			InitHoldWaterStandBy();
			mIsInit = false;
		}
	}

	void InitHoldSwim() {
		foreach (var a in mAnimationModel) {
			GetAnimator(a).CrossFadeInFixedTime("HoldSwim", 0.2f);
		}
	}

	void UpdateHoldSwim() {
		if (mIsInit) {
			InitHoldSwim();
			mIsInit = false;
		}
	}



	void InitWalk() {
		foreach (var a in mAnimationModel) {
			GetAnimator(a).CrossFadeInFixedTime("Walk", 0.2f);
		}
	}

	void UpdateWalk() {
		if (mIsInit) {
			InitWalk();
			mIsInit = false;
		}

		foreach (var a in mAnimationModel) {
			GetAnimator(a).SetFloat("Speed", mSpeed);
		}
	}


	void InitJumpStart() {
		foreach (var a in mAnimationModel) {
			GetAnimator(a).CrossFadeInFixedTime("JumpStart", 0.2f);
		}
		mBeforePosition = transform.position;
	}

	void UpdateJumpStart() {
		if (mIsInit) {
			InitJumpStart();
			mIsInit = false;
		}

		if (IsAnimationEnd("JumpStart")) {
			//ChangeState(CState.cJumpMid);
		}

		//落下しているなら
		if (IsFall()) {
			ChangeState(CState.cJumpFall);
		}

		mBeforePosition = transform.position;
	}


	void InitJumpMid() {
		foreach (var a in mAnimationModel) {
			GetAnimator(a).CrossFadeInFixedTime("JumpMid", 0.0f);
		}
		mBeforePosition = transform.position;
	}

	void UpdateJumpMid() {
		if (mIsInit) {
			InitJumpMid();
			mIsInit = false;
		}

		//落下しているなら
		if(IsFall()) {
			ChangeState(CState.cJumpFall);
		}

		mBeforePosition = transform.position;
	}

	bool IsFall() {
		if(IsHover() == false) {
			if (mBeforePosition.y > transform.position.y) {
				return true;
			}
		}
		else {
			if (mBeforePosition.y < transform.position.y) {
				return true;
			}
		}
		return false;
	}

	bool IsHover() {
		return GetComponent<Player>().RotVec.y >= 0.5f;
	}

	bool IsWaterSurface() {
		return GetComponent<WaterState>().IsWaterSurface;
	}

	void InitJumpFall() {
		foreach (var a in mAnimationModel) {
			GetAnimator(a).CrossFadeInFixedTime("JumpFall", 0.2f);
		}
	}

	void UpdateJumpFall() {
		if (mIsInit) {
			InitJumpFall();
			mIsInit = false;
		}
	}


	void InitJumpLand() {
		foreach (var a in mAnimationModel) {
			GetAnimator(a).CrossFadeInFixedTime("JumpLand", 0.2f);
		}
	}

	void UpdateJumpLand() {
		if (mIsInit) {
			InitJumpLand();
			mIsInit = false;
		}

		if (IsAnimationEnd("JumpLand")) {
			ChangeState(CState.cStandBy);
		}
	}

	void InitCatch() {
		foreach (var a in mAnimationModel) {
			GetAnimator(a).CrossFadeInFixedTime("Catch", 0.2f);
		}
		foreach (var a in mAnimationModel) {
			GetAnimator(a).SetFloat("CatchSpeed", 1.0f);
		}
	}

	void UpdateCatch() {
		if (mIsInit) {
			InitCatch();
			mIsInit = false;
		}

		if (IsAnimationEnd("Catch")) {
			mCompleteCatch = true;
		}
	}

	void InitCatchFailed() {
		foreach (var a in mAnimationModel) {
			GetAnimator(a).SetFloat("CatchSpeed", -1.0f);
		}
	}

	bool EndCatchFailed() {
		if (!mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Catch")) return false;
		Debug.Log("CatchFailedTime:" + mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
		return mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.0f;
	}

	void UpdateCatchFailed() {
		if (mIsInit) {
			InitCatchFailed();
			mIsInit = false;
		}

		mCatchFailedStateTime -= Time.deltaTime;
		mStateTime = mCatchFailedStateTime;

		if (EndCatchFailed()) {
			mCompleteCatchFailed = true;
		}
	}



	void InitHoldStandBy() {
		foreach (var a in mAnimationModel) {
			GetAnimator(a).CrossFadeInFixedTime("HoldStandBy", 0.2f);
		}
	}

	void UpdateHoldStandBy() {
		if (mIsInit) {
			InitHoldStandBy();
			mIsInit = false;
		}
	}


	void InitHoldWalk() {
		foreach (var a in mAnimationModel) {
			GetAnimator(a).CrossFadeInFixedTime("HoldWalk", 0.2f);
		}
	}

	void UpdateHoldWalk() {
		if (mIsInit) {
			InitHoldWalk();
			mIsInit = false;
		}

		foreach (var a in mAnimationModel) {
			GetAnimator(a).SetFloat("Speed", mSpeed);
		}
	}


	void InitHoldJumpStart() {
		foreach (var a in mAnimationModel) {
			GetAnimator(a).CrossFadeInFixedTime("HoldJumpStart", 0.2f);
		}
		mBeforePosition = transform.position;
	}

	void UpdateHoldJumpStart() {
		if (mIsInit) {
			InitHoldJumpStart();
			mIsInit = false;
		}

		if (IsAnimationEnd("HoldJumpStart")) {
			//ChangeState(CState.cHoldJumpMid);
		}

		//落下しているなら
		if (IsFall()) {
			ChangeState(CState.cHoldJumpFall);
		}

		mBeforePosition = transform.position;
	}


	void InitHoldJumpMid() {
		foreach (var a in mAnimationModel) {
			GetAnimator(a).CrossFadeInFixedTime("HoldJumpMid", 0.0f);
		}
		mBeforePosition = transform.position;
	}

	void UpdateHoldJumpMid() {
		if (mIsInit) {
			InitHoldJumpMid();
			mIsInit = false;
		}

		//落下しているなら
		if (IsFall()) {
			ChangeState(CState.cHoldJumpFall);
		}

		mBeforePosition = transform.position;
	}


	void InitHoldJumpFall() {
		foreach (var a in mAnimationModel) {
			GetAnimator(a).CrossFadeInFixedTime("HoldJumpFall", 0.2f);
		}
	}

	void UpdateHoldJumpFall() {
		if (mIsInit) {
			InitHoldJumpFall();
			mIsInit = false;
		}
	}


	void InitHoldJumpLand() {
		foreach (var a in mAnimationModel) {
			GetAnimator(a).CrossFadeInFixedTime("HoldJumpLand", 0.2f);
		}
	}

	void UpdateHoldJumpLand() {
		if (mIsInit) {
			InitHoldJumpLand();
			mIsInit = false;
		}

		if (IsAnimationEnd("HoldJumpLand")) {
			ChangeState(CState.cHoldStandBy);
		}
	}

	void InitRelease() {
		foreach (var a in mAnimationModel) {
			GetAnimator(a).CrossFadeInFixedTime("Release", 0.2f);
		}
	}

	void UpdateRelease() {
		if (mIsInit) {
			InitRelease();
			mIsInit = false;
		}

		if (IsAnimationEnd("Release")) {
			mCompleteRelease = true;
		}
	}


	void InitHandSpring() {
		foreach (var a in mAnimationModel) {
			GetAnimator(a).CrossFadeInFixedTime("HandSpring", 0.2f);
		}
	}

	void UpdateHandSpring() {
		if (mIsInit) {
			InitHandSpring();
			mIsInit = false;
		}
	}

	void InitHoldHandSpring() {
		foreach (var a in mAnimationModel) {
			GetAnimator(a).CrossFadeInFixedTime("HoldHandSpring", 0.2f);
		}
	}

	void UpdateHoldHandSpring() {
		if (mIsInit) {
			InitHoldHandSpring();
			mIsInit = false;
		}
	}

	void InitFly() {
		foreach (var a in mAnimationModel) {
			GetAnimator(a).CrossFadeInFixedTime("Fly", 0.4f);
		}
	}

	void UpdateFly() {
		if (mIsInit) {
			InitFly();
			mIsInit = false;
		}
	}

	void InitHoldFly() {
		foreach (var a in mAnimationModel) {
			GetAnimator(a).CrossFadeInFixedTime("Fly", 0.4f);
		}
	}

	void UpdateHoldFly() {
		if (mIsInit) {
			InitHoldFly();
			mIsInit = false;
		}
	}


	GameObject mBox;

	public Vector3 GetBoxPosition() {
		Vector3 lRes = mCatchEndBoxPosition.position;

		//持ち上げている途中なら
		if (IsCatching() || IsCatchFailed()) {
			//また持ち上げていないなら
			if (mBeforeCatch) {
				//開始時間を超えていて
				if (0.3f <= mStateTime) {
					if (IsHover() == false) {
						if (mHandTransform.position.y >= mBox.transform.position.y - 0.25f) {
							mBeforeCatch = false;   //プレイヤーが上向きで、手の位置がボックスより上なら、持ち始める
						}
					}
					else {
						if (mHandTransform.position.y <= mBox.transform.position.y + 0.25f) {
							mBeforeCatch = false;   //プレイヤーが下向きで、手の位置がボックスよりも下なら、持ち始める
						}
					}
				}
				

				if(mBeforeCatch) {
					lRes = mBox.transform.position;
					return ToZeroZ(lRes);
				}
				//持ち始めたなら
				else {
					mStartDifference = mBox.transform.position - mHandTransform.position;
					mEndDifference = mCatchEndBoxPosition.position - mCatchEndHandPosition.position;
					mCatchStartTime = mStateTime;
				}
			}

			//終了の時間を超えていたら
			if (mCatchEndTime <= mStateTime) {
				lRes = mCatchEndBoxPosition.position;
				return ToZeroZ(lRes);
			}
			else {
				float lUnder = (mCatchEndTime - mCatchStartTime);
				if(Mathf.Approximately(0.0f, lUnder)) {
					lRes = mCatchEndBoxPosition.position;
					return ToZeroZ(lRes);
				}
				float lRate = (mStateTime - mCatchStartTime) / lUnder;

				Vector3 lDifference = Vector3.Lerp(mStartDifference, mEndDifference, Mathf.Clamp01(lRate));
				lRes = mHandTransform.position + lDifference;
				mLiftDifference = lDifference;
			}
		}
		else if (IsReleasing()) {

			float lEndTime = mReleaseEndTime;
			if(IsWaterSurface()) {
				lEndTime = mWaterReleaseEndTime;
			}

			if (mReleaseStartTime <= mStateTime && mStateTime < lEndTime) {
				Vector3 lStartDifference;
				Vector3 lEndDifference;
				float lRate;
				
				lStartDifference = mCatchEndBoxPosition.position - mCatchEndHandPosition.position;

				//水面なら
				if(IsWaterSurface()) {
					lEndDifference = mWaterReleaseEndBoxPosition.position - mWaterReleaseEndHandPosition.position;
					lRate = (mStateTime - mReleaseStartTime) / (mWaterReleaseEndTime - mReleaseStartTime);
				}
				//そうでないなら
				else {
					lEndDifference = mReleaseEndBoxPosition.position - mReleaseEndHandPosition.position;
					lRate = (mStateTime - mReleaseStartTime) / (mReleaseEndTime - mReleaseStartTime);
				}
				lRes = mHandTransform.position + Vector3.Lerp(mStartDifference, mEndDifference, Mathf.Clamp01(lRate));
			}
			else if (mStateTime < mReleaseStartTime) {
				lRes = mCatchEndBoxPosition.position;
				return ToZeroZ(lRes);
			}
			else {
				if(IsWaterSurface()) {
					lRes = mWaterReleaseEndBoxPosition.position;
				}
				else {
					lRes = mReleaseEndBoxPosition.position;
				}
				return ToZeroZ(lRes);
			}
		}

		return ToZeroZ(lRes);
	}
	Vector3 ToZeroZ(Vector3 aVec) {
		return new Vector3(aVec.x, aVec.y, 0.0f);
	}

	public bool CompleteCatch() {
		return mCompleteCatch;
	}
	public bool CompleteRelease() {
		return mCompleteRelease;
	}

	public bool IsCatching() {
		return mState == CState.cCatch;
	}
	public bool IsReleasing() {
		return mState == CState.cRelease;
	}

	public bool CompleteCatchFailed() {
		return mCompleteCatchFailed;
	}

	public bool IsCatchFailed() {
		return mState == CState.cCatchFailed;
	}


	bool IsAnimationEnd(string aAnimationName) {
		if (!mAnimator.GetCurrentAnimatorStateInfo(0).IsName(aAnimationName)) return false;
		return mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
	}

	public float GetStateTime() {
		return mStateTime;
	}



	#endregion

	public void SetSpeed(float aSpeed) {
		mSpeed = aSpeed * 1.5f;
	}
	public void SetLandSpeed(float aSpeed) {
		foreach (var a in mAnimationModel) {
			GetAnimator(a).SetFloat("LandSpeed", aSpeed);
		}
	}

	public void StartStandBy() {
		if (IsLiftAction()) return;
		if (mState != CState.cWalk) return;	//歩き状態からしか外からは呼び出せない
		ChangeState(CState.cStandBy);
	}
	public void StartWalk() {
		if (IsLiftAction()) return;
		if (mState != CState.cStandBy && mState != CState.cJumpLand) return; //立ち止まり状態からしか外からは呼び出せない
		ChangeState(CState.cWalk);
	}

	public void StartWaterStandBy() {
		//		if (!StartLandCheck()) return;
		if (IsLiftAction()) return;
		ChangeState(CState.cWaterStandBy);
	}
	public void StartHoldWaterStandBy() {
		//		if (!StartHoldLandCheck()) return;
		if (IsLiftAction()) return;
		ChangeState(CState.cHoldWaterStandBy);
	}
	public void StartSwim() {
		//		if (!StartLandCheck()) return;
		if (IsLiftAction()) return;
		ChangeState(CState.cSwim);
	}
	public void StartHoldSwim() {
		//		if (!StartHoldLandCheck()) return;
		if (IsLiftAction()) return;
		ChangeState(CState.cHoldSwim);
	}

	public void StartJump() {
		if (IsLiftAction()) return;
		ChangeState(CState.cJumpStart);
	}

	public void StartFall() {
		if (IsLiftAction()) return;
		ChangeState(CState.cJumpFall);
	}

	bool StartLandCheck() {
		if (mState == CState.cJumpLand) return false;
		if (mState == CState.cJumpStart) return true;
		if (mState == CState.cJumpMid) return true;
		if (mState == CState.cJumpFall) return true;
		if (mState == CState.cFly) return true;
		if (mState == CState.cHoldFly) return true;
		if ((mState == CState.cHandSpring) && (!Pl.IsHandSpringWeit)) return true;
		return false;
	}

	public void StartLand() {
		if (IsLiftAction()) return;
		if (!StartLandCheck()) return;
		ChangeState(CState.cJumpLand);
	}

	public void StartCatch(GameObject aBox) {
		mBox = aBox;
		ChangeState(CState.cCatch);
		mCompleteCatch = false;
		mBeforeCatch = true;
	}

	public void FailedCatch() {
		mCatchFailedStateTime = mStateTime;
		ChangeState(CState.cCatchFailed);
		mCompleteCatchFailed = false;
	}


	public void StartHoldStandBy() {
		if (IsLiftAction()) return;
		if (mState != CState.cHoldWalk) return; //歩き状態からしか外からは呼び出せない
		ChangeState(CState.cHoldStandBy);
	}
	public void StartHoldWalk() {
		if (IsLiftAction()) return;
		if (mState != CState.cHoldStandBy && mState != CState.cHoldJumpLand) return; //立ち止まり状態からしか外からは呼び出せない
		ChangeState(CState.cHoldWalk);
	}

	public void StartHoldJump() {
		if (IsLiftAction()) return;
		ChangeState(CState.cHoldJumpStart);
	}

	public void StartHoldFall() {
		if (IsLiftAction()) return;
		ChangeState(CState.cHoldJumpFall);
	}

	bool StartHoldLandCheck() {
		if (mState == CState.cHoldJumpLand) return false;
		if (mState == CState.cHoldJumpStart) return true;
		if (mState == CState.cHoldJumpMid) return true;
		if (mState == CState.cHoldJumpFall) return true;
		return false;
	}

	public void StartHoldLand() {
		if (!StartHoldLandCheck()) return;
		if (IsLiftAction()) return;
		ChangeState(CState.cHoldJumpLand);
	}


	public void StartHandSpring() {
		if (IsLiftAction()) return;
		ChangeState(CState.cHandSpring);
	}

	public void StartHoldHandSpring() {
		if (IsLiftAction()) return;
		ChangeState(CState.cHoldHandSpring);
	}

	public void StartFly() {
		if (IsLiftAction()) return;
		ChangeState(CState.cFly);
	}

	public void StartHoldFly() {
		if (IsLiftAction()) return;
		ChangeState(CState.cHoldFly);
	}

	public void StartRelease() {
		mStartDifference = mCatchEndBoxPosition.position - mCatchEndHandPosition.position;
		mEndDifference = mReleaseEndBoxPosition.position - mReleaseEndHandPosition.position;
		ChangeState(CState.cRelease);
		mCompleteRelease = false;
	}

	public void ExitCatchFailed() {
		ChangeState(CState.cStandBy);
	}
	public void ExitCatch() {
		ChangeState(CState.cHoldStandBy);
	}
	public void ExitRelease() {
		ChangeState(CState.cStandBy);
	}

	bool IsLiftAction() {
		if (mState == CState.cCatch) return true;
		if (mState == CState.cCatchFailed) return true;
		if (mState == CState.cRelease) return true;
		return false;
	}


	//立ち止まるアニメーションで、ループの切れ目かどうか
	public bool IsStandByAnimationFinish() {
		if (!mAnimator.GetCurrentAnimatorStateInfo(0).IsName("StandBy")) return false;


		bool lRes = false;
		float lTime = mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
		float lLoopTime = lTime % 1.0f;

		if (lTime >= 1.0f && lLoopTime > 0.9f) {
			if(mBeforeStandByLoopTime <= 0.9f) {
				lRes = true;
			}
		}
		mBeforeStandByLoopTime = lLoopTime;
		return lRes;
	}
}



/*


	cNone
	cStandBy
	cWalk
	cJumpStart
	cJumpMid
	cJumpFall
	cJumpLand
	cHoldStandBy
	cHoldWalk
	cHoldJumpStart
	cHoldJumpMid
	cHoldJumpFall
	cHoldJumpLand
	cCatch
	cRelease

	cWaterStandBy
	cWaterWalk
	cWaterJumpStart
	cWaterJumpMid
	cWaterJumpFall
	cWaterJumpLand
	cWaterHoldStandBy
	cWaterHoldWalk
	cWaterHoldJumpStart
	cWaterHoldJumpMid
	cWaterHoldJumpFall
	cWaterHoldJumpLand
	cWaterCatch
	cWaterRelease

 
*/
