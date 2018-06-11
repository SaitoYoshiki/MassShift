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
	GameObject mTopStaticWeightBox;

	[SerializeField]
	GameObject mBottomStaticWeightBox;

	[SerializeField]
	int mClearArea = 2;



	// Use this for initialization
	void Start() {

		mPlayer = FindObjectOfType<Player>();
		mTransition = FindObjectOfType<StageTransition>();
		mStageSelectScroll = FindObjectOfType<StageSelectScroll>();

		mPause = FindObjectOfType<Pause>();

		//ゲーム進行のコルーチンを開始
		StartCoroutine(StageSelectMain());
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
	int mSelectStageNum = -1;

	IEnumerator StageSelectMain() {

		//重さを移せないようにする
		OnCanShiftOperation(false);
		mPause.canPause = false;

		//プレートの色を変える
		SetEnterColor(-1);


		//ステージから来たなら、ズーム終了後のカメラ位置を変更
		int lAreaNum = Area.sNowAreaNumber;
		if (lAreaNum == 1 || lAreaNum == 2 || lAreaNum == 3) {
			mCameraMove.mEndPosition = mStageSelectScroll.mAreaCameraPosition[lAreaNum - 1].transform.position;
		}

		//プレイヤーの位置を変更
		if (lAreaNum == 1 || lAreaNum == 2 || lAreaNum == 3) {
			Goal g = mGoal[(Area.sNowAreaNumber - 1) * 5 + (Area.sNowStageNumber - 1)];
			Vector3 lNewPlayerPosition = g.transform.position;
			lNewPlayerPosition += g.transform.rotation * Vector3.up * 1.0f;
			lNewPlayerPosition.z = 0.0f;
			mPlayer.transform.position = lNewPlayerPosition;
		}

		if(lAreaNum == 1) {
			mPlayer.GetComponent<WeightManager>().WeightLv = WeightManager.Weight.light;
			mTopStaticWeightBox.GetComponent<WeightManager>().WeightLv = WeightManager.Weight.flying;
			mBottomStaticWeightBox.GetComponent<WeightManager>().WeightLv = WeightManager.Weight.heavy;
		}
		if (lAreaNum == 2) {
			mPlayer.GetComponent<WeightManager>().WeightLv = WeightManager.Weight.flying;
			mTopStaticWeightBox.GetComponent<WeightManager>().WeightLv = WeightManager.Weight.light;
			mBottomStaticWeightBox.GetComponent<WeightManager>().WeightLv = WeightManager.Weight.heavy;
		}
		if (lAreaNum == 3) {
			mPlayer.GetComponent<WeightManager>().WeightLv = WeightManager.Weight.heavy;
			mTopStaticWeightBox.GetComponent<WeightManager>().WeightLv = WeightManager.Weight.flying;
			mBottomStaticWeightBox.GetComponent<WeightManager>().WeightLv = WeightManager.Weight.light;
		}

		//クリアしたエリアによって、ボックスを消して次のエリアに行けないようにする
		if (mClearArea == 0) {
			mTopStaticWeightBox.SetActive(false);
			mBottomStaticWeightBox.SetActive(false);
		}
		if (mClearArea == 1) {
			mBottomStaticWeightBox.SetActive(false);
		}


		//カメラの開始地点を決める
		if (lAreaNum == 0 || cameraMove.fromTitle) {
			//ステージセレクトの左端から始まる
			mCameraMove.mStartPosition = new Vector3(-17.0f, -3.5f, 45.0f);
		}
		else {
			//カメラの開始地点をプレイヤーにズームしたところからにする
			mCameraMove.mStartPosition = GetPlayerZoomCameraPosition();
		}
		
		//カメラをズームされた位置に移動
		mCameraMove.MoveStartPosition();


		//プレイヤーを移動不可にする
		CanMovePlayer(false);

		
		// タイトルシーンからの遷移でなければ
        if (!cameraMove.fromTitle) {
            //ステージ開始時の演出
            mTransition.OpenDoorParent();

            //演出が終了するまで待機
            while (true) {
                if (mTransition.GetOpenEnd()) break;
                yield return null;
            }
        }
        else {
            //cameraMove.fromTitle = false;
        }

		//BGMを流し始める
		var t = SoundManager.SPlay(mStageSelectBGMPrefab);
		SoundManager.SFade(t, 0.0f, SoundManager.SVolume(mStageSelectBGMPrefab), 2.0f);

		int lSelectStageNum = -1;
		int lBeforeSelectStageNum = -1;

		int lDecideSelectStageNum = -1;


		//カメラのズームアウトを始める
		mCameraMove.MoveStart();


		//左端から始まるなら
		if(lAreaNum == 0 || cameraMove.fromTitle) {

			cameraMove.fromTitle = false;

			while (true) {
				//カメラのズームアウトが終わるまで待機
				if (mCameraMove.IsMoveEnd) {
					break;
				}
				yield return null;
			}

			mStageSelectScroll.mIsScroll = true;    //スクロールが行えるようになる

			//プレイヤーを自動で歩かせる
			//
			CanMovePlayer(true);    //プレイヤーは動けるようにするが、ユーザーの入力は受け付けない
			var v = mPlayer.GetComponent<VirtualController>();
			//v.selfUpdateRetAxis = false;
			float cWalkTime = 1.0f;
			VirtualController.SetAxis(VirtualController.CtrlCode.Horizontal, 1.0f, cWalkTime);
			VirtualController.SetAxis(VirtualController.CtrlCode.Jump, 0.0f, cWalkTime);
			VirtualController.SetAxis(VirtualController.CtrlCode.Lift, 0.0f, cWalkTime);
			VirtualController.SetAxis(VirtualController.CtrlCode.Vertical, 0.0f, cWalkTime);

			yield return new WaitForSeconds(cWalkTime);
		}
		else {
			while(true) {
				//カメラのズームアウトが終わってから、移す操作を出来るようになる
				if (mCameraMove.IsMoveEnd) {
					break;
				}
				yield return null;
			}
		}

		OnCanShiftOperation(true);  //重さを移せるようになる
		mCameraMove.IsMoveEnd = false;
		mStageSelectScroll.mIsScroll = true;    //スクロールが行えるようになる
		mPause.canPause = true; //ポーズが出来るようになる
		CanMovePlayer(true);


		//ゲームメインのループ
		while (true) {
			
			//ポーズ中なら
			if (mPause.pauseFlg) {
				Cursor.visible = true;
			}
			else {
				Cursor.visible = false;
			}


			//現在いる場所のドアを開くのと、プレートを光らせるのの更新
			lSelectStageNum = mSelectStageNum;
			if(lSelectStageNum != lBeforeSelectStageNum) {
				SetEnterColor(mSelectStageNum);
				OpenDoor(lSelectStageNum, true);
				OpenDoor(lBeforeSelectStageNum, false);
			}
			lBeforeSelectStageNum = lSelectStageNum;
			
			bool lIsEnter = Input.GetKeyDown(KeyCode.W);

			//ゴール判定
			//
			if(mSelectStageNum != -1) {

				//もし入る操作が行われているなら
				if (lIsEnter) {
					lDecideSelectStageNum = mSelectStageNum;
					break;
				}
			}
			
			
			yield return null;	//ゲームメインを続ける
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
