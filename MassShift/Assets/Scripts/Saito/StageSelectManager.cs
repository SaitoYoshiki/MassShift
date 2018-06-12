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

	StageTransition mTransition;
	Pause mPause;
	StageSelectScroll mStageSelectScroll;

    [SerializeField]
    MoveTransform mCameraMove;

	[SerializeField]
	StageSelectEnterUI mEnterUI;

	[SerializeField]
	GameObject mTopStaticWeightBox;

	[SerializeField]
	GameObject mBottomStaticWeightBox;

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


		//ゲーム進行のコルーチンを開始
		//
		mFromTitle = cameraMove.fromTitle;
		cameraMove.fromTitle = false;

		//タイトル用の演出のコルーチン
		if (Area.GetAreaNumber() == 0 || mFromTitle) {
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
		SetEnterColor(-1);


		//ズーム終了後のカメラ位置を変更
		mCameraMove.mEndPosition = mStageSelectScroll.mAreaCameraPosition[Area.sBeforeAreaNumber - 1].transform.position;

		//プレイヤーの位置を変更
		Goal g = mGoal[(Area.sBeforeAreaNumber - 1) * 5 + (Area.sBeforeStageNumber - 1)];
		Vector3 lNewPlayerPosition = g.transform.position;
		lNewPlayerPosition += g.transform.rotation * Vector3.up * 1.0f;
		lNewPlayerPosition.z = 0.0f;
		mPlayer.transform.position = lNewPlayerPosition;


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


		//クリアしたエリアによって、ボックスを消して次のエリアに行けないようにする
		if (StageClearManager.Instance.CanGoArea(2) == false) {
			mTopStaticWeightBox.SetActive(false);
			mBottomStaticWeightBox.SetActive(false);
		}
		else if (StageClearManager.Instance.CanGoArea(3) == false) {
			mBottomStaticWeightBox.SetActive(false);
		}


		//カメラの開始地点をプレイヤーにズームしたところからにする
		mCameraMove.mStartPosition = GetPlayerZoomCameraPosition();

		//カメラをズームされた位置に移動
		mCameraMove.MoveStartPosition();


		//プレイヤーを移動不可にする
		CanMovePlayer(false);


		//ステージ開始時の演出
		mTransition.OpenDoorParent();

		//演出が終了するまで待機
		while (true) {
			if (mTransition.GetOpenEnd()) break;
			yield return null;
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

		//クリアしたエリアによって、ボックスを消して次のエリアに行けないようにする
		if (StageClearManager.Instance.CanGoArea(2) == false) {
			mTopStaticWeightBox.SetActive(false);
			mBottomStaticWeightBox.SetActive(false);
		}
		else if (StageClearManager.Instance.CanGoArea(3) == false) {
			mBottomStaticWeightBox.SetActive(false);
		}


		//カメラの開始地点を決める
		mCameraMove.mStartPosition = new Vector3(-19.5f, -3.5f, 45.0f);	//ステージセレクトの左端から始まる


		//カメラをズームされた位置に移動
		mCameraMove.MoveStartPosition();


		//プレイヤーを移動不可にする
		CanMovePlayer(false);


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

		mStageSelectScroll.mIsScroll = true;    //スクロールが行えるようになる
		mStageSelectScroll.mCameraMoveSpeed = 10.0f;


		//プレイヤーを自動で歩かせる
		//
		CanMovePlayer(true);    //プレイヤーは動けるようにするが、ユーザーの入力は受け付けない
		var v = mPlayer.GetComponent<VirtualController>();

		float cWalkTime = 3.0f; //プレイヤーを自動で歩かせる秒数
		VirtualController.SetAxis(VirtualController.CtrlCode.Horizontal, 0.5f, cWalkTime);
		VirtualController.SetAxis(VirtualController.CtrlCode.Jump, 0.0f, cWalkTime);
		VirtualController.SetAxis(VirtualController.CtrlCode.Lift, 0.0f, cWalkTime);
		VirtualController.SetAxis(VirtualController.CtrlCode.Vertical, 0.0f, cWalkTime);

		//歩かせている間待機
		yield return new WaitForSeconds(cWalkTime);


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
			if (mSelectTime >= 1.0f && !mSelectInit) {
				mSelectInit = true;

				if (lSelectStageNum != -1) {
					Vector3 lUIPosition = mGoal[lSelectStageNum].transform.position;
					lUIPosition += mGoal[lSelectStageNum].transform.rotation * Vector3.up * 5.0f;
					mEnterUI.SetPosition(lUIPosition);
					mEnterUI.SetRotation(mGoal[lSelectStageNum].transform.rotation);
					mEnterUI.StartAnimation();
				}
			}


			bool lIsEnter = Input.GetKeyDown(KeyCode.W);

			//ゴール判定
			//
			if (mSelectStageNum != -1) {

				//もし入る操作が行われているなら
				if (lIsEnter) {
					lDecideSelectStageNum = mSelectStageNum;
					break;
				}
			}


			yield return null;  //ゲームメインを続ける
		}

		//ステージ終了時の演出
		mTransition.CloseDoorParent();

		//演出が終了するまで待機
		while (true) {
			if (mTransition.GetCloseEnd()) break;
			yield return null;
		}

		//ステージ遷移
		UnityEngine.SceneManagement.SceneManager.LoadScene(Area.GetStageSceneName((lDecideSelectStageNum / 5) + 1, (lSelectStageNum % 5) + 1));

	}



	bool CanEnter(Goal lGoal) {
		if (lGoal == null) return false;
		if (!lGoal.IsInPlayer(mPlayer)) return false;
		return true;
	}

	void SetEnterColor(int aIndex) {
		foreach(var t in mText) {
			t.color = mStagePlateOffColor;
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
		mPlayer.CanJump = aCanMove;
		mPlayer.CanRotation = aCanMove;
	}

	Vector3 GetPlayerZoomCameraPosition() {
		Player p = FindObjectOfType<Player>();
		Vector3 lPosition = p.transform.position;
		lPosition.y -= 1.0f;
		lPosition.z = 40.0f;
		return lPosition;
	}
}
