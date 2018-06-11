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

    [SerializeField]
    MoveTransform mCameraMove;

    [SerializeField]
    Vector3 cameraStartPos;

	// Use this for initialization
	void Start() {

		mPlayer = FindObjectOfType<Player>();
		mTransition = FindObjectOfType<StageTransition>();

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

		//プレートの色を変える
		SetEnterColor(-1);

        // タイトルシーンからの遷移であれば
        if (cameraMove.fromTitle) {
            // カメラの初期位置を変更
            mCameraMove.mStartPosition = cameraStartPos;
        }

        //カメラをズームされた位置に移動
        mCameraMove.MoveStartPoisition();

		//LimitPlayDoorSE();

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
            cameraMove.fromTitle = false;
        }

		//BGMを流し始める
		var t = SoundManager.SPlay(mStageSelectBGMPrefab);
		SoundManager.SFade(t, 0.0f, 1.0f, 2.0f);

		int lSelectStageNum = -1;
		int lBeforeSelectStageNum = -1;

		int lDecideSelectStageNum = -1;

		//カメラのズームアウトを始める
		mCameraMove.MoveStart();

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


	//ドアが開く音を1つに制限する
	void LimitPlayDoorSE() {
		for(int i = 1; i < mGoal.Count; i++) {
			mGoal[i].mPlayOpenSE = false;
		}
	}
}
