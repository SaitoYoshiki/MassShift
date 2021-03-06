﻿using System.Collections;
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

	GameObject mBGMInstance;

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
        if (mPause != null) {
            mPause.pauseEvent.Invoke();
        }

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
		CanJumpPlayer(false);
		OnCanShiftOperation(false);

        if (mPause != null) {
            mPause.canPause = false;
        }

		//BGMを再生する
		int lAreaNumber = Area.GetAreaNumber();
		if(lAreaNumber == -1) {

		}
		else {
			GameObject lBGMPrefab = null;

			if(lAreaNumber <= 3) {
				lBGMPrefab = mAreaBGM[lAreaNumber];
			}
			else if(lAreaNumber == 4) {
				int lStageNumber = Area.GetStageNumber();
				if(lStageNumber == 1) {
					lBGMPrefab = mAreaBGM[1];
				}
				else if (lStageNumber == 2) {
					lBGMPrefab = mAreaBGM[2];
				}
				else if (lStageNumber == 3) {
					lBGMPrefab = mAreaBGM[3];
				}
				else {
					lBGMPrefab = mAreaBGM[3];
				}
			}

			//BGMを流し始める
			mBGMInstance = SoundManager.SPlay(lBGMPrefab);
			SoundManager.SFade(mBGMInstance, 0.0f, SoundManager.SVolume(lBGMPrefab), 2.0f);
		}

		// タイトルシーンからの遷移なら
        if (cameraMove.fromTitle){
			cameraMove.fromTitle = false;
			Debug.Log("fromTitle" + cameraMove.fromTitle);
			yield return null;
		}
        else {
            // ステージ開始時のドア開き
            if (mTransition != null) {
                //ステージ開始時の演出
                mTransition.OpenDoorParent();

                //演出が終了するまで待機
                while (true) {
                    if (mTransition.GetOpenEnd()) break;
                    yield return null;
                }
            }
        }


		//ゲームメインの開始
		//

		//プレイヤーが操作可能になる
		CanMovePlayer(true);
		CanJumpPlayer(true);

        // エンディングならポーズ不可
        if (mPause != null && SceneManager.GetActiveScene().name != "Ending") {
            mPause.canPause = true;
        }
        else {

        }

		//カメラのズームアウトを始める
		mCameraMove.MoveStart();


		//ゲームメインのループ
		while (true) {

			//カメラのズームアウトが終わってから、移す操作を出来るようになる
			if(mCameraMove.IsMoveEnd) {
				OnCanShiftOperation(true);
				mCameraMove.IsMoveEnd = false;
			}

            if (mPause != null) {
                //ポーズ中なら
                if (mPause.pauseFlg) {
                    //mMassShift.CanShift = false;
                    Cursor.visible = true;
                }
                else {
                    //mMassShift.CanShift = true;
                    Cursor.visible = false;
                }
            }

			//ゴール判定
			//
			if (CanGoal(mGoal) || _Debug_ClearFlag) {
				break;
			}

			yield return null;	//ゲームメインを続ける
		}

		//重さを移せないようにする
		OnCanShiftOperation(false);
        if (mPause != null) {
            mPause.canPause = false;
        }

		//プレイヤーを操作不可にする
		CanInputPlayer(false);
		CanJumpPlayer(false);

		//プレイヤーの回転が終わるまで待つ
		while (true) {
			if (mPlayer.IsRotation == false) {
				break;
			}
			yield return null;
		}


		//カメラのズームイン
		//

		//ズーム終了後のカメラ位置を変更
		mCameraMove.mEndPosition = GetGoalZoomCameraPosition();

		//カメラの開始地点を現在のカメラ位置にする
		mCameraMove.mStartPosition = mCameraMove.transform.position;

		mCameraMove.MoveStart();

		//カメラのズームイン終了まで待つ
		while (true) {
			if (mCameraMove.IsMoveEnd) {
				break;
			}
			yield return null;
		}


		//プレイヤーをゴールの中心まで歩かせる
		//
		mPlayer.GetComponent<MoveManager>().mask = LayerMask.GetMask(new string[] { "Stage", "Player", "Fence" });

		Vector3 lGoalCenter = mGoal.transform.position;

		//左側にいるなら
		if (mPlayer.transform.position.x <= lGoalCenter.x) {
			//右に歩かせる
			VirtualController.SetAxis(VirtualController.CtrlCode.Horizontal, 0.5f, 30.0f);

			//ゴールの中心を超えたら、歩かせるのをやめる
			while (true) {
				if (mPlayer.transform.position.x >= lGoalCenter.x) {
					Vector3 lPos = mPlayer.transform.position;
					lPos.x = lGoalCenter.x;
					mPlayer.transform.position = lPos;  //補正
					break;
				}
				yield return null;
			}
		}
		//左側にいるなら
		else if (mPlayer.transform.position.x > lGoalCenter.x) {
			//左に歩かせる
			VirtualController.SetAxis(VirtualController.CtrlCode.Horizontal, -0.5f, 30.0f);

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

		//左右移動の入力を消す
		mPlayer.GetComponent<MoveManager>().StopMoveHorizontalAll();
		VirtualController.SetAxis(VirtualController.CtrlCode.Horizontal, 0.0f, 30.0f);


		//プレイヤーの回転が終わるまで待つ
		//
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


		//BGMを止める
		SoundManager.SFade(mBGMInstance, SoundManager. SVolume(mBGMInstance), 0.0f, 0.5f, true);


		//移した回数を保存
		//
		
		//チュートリアル以外のステージなら
		if (Area.IsInStage() && Area.GetAreaNumber() != 0) {
			int lAreaNum = Area.GetAreaNumber();
			int lStageNum = Area.GetStageNumber();

			int lBeforeTimes = ScoreManager.Instance.ShiftTimes(lAreaNum, lStageNum);
			int lNowTimes = ScoreManager.Instance.ShiftTimes();

			//今までと今回で、移した回数の最小値を保存する
			bool lIsShortest = false;
			int lNewTimes = lBeforeTimes;
			if(lBeforeTimes == ScoreManager.cInvalidScoreTimes) {
				lNewTimes = lNowTimes;
			}
			if (lNowTimes < lBeforeTimes) {
				lNewTimes = lNowTimes;
				lIsShortest = true;
			}

			ScoreManager.Instance.ShiftTimes(lAreaNum, lStageNum, lNewTimes);
			ScoreManager.Instance.IsShortestTimes = lIsShortest;
		}

		//データを保存
		SaveData.Instance.Save();
		

		Cursor.visible = true;
		mResult.canGoal = true;
	}
	

	bool CanGoal(Goal lGoal) {

		//全てのボタンがオンでないなら
		if (!mGoal.IsAllButtonOn) {
			return false;   //ゴールできない
		}

		//プレイヤーがゴール枠に完全に入っていないなら
		if (!lGoal.IsInPlayer(mPlayer)) {
			return false;   //ゴールできない
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

		//持ち降ろしの最中なら
		if (mPlayer.GetComponent<Lifting>().IsLiftCantMove) {
			return false;
		}

		//ボックスを持っているなら
		if (mPlayer.GetComponent<Lifting>().IsLifting) {
			return false;
		}

		return true;    //ゴール可能

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
	Vector3 GetGoalZoomCameraPosition() {
		Vector3 lPosition = mGoal.transform.position;
		if(mGoal.transform.up.y <= 0.0f) {
			lPosition.y -= 2.0f;
		}
		lPosition.z = 40.0f;
		return lPosition;
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
}
