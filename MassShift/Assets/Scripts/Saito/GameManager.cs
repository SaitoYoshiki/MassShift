using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	MassShift mMassShift;
	Player mPlayer;
	Goal mGoal;

	[SerializeField, EditOnPrefab]
	List<GameObject> mAreaBGM;

	StageTransition mTransition;

	Result mResult;

	Pause mPause;

	[SerializeField]
	bool _Debug_ClearFlag = false;	//クリアしたことにするフラグ

	// Use this for initialization
	void Start() {
		mMassShift = FindObjectOfType<MassShift>();
		mPlayer = FindObjectOfType<Player>();
		mGoal = FindObjectOfType<Goal>();

		mTransition = FindObjectOfType<StageTransition>();
		mResult = FindObjectOfType<Result>();
		mPause = FindObjectOfType<Pause>();

		Time.timeScale = 1.0f;
		mPause.pauseEvent.Invoke();

        //ゲーム進行のコルーチンを開始
        if (!cameraMove.fromTitle) {
            StartCoroutine(GameMain());
        }
        else {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }
	}

	// Update is called once per frame
	void Update() {

	}

    void OnActiveSceneChanged(Scene i_preChangedScene, Scene i_postChangedScene) {
        StartCoroutine(GameMain());
    }

	IEnumerator GameMain() {

		float lTakeTime;

		//ステージ開始時の演出
		//

		//if(Area.GetAreaNumber() == 0 || Area.GetAreaNumber() == 1) {
		{
			//プレイヤーを操作不可に
			OnCantOperation();

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
                SceneManager.activeSceneChanged -= OnActiveSceneChanged;
                yield return null;
                //yield return new WaitForSeconds(1.0f);
            }
		}

		//BGMを再生する
		int lAreaNumber = Area.GetAreaNumber();
		if(lAreaNumber != -1) {
			SoundManager.SPlay(mAreaBGM[lAreaNumber]);
		}


		//ゲームメインの開始
		//

		//プレイヤーが操作可能になる
		OnCanOperation();

		//ゲームメインのループ
		while (true) {

			//ポーズ中なら
			if(mPause.pauseFlg) {
				//mMassShift.CanShift = false;
				Cursor.visible = true;
			}
			else {
				//mMassShift.CanShift = true;
				Cursor.visible = false;
			}

			//ゴール判定
			//
			if (CanGoal() || _Debug_ClearFlag) {
				break;
			}

			yield return null;	//ゲームメインを続ける
		}


		//ゴール時の、プレイヤーがドアから出ていく演出
		//

		//Playerを操作不可にする
		OnCantOperation();

		Cursor.visible = true;
		mResult.canGoal = true;
	}

	bool CanGoal() {
		
		//全てのボタンがオンでないなら
		if (!mGoal.IsAllButtonOn) {
			return false;   //ゴールできない
		}

		//プレイヤーがゴール枠に完全に入っていないなら
		if (!mGoal.IsInPlayer(mPlayer)) {
			return false;   //ゴールできない
		}

		//プレイヤーが接地していないなら
		if(mPlayer.GetComponent<Landing>().IsLanding == false) {
			return false;	//ゴールできない
		}

		//重さを移した後1秒以内なら
		if(mMassShift) {
			//return false;	//ゴールできない
		}

		return true;	//ゴール可能
	}

	void OnCantOperation() {
		mMassShift.CanShift = false;    //重さを移せない
		mPlayer.CanWalk = false;
		mPlayer.CanJump = false;
		mPlayer.CanRotation = false;
		mPause.canPause = false;
	}
	void OnCanOperation() {
		mMassShift.CanShift = true;    //重さを移せる
		mPlayer.CanWalk = true;
		mPlayer.CanJump = true;
		mPlayer.CanRotation = true;
		mPause.canPause = true;
	}
}
