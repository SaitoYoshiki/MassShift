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

	[SerializeField]
	MoveTransform mCameraMove;

	[SerializeField, EditOnPrefab, Tooltip("重さを移してから何秒間はゴールできないか")]
	float mCanGoalTimeFromShift = 1.0f;

	StageTransition mTransition;

	Result mResult;

	Pause mPause;

	[SerializeField]
	bool _Debug_ClearFlag = false;	//クリアしたことにするフラグ

    [SerializeField]
    Vector3 cameraStartPos;

	// Use this for initialization
	void Start() {

		//エリア番号とステージ番号を書き込む
		Area.sNowAreaNumber = Area.GetAreaNumber();
		Area.sNowStageNumber = Area.GetStageNumber();

		//コンポーネントのキャッシュ
		mMassShift = FindObjectOfType<MassShift>();
		mPlayer = FindObjectOfType<Player>();
		mGoal = FindObjectOfType<Goal>();

		mTransition = FindObjectOfType<StageTransition>();
		mResult = FindObjectOfType<Result>();
		mPause = FindObjectOfType<Pause>();

		Time.timeScale = 1.0f;
		mPause.pauseEvent.Invoke();

        //ゲーム進行のコルーチンを開始
        StartCoroutine(GameMain());
	}

	// Update is called once per frame
	void Update() {

	}

	IEnumerator GameMain() {

		float lTakeTime;

		//ステージ開始時の演出

		mCameraMove.mStartPosition = GetPlayerZoomCameraPosition();

		//カメラをズームされた位置に移動
		mCameraMove.MoveStartPoisition();

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
                Debug.Log("fromTitle"+cameraMove.fromTitle);
                yield return null;
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

		//カメラのズームアウトを始める
		mCameraMove.MoveStart();


		//ゲームメインのループ
		while (true) {

			//カメラのズームアウトが終わってから、移す操作を出来るようになる
			if(mCameraMove.IsMoveEnd) {
				OnCanShiftOperation();
				mCameraMove.IsMoveEnd = false;
			}

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
		if(mMassShift.FromLastShiftTime <= mCanGoalTimeFromShift) {
			return false;	//ゴールできない
		}

		return true;	//ゴール可能
	}

	//重さを移せなくなり、プレイヤーも動かせなくなる操作
	//
	void OnCantOperation() {
		mMassShift.CanShift = false;
		mMassShift.mInvisibleCursor = true;
		mPlayer.CanWalk = false;
		mPlayer.CanJump = false;
		mPlayer.CanRotation = false;
		mPause.canPause = false;
	}

	//プレイヤーが動けるようになり、ポーズも出来るようになる操作
	//
	void OnCanOperation() {
		mPlayer.CanWalk = true;
		mPlayer.CanJump = true;
		mPlayer.CanRotation = true;
		mPause.canPause = true;
	}

	//重さを移せるようになる操作
	//
	void OnCanShiftOperation() {
		mMassShift.CanShift = true;    //重さを移せる
		mMassShift.mInvisibleCursor = false;
	}

	Vector3 GetPlayerZoomCameraPosition() {
		Player p = FindObjectOfType<Player>();
		Vector3 lPosition = p.transform.position;
		lPosition.y -= 1.0f;
		lPosition.z = 40.0f;
		return lPosition;
	}
}
