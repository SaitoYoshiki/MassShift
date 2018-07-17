using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectManager : MonoBehaviour {

	[SerializeField]
	List<Goal> mGoal;

	[SerializeField]
	List<TextMesh> mText;

	Player mPlayer;

	[SerializeField, EditOnPrefab]
	GameObject mStageSelectBGMPrefab;

	[SerializeField]
	Color mStagePlateOnColor;

	[SerializeField]
	Color mStagePlateOffColor;

    [SerializeField]
    Color mStagePlateClearedColor;

	StageTransition mTransition;
	Pause mPause;
	StageSelectScroll mStageSelectScroll;

    [SerializeField]
    MoveTransform mCameraMove;

	[SerializeField]
	StageSelectEnterUI mEnterUI;

	GoalBlackCurtain mGoalBlack;

	[SerializeField]
	GameObject mTopStaticWeightBox;

	[SerializeField]
	GameObject mBottomStaticWeightBox;


	[SerializeField]
	float mToStageBeforeRotateTime = 0.0f;

	[SerializeField]
	float mToStageRotateTime = 0.5f;

	[SerializeField]
	float mToStageBeforeWalkingTime = 0.5f;

	[SerializeField]
	float mToStageWalkingTime = 1.0f;

	[SerializeField]
	float mToStageAfterWalkingTime = 0.5f;


	[SerializeField]
	float mFromStageBeforeWalkingTime = 0.0f;

	[SerializeField]
	float mFromStageWalkingTime = 1.0f;

	[SerializeField]
	float mFromStageBeforeRotateTime = 0.5f;

	[SerializeField]
	float mFromStageRotateTime = 0.5f;

	[SerializeField]
	float mFromStageAfterRotateTime = 0.5f;


	
	int mSelectStageNum = -1;
	float mSelectTime = 0.0f;   //選び続けている秒数
	bool mSelectInit = false;

	bool mFromTitle;


	// Use this for initialization
	void Start() {

		mPlayer = FindObjectOfType<Player>();
		mTransition = FindObjectOfType<StageTransition>();
		mStageSelectScroll = FindObjectOfType<StageSelectScroll>();

		mPause = FindObjectOfType<Pause>();

		mGoalBlack = FindObjectOfType<GoalBlackCurtain>();

		Time.timeScale = 1.0f;
		mPause.pauseEvent.Invoke();

		//ゲーム進行のコルーチンを開始
		//
		mFromTitle = cameraMove.fromTitle;
		cameraMove.fromTitle = false;

		//タイトル用の演出のコルーチン
		if (Area.sBeforeAreaNumber == 0 || mFromTitle) {
			StartCoroutine(StageSelectMain_FromTitle());
		}
		//ステージからの演出のコルーチン
		else {
			StartCoroutine(StageSelectMain_FromStage());
		}
	}

	// Update is called once per frame
	void Update() {

	}

	private void FixedUpdate() {
		//ゴール判定
		//
		mSelectStageNum = -1;
		for (int i = 0; i < mGoal.Count; i++) {
			if (CanEnter(mGoal[i])) {
				mSelectStageNum = i;
				break;
			}
		}
	}


	//ステージから来た時の演出
	//
	IEnumerator StageSelectMain_FromStage() {

		//重さを移せないようにする
		OnCanShiftOperation(false);
		mPause.canPause = false;

		//プレートの色を変える
		SetEnterColor((Area.sBeforeAreaNumber - 1) * 5 + (Area.sBeforeStageNumber - 1));


		//ズーム終了後のカメラ位置を変更
		mCameraMove.mEndPosition = mStageSelectScroll.mAreaCameraPosition[Area.sBeforeAreaNumber - 1].transform.position;

		//プレイヤーの位置を変更
		Goal g = mGoal[(Area.sBeforeAreaNumber - 1) * 5 + (Area.sBeforeStageNumber - 1)];
		Vector3 lNewPlayerPosition = g.transform.position;
		lNewPlayerPosition += g.transform.rotation * Vector3.up * 1.0f;
		lNewPlayerPosition.z = 0.0f;
		mPlayer.transform.position = lNewPlayerPosition;


		//カメラの開始地点をプレイヤーにズームしたところからにする
		mCameraMove.mStartPosition = GetPlayerZoomCameraPosition(g.transform.up.y <= 0.0f);


		//逆向きの天井なら
		if (Area.sBeforeAreaNumber == 2) {
			//プレイヤーのデフォルト位置を逆向きに
			mPlayer.GetComponent<Player>().InitRotation();
		}

		//カメラを見ていないようにする
		mPlayer.CameraLookRatio = 0.0f;
		mPlayer.LookCamera();


		//戻ってきたエリアによって、プレイヤーとボックスの重さを調整
		if (Area.sBeforeAreaNumber == 1) {
			mPlayer.GetComponent<WeightManager>().WeightLv = WeightManager.Weight.light;
			mTopStaticWeightBox.GetComponent<WeightManager>().WeightLv = WeightManager.Weight.flying;
			mBottomStaticWeightBox.GetComponent<WeightManager>().WeightLv = WeightManager.Weight.heavy;
		}
		if (Area.sBeforeAreaNumber == 2) {
			mPlayer.GetComponent<WeightManager>().WeightLv = WeightManager.Weight.flying;
			mTopStaticWeightBox.GetComponent<WeightManager>().WeightLv = WeightManager.Weight.light;
			mBottomStaticWeightBox.GetComponent<WeightManager>().WeightLv = WeightManager.Weight.heavy;
		}
		if (Area.sBeforeAreaNumber == 3) {
			mPlayer.GetComponent<WeightManager>().WeightLv = WeightManager.Weight.heavy;
			mTopStaticWeightBox.GetComponent<WeightManager>().WeightLv = WeightManager.Weight.flying;
			mBottomStaticWeightBox.GetComponent<WeightManager>().WeightLv = WeightManager.Weight.light;
		}


		//エリアの開放状況によって、ボックスを消す
		HideBoxByAreaOpen();


		//カメラをズームされた位置に移動
		mCameraMove.MoveStartPosition();


		mPlayer.transform.position += new Vector3(0.0f, 0.0f, 3.0f);    //少し奥に移動


		//プレイヤーを移動不可にする
		CanMovePlayer(false);
		CanJumpPlayer(false);

		OnPlayerEffect(true);   //プレイヤーの更新を切る


		//ステージ開始時の演出
		mTransition.OpenDoorParent();

		//演出が終了するまで待機
		while (true) {
			if (mTransition.GetOpenEnd()) break;
			yield return null;
		}



		//
		//プレイヤーがドアから出てくる演出
		//
		yield return WalkExitDoor(g);



		//BGMを流し始める
		var t = SoundManager.SPlay(mStageSelectBGMPrefab);
		SoundManager.SFade(t, 0.0f, SoundManager.SVolume(mStageSelectBGMPrefab), 2.0f);


		//
		//エリア開放イベント
		//

		//エリア2が開放されているが、エリア2の開放イベントが行われていない場合
		if(Area.CanGoArea(2) && SaveData.Instance.mEventDoneFlag.mArea2Open == false) {
			yield return AreaOpenEvent(2);
			SaveData.Instance.mEventDoneFlag.mArea2Open = true; //イベントを行った
			SaveData.Instance.Save();   //ファイルにセーブ
		}

		//エリア3が開放されているが、エリア3の開放イベントが行われていない場合
		if (Area.CanGoArea(3) && SaveData.Instance.mEventDoneFlag.mArea3Open == false) {
			yield return AreaOpenEvent(3);
			SaveData.Instance.mEventDoneFlag.mArea3Open = true; //イベントを行った
			SaveData.Instance.Save();	//ファイルにセーブ
		}


		//カメラのズームアウトを始める
		mCameraMove.MoveStart();


		//カメラのズームアウトが終わるまで待機
		while (true) {
			if (mCameraMove.IsMoveEnd) {
				break;
			}
			yield return null;
		}

		g.mOpenForce = false;    //ドアを強制的に開かなくする

		//ステージセレクトメインの開始
		//
		StartCoroutine(StageSelectMain());
	}


	//タイトルとチュートリアルから来た時の、自動で歩く演出のあるもの
	//
	IEnumerator StageSelectMain_FromTitle() {

		//重さを移せないようにする
		OnCanShiftOperation(false);
		mPause.canPause = false;

		//プレートの色を変える
		SetEnterColor(-1);

		//プレイヤーとボックスの重さを調整
		mPlayer.GetComponent<WeightManager>().WeightLv = WeightManager.Weight.light;
		mTopStaticWeightBox.GetComponent<WeightManager>().WeightLv = WeightManager.Weight.flying;
		mBottomStaticWeightBox.GetComponent<WeightManager>().WeightLv = WeightManager.Weight.heavy;


		//エリアの開放状況によって、重さボックスを消す
		HideBoxByAreaOpen();


		//カメラの開始地点を決める
		mCameraMove.mStartPosition = new Vector3(-19.5f, -3.5f, 45.0f);	//ステージセレクトの左端から始まる


		//カメラをズームされた位置に移動
		mCameraMove.MoveStartPosition();


		//プレイヤーを移動不可にする
		CanMovePlayer(false);
		CanJumpPlayer(false);

		// タイトルシーンからの遷移でなければ
		if (!mFromTitle) {
			//ステージ開始時の演出
			mTransition.OpenDoorParent();

			//演出が終了するまで待機
			while (true) {
				if (mTransition.GetOpenEnd()) break;
				yield return null;
			}
		}

		//BGMを流し始める
		var t = SoundManager.SPlay(mStageSelectBGMPrefab);
		SoundManager.SFade(t, 0.0f, SoundManager.SVolume(mStageSelectBGMPrefab), 2.0f);
		

		//カメラのズームアウトを始める
		mCameraMove.MoveStart();


		//カメラのズームアウトが終わるまで待機
		while (true) {
			if (mCameraMove.IsMoveEnd) {
				break;
			}
			yield return null;
		}

		mStageSelectScroll.mIsScroll = false;    //スクロールが行えるようになる

		//プレイヤーを自動で歩かせる
		//
		CanMovePlayer(true);    //プレイヤーは動けるようにするが、ユーザーの入力は受け付けない
		var v = mPlayer.GetComponent<VirtualController>();

		float cWalkTime = 1.1f; //プレイヤーを自動で歩かせる秒数
		VirtualController.SetAxis(VirtualController.CtrlCode.Horizontal, 1.0f, cWalkTime);
		VirtualController.SetAxis(VirtualController.CtrlCode.Jump, 0.0f, cWalkTime);
		VirtualController.SetAxis(VirtualController.CtrlCode.Lift, 0.0f, cWalkTime);
		VirtualController.SetAxis(VirtualController.CtrlCode.Vertical, 0.0f, cWalkTime);


		float lBeforeCameraMoveTime = mCameraMove.mTakeTime;

		mCameraMove.mStartPosition = mCameraMove.transform.position;
		mCameraMove.mEndPosition = mStageSelectScroll.mAreaCameraPosition[0].transform.position;

		const float cCameraMoveTime = 1.0f;
		mCameraMove.mTakeTime = cCameraMoveTime;

		yield return new WaitForSeconds(cWalkTime - cCameraMoveTime);

		mCameraMove.MoveStart();

		//歩かせている間待機
		yield return new WaitForSeconds(cCameraMoveTime);


		//カメラの移動にかける時間を戻す
		mCameraMove.mTakeTime = lBeforeCameraMoveTime;



		//
		//エリア開放イベント
		//

		//エリア2が開放されているが、エリア2の開放イベントが行われていない場合
		if (Area.CanGoArea(2) && SaveData.Instance.mEventDoneFlag.mArea2Open == false) {
			yield return AreaOpenEvent(2);
			SaveData.Instance.mEventDoneFlag.mArea2Open = true; //イベントを行った
			SaveData.Instance.Save();   //ファイルにセーブ
		}

		//エリア3が開放されているが、エリア3の開放イベントが行われていない場合
		if (Area.CanGoArea(3) && SaveData.Instance.mEventDoneFlag.mArea3Open == false) {
			yield return AreaOpenEvent(3);
			SaveData.Instance.mEventDoneFlag.mArea3Open = true; //イベントを行った
			SaveData.Instance.Save();   //ファイルにセーブ
		}


		//メインのコルーチンの開始
		//
		StartCoroutine(StageSelectMain());
	}



	//ステージセレクトのメインの更新
	//
	IEnumerator StageSelectMain() {

		OnCanShiftOperation(true);  //重さを移せるようになる
		mStageSelectScroll.mIsScroll = true;    //スクロールが行えるようになる
		mStageSelectScroll.mCameraMoveSpeed = 50.0f;
		mPause.canPause = true; //ポーズが出来るようになる
		CanMovePlayer(true);
		CanJumpPlayer(true);


		int lSelectStageNum = -1;
		int lBeforeSelectStageNum = -1;
		int lDecideSelectStageNum = -1;


		//メインのループ
		//
		while (true) {

			//ポーズ中ならカーソルを出す
			if (mPause.pauseFlg) {
				Cursor.visible = true;
			}
			else {
				Cursor.visible = false;
			}


			//現在いる場所のドアを開くのと、プレートを光らせるのの更新
			lSelectStageNum = mSelectStageNum;
			if (lSelectStageNum != lBeforeSelectStageNum) {
				SetEnterColor(mSelectStageNum);
				OpenDoor(lSelectStageNum, true);
				OpenDoor(lBeforeSelectStageNum, false);

				if (lBeforeSelectStageNum != -1) {
					mEnterUI.StopAnimation();
				}

				mSelectInit = false;
				mSelectTime = 0.0f;
			}
			lBeforeSelectStageNum = lSelectStageNum;


			//入る為のキー操作のUI表示
			//
			mSelectTime += Time.deltaTime;
			if (mSelectTime >= 0.4f && !mSelectInit) {
				mSelectInit = true;

				if (lSelectStageNum != -1) {
					Vector3 lUIPosition = mGoal[lSelectStageNum].transform.position;
					lUIPosition += mGoal[lSelectStageNum].transform.rotation * Vector3.down * 2.0f;
					mEnterUI.SetPosition(lUIPosition);
					mEnterUI.SetRotation(mGoal[lSelectStageNum].transform.rotation);
					mEnterUI.StartAnimation();
				}
			}


			bool lIsEnter = Input.GetKey(KeyCode.W);

			//ゴール判定
			//
			if (mSelectStageNum != -1) {

				//もし入る操作が行われているなら
				if (lIsEnter && CanGoal(mGoal[mSelectStageNum])) {
					lDecideSelectStageNum = mSelectStageNum;
					break;
				}
			}


			yield return null;  //ゲームメインを続ける
		}


		//重さを移せないようにする
		OnCanShiftOperation(false);
		mPause.canPause = false;

		//プレイヤーが入力できないようにする
		CanInputPlayer(false);

		CanJumpPlayer(false);

		//プレイヤーの回転が終わるまで待つ
		while (true) {
			if(mPlayer.IsRotation == false) {
				break;
			}
			yield return null;
		}


		//UIを消す
		mEnterUI.gameObject.SetActive(false);

		//ズーム終了後のカメラ位置を変更
		mCameraMove.mEndPosition = GetPlayerZoomCameraPosition(mGoal[lDecideSelectStageNum].transform.up.y <= 0.0f);


		//カメラの開始地点を現在のカメラ位置にする
		mCameraMove.mStartPosition = mCameraMove.transform.position;


		//プレイヤーをゴールの中心まで歩かせる
		Vector3 lGoalCenter = mGoal[lDecideSelectStageNum].transform.position;

		//左側にいるなら
		if(mPlayer.transform.position.x <= lGoalCenter.x) {
			//右に歩かせる
			VirtualController.SetAxis(VirtualController.CtrlCode.Horizontal, 1.0f, 30.0f);

			//ゴールの中心を超えたら、歩かせるのをやめる
			while(true) {
				if(mPlayer.transform.position.x >= lGoalCenter.x) {
					Vector3 lPos = mPlayer.transform.position;
					lPos.x = lGoalCenter.x;
					mPlayer.transform.position = lPos;  //補正
					break;
				}
				yield return null;
			}
		}
		//左側にいるなら
		else if (mPlayer.transform.position.x >= lGoalCenter.x) {
			//左に歩かせる
			VirtualController.SetAxis(VirtualController.CtrlCode.Horizontal, -1.0f, 30.0f);

			//ゴールの中心を超えたら、歩かせるのをやめる
			while (true) {
				if (mPlayer.transform.position.x <= lGoalCenter.x) {
					Vector3 lPos = mPlayer.transform.position;
					lPos.x = lGoalCenter.x;
					mPlayer.transform.position = lPos;  //補正
					break;
				}
				yield return null;
			}
		}

		mPlayer.GetComponent<MoveManager>().StopMoveHorizontalAll();
		VirtualController.SetAxis(VirtualController.CtrlCode.Horizontal, 0.0f, 30.0f);

		//プレイヤーの回転が終わるまで待つ
		while (true) {
			if (mPlayer.IsRotation == false) {
				break;
			}
			yield return null;
		}


		//プレイヤーを移動不可にする
		CanMovePlayer(false);
		CanJumpPlayer(false);

		OnPlayerEffect(true);   //プレイヤーの更新を切る

		mPlayer.GetComponent<PlayerAnimation>().ChangeState(PlayerAnimation.CState.cStandBy);


		mStageSelectScroll.mIsScroll = false;


		//ステージセレクトでは、ズームインしない仕様にした
		/*
		mCameraMove.MoveStart();

		//カメラのズームイン終了まで待つ
		while (true) {
			if (mCameraMove.IsMoveEnd) {
				break;
			}
			yield return null;
		}
		*/


		Goal g = mGoal[lDecideSelectStageNum];

		//
		//プレイヤーがドアに入っていく演出
		//
		yield return WalkEnterDoor(g);

		
		//ステージ終了時の演出
		mTransition.CloseDoorParent();

		//演出が終了するまで待機
		while (true) {
			if (mTransition.GetCloseEnd()) break;
			yield return null;
		}

		//ステージ遷移
		UnityEngine.SceneManagement.SceneManager.LoadScene(Area.GetStageSceneName((lDecideSelectStageNum / 5) + 1, (lDecideSelectStageNum % 5) + 1));

	}



	bool CanEnter(Goal lGoal) {

		//プレイヤーがゴール枠に完全に入っていないなら
		if (!lGoal.IsInPlayer(mPlayer)) {
			return false;   //ゴールできない
		}

		return true;    //ゴール可能
	}

	bool CanGoal(Goal lGoal) {

		//ゴールに入れないなら
		if(CanEnter(lGoal) == false) {
			return false;	//ゴールできない
		}

		//プレイヤーが接地していないなら
		if (mPlayer.GetComponent<Landing>().IsLanding == false) {
			return false;   //ゴールできない
		}

		//重さを移している最中なら
		if (mMassShift.IsShift) {
			return false;   //ゴールできない
		}

		//重さを移した後1秒以内なら
		if (mMassShift.FromLastShiftTime <= 1.0f) {
			return false;   //ゴールできない
		}

		return true;    //ゴール可能

	}

	void SetEnterColor(int aIndex) {
        int index = 0;
		foreach(var t in mText) {
            // ステージがクリア済みなら
            if (ScoreManager.Instance.ShiftTimes((index / 5) + 1, (index % 5) + 1) != -1) {
                // ステージ名の色を変える
                t.color = mStagePlateClearedColor;
            }
            else{
                // ステージ名の色を灰色に
                t.color = mStagePlateOffColor;
            }

            index++;
		}
		if (aIndex == -1) return;
		mText[aIndex].color = mStagePlateOnColor;
	}
	void OpenDoor(int aIndex, bool aIsOpen) {
		if (aIndex == -1) return;
		mGoal[aIndex].mOpenForce = aIsOpen;
	}
	

	MassShift mMassShift {
		get {
			return FindObjectOfType<MassShift>();
		}
	}

	//重さを移せるようになる
	//
	void OnCanShiftOperation(bool aCanShift) {
		mMassShift.CanShift = aCanShift;    //重さを移せる
		mMassShift.mInvisibleCursor = !aCanShift;
	}


	void CanMovePlayer(bool aCanMove) {
		mPlayer.CanWalk = aCanMove;
		mPlayer.CanRotation = aCanMove;
	}
	void CanJumpPlayer(bool aCanMove) {
		mPlayer.CanJump = aCanMove;
	}

	//プレイヤーの入力を可能にしたり、不可能にしたりする
	//
	void CanInputPlayer(bool aCan) {
		float lTime = 0.0f;
		if (aCan == false) {
			lTime = 1.0f * 60.0f * 60.0f * 24.0f;
		}
		VirtualController.SetAxis(VirtualController.CtrlCode.Horizontal, 0.0f, lTime);
		VirtualController.SetAxis(VirtualController.CtrlCode.Jump, 0.0f, lTime);
		VirtualController.SetAxis(VirtualController.CtrlCode.Lift, 0.0f, lTime);
		VirtualController.SetAxis(VirtualController.CtrlCode.Vertical, 0.0f, lTime);
	}


	//プレイヤーの演出用
	void OnPlayerEffect(bool aIsEffect) {
		mPlayer.GetComponent<MoveManager>().enabled = !aIsEffect;
		mPlayer.GetComponent<Player>().enabled = !aIsEffect;
	}

	Vector3 GetPlayerZoomCameraPosition(bool aIsReverse) {
		Player p = FindObjectOfType<Player>();
		Vector3 lPosition = p.transform.position;
		lPosition.y -= 0.0f;

		//プレイヤーが逆向きなら
		if(aIsReverse) {
			lPosition.y -= 2.0f;	//位置を調節
		}
		lPosition.z = 35.0f;
		return lPosition;
	}

    public int SelectStageNum {
        get{
            return mSelectStageNum;
        }
    }


	//エリアの開放イベント
	//
	IEnumerator AreaOpenEvent(int aAreaNumber) {

		//エラーチェック
		if (aAreaNumber == 2) {}
		else if (aAreaNumber == 3) {}
		else {
			Debug.LogError("エリア2、エリア3以外でエリア開放イベントが呼ばれました", this);
		}



		//元のデータを保存
		Vector3 lBeforeCameraPosition = mCameraMove.transform.position;
		Vector3 lBeforeCameraMoveStartPosition = mCameraMove.mStartPosition;
		Vector3 lBeforeCameraMoveEndPosition = mCameraMove.mEndPosition;
		float lBeforeCameraMoveTakeTime = mCameraMove.mTakeTime;


		//
		//カメラを現在地点から、演出する位置まで動かす
		//

		Vector3 lEventCameraPosition = Vector3.zero;

		//エリア2の開放演出なら
		if(aAreaNumber == 2) {
			lEventCameraPosition = mTopStaticWeightBox.transform.position;
			lEventCameraPosition.z = 35.0f;
		}
		//エリア3の開放演出なら
		else if (aAreaNumber == 3) {
			lEventCameraPosition = mBottomStaticWeightBox.transform.position;
			lEventCameraPosition.z = 35.0f;
		}

		//移動の情報の設定
		mCameraMove.mTakeTime = 2.0f;
		mCameraMove.mStartPosition = mCameraMove.transform.position;
		mCameraMove.mEndPosition = lEventCameraPosition;
		mCameraMove.MoveStart();

		//動き終わるまで待機
		while (true) {
			if (mCameraMove.IsMoveEnd == true) {
				break;
			}
			yield return null;
		}


		//
		//開放演出
		//

		if (aAreaNumber == 2) {

			//
			//ボックスが壁からにゅるっと出てくる
			//

			mTopStaticWeightBox.SetActive(true);

			Vector3 lStart = mTopStaticWeightBox.transform.position;
			lStart.x += 3.0f;
			Vector3 lEnd = mTopStaticWeightBox.transform.position;
			
			float lTime = 0.0f;
			const float cTakeTime = 3.0f;
			while (true) {
				lTime += Time.deltaTime;

				mTopStaticWeightBox.transform.position = Vector3.Lerp(lStart, lEnd, Mathf.Clamp01(lTime / cTakeTime));

				if(lTime >= cTakeTime) {
					break;
				}
				yield return null;
			}
		}
		else if (aAreaNumber == 3) {

			//
			//ボックスが壁からにゅるっと出てくる
			//

			mBottomStaticWeightBox.SetActive(true);

			Vector3 lStart = mBottomStaticWeightBox.transform.position;
			lStart.x += 3.0f;
			Vector3 lEnd = mBottomStaticWeightBox.transform.position;

			float lTime = 0.0f;
			const float cTakeTime = 3.0f;
			while (true) {
				lTime += Time.deltaTime;

				mBottomStaticWeightBox.transform.position = Vector3.Lerp(lStart, lEnd, Mathf.Clamp01(lTime / cTakeTime));

				if (lTime >= cTakeTime) {
					break;
				}
				yield return null;
			}

		}



		//
		//カメラを現在地点から、元の位置まで動かす
		//

		//移動の情報の設定
		mCameraMove.mTakeTime = 2.0f;
		mCameraMove.mStartPosition = mCameraMove.transform.position;
		mCameraMove.mEndPosition = lBeforeCameraPosition;
		mCameraMove.MoveStart();

		//動き終わるまで待機
		while(true) {
			if(mCameraMove.IsMoveEnd == true) {
				break;
			}
			yield return null;
		}


		//元のデータに戻す
		mCameraMove.mStartPosition = lBeforeCameraMoveStartPosition;
		mCameraMove.mEndPosition = lBeforeCameraMoveEndPosition;
		mCameraMove.mTakeTime = lBeforeCameraMoveTakeTime;
	}


	//エリアの開放状況によって、重さボックスを消したりする
	void HideBoxByAreaOpen() {

		//クリアしたエリアによって、ボックスを消して次のエリアに行けないようにする
		if (Area.CanGoArea(2) == false) {
			mTopStaticWeightBox.SetActive(false);
		}
		if (Area.CanGoArea(3) == false) {
			mBottomStaticWeightBox.SetActive(false);
		}

		//もしエリア開放イベントがされていなかったら、ボックスを出さないようにしておく
		if (SaveData.Instance.mEventDoneFlag.mArea2Open == false) {
			mTopStaticWeightBox.SetActive(false);
		}
		if (SaveData.Instance.mEventDoneFlag.mArea3Open == false) {
			mBottomStaticWeightBox.SetActive(false);
		}

	}

	
	//ドアに入る歩く動き
	IEnumerator WalkEnterDoor(Goal g) {

		yield return new WaitForSeconds(mToStageBeforeRotateTime);


		g.mOpenForce = true;    //ドアを強制的に開く

		mGoalBlack.transform.position = g.transform.position;   //黒い背景をゴールのところに移動させる
		mGoalBlack.transform.rotation = g.transform.rotation;

		//歩きアニメーションの再生
		mPlayer.GetComponent<PlayerAnimation>().SetSpeed(0.3f);
		mPlayer.GetComponent<PlayerAnimation>().ChangeState(PlayerAnimation.CState.cWalk);


		//プレイヤーを回転させていく
		//

		bool lIsRight = mPlayer.RotVec.x > 0.0f;

		if (lIsRight) {
			float lAngle = 0.0f;
			while (true) {

				//プレイヤーのモデルの回転を、ゆっくり元に戻す
				mPlayer.CameraLookRatio = Mathf.Clamp01(mPlayer.CameraLookRatio - 1.0f / mToStageRotateTime * Time.deltaTime);
				mPlayer.LookCamera();

				//プレイヤーを回転させる
				lAngle -= 90.0f / mToStageRotateTime * Time.deltaTime;
				mPlayer.transform.rotation = Quaternion.Euler(0.0f, lAngle, 0.0f);
				if (lAngle <= -90.0f) {
					mPlayer.transform.rotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
					break;
				}

				yield return null;
			}
		}
		else {
			float lAngle = 0.0f;
			while (true) {

				//プレイヤーのモデルの回転を、ゆっくり元に戻す
				mPlayer.CameraLookRatio = Mathf.Clamp01(mPlayer.CameraLookRatio - 1.0f / mToStageRotateTime * Time.deltaTime);
				mPlayer.LookCamera();

				//プレイヤーを回転させる
				lAngle += 90.0f / mToStageRotateTime * Time.deltaTime;
				mPlayer.transform.rotation = Quaternion.Euler(0.0f, lAngle, 0.0f);
				if (lAngle >= 90.0f) {
					mPlayer.transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
					break;
				}

				yield return null;
			}
		}


		yield return new WaitForSeconds(mToStageBeforeWalkingTime);


		//プレイヤーを歩かせる
		//

		mPlayer.GetComponent<PlayerAnimation>().SetSpeed(1.0f);

		mGoalBlack.StartFade(0.0f, 1.0f, 0.0f, mToStageWalkingTime);

		while (true) {
			mPlayer.transform.position += new Vector3(0.0f, 0.0f, 3.0f / mToStageWalkingTime * Time.deltaTime);
			if (mPlayer.transform.position.z >= 3.0f) {
				break;
			}

			yield return null;
		}

		yield return new WaitForSeconds(mToStageAfterWalkingTime);


		//歩くのをやめる
		mPlayer.GetComponent<PlayerAnimation>().ChangeState(PlayerAnimation.CState.cStandBy);

	}
	
	//ドアから出る歩く動き
	IEnumerator WalkExitDoor(Goal g) {

		yield return new WaitForSeconds(mFromStageBeforeWalkingTime);

		g.mOpenForce = true;    //ドアを強制的に開く

		mGoalBlack.transform.position = g.transform.position;
		mGoalBlack.transform.rotation = g.transform.rotation;

		mPlayer.transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f); //回転させる

		//歩きアニメーションの再生
		mPlayer.GetComponent<PlayerAnimation>().SetSpeed(1.0f);
		mPlayer.GetComponent<PlayerAnimation>().ChangeState(PlayerAnimation.CState.cWalk);

		//zが0.0fになるまで移動させる。同時に、黒い板を透明にしていく
		mGoalBlack.StartFade(1.0f, 0.0f, 0.0f, 2.0f);

		while (true) {
			mPlayer.transform.position += new Vector3(0.0f, 0.0f, -3.0f / mFromStageWalkingTime * Time.deltaTime);
			if (mPlayer.transform.position.z <= 0.0f) {
				Vector3 lPos = mPlayer.transform.position;
				lPos.z = 0.0f;
				mPlayer.transform.position = lPos;
				break;
			}

			yield return null;
		}

		yield return new WaitForSeconds(mFromStageBeforeRotateTime);

		//プレイヤーの回転を元に戻す

		mPlayer.GetComponent<PlayerAnimation>().SetSpeed(0.3f);

		float lAngle = 90.0f;
		while (true) {
			lAngle -= 90.0f / mFromStageRotateTime * Time.deltaTime;

			mPlayer.transform.rotation = Quaternion.Euler(0.0f, lAngle, 0.0f);
			if (lAngle <= 0.0f) {
				mPlayer.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
				break;
			}

			yield return null;
		}

		OnPlayerEffect(false);   //プレイヤーの更新を戻す

		yield return new WaitForSeconds(mFromStageAfterRotateTime);

	}
	
}
