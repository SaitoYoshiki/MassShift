using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningManager : MonoBehaviour {

	#region CameraMove

	[SerializeField, Tooltip("カメラ")]
	GameObject mCamera;

	[SerializeField, Tooltip("カメラがズームアウトした位置")]
	GameObject mCameraZoomOutPosition;

	#endregion

	#region Sound

	[SerializeField, Tooltip("BGM")]
	GameObject mBGMPrefab;

	[SerializeField, Tooltip("起動音")]
	GameObject mAwakeSEPrefab;

	[SerializeField, Tooltip("カプセルが開く音")]
	GameObject mOpenSEPrefab;

	[SerializeField, Tooltip("びっくり音")]
	GameObject mSurpriseSEPrefab;

	#endregion

	[SerializeField, Tooltip("びっくりアイコン")]
	GameObject mSurpriseIcon;

	[SerializeField, Tooltip("カプセルのアニメーション")]
	Animator mCapsuleAnimator;

	// Use this for initialization
	void Start () {

		//オープニングのコルーチンを開始する
		StartCoroutine(OpeningCoroutine());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator OpeningCoroutine() {

		//
		//初期化
		//

		Player lPlayer = FindObjectOfType<Player>();

		//プレイヤーコンポーネントを切る
		lPlayer.enabled = false;

		//プレイヤーのモデルが少しだけこちらを向いているのを戻す
		lPlayer.CameraLookRatio = 0.0f;
		lPlayer.LookCamera();

		//プレイヤーの見た目の重さを0にする
		lPlayer.GetComponent<WeightManager>().WeightLvSeem = WeightManager.Weight.flying;

		//プレイヤーのアニメーションを止める
		lPlayer.GetComponent<PlayerAnimation>().StartNone();
		
		//BGMの再生
		SoundManager.SPlay(mBGMPrefab);


		//TODO:フェードイン

		//待機
		yield return new WaitForSeconds(1.0f);


		//
		//プレイヤーが起動する
		//

		//プレイヤーの見た目の重さを1にする
		FindObjectOfType<Player>().GetComponent<WeightManager>().WeightLvSeem = WeightManager.Weight.light;

		//プレイヤーの起動音の再生
		SoundManager.SPlay(mAwakeSEPrefab);
		
		//プレイヤーのアニメーションを再生
		lPlayer.GetComponent<PlayerAnimation>().ChangeState(PlayerAnimation.CState.cStandBy);

		//待機
		yield return new WaitForSeconds(1.0f);


		//
		//カメラのズームを戻す
		//

		//カメラの目標地点の設定
		mCamera.GetComponent<OpeningCameraMove>().mTarget = mCameraZoomOutPosition.transform.position;
		mCamera.GetComponent<OpeningCameraMove>().mSpeed = 10.0f;
		mCamera.GetComponent<OpeningCameraMove>().mIsMove = true;

		//カメラが移動完了するまで待つ
		while (true) {
			if (mCamera.GetComponent<OpeningCameraMove>().IsReach) break;
			yield return null;
		}


		//
		//カプセルを開く
		//

		//カプセルを開く
		mCapsuleAnimator.SetBool("IsOpen", true);

		//カプセルが開く音の再生
		SoundManager.SPlay(mOpenSEPrefab);

		//待機
		yield return new WaitForSeconds(3.0f);


		//
		//プレイヤーが歩き出す
		//
		const float cWalkSpeed = 2.0f;


		//歩くアニメーションの再生
		lPlayer.GetComponent<PlayerAnimation>().SetSpeed(0.5f * cWalkSpeed);
		lPlayer.GetComponent<PlayerAnimation>().ChangeState(PlayerAnimation.CState.cWalk);

		//歩く処理
		float lTime = 0.0f;
		while(true) {
			lPlayer.transform.position += Vector3.right * Time.deltaTime * cWalkSpeed;

			lTime += Time.deltaTime;
			if(lTime >= 3.0f) {
				break;	//終了
			}
			yield return null;
		}

		//歩くアニメーションを止める
		lPlayer.GetComponent<PlayerAnimation>().ChangeState(PlayerAnimation.CState.cStandBy);

		//待機
		yield return new WaitForSeconds(1.0f);


		//
		//あたりを見回す
		//

		//見回すアニメーションの再生
		lPlayer.GetComponent<PlayerAnimation>().ChangeState(PlayerAnimation.CState.cSwim);

		//待機
		yield return new WaitForSeconds(1.5f);


		//
		//脱出マークを見つけて驚く処理
		//

		//発見して驚く
		lPlayer.GetComponent<PlayerAnimation>().ChangeState(PlayerAnimation.CState.cRelease);

		//驚く音の再生
		SoundManager.SPlay(mSurpriseSEPrefab);

		//驚くアイコンの表示
		mSurpriseIcon.SetActive(true);

		//驚いている間、待機
		yield return new WaitForSeconds(1.5f);

		//驚くアイコンの表示
		mSurpriseIcon.SetActive(false);

		//驚き終わった後の待機
		yield return new WaitForSeconds(0.5f);


		//
		//画面外まで歩いていく
		//

		//歩くアニメーションの再生
		lPlayer.GetComponent<PlayerAnimation>().ChangeState(PlayerAnimation.CState.cWalk);

		//歩く処理
		lTime = 0.0f;
		while (true) {
			lPlayer.transform.position += Vector3.right * Time.deltaTime * cWalkSpeed;

			lTime += Time.deltaTime;
			if (lTime >= 3.0f) {
				break;  //終了
			}
			yield return null;
		}

		//画面外に出た後の待機
		yield return new WaitForSeconds(0.5f);

		//TODO:フェードアウト


		yield return null;
	}
}
