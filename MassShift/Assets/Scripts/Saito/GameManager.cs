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

	[SerializeField]
	GoalBlackCurtain mGoalBlack;

	[SerializeField, EditOnPrefab, Tooltip("重さを移してから何秒間はゴールできないか")]
	float mCanGoalTimeFromShift = 1.0f;


	[SerializeField]
	float mGoalBeforeRotateTime = 0.0f;

	[SerializeField]
	float mGoalRotateTime = 0.5f;

	[SerializeField]
	float mGoalBeforeWalkingTime = 0.5f;

	[SerializeField]
	float mGoalWalkingTime = 1.0f;

	[SerializeField]
	float mGoalAfterWalkingTime = 0.5f;

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
		Area.sBeforeAreaNumber = Area.GetAreaNumber();
		Area.sBeforeStageNumber = Area.GetStageNumber();

		//コンポーネントのキャッシュ
		mMassShift = FindObjectOfType<MassShift>();
		mPlayer = FindObjectOfType<Player>();
		mGoal = FindObjectOfType<Goal>();

		mTransition = FindObjectOfType<StageTransition>();
		mResult = FindObjectOfType<Result>();
		mPause = FindObjectOfType<Pause>();

		mGoalBlack = FindObjectOfType<GoalBlackCurtain>();

		//ポーズ画面から来たら、ポーズを戻す
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

		//タイトルから来ていないなら、プレイヤーに寄った位置からズームアウトを開始する
		if(cameraMove.fromTitle == false) {
			mCameraMove.mStartPosition = GetPlayerZoomCameraPosition();
		}
		//タイトルからなら
		else {
			//mCameraMove.mStartPosition = new Vector3(0.0f, 0.0f, 45.0f);
		}

		//カメラをズームされた位置に移動
		mCameraMove.MoveStartPosition();


		//プレイヤーを操作不可に
		CanMovePlayer(false);
		OnCanShiftOperation(false);
		mPause.canPause = false;


		//BGMを再生する
		int lAreaNumber = Area.GetAreaNumber();
		if (lAreaNumber != -1) {
			GameObject lBGMPrefab = mAreaBGM[lAreaNumber];

			//BGMを流し始める
			var t = SoundManager.SPlay(lBGMPrefab);
			SoundManager.SFade(t, 0.0f, SoundManager.SVolume(lBGMPrefab), 2.0f);
		}


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
			Debug.Log("fromTitle" + cameraMove.fromTitle);
			yield return null;
		}


		//ゲームメインの開始
		//

		//プレイヤーが操作可能になる
		CanMovePlayer(true);
		mPause.canPause = true;

		//カメラのズームアウトを始める
		mCameraMove.MoveStart();


		//ゲームメインのループ
		while (true) {

			//カメラのズームアウトが終わってから、移す操作を出来るようになる
			if(mCameraMove.IsMoveEnd) {
				OnCanShiftOperation(true);
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
		CanMovePlayer(false);


		//重さを移せないようにする
		OnCanShiftOperation(false);
		mPause.canPause = false;

		//ズーム終了後のカメラ位置を変更
		mCameraMove.mEndPosition = GetPlayerZoomCameraPosition();


		//カメラの開始地点を現在のカメラ位置にする
		mCameraMove.mStartPosition = mCameraMove.transform.position;


		//プレイヤーを移動不可にする
		CanMovePlayer(false);

		//カメラを見ていないようにする
		mPlayer.GetComponent<Player>().CameraLookRatio = 0.0f;

		OnPlayerEffect(true);   //プレイヤーの更新を切る
		mPlayer.GetComponent<PlayerAnimation>().ChangeState(PlayerAnimation.CState.cStandBy);

		mCameraMove.MoveStart();

		//カメラのズームイン終了まで待つ
		while (true) {
			if (mCameraMove.IsMoveEnd) {
				break;
			}
			yield return null;
		}


		//
		//プレイヤーがドアに入っていく演出
		//

		yield return new WaitForSeconds(mGoalBeforeRotateTime);

		mGoal.mOpenForce = true;    //ドアを強制的に開く

		mGoalBlack.transform.position = mGoal.transform.position;   //黒い背景をゴールのところに移動させる
		mGoalBlack.transform.rotation = mGoal.transform.rotation;

		//歩きアニメーションの再生
		mPlayer.GetComponent<PlayerAnimation>().SetSpeed(0.3f);
		mPlayer.GetComponent<PlayerAnimation>().ChangeState(PlayerAnimation.CState.cWalk);


		//プレイヤーを回転させていく
		//
		bool lIsRight = mPlayer.RotVec.x > 0.0f;

		//右を向いているなら
		if (lIsRight) {
			float lAngle = 0.0f;
			while (true) {

				//プレイヤーのモデルの回転を、ゆっくり元に戻す
				mPlayer.CameraLookRatio = Mathf.Clamp01(mPlayer.CameraLookRatio - 1.0f / mGoalRotateTime * Time.deltaTime);
				mPlayer.LookCamera();

				//プレイヤーを回転させる
				lAngle -= 90.0f / mGoalRotateTime * Time.deltaTime;
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
				mPlayer.CameraLookRatio = Mathf.Clamp01(mPlayer.CameraLookRatio - 1.0f / mGoalRotateTime * Time.deltaTime);
				mPlayer.LookCamera();

				//プレイヤーを回転させる
				lAngle += 90.0f / mGoalRotateTime * Time.deltaTime;
				mPlayer.transform.rotation = Quaternion.Euler(0.0f, lAngle, 0.0f);
				if (lAngle >= 90.0f) {
					mPlayer.transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
					break;
				}

				yield return null;
			}
		}

		yield return new WaitForSeconds(mGoalBeforeWalkingTime);

		//プレイヤーを歩かせる
		//

		mPlayer.GetComponent<PlayerAnimation>().SetSpeed(1.0f);

		mGoalBlack.StartFade(0.0f, 1.0f, 0.0f, mGoalWalkingTime);

		while (true) {
			mPlayer.transform.position += new Vector3(0.0f, 0.0f, 3.0f / mGoalWalkingTime * Time.deltaTime);
			if (mPlayer.transform.position.z >= 3.0f) {
				break;
			}

			yield return null;
		}

		yield return new WaitForSeconds(mGoalAfterWalkingTime);

		//歩くのをやめる
		mPlayer.GetComponent<PlayerAnimation>().ChangeState(PlayerAnimation.CState.cStandBy);

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

		//プレイヤーが回転中なら
		if (mPlayer.GetComponent<Player>().IsRotation) {
			return false;   //ゴールできない
		}

		//重さを移した後1秒以内なら
		if (mMassShift.FromLastShiftTime <= mCanGoalTimeFromShift) {
			return false;	//ゴールできない
		}

		return true;	//ゴール可能
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

	//プレイヤーの演出用
	void OnPlayerEffect(bool aIsEffect) {
		mPlayer.GetComponent<MoveManager>().enabled = !aIsEffect;
		mPlayer.GetComponent<Player>().enabled = !aIsEffect;
	}

	Vector3 GetPlayerZoomCameraPosition() {
		Player p = FindObjectOfType<Player>();
		Vector3 lPosition = p.transform.position;
		lPosition.y -= 1.0f;
		lPosition.z = 40.0f;
		return lPosition;
	}
}
