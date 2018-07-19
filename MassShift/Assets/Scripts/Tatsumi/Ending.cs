using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ending : MonoBehaviour {
	[Space, SerializeField]
	Transform plTrans = null;
	[SerializeField]
	Transform shiftFromTrans = null;
	[SerializeField]
	Transform shiftToTrans = null;
	[SerializeField]
	BoxCollider boxLockCol= null;
	[SerializeField]
	Transform camZoomPoint = null;
	[SerializeField]
	Transform doorFrontTrans = null;
	[SerializeField]
	Transform doorInPointTrans = null;
	[SerializeField]
	Animator doorAnim = null;
	[SerializeField]
	GameObject shiftParticle = null;

	[Space, SerializeField]
	int particleNum = 100;
	[SerializeField]
	float particleStandbyTime = 0.1f;
	[SerializeField]
	float particleRandomVec = 1.0f;
	[SerializeField]
	float camZoomTime = 1.0f;
	[SerializeField]
	float doorLookTime = 1.0f;
	[SerializeField]
	float doorInTime = 2.0f;
	[SerializeField]
	float shiftHopForce = 25.0f;

	[Space, SerializeField]
	GameObject musicSoundPrefab = null;
	[SerializeField]
	GameObject shiftSoundPrefab = null;
	[SerializeField]
	GameObject impactSoundPrefab = null;
	[SerializeField]
	GameObject doorOpenSoundPrefab = null;

	[Space, SerializeField]
	Transform sizeUpParticle = null;
	[SerializeField]
	float sizeUpMin = 1.0f;
	[SerializeField]
	float sizeUpMax = 1.0f;

	[Space, SerializeField]
	Transform armTrans = null;
	[SerializeField]
	float armMoveTime = 0.2f;
	[SerializeField]
	Transform armBefPoint = null;
	[SerializeField]
	Transform armAftPoint = null;

	[Space, SerializeField]
	Transform impactEffectTrans = null;
	[SerializeField]
	GameObject impactEffect = null;
	[SerializeField]
	float shakeTime = 0.5f;
	[SerializeField]
	float shakeMagnitude = 0.5f;

	[Space, SerializeField]
	Material backgourndCellAfterMat;
	[SerializeField]
	List<BackgroundCell> backgroundCellList = new List<BackgroundCell>();


	[Space, SerializeField]
	Transform creditTrans = null;
	[SerializeField]
	Transform creditBeginPoint = null;
	[SerializeField]
	Transform creditEndPoint = null;
	[SerializeField]
	float creditTime = 10.0f;

	void Start() {
		// 背景のセルを全て取得
		backgroundCellList.AddRange(FindObjectsOfType<BackgroundCell>());

		// エンディングコルーチン
		StartCoroutine(EndingCoroutine());
	}

	IEnumerator EndingCoroutine() {
		// プレイヤーから自機操作権限を奪う
		for (int idx = 0; idx < (int)VirtualController.CtrlCode.Max; idx++) {
			VirtualController.SetAxis((VirtualController.CtrlCode)idx, 0.0f, float.MaxValue);
		}

		// 重さ移しコンポーネントを取得
		MassShift shift = GameObject.FindObjectOfType<MassShift>();

		// プレイヤーから重さ移し権限を奪う
		shift.mOverrideInputFromScript = true;

		// ムービー帯を表示
		Camera.main.GetComponent<MovieBelt>().IsDisplay = true;

		// カーソル位置をプレイヤー位置に移動
		shift.SetCursorPosition(plTrans.position);

		// 待機
		yield return new WaitForSeconds(1.6f);

		// BGM再生
		SoundManager.SPlay(musicSoundPrefab);

		// 待機
		yield return new WaitForSeconds(2.4f);

		// カーソル位置のオブジェクトを生成
		GameObject cursorPoint = new GameObject("CursorPoint");
		cursorPoint.transform.parent = transform;

		// カーソルを移す元に移動
		HermiteCurveMove hermite = cursorPoint.AddComponent<HermiteCurveMove>();
		hermite.SetPoints(plTrans.position, shiftFromTrans.position, ((plTrans.position + shiftFromTrans.position) * 0.5f));
		while (hermite.Ratio <= 1.0f) {
			shift.SetCursorPosition(hermite.Point);
			yield return null;
		}
		shift.SetCursorPosition(shiftFromTrans.position);
		Destroy(hermite);

		// 待機
		yield return new WaitForSeconds(0.5f);

		// 重さ移し入力を有効化
		shift.mInputClick = true;

		// 待機
		yield return new WaitForSeconds(0.5f);

		// カーソルを移す先に移動
		hermite = cursorPoint.AddComponent<HermiteCurveMove>();
		hermite.SetPoints(shiftFromTrans.position, shiftToTrans.position, ((shiftFromTrans.position + shiftToTrans.position) * 0.5f));
		Debug.LogWarning("LoopStart");
		while (hermite.Ratio <= 1.0f) {
			shift.SetCursorPosition(hermite.Point);
			yield return null;
		}
		shift.SetCursorPosition(shiftToTrans.position);
		Debug.LogWarning("LoopEnd");
		Destroy(hermite);

		// 待機
		yield return new WaitForSeconds(0.5f);

		// 重さ移し入力を無効化
		shift.mInputClick = false;

		// カーソルを非表示
		shift.mInvisibleCursor = true;

		// カーソルを移動する時間を設定
		float cursorMoveTime = (Time.time + shift.ShiftTimes);

		// 重さ移し演出を強化
		List<GameObject> particleList = new List<GameObject>();
		Vector3 center = ((shiftFromTrans.position + shiftToTrans.position) * 0.5f);
		Vector3 pointA = new Vector3(((shiftFromTrans.position.x - center.x) * particleRandomVec + center.x), ((shiftToTrans.position.y - center.y) * particleRandomVec + center.y), center.z);
		Vector3 pointB = new Vector3(((shiftToTrans.position.x - center.x) * particleRandomVec + center.x), ((shiftFromTrans.position.y - center.y) * particleRandomVec + center.y), center.z);
		float lastWaitTime = Time.time;
		int endCnt = 1;
		SetSizeUpParticle((float)endCnt / (float)particleNum);
		for (int cnt = 1; cnt < particleNum; cnt++) {

			GameObject newParticle = Instantiate(shiftParticle);
			hermite = newParticle.AddComponent<HermiteCurveMove>();

			//float ratio = Mathf.Clamp((Random.value * 5.0f) / 5.0f, 0.0f, 1.0f);    // 0.0f～1.0f、0.0fや1.0fより0.5f付近が出やすい
			//Vector3 midPoint = ((pointA * ratio) + (pointB * (1.0f - ratio)));

			Vector3 vec = new Vector3((Random.value * 2.0f - 1.0f), (Random.value * 2.0f - 1.0f), 0.0f).normalized;
			Vector3 midPoint = (shiftFromTrans.position) + vec * shiftHopForce;

			hermite.SetPoints(shiftFromTrans.position, shiftToTrans.position, midPoint);
			hermite.EndDestroy = true;
			particleList.Add(newParticle);

			SoundManager.SPlay(shiftSoundPrefab);

			while (Time.time < (lastWaitTime + particleStandbyTime)) {
				// カーソル移動判定
				if (cursorMoveTime < Time.time) {
					shift.SetCursorPosition(Vector3.right * 100.0f);
				}

				// 終了したパーティクルをカウント
				while ((particleList.Count > 0) && !particleList[0]) {
					particleList.RemoveAt(0);
					endCnt++;
					SetSizeUpParticle((float)endCnt / (float)particleNum);
				}
				yield return null;
			}
			lastWaitTime = Time.time;
			//yield return new WaitForSeconds(particleStandbyTime);
		}

		// 待機
		lastWaitTime = Time.time;
		while (endCnt < (particleNum - 1)) {
			// 終了したパーティクルをカウント
			while ((particleList.Count > 0) && !particleList[0]) {
				SoundManager.SPlay(shiftSoundPrefab);
				particleList.RemoveAt(0);
				endCnt++;
				SetSizeUpParticle((float)endCnt / (float)particleNum);
				lastWaitTime = Time.time;
			}
			yield return null;
		}
		SetSizeUpParticle(1.0f);

		// 落下開始
		boxLockCol.enabled = false;

		// 待機
		yield return new WaitForSeconds(0.04f);

		// アーム移動
		SoundManager.SPlay(impactSoundPrefab);
		float armMoveBeginTime = Time.time;
		while (true) {
			if ((armMoveBeginTime + armMoveTime) <= Time.time) {
				break;
			}
			float ratio = ((Time.time - armMoveBeginTime) / armMoveTime);
			armTrans.position = new Vector3(Mathf.Lerp(armBefPoint.position.x, armAftPoint.position.x, ratio), Mathf.Lerp(armBefPoint.position.y, armAftPoint.position.y, ratio), Mathf.Lerp(armBefPoint.position.z, armAftPoint.position.z, ratio));
			armTrans.rotation = Quaternion.Slerp(armBefPoint.rotation, armAftPoint.rotation, ratio);
			yield return null;
		}

		// 待機
		yield return new WaitForSeconds(0.5f);

		// 着地演出
		ShakeCamera.ShakeAll(shakeTime, shakeMagnitude);
		GameObject impactEffectObj = Instantiate(impactEffect);
		impactEffectObj.transform.position = impactEffectTrans.position;
		SoundManager.SPlay(impactSoundPrefab);

		// 背景セルの色を全て変更
		foreach (var cell in backgroundCellList) {
			cell.PowerfulLighting(backgourndCellAfterMat);
		}
			
		// 待機
		yield return new WaitForSeconds(1.0f);

		// カメラを扉にズーム
		float zoomBeginTime = Time.time;
		Vector3 defCamPos = Camera.main.transform.position;
		while (true) {
			float ratio = ((Time.time - zoomBeginTime) / camZoomTime);

			if (ratio >= 1.0f) {
				Camera.main.transform.position = camZoomPoint.position;
				break;
			}

			Camera.main.transform.position = new Vector3(Mathf.Lerp(defCamPos.x, camZoomPoint.position.x, ratio), Mathf.Lerp(defCamPos.y, camZoomPoint.position.y, ratio), Mathf.Lerp(defCamPos.z, camZoomPoint.position.z, ratio));
			yield return null;
		}

		// 扉を開く
		doorAnim.Play("Open");
		SoundManager.SPlay(doorOpenSoundPrefab);

		// 待機
		yield return new WaitForSeconds(3.0f);

		// 自機を扉の前に移動
		while (plTrans.position.x < doorFrontTrans.position.x) {
			VirtualController.SetAxis(VirtualController.CtrlCode.Horizontal, 0.1f);
			yield return null;
		}

		// 自機の位置を補正
		plTrans.GetComponent<MoveManager>().StopMoveHorizontalAll();
		VirtualController.SetAxis(VirtualController.CtrlCode.Horizontal, 0.0f, float.MaxValue);
		plTrans.position = new Vector3(doorFrontTrans.position.x, plTrans.position.y, plTrans.position.z);

		// 自機を扉に向ける
		Player pl = plTrans.GetComponent<Player>();
		pl.CamLookDown = true;
		pl.CameraLookRatio = 0.0f;
		pl.LookCamera();
		pl.enabled = false;
		Quaternion defPlRot = pl.transform.rotation;
		Quaternion doorLookRot = Quaternion.LookRotation(doorInPointTrans.position - pl.transform.position) * Quaternion.Euler(0.0f, -90.0f, 0.0f);
		
		float doorLookBeginTime = Time.time;
		while (true) {
			float ratio = ((Time.time - doorLookBeginTime) / doorLookTime);
			if (ratio >= 1.0f) {
				plTrans.rotation = doorLookRot;
				break;
			}
			plTrans.rotation = Quaternion.Slerp(defPlRot, doorLookRot, ratio);
			yield return null;
		}

		// 待機
		PlayerAnimation plAnim = pl.GetComponent<PlayerAnimation>();
		plAnim.StartStandBy();
		yield return new WaitForSeconds(1.0f);
		plAnim.StartWalk();

		// 自機を扉の中に移動
		Camera.main.transform.parent = plTrans;
		Vector3 defPlPos = plTrans.position;
		float doorInBeginTime = Time.time;
		while (true) {
			float ratio = ((Time.time - doorInBeginTime) / doorInTime);
			if (ratio >= 1.0f) {
				plTrans.position = doorInPointTrans.position;
				break;
			}
			plTrans.position = new Vector3(Mathf.Lerp(defPlPos.x, doorInPointTrans.position.x, ratio), Mathf.Lerp(defPlPos.y, doorInPointTrans.position.y, ratio), Mathf.Lerp(defPlPos.z, doorInPointTrans.position.z, ratio));
			yield return null;
		}

		// 待機
		yield return new WaitForSeconds(0.75f);

		// 立ち止まる
		plAnim.StartStandBy();

		// ムービー帯解除
		Camera.main.GetComponent<MovieBelt>().IsDisplay = false;
		while (Camera.main.GetComponent<MovieBelt>().Ratio <= 0.0f) {
			yield return null;
		};

		// 待機
		yield return new WaitForSeconds(0.5f);

		float creditBeginTime = Time.time;
		while (true) {
			float ratio = ((Time.time - creditBeginTime) / creditTime);
			if (ratio >= 1.0f) {
				creditTrans.position = creditEndPoint.position;
				break;
			}
			creditTrans.position = new Vector3(Mathf.Lerp(creditBeginPoint.position.x, creditEndPoint.position.x, ratio), Mathf.Lerp(creditBeginPoint.position.y, creditEndPoint.position.y, ratio), Mathf.Lerp(creditBeginPoint.position.z, creditEndPoint.position.z, ratio));
			yield return null;
		}



		// シーン遷移
		((ChangeScene)FindObjectOfType(typeof(ChangeScene))).OnTitleButtonDown();	// タイトルへ
	}

	void SetSizeUpParticle(float _ratio) {
		sizeUpParticle.localScale = Vector3.one * Mathf.Lerp(sizeUpMin, sizeUpMax, _ratio);
	}
}
