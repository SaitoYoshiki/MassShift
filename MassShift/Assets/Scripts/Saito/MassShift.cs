using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MassShift : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{

		mMassShiftLine = Instantiate(mMassShiftLinePrefab, transform);
		mMassShiftLine.name = "MassShiftLine";

		ResizeShareLine();

		mCursor = Instantiate(mCursorPrefab, transform);
		mCursor.name = "Cursor";

		mLightBallTemplate = Instantiate(mLightBallPrefab, transform);
		mLightBallTemplate.name = "LightBallTemplate";
		mLightBallTemplate.SetActive(false);

		mLightBall = null;

		mAllWeightModel = FindObjectsOfType<WeightManager>();
		mPlayer = FindObjectOfType<Player>();
		mStageShiftUI = FindObjectOfType<StageShiftUI>();

		Cursor.visible = false;
	}

	// Update is called once per frame
	void Update() {

		if (Time.timeScale == 0.0f) return;

		UpdateCanShift();
		
		UpdateState();
		mFromLastShiftTime += Time.deltaTime;
	}

	void UpdateCanShift() {
		//プレイヤーが重さを移せるなら
		if (PlayerCanShift()) {
			_mPlayerCanShift = true;
		}
		else {
			_mPlayerCanShift = false;
		}

		if (CanShift == true) {
			if (mState == CSelectState.cCantShift) {
				ChangeState(CSelectState.cNormal);
			}
		}
		else {
			ChangeState(CSelectState.cCantShift);
		}
	}

	void PlayerIsShift(bool aValue) {
		if (mPlayer == null) return;
		//p.IsShift = aValue;
	}
	bool PlayerCanShift() {
		if (mPlayer == null) return true;
		return true;
		//return p.CanShift;
	}


	void UpdateState()
	{

		switch (mState)
		{
			case CSelectState.cNormal:
				MoveCursor();
				UpdateNormal();
				break;
			case CSelectState.cClick:
				MoveCursor();
				UpdateClickRightClick();
				break;
			case CSelectState.cDrag:
				MoveCursor();
				UpdateDrag();
				break;
			case CSelectState.cMoveSourceToDest:
				MoveCursor();
				UpdateMoveSourceToDest();
				break;
			case CSelectState.cMoveFromShare:
				MoveCursor();
				UpdateMoveFromShare();
				break;
			case CSelectState.cMoveToShare:
				MoveCursor();
				UpdateMoveToShare();
				break;
			case CSelectState.cReturnToSource:
				MoveCursor();
				UpdateReturnToSource();
				break;
			case CSelectState.cReturnToShare:
				MoveCursor();
				UpdateReturnToShare();
				break;
			case CSelectState.cSuccess:
				MoveCursor();
				UpdateSuccess();
				break;
			case CSelectState.cFail:
				MoveCursor();
				UpdateFail();
				break;
			case CSelectState.cCantShift:
				MoveCursor();
				UpdateCantShift();
				break;
		}
	}

	void ChangeState(CSelectState aState) {
		mBeforeState = mState;
		mState = aState;
		mInitState = true;
	}


	//通常時
	void UpdateNormal() {

		if (mInitState == true) {

			mInitState = false;

			//移す線を消す
			mMassShiftLine.SetActive(false);

			//モデルのハイライトを消す
			ShowAllModelHilight(false, Color.white);
			ShowModelHilight(mSelect, false, Color.white);
			ShowModelHilight(mSource, false, Color.white);
			ShowModelHilight(mDest, false, Color.white);

			//通常時のカーソルを表示
			ChangeCursorState(CCursorState.cNormal);

			//重さを移せるかのUIを消す
			mStageShiftUI.GetSourceUI().NotShow();
			mStageShiftUI.GetDestUI().NotShow();

			mSource = null;
			mDest = null;
			mSelect = null;

			//プレイヤーに、現在移していないと伝える
			PlayerIsShift(false);
		}

		//選択中のオブジェクトを更新
		mBeforeSelect = mSelect;
		mSelect = GetNearestObject(GetNearWeightObject());


		//モデルのハイライトを更新
		if (mBeforeSelect != mSelect) {

			if (mBeforeSelect != null) {
				ShowModelHilight(mBeforeSelect, false, Color.white);
			}

			if (mSelect != null) {

				bool lCanSelect = false;
				if (CanShiftSource(mSelect)) {
					lCanSelect = true;
				}

				if (lCanSelect) {
					ShowModelHilight(mSelect, true, mCanSelectColor * mCanSelectColorPower);
				}
				else {
					ShowModelHilight(mSelect, true, mCanNotSelectColor * mCanNotSelectColorPower);
				}
			}
		}


		//共有ボックス同士の線の更新
		//

		if(mBeforeSelect != mSelect) {
			//選択されなくなったらアクティブでなくす
			if (IsShareWeightBox(mBeforeSelect)) {
				SetActiveShareLine(mMassShiftShareLineToSource, false);
			}

			//選択されていたらアクティブに
			if (IsShareWeightBox(mSelect)) {
				SetActiveShareLine(mMassShiftShareLineToSource, true);
			}
		}

		//もし共有ボックスが選択されていたらアクティブに
		if (IsShareWeightBox(mSelect)) {
			UpdateShareLine(mSelect, mMassShiftShareLineToSource, true);
		}


		//もし重さを移すボタンが押されたら
		if (GetShiftButton()) {

			//移す元が選ばれている場合
			if (mSelect != null) {
				mSource = mSelect;
				ChangeState(CSelectState.cClick);	//クリック状態へ
			}
			//移す元が選ばれていない場合
			else {
				ChangeState(CSelectState.cFail);	//失敗
			}
		}
	}

	//ドラッグ時
	void UpdateDrag() {

		if (mInitState == true) {
			mInitState = false;

			//移す線を出す
			mMassShiftLine.SetActive(true);

			//移す元のハイライトを出す
			ShowAllModelHilight(true, mSourceColor * mSourceColorPower);

			ShowModelHilight(mSource, true, mSourceColor * mSourceColorPower);


			//共有ボックス間の線を表示するか
			if (IsShareWeightBox(mSource)) {
				SetActiveShareLine(mMassShiftShareLineToSource, true);
			}
			else {
				SetActiveShareLine(mMassShiftShareLineToSource, false);
			}


			//ソースの重さを移せるかのUIを更新
			if (CanShiftSource(mSource)) {
				if (MoveAfterLight(mSource) > 0.0f) {
					mStageShiftUI.GetSourceUI().ShowUp();
				}
				else {
					mStageShiftUI.GetSourceUI().ShowDown();
				}
			}
			else {
				mStageShiftUI.GetSourceUI().ShowFail();
			}


			//通常時ではないカーソルを表示
			ChangeCursorState(CCursorState.cShotLineThrough);
		}

		//選択されているオブジェクトを更新
		mBeforeSelect = mSelect;
		mSelect = GetNearestObject(GetNearWeightObject(), mSource);


		//モデルのハイライトを更新
		if (mBeforeSelect != mSelect) {

			if (mBeforeSelect != null) {
				ShowModelHilight(mBeforeSelect, true, mSourceColor * mSourceColorPower);
			}

			if (mSelect != null) {

				bool lCanSelect = false;
				if (CanShiftDest(mSelect) && CanShiftShare(mSource, mSelect)) {
					lCanSelect = true;
				}

				if(CanShiftDest(mSelect)) {
					if (MoveAfterHeavy(mSelect) > 0.0f) {
						mStageShiftUI.GetDestUI().ShowUp();
					}
					else {
						mStageShiftUI.GetDestUI().ShowDown();
					}
				}
				else {
					mStageShiftUI.GetDestUI().ShowFail();
				}

				if (lCanSelect) {
					ShowModelHilight(mSelect, true, mCanSelectColor * mCanSelectColorPower);
					ShowModelHilight(mSource, true, mCanSelectColor * mCanSelectColorPower);
				}
				else {
					ShowModelHilight(mSelect, true, mCanNotSelectColor * mCanNotSelectColorPower);
					ShowModelHilight(mSource, true, mCanNotSelectColor * mCanNotSelectColorPower);
				}
			}
		}


		//共有ボックス間の線の更新
		//

		if (mBeforeSelect != mSelect) {
			//選択されなくなったらアクティブでなくす
			if (IsShareWeightBox(mBeforeSelect)) {
				SetActiveShareLine(mMassShiftShareLineFromDest, false);
			}

			//選択されていたらアクティブに
			if (IsShareWeightBox(mSelect)) {
				SetActiveShareLine(mMassShiftShareLineFromDest, true);
			}
		}

		//もし共有ボックスが選択されていたら更新する
		if (IsShareWeightBox(mSelect)) {
			UpdateShareLine(mSelect, mMassShiftShareLineFromDest, false);
		}
		if (IsShareWeightBox(mSource)) {
			UpdateShareLine(mSource, mMassShiftShareLineToSource, true);
		}



		//選択されているオブジェクトがあるなら
		if (mSelect != null) {
			UpdateMassShiftLine(mMassShiftLine, mSource, mSelect);	//移す元から選択先へ

			//移す元から移せるなら
			if(CanShiftSource(mSource)) {
				ShowModelHilight(mSource, true, mCanSelectColor * mCanSelectColorPower);
			}
			else {
				ShowModelHilight(mSource, true, mCanNotSelectColor * mCanNotSelectColorPower);
			}

			//移す先へ移せるなら
			if(CanShiftDest(mSelect)) {
				ShowModelHilight(mSelect, true, mCanSelectColor * mCanSelectColorPower);
			}
			else {
				ShowModelHilight(mSelect, true, mCanNotSelectColor * mCanNotSelectColorPower);
			}
		}
		else {
			UpdateMassShiftLine(mMassShiftLine, mSource, mCursor);  //移す元からカーソルへ

			//移す元から移せるなら
			if (CanShiftSource(mSource)) {
				ShowModelHilight(mSource, true, mCanSelectColor * mCanSelectColorPower);
			}
			else {
				ShowModelHilight(mSource, true, mCanNotSelectColor * mCanNotSelectColorPower);
			}
		}

		//重さを移せるかなどのUIの位置を更新
		//
		if (mSource != null) {
			mStageShiftUI.GetSourceUI().SetPosition(mSource.transform.position);
		}
		if (mSelect != null) {
			mStageShiftUI.GetDestUI().SetPosition(mSelect.transform.position);
		}


		//移す操作がされないと
		if (!GetShiftButton()) {

			//移す先が存在しない場合
			if (mSelect == null) {
				ChangeState(CSelectState.cFail);    //失敗
				return;
			}

			//選択元から重さを移せるとき
			if (CanShiftSource(mSource)) {
				mDest = mSelect;

				//プレイヤーに、現在移している最中だと伝える
				PlayerIsShift(true);

				//共有ボックスなら
				if (mSource.GetComponent<ShareWeightBox>()) {
					ChangeState(CSelectState.cMoveFromShare);   //共有ボックスから重さが集まる状態へ
					ShowAllModelHilight(false, Color.white);
					GetComponent<HitStop>().StartHitStop();
					return;
				}
				else {
					ChangeState(CSelectState.cMoveSourceToDest);    //重さを移す状態へ
					ShowAllModelHilight(false, Color.white);
					GetComponent<HitStop>().StartHitStop();
					return;
				}
			}
			//選択元から重さを移せない時
			else {
				//失敗
				MassShiftFail(mSource);
				SoundManager.SPlay(mCantShiftSE);
				ChangeState(CSelectState.cFail);
				return;
			}
		}
	}

	//共有ボックスが移し元になり、他の共有ブロックの重さが移ってくる状態
	void UpdateMoveFromShare() {

		if (mInitState == true) {

			mInitState = false;

			//カーソルを移せない表示に
			ChangeCursorState(CCursorState.cCanNotShift);

			//移す線を非表示に
			mMassShiftLine.SetActive(false);

			//共有ボックス間の線を非表示に
			SetActiveShareLine(mMassShiftShareLineFromDest, false);
			SetActiveShareLine(mMassShiftShareLineToSource, false);

			//選択先のハイライトを消し、移し先のハイライトを表示
			ShowModelHilight(mSelect, false, Color.white);
			ShowModelHilight(mDest, true, mDestColor * mDestColorPower);

			//重さを移せるかのUIを消す
			mStageShiftUI.GetSourceUI().NotShow();
			mStageShiftUI.GetDestUI().NotShow();


			//共有ボックスの処理
			//

			ShareWeightBox lSourceShare = mSource.GetComponent<ShareWeightBox>();
			mLightBallShare.Clear();

			//共有ボックスの数だけ光の弾を生成
			foreach (var s in lSourceShare.GetShareAllListExceptOwn()) {

				GameObject l = Instantiate(mLightBallSharePrefab, transform);
				l.GetComponent<LightBall>().InitPoint(GetMassPosition(s.gameObject), GetMassPosition(mSource));
				l.GetComponent<LightBall>().PlayEffect();
				mLightBallShare.Add(l);

				s.GetComponent<WeightManager>().WeightLvSeem -= GetShiftWeight();	//見かけの重さを減らす
			}


			//全ての光る球の到達する時間は一定だ
			//移す元の共有ボックスと、他の共有ボックスの一番近い距離を求め、その時間で到達するように速度を変更する
			//
			float lMinDistance = float.MaxValue;
			foreach (var l in mLightBallShare) {
				LightBall lc = l.GetComponent<LightBall>();
				lMinDistance = Mathf.Min((lc.From - lc.To).magnitude, lMinDistance);
			}

			foreach (var l in mLightBallShare) {
				LightBall lc = l.GetComponent<LightBall>();
				float lSpeed = (lc.From - lc.To).magnitude / lMinDistance * mLightBallTemplate.GetComponent<LightBall>().mMoveSpeed;
				lc.mMoveSpeed = Mathf.Min(lSpeed, mLightBallTemplate.GetComponent<LightBall>().mMoveSpeed * 2);	//速度を制限する
			}

			SoundManager.SPlay(mShiftSourceSE);
		}


		//全ての光の弾が移し元へ到達するまで、光の弾の更新を続ける
		//

		var lShareList = mSource.GetComponent<ShareWeightBox>().GetShareAllListExceptOwn();

		bool lAllReach = true;
		for (int i = 0; i < mLightBallShare.Count; i++) {

			LightBall l = mLightBallShare[i].GetComponent<LightBall>();
			ShareWeightBox s = lShareList[i].GetComponent<ShareWeightBox>();
			l.SetPoint(GetMassPosition(s.gameObject), GetMassPosition(mSource));
			l.UpdatePoint();
			if (l.IsReached == false) {
				lAllReach = false;
			}
		}

		//全て届いたら
		if (lAllReach == true) {
			//光の弾を消去する
			foreach (var s in mLightBallShare) {
				DestroyLightBall(s);
			}
			ChangeState(CSelectState.cMoveSourceToDest);    //移し元から移し先へ移す状態へ
		}
	}


	//共有ボックスが移し先になり、他の共有ブロックへ重さを移す状態
	//
	void UpdateMoveToShare() {

		if (mInitState == true) {

			mInitState = false;


			//共有ボックスの処理
			//

			ShareWeightBox lDestShare = mDest.GetComponent<ShareWeightBox>();
			mLightBallShare.Clear();

			//共有ボックスの数だけ光の弾を生成
			foreach (var s in lDestShare.GetShareAllListExceptOwn()) {

				GameObject l = Instantiate(mLightBallSharePrefab, transform);
				l.GetComponent<LightBall>().InitPoint(GetMassPosition(mDest), GetMassPosition(s.gameObject));
				l.GetComponent<LightBall>().PlayEffect();
				mLightBallShare.Add(l);
			}


			//全ての光る球の到達する時間は一定だ
			//移す先の共有ボックスと、他の共有ボックスの一番近い距離を求め、その時間で到達するように速度を変更する
			//
			float lMinDistance = float.MaxValue;
			foreach (var l in mLightBallShare) {
				LightBall lc = l.GetComponent<LightBall>();
				lMinDistance = Mathf.Min((lc.From - lc.To).magnitude, lMinDistance);
			}

			foreach (var l in mLightBallShare) {
				LightBall lc = l.GetComponent<LightBall>();
				float lSpeed = (lc.From - lc.To).magnitude / lMinDistance * mLightBallTemplate.GetComponent<LightBall>().mMoveSpeed;
				lc.mMoveSpeed = Mathf.Min(lSpeed, mLightBallTemplate.GetComponent<LightBall>().mMoveSpeed * 2);    //速度を制限する
			}

			SoundManager.SPlay(mShiftDestSE);
		}


		//全ての光の弾が他の共有ボックスへ到達するまで、光の弾の更新を続ける
		//

		var lShareList = mDest.GetComponent<ShareWeightBox>().GetShareAllListExceptOwn();

		bool lAllReach = true;
		for (int i = 0; i < mLightBallShare.Count; i++) {

			LightBall l = mLightBallShare[i].GetComponent<LightBall>();
			ShareWeightBox s = lShareList[i].GetComponent<ShareWeightBox>();
			l.SetPoint(GetMassPosition(mDest), GetMassPosition(s.gameObject));
			l.UpdatePoint();
			if (l.IsReached == false) {
				lAllReach = false;
			}
		}

		//全て到達したら
		if (lAllReach == true) {

			//光の弾の削除
			foreach (var s in mLightBallShare) {
				DestroyLightBall(s);
			}

			foreach (var s in mDest.GetComponent<ShareWeightBox>().GetShareAllListExceptOwn()) {
				s.GetComponent<WeightManager>().WeightLvSeem += GetShiftWeight();
			}
			ChangeState(CSelectState.cSuccess); //成功状態へ
		}
	}


	//移し元から移し先へ、重さを移している状態
	//
	void UpdateMoveSourceToDest() {

		if (mInitState == true) {

			mInitState = false;

			//カーソルを移せない表示に
			ChangeCursorState(CCursorState.cCanNotShift);

			//移すのに表示する線を消す
			mMassShiftLine.SetActive(false);

			//共有ボックス間の線を非表示に
			SetActiveShareLine(mMassShiftShareLineFromDest, false);
			SetActiveShareLine(mMassShiftShareLineToSource, false);

			//選択先のハイライトを消し、移し先のハイライトを表示
			ShowModelHilight(mSelect, false, Color.white);
			ShowModelHilight(mDest, true, mDestColor * mDestColorPower);

			//重さを移せるかのUIを消す
			mStageShiftUI.GetSourceUI().NotShow();
			mStageShiftUI.GetDestUI().NotShow();

			//光の弾の生成と、設定
			//
			mLightBall = Instantiate(mLightBallPrefab, transform);

			//射線が通っているかを判別するのに、移し先と移し元を無視する
			mLightBall.GetComponent<LightBall>().mIgnoreList.Clear();
			mLightBall.GetComponent<LightBall>().mIgnoreList.Add(mSource);
			mLightBall.GetComponent<LightBall>().mIgnoreList.Add(mDest);

			mLightBall.GetComponent<LightBall>().InitPoint(GetMassPosition(mSource), GetMassPosition(mDest));
			mLightBall.GetComponent<LightBall>().PlayEffect();

			mSource.GetComponent<WeightManager>().WeightLvSeem -= GetShiftWeight(); //見かけの重さを減らす

			SoundManager.SPlay(mShiftSourceSE);
		}


		var lLightBall = mLightBall.GetComponent<LightBall>();

		//光の弾の位置の更新
		lLightBall.SetPoint(GetMassPosition(mSource), GetMassPosition(mDest));
		lLightBall.UpdatePoint();


		//もし障害物に当たっていたら
		if (lLightBall.IsHit) {
			SoundManager.SPlay(mCancelShiftSE);
			GenerateShiftFailedEffect(lLightBall.mHitPosition, lLightBall.mHitDirection);
			ChangeState(CSelectState.cReturnToSource);
			return;
		}

		//もし移し先へ到達していたら
		if (lLightBall.IsReached) {
			
			//移せるなら
			if (CanShiftDest(mDest) && CanShiftShare(mSource, mDest)) {

				//それが共有ボックスなら
				if (mDest.GetComponent<ShareWeightBox>()) {
					ChangeState(CSelectState.cMoveToShare); //他の共有ボックスへ重さを伝播する
					return;
				}
				else {
					ChangeState(CSelectState.cSuccess); //成功
					GetComponent<HitStop>().StartHitStop();
					return;
				}
			}
			//移せないなら
			else {
				MassShiftFail(mDest);
				ChangeState(CSelectState.cReturnToSource);  //移し元へ光の弾は帰っていく
				SoundManager.SPlay(mCantShiftSE);
				GenerateShiftFailedEffect(lLightBall);
				return;
			}
		}
	}


	//移すのに成功した状態
	//
	void UpdateSuccess()
	{
		ShareWeightBox lSourceShare = mSource.GetComponent<ShareWeightBox>();
		ShareWeightBox lDestShare = mDest.GetComponent<ShareWeightBox>();
		
		mSource.GetComponent<WeightManager>().PushWeight(mDest.GetComponent<WeightManager>(), GetShiftWeight());
		

		//共有ボックスなら、他の共有ボックスに重さを伝える
		//

		//もし移し元が共有ボックスなら
		if (lSourceShare != null) {
			foreach (var s in lSourceShare.GetShareAllListExceptOwn()) {
				s.GetComponent<WeightManager>().WeightLv = mSource.GetComponent<WeightManager>().WeightLv;
			}
		}

		//もし移し先が共有ボックスなら
		if (lDestShare != null) {
			foreach (var s in lDestShare.GetShareAllListExceptOwn()) {
				s.GetComponent<WeightManager>().WeightLv = mDest.GetComponent<WeightManager>().WeightLv;
			}
		}

		DestroyLightBall(mLightBall);	//光の弾を消去する
		ChangeState(CSelectState.cNormal);  //通常状態へ

		SoundManager.SPlay(mShiftDestSE);

		mFromLastShiftTime = 0.0f;
	}


	//重さを移すのに失敗して、移し元へ光の弾が戻っていく状態
	//
	void UpdateReturnToSource() {

		if (mInitState == true) {

			mInitState = false;

			//移し元のハイライトを表示して、移し先のハイライトを消す
			ShowModelHilight(mSource, true, mSourceColor * mSourceColorPower);
			ShowModelHilight(mDest, false, Color.white);

			//光の弾を戻す
			mLightBall.GetComponent<LightBall>().InitPoint(GetMassPosition(mLightBall), GetMassPosition(mSource));
			mLightBall.GetComponent<LightBall>().PlayEffect();
		}

		//光の弾の更新
		mLightBall.GetComponent<LightBall>().SetPoint(mLightBall.GetComponent<LightBall>().From, GetMassPosition(mSource));
		mLightBall.GetComponent<LightBall>().UpdatePoint();

		//光の弾が移し元へ到達したら
		if (mLightBall.GetComponent<LightBall>().IsReached) {

			mSource.GetComponent<WeightManager>().WeightLvSeem += GetShiftWeight();

			DestroyLightBall(mLightBall);
			if (mSource.GetComponent<ShareWeightBox>()) {
				ChangeState(CSelectState.cReturnToShare);
			}
			else {
				ChangeState(CSelectState.cFail);
				SoundManager.SPlay(mShiftSourceSE);
			}
		}
	}


	//重さを移すのに失敗して、移し元から共有ブロックへ光の弾が戻っていく状態
	//
	void UpdateReturnToShare() {

		if (mInitState == true)
		{

			mInitState = false;


			//共有ボックスの処理
			//

			ShareWeightBox lSourceShare = mSource.GetComponent<ShareWeightBox>();
			mLightBallShare.Clear();

			//共有ボックスの数だけ光の弾を生成
			foreach (var s in lSourceShare.GetShareAllListExceptOwn()) {

				GameObject l = Instantiate(mLightBallSharePrefab, transform);
				l.GetComponent<LightBall>().InitPoint(GetMassPosition(mSource), GetMassPosition(s.gameObject));
				l.GetComponent<LightBall>().PlayEffect();
				mLightBallShare.Add(l);
			}


			//全ての光る球の到達する時間は一定だ
			//移す先の共有ボックスと、他の共有ボックスの一番近い距離を求め、その時間で到達するように速度を変更する
			//
			float lMinDistance = float.MaxValue;
			foreach (var l in mLightBallShare) {
				LightBall lc = l.GetComponent<LightBall>();
				lMinDistance = Mathf.Min((lc.From - lc.To).magnitude, lMinDistance);
			}

			foreach (var l in mLightBallShare) {
				LightBall lc = l.GetComponent<LightBall>();
				float lSpeed = (lc.From - lc.To).magnitude / lMinDistance * mLightBallTemplate.GetComponent<LightBall>().mMoveSpeed;
				lc.mMoveSpeed = Mathf.Min(lSpeed, mLightBallTemplate.GetComponent<LightBall>().mMoveSpeed * 2);    //速度を制限する
			}

			SoundManager.SPlay(mShiftSourceSE);
		}


		//全ての光の弾が他の共有ボックスへ到達するまで、光の弾の更新を続ける
		//

		var lShareList = mSource.GetComponent<ShareWeightBox>().GetShareAllListExceptOwn();

		bool lAllReach = true;
		for (int i = 0; i < mLightBallShare.Count; i++) {

			LightBall l = mLightBallShare[i].GetComponent<LightBall>();
			ShareWeightBox s = lShareList[i].GetComponent<ShareWeightBox>();
			l.SetPoint(GetMassPosition(mSource), GetMassPosition(s.gameObject));
			l.UpdatePoint();
			if (l.IsReached == false) {
				lAllReach = false;
			}
		}

		//全て到達したら
		if (lAllReach == true) {

			//光の弾の削除
			foreach (var s in mLightBallShare) {
				DestroyLightBall(s);
			}

			//見かけの重さを戻す
			foreach (var s in mSource.GetComponent<ShareWeightBox>().GetShareAllListExceptOwn()) {
				s.GetComponent<WeightManager>().WeightLvSeem += GetShiftWeight();
			}
			ChangeState(CSelectState.cFail);    //失敗状態へ
			SoundManager.SPlay(mShiftSourceSE);
		}

	}


	//重さを移すのに失敗した状態
	//
	void UpdateFail() {

		if (mInitState == true) {

			mInitState = false;

			//ハイライトを全て消す
			ShowAllModelHilight(false, Color.white);
			ShowModelHilight(mSelect, false, Color.white);
			ShowModelHilight(mSource, false, Color.white);
			ShowModelHilight(mDest, false, Color.white);

			//移す表示の線を消す
			mMassShiftLine.SetActive(false);

			//共有ボックス間の線を非表示に
			SetActiveShareLine(mMassShiftShareLineFromDest, false);
			SetActiveShareLine(mMassShiftShareLineToSource, false);

			//重さを移せるかのUIを消す
			mStageShiftUI.GetSourceUI().NotShow();
			mStageShiftUI.GetDestUI().NotShow();


			ChangeCursorState(CCursorState.cCanNotShift);
		}

		//重さを移すボタンが押されていないなら
		if (GetShiftButton() == false) {
			ChangeState(CSelectState.cNormal);
		}
	}


	//重さを移せない状態
	//
	void UpdateCantShift() {

		if (mInitState == true) {

			mInitState = false;

			//ハイライトを全て消す
			ShowAllModelHilight(false, Color.white);
			ShowModelHilight(mSelect, false, Color.white);
			ShowModelHilight(mSource, false, Color.white);
			ShowModelHilight(mDest, false, Color.white);

			//移す表示の線を消す
			mMassShiftLine.SetActive(false);

			ChangeCursorState(CCursorState.cCanNotShift);
		}

		//外部からCanShiftにtrueを入れられないと、この状態からは変化しない
	}


	//現在のカーソルの位置で、SelectAreaが反応するWeightObjectを返す
	//
	GameObject[] GetNearWeightObject() {
		Transform t = mCursor.transform.Find("Collider");
		LayerMask l = LayerMask.GetMask(new string[] { "SelectArea" });
		return Physics.OverlapBox(t.position, t.localScale / 2, t.rotation, l).Select(x => x.transform.parent.gameObject).ToArray();
	}
	//引数のGameObjectの中で、カーソルから一番距離の近いものを返す
	//
	GameObject GetNearestObject(GameObject[] aGameObject) {
		return GetNearestObject(aGameObject, null);
	}
	//引数のGameObjectの中で、カーソルから一番距離の近いものを返す（aRemoveは無視する）
	//
	GameObject GetNearestObject(GameObject[] aGameObject, GameObject aRemove) {

		float lMinDistQuad = float.MaxValue;
		GameObject lRes = null;

		foreach (var t in aGameObject) {
			if (t == aRemove) continue; //無視する

			float lDistQuad = (mCursor.transform.position - t.transform.position).sqrMagnitude;
			if (lDistQuad <= lMinDistQuad) {
				//一番近いオブジェクトと距離を更新
				lRes = t;
				lMinDistQuad = lDistQuad;
			}
		}
		return lRes;
	}


	//重さを移す状態遷移
	//
	enum CSelectState {

		cNormal,
		cClick,
		cDrag,
		cMoveFromShare,
		cMoveToShare,
		cMoveSourceToDest,
		cSuccess,
		cReturnToSource,
		cReturnToShare,
		cFail,
		cCantShift
	}

	CSelectState mState;	//現在の状態

	CSelectState mBeforeState;	//以前の状態
	bool mInitState = true;	//その状態に来て初めてのフレームか
	

	GameObject mSource; //うつし元
	GameObject mDest;   //うつし先

	GameObject mBeforeSelect; //現在選択している奴
	GameObject mSelect; //現在選択している奴

	bool mShiftDouble;  //重さを2つ移すモードか

	float mFromLastShiftTime = 0.0f;	//最後に重さを移してから何秒経過しているか
	public float FromLastShiftTime {
		get {
			return mFromLastShiftTime;
		}
	}

	StageShiftUI mStageShiftUI;


	//
	//カーソルの状態
	enum CCursorState {
		cCanNotShift,	//物を持っているときなど、移せない時
		cNormal,	//移せる。何も操作していない時
		cShotLineThrough,	//選択時、射線が通っている
		cShotLineNotThrough,	//選択時、射線が通っていない
		cInvisible,	//カーソルが見えない
	}

	[SerializeField, EditOnPrefab]
	GameObject mMassShiftLinePrefab;

	GameObject mMassShiftLine;


	[SerializeField, EditOnPrefab]
	GameObject mMassShiftShareLinePrefab;

	List<GameObject> mMassShiftShareLineToSource;   //移す元の共有ボックスからその他へ
	List<GameObject> mMassShiftShareLineFromDest;   //移す先の共有ボックスからその他へ

	[SerializeField]
	GameObject mMassShiftShareLineParent;


	[SerializeField, EditOnPrefab]
	GameObject mCursorPrefab;

	GameObject mCursor;

	[SerializeField, EditOnPrefab, Tooltip("重さを移す玉")]
	GameObject mLightBallPrefab;

	[SerializeField, EditOnPrefab, Tooltip("共有ボックスの、重さを移す玉")]
	GameObject mLightBallSharePrefab;

	GameObject mLightBallTemplate;	//ひな型としてインスタンス化しておく
	GameObject mLightBall;	//重さを移すときに使う
	List<GameObject> mLightBallShare = new List<GameObject>();  //共有ブロック間で移すときに使う


	[SerializeField, EditOnPrefab, Tooltip("重さを移すのに失敗したときのエフェクト")]
	GameObject mShiftFailedEffectPrefab;

	[SerializeField]
	Color mCanSelectColor;

	[SerializeField]
	float mCanSelectColorPower = 1.0f;

	[SerializeField]
	Color mCanNotSelectColor;

	[SerializeField]
	float mCanNotSelectColorPower = 1.0f;

	[SerializeField]
	Color mSourceColor;

	[SerializeField]
	float mSourceColorPower = 1.0f;

	[SerializeField]
	Color mDestColor;

	[SerializeField]
	float mDestColorPower = 1.0f;


	#region Input

	void UpdateClickRightClick() {

		if (mInitState == true) {

			mInitState = false;
			mShiftDouble = false;

			//重さを移す表示の線を出す
			mMassShiftLine.SetActive(true);

			//移し元のハイライトを表示し、選択中のハイライトを消す
			ShowModelHilight(mSelect, false, Color.white);
			ShowModelHilight(mSource, true, mSourceColor * mSourceColorPower);

			//カーソルの種類を変更
			ChangeCursorState(CCursorState.cShotLineThrough);

			mBeforeSelect = null;
			mSelect = null;


			//ダブル選択はできない仕様なので、この後の処理をスキップ
			UpdateMassShiftLine(mMassShiftLine, mSource, mCursor);
			ChangeState(CSelectState.cDrag);    //ドラッグの状態へ
			return;
		}

		//重さを移す表示の線を出す
		UpdateMassShiftLine(mMassShiftLine, mSource, mCursor);

		//カーソルから一番近いオブジェクトが、移し元ではなくなると
		if (GetNearestObject(GetNearWeightObject()) != mSource) {
			ChangeState(CSelectState.cDrag);	//ドラッグの状態へ
			return;
		}

		//右クリックが押されると
		if (GetDoubleShiftButton()) {
			//移し元の重さが2なら
			if (mSource.GetComponent<WeightManager>().WeightLv == WeightManager.Weight.heavy) {
				mShiftDouble = true;	//2つ移すモードへ
			}
		}
		//右クリックが離されると
		else {
			mShiftDouble = false;
		}

		//移すボタンが押されなくなると
		if (!GetShiftButton()) {
			ChangeState(CSelectState.cFail);	//失敗
		}
	}

	bool GetShiftButton() {

		if (Utility.IsJoystickConnect()) {
			return Input.GetAxis("JoyShift") >= mShiftOnValue;
		}
		return Input.GetKey(KeyCode.Mouse0);
	}
	bool GetDoubleShiftButton() {
		return Input.GetKey(KeyCode.Mouse1);
	}

	float GetShiftXAxis() {
		if (Utility.IsJoystickConnect()) {
			//return Input.GetAxis("JoyShiftHorizontal");
			return 0.0f;
		}
		else {
			if (Input.GetKey(KeyCode.J)) return -1.0f;
			if (Input.GetKey(KeyCode.L)) return 1.0f;
		}
		return 0.0f;
	}
	float GetShiftYAxis()
	{
		if (Utility.IsJoystickConnect()) {
			//return Input.GetAxis("JoyShiftVertical");
			return 0.0f;
		}
		else {
			if (Input.GetKey(KeyCode.I)) return 1.0f;
			if (Input.GetKey(KeyCode.K)) return -1.0f;
		}
		return 0.0f;
	}

	void MoveCursor() {
		//ジョイスティックが接続されていたら
		if (Utility.IsJoystickConnect()) {
			MoveCursorByJoistick(); //カーソルで動かす
		}
		else {
			MoveCursorByMouse();	//マウスで動かす
		}
	}

	//マウスでカーソルを動かす
	//
	void MoveCursorByMouse() {

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Plane plane = new Plane(new Vector3(0.0f, 0.0f, -1.0f), 0.0f);
		//FindObjectOfType<TextDebug>().SetText("RayOrigin:" + ray.origin);

		float enter = 0.0f;
		if (plane.Raycast(ray, out enter)) {
			Vector3 lPosition = ray.GetPoint(enter);
			mCursor.transform.position = ClampCursorInScreen(lPosition);
		}
	}

	//カーソルを画面内に収める
	//
	Vector3 ClampCursorInScreen(Vector3 aPosition) {

		Vector3 lViewportPoint = Camera.main.WorldToViewportPoint(aPosition);

		//Debug.Log("ViewportPoint:" + lViewportPoint);

		const float cXOffset = 0.02f;
		float cYOffset = cXOffset * Screen.width / Screen.height;

		lViewportPoint.x = Mathf.Clamp(lViewportPoint.x , 0.0f + cXOffset, 1.0f - cXOffset);
		lViewportPoint.y = Mathf.Clamp(lViewportPoint.y, 0.0f + cYOffset, 1.0f - cYOffset);

		Vector3 lWorldPoint = Camera.main.ViewportToWorldPoint(lViewportPoint);
		return lWorldPoint;
	}


	[SerializeField, Tooltip("カーソルの横方向の移動速度"), EditOnPrefab]
	float mCursorMoveXSpeed = 10.0f;

	[SerializeField, Tooltip("カーソルの縦方向の移動速度"), EditOnPrefab]
	float mCursorMoveYSpeed = 10.0f;

	[SerializeField, Tooltip("重さを移す操作が有効になる、トリガーの最低入力値"), EditOnPrefab]
	float mShiftOnValue = 0.8f;


	bool _mInvisibleCursor = false;

	//カーソルを見えなくする
	public bool mInvisibleCursor {
		get {
			return _mInvisibleCursor || _Debug_mInvisibleCursor;
		}
		set {
			_mInvisibleCursor = value;
		}
	}

	[SerializeField, Tooltip("カーソルを見えなくする（デバッグ）")]
	bool _Debug_mInvisibleCursor = false;


	//ジョイスティックでカーソルを動かす
	//
	void MoveCursorByJoistick() {

		//移動量の計算
		Vector3 lMoveDelta = new Vector3();
		lMoveDelta.x = GetShiftXAxis() * Time.deltaTime * mCursorMoveXSpeed;
		lMoveDelta.y = GetShiftYAxis() * Time.deltaTime * mCursorMoveYSpeed;


		//移動先の座標が画面内に収まっているかの判定
		Vector3 lNewPosition = mCursor.transform.position + lMoveDelta;
		Vector3 lNewPositionInScreen = Camera.main.WorldToViewportPoint(lNewPosition);

		//収まっていなかったら、元の位置に戻す
		if (-1.0f <= lNewPositionInScreen.x && lNewPositionInScreen.x <= 1.0f) {
			if (-1.0f <= lNewPositionInScreen.y && lNewPositionInScreen.y <= 1.0f) {
				mCursor.transform.position = lNewPosition;
			}
		}
	}

#endregion


	//重さを移す場所を取得する
	//
	Vector3 GetMassPosition(GameObject aGameObject) {

		//プレイヤー用
		Transform lMassPosition = aGameObject.transform.Find("Offset/RotOffset/Rotation/ModelOffset/WeightPosition");
		if (lMassPosition != null) {
			return lMassPosition.position;
		}

		//その他のボックス用
		lMassPosition = aGameObject.transform.Find("WeightParticle");
		if (lMassPosition != null) {
			return lMassPosition.position;
		}

		return aGameObject.transform.position;
	}


	//各オブジェクトの、重さを移すのに失敗したときの動作を呼び出す
	//
	void MassShiftFail(GameObject aTarget) {
		if (aTarget == null) return;
		MassShiftFailed m = aTarget.GetComponent<MassShiftFailed>();
		if (m == null) return;
		m.MassShiftFail();
	}

	//重さを移すのに失敗したときのエフェクトを生成する
	//
	void GenerateShiftFailedEffect(Vector3 aPosition, Vector3 aDirection) {
		var g = Instantiate(mShiftFailedEffectPrefab, transform);
		g.transform.position = aPosition;
		g.transform.rotation = Quaternion.FromToRotation(Vector3.up, aDirection);
	}

	void GenerateShiftFailedEffect(LightBall aLightBall) {
		GenerateShiftFailedEffect(aLightBall.transform.position, aLightBall.To - aLightBall.From);
	}


	//重さを移す表示線の更新
	//
	void UpdateMassShiftLine(GameObject aMassShiftLine, GameObject aFrom, GameObject aTo) {

		MassShiftLine lMassShiftLine = aMassShiftLine.GetComponent<MassShiftLine>();

		Vector3 lFromPosition = GetMassPosition(aFrom);
		Vector3 lToPosition = GetMassPosition(aTo);

		//重さを移す表示の線の、位置を更新
		lMassShiftLine.SetLinePosition(lFromPosition, lToPosition);

		
		//移せるかどうかの判別
		//

		//射線が通っているか
		bool lIsThrough = IsThrough(aFrom, aTo);


		//移し元から移せるか
		bool lCanShiftSource = CanShiftSource(aFrom);

		//移し先に移せるか
		bool lCanShiftDest = CanShiftDest(aTo) || aTo.GetComponent<WeightManager>() == null;	//移し先がカーソルならtrueになるように

		//移し元と移し先が共有ボックスでないか
		bool lCanShiftShare = CanShiftShare(aFrom, aTo);

		//移せるなら
		if (lIsThrough && lCanShiftSource && lCanShiftDest && lCanShiftShare) {
			//線とカーソルを、射線が通っているときの色にする
			lMassShiftLine.ChangeColor(mCanSelectColor * mCanSelectColorPower);
			ChangeCursorState(CCursorState.cShotLineThrough);
			lMassShiftLine.UpdatePosition();    //線を移動させる
		}
		else {
			//通っていない色にする
			lMassShiftLine.ChangeColor(mCanNotSelectColor * mCanNotSelectColorPower);
			ChangeCursorState(CCursorState.cShotLineNotThrough);
		}
	}

	bool IsThrough(GameObject aFrom, GameObject aTo) {
		return mLightBallTemplate.GetComponent<LightBall>().ThroughShotLine(GetMassPosition(aFrom), GetMassPosition(aTo), new GameObject[] { aFrom, aTo }.ToList());
	}


	void ChangeCursorState(CCursorState aCursorState) {

		GameObject lNormal = mCursor.transform.Find("Model/Normal").gameObject;
		GameObject lSelect = mCursor.transform.Find("Model/Select").gameObject;

		if(mInvisibleCursor) {
			aCursorState = CCursorState.cInvisible;
		}

		if (aCursorState == CCursorState.cNormal) {
			lNormal.SetActive(false);
			lSelect.SetActive(true);
			lSelect.GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.white);
		}
		else if(aCursorState == CCursorState.cCanNotShift) {
			lNormal.SetActive(false);
			lSelect.SetActive(true);
			lSelect.GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.white * 0.25f);
		}
		else if (aCursorState == CCursorState.cShotLineThrough) {
			lNormal.SetActive(false);
			lSelect.SetActive(true);
			lSelect.GetComponentInChildren<Renderer>().material.SetColor("_Color", mCanSelectColor * mCanSelectColorPower);
		}
		else if (aCursorState == CCursorState.cShotLineNotThrough) {
			lNormal.SetActive(false);
			lSelect.SetActive(true);
			lSelect.GetComponentInChildren<Renderer>().material.SetColor("_Color", mCanNotSelectColor * mCanNotSelectColorPower);
		}
		else if (aCursorState == CCursorState.cInvisible) {
			lNormal.SetActive(false);
			lSelect.SetActive(false);
		}
	}
	

	//そのモデルのハイライトを表示・非表示にしたり、色を変更する
	//
	void ShowAllModelHilight(bool aIsShow, Color aColor) {

		foreach(var w in mAllWeightModel) {
			ShowModelHilight(w.gameObject, aIsShow, aColor);
		}
	}
	WeightManager[] mAllWeightModel;

	Player mPlayer;

	//そのモデルのハイライトを表示・非表示にしたり、色を変更する
	//
	void ShowModelHilight(GameObject aModel, bool aIsShow, Color aColor) {

		if (aModel == null) return;

		Transform lFrame = aModel.transform.Find("Model/Hilight");
		if (lFrame == null) {
			lFrame = aModel.transform.Find("Offset/RotOffset/Rotation/ModelOffset/Model/CameraLook/Hilight");   //プレイヤー用
			if (lFrame == null) return;
		}

		if (aIsShow == false) {
			for(int i = 0; i < lFrame.childCount; i++) {
				lFrame.GetChild(i).gameObject.SetActive(false);
			}
		}
		else {
			for (int i = 0; i < lFrame.childCount; i++) {
				lFrame.GetChild(i).gameObject.SetActive(true);
			}
			Utility.ChangeMaterialColor(lFrame.gameObject, null, "_Color", aColor);
		}
	}


	//重さを移すときに表示する、光の弾を消す
	//
	void DestroyLightBall(GameObject g) {
		g.GetComponent<LightBall>().StopEffect();
		Destroy(g, 1.0f);
	}

	//その移し元から、重さを移せるか
	//
	bool CanShiftSource(GameObject aSource) {
		if (aSource == null) {
			return false;
		}

		WeightManager lWM = aSource.GetComponent<WeightManager>();
		if(lWM == null) {
			return false;
		}

		//重さが0なら移せない
		if (lWM.WeightLv == WeightManager.Weight.flying) {
			return false;
		}

		return true;
	}

	//その移し元から移し先へ、重さを移せるか
	//
	bool CanShiftDest(GameObject aDest) {

		if (aDest == null) {
			return false;
		}

		WeightManager lDestWM = aDest.GetComponent<WeightManager>();
		if(lDestWM == null) {
			return false;	//どちらかがWeightManagerを持たないなら移せない
		}

		//移し先のボックスの重さが2なら
		if (lDestWM.WeightLv == WeightManager.Weight.heavy) {
			return false;	//移せない
		}

		return true;
	}


	//共有ボックスの組み合わせかどうか
	//
	bool CanShiftShare(GameObject aSource, GameObject aDest) {

		if (aSource == null) {
			return false;
		}
		if (aDest == null) {
			return false;
		}

		ShareWeightBox lSourceShare = aSource.GetComponent<ShareWeightBox>();
		ShareWeightBox lDestShare = aDest.GetComponent<ShareWeightBox>();

		//両方とも共有ボックスで
		if (lSourceShare != null && lDestShare != null) {
			//同じ共有グループなら
			if (lSourceShare.IsShare(lDestShare)) {
				return false;   //移せない
			}
		}

		return true;
	}


	//どれだけの重さを移すのか
	int GetShiftWeight() {
		if(mShiftDouble) {
			return 2;
		}
		return 1;
	}



	//共有同士で引く線の数を変更する
	//
	void ResizeShareLine() {

		//既に作成されているインスタンスを削除
		for(int i = mMassShiftShareLineParent.transform.childCount - 1; i >= 0; i--) {
			Destroy(mMassShiftShareLineParent.transform.GetChild(i).gameObject);
		}
		
		//リストのクリア
		mMassShiftShareLineToSource = new List<GameObject>();
		mMassShiftShareLineFromDest = new List<GameObject>();

		var lShare = FindObjectsOfType<ShareWeightBox>();
		ResizeShareLine(mMassShiftShareLineToSource, lShare.Length - 1);
		ResizeShareLine(mMassShiftShareLineFromDest, lShare.Length - 1);
	}
	void ResizeShareLine(List<GameObject> aList, int aCount) {

		for (int i = 0; i < aCount; i++) {
			GameObject g = Instantiate(mMassShiftShareLinePrefab, mMassShiftShareLineParent.transform);
			aList.Add(g);
		}

		SetColorShareLine(aList, Color.cyan * 1.2f);
		SetActiveShareLine(aList, false);
	}

	//共有同士で引く線のアクティブを変える
	//
	void SetActiveShareLine(List<GameObject> aShareLineList, bool aIsActive) {
		for (int i = 0; i < aShareLineList.Count; i++) {
			aShareLineList[i].SetActive(aIsActive);
		}
	}

	//共有同士で引く線の色を変える
	//
	void SetColorShareLine(List<GameObject> aShareLineList, Color aColor) {
		for (int i = 0; i < aShareLineList.Count; i++) {
			aShareLineList[i].GetComponent<MassShiftLine>().ChangeColor(aColor);
		}
	}

	//共有同士で引く線を更新する
	//
	void UpdateShareLine(GameObject aShareWeightBox, List<GameObject> aShareLineList, bool aToSource) {
		ShareWeightBox s = aShareWeightBox.GetComponent<ShareWeightBox>();

		var lShareList = s.GetShareAllListExceptOwn();
		for (int i = 0; i < lShareList.Count; i++) {
			MassShiftLine lLine = aShareLineList[i].GetComponent<MassShiftLine>();

			//線の始めと終わりを求める
			Vector3 lLineStart = lShareList[i].transform.position;
			Vector3 lLineEnd = s.transform.position;

			//もし移し元に行く線ではないなら
			//移し先からの線なので、線の始めと終わりを入れ替える
			if(aToSource == false) {
				Vector3 t = lLineEnd;
				lLineEnd = lLineStart;
				lLineStart = t;
			}

			lLine.SetLinePosition(lLineStart, lLineEnd);
			lLine.UpdatePosition();
		}
	}

	//共有ボックスかどうか
	//
	bool IsShareWeightBox(GameObject aObject) {
		if (aObject == null) return false;
		return aObject.GetComponent<ShareWeightBox>() != null;
	}


	//重くなった後どちらに行くか
	float MoveAfterHeavy(GameObject aObject) {
		return -1.0f;
	}
	//軽くなった後どちらに行くか
	float MoveAfterLight(GameObject aObject) {
		return 1.0f;
	}


	#region SE List

	[SerializeField]
	GameObject mCantShiftSE;

	[SerializeField]
	GameObject mCancelShiftSE;

	[SerializeField]
	GameObject mShiftSourceSE;

	[SerializeField]
	GameObject mShiftDestSE;

#endregion



	#region CanShift

	bool _mCanShift = true;
	bool _mPlayerCanShift = true;

	//重さを移せる状態かどうか
	//
	public bool CanShift {
		get {
			return _mCanShift && _mPlayerCanShift;	//両方がtrueでないと移せない
		}
		set {
			_mCanShift = value;
		}
	}

#endregion



}
