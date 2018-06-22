using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Button : MonoBehaviour {

	// Use this for initialization
	void Start () {
		mLedgeStartPosition = mLedge.transform.position;
		ChangeLightColor(mButtonOffColor * mButtonOffColorPower);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void FixedUpdate() {
		UpdateIsPush();

		UpdatePushRate();
		MoveLedge();

		UpdateEffect();
	}

	//押されている割合を更新する
	void UpdatePushRate() {

		if (IsPush) {
			//mPushRate += 1.0f / mPushTakeTime * Time.deltaTime;
			mPushingTime += Time.fixedDeltaTime;
			if(mPushingTime >= 0.1f) {
				mPushRate = 1.0f;   //いきなり最大まで押し込む
			}
			else {
				mPushRate = 0.0f;
			}
		}
		else {
			mPushRate -= 1.0f / mReleaseTakeTime * Time.deltaTime;
			//mPushRate = 0.0f;   //いきなり戻る
			mPushingTime = 0.0f;
		}

		mPushRate = Mathf.Clamp01(mPushRate);
	}

	//ボタンを動かす
	void MoveLedge() {
		Vector3 lNewPosition = mLedgeStartPosition + (mLedgeMoveEnd.transform.position - mLedgeMoveStart.transform.position) * mPushRate;
		MoveManager.MoveTo(lNewPosition, mLedge, LayerMask.GetMask(new string[] { "Box", "Player"}), true);
	}

	//ライトを点灯させる
	void UpdateEffect() {

		//点灯した瞬間
		if(mBeforeButtonOn == false && IsButtonOn == true) {
			ChangeLightColor(mButtonOnColor * mButtonOnColorPower);
			SoundManager.SPlay(mPushSE);
			var g = Instantiate(mPushEffect, transform);
			g.transform.position = mPushEffectTransform.transform.position;
			g.transform.rotation = mPushEffectTransform.transform.rotation;
		}

		//消えた瞬間
		if (mBeforeButtonOn == true && IsButtonOn == false) {
			ChangeLightColor(mButtonOffColor * mButtonOffColorPower);
		}

		mBeforeButtonOn = IsButtonOn;
	}

	//ライトの色を変える
	void ChangeLightColor(Color aColor) {
		Utility.ChangeMaterialColor(mLightModel, mLightMaterial, "_EmissionColor", aColor);
	}

#if UNITY_EDITOR

	[ContextMenu("Resize")]
	public void Resize() {
		if (this == null) return;
		if (EditorUtility.IsPrefab(gameObject)) return;

		switch (mDirection) {
			case CDirection.cUp:
				transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
				break;
			case CDirection.cDown:
				transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
				break;
			case CDirection.cLeft:
				transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
				break;
			case CDirection.cRight:
				transform.rotation = Quaternion.Euler(0.0f, 0.0f, 270.0f);
				break;
			case CDirection.cNone:
				Debug.LogError("Button Direction Is None", this);
				break;
		}
	}

	private void OnValidate() {
		Resize();
	}

#endif


	[SerializeField, Tooltip("押されるのにかかる時間"), EditOnPrefab]
	float mPushTakeTime;

	[SerializeField, Tooltip("離されるのにかかる時間"), EditOnPrefab]
	float mReleaseTakeTime;

	enum CDirection {
		cNone,
		cUp,
		cDown,
		cLeft,
		cRight
	}
	[SerializeField, Tooltip("ボタンの方向")]
	CDirection mDirection;

	[SerializeField, Tooltip("ボタンがオンになる、押される割合"), EditOnPrefab]
	float mPushRateOn = 1.0f;

	[SerializeField, Tooltip("ボタンがオフになる、押される割合"), EditOnPrefab]
	float mPushRateOff = 1.0f;

	[SerializeField, Tooltip("ボタンがオンになる時の音"), EditOnPrefab]
	GameObject mPushSE;

	[SerializeField, Tooltip("ボタンが押されたときに発生するエフェクト"), EditOnPrefab]
	GameObject mPushEffect;

	[SerializeField, Tooltip("ボタンが押されたときに発生するエフェクトの、発生位置"), EditOnPrefab]
	GameObject mPushEffectTransform;


	float mPushingTime = 0.0f;	//押し続けられている時間

	float mPushRate;    //現在押されている割合

	//ボタンの上にオブジェクトが乗って、押されているか
	bool _IsPush = false;
	public bool IsPush {
		get {
			if (mIsPush_Debug) return true;
			return _IsPush;
		}
	}

	//プレイヤーがボタンを押しているか
	public bool IsPlayerPush {
		get {
			foreach(var h in mHitObjectList) {
				if(h.layer == LayerMask.NameToLayer("Player")) {
					return true;
				}
			}
			return false;
		}
	}


	//ボタンの上にオブジェクトが乗っているかの更新
	//
	void UpdateIsPush() {
		LayerMask lLayerMask = LayerMask.GetMask(new string[] { "Box", "Player" });

		mHitObjectList.Clear();

		bool lIsPush = false;

		Collider[] lHitColliders = Physics.OverlapBox(mWeightCheckCollider.transform.position, mWeightCheckCollider.transform.localScale / 2.0f, mWeightCheckCollider.transform.rotation, lLayerMask);
		foreach(var c in lHitColliders) {
			if(c.gameObject.layer == LayerMask.NameToLayer("Box") || c.gameObject.layer == LayerMask.NameToLayer("Player")) {
				lIsPush = true;
				mHitObjectList.Add(c.gameObject);
			}
		}

		_IsPush = lIsPush;
	}


	[SerializeField]
	bool mIsPush_Debug;

	//ボタンが完全に押されているか
	public bool IsButtonOn {
		get {
			//オンになる割合とオフになる割合が違う
			if (IsPush) {
				if (mPushRate >= mPushRateOn) return true;
			}
			else {
				if (mPushRate >= mPushRateOff) return true;
			}
			return false;
		}
	}
	bool mBeforeButtonOn = false;

	List<GameObject> mHitObjectList = new List<GameObject>();	//押しているオブジェクトのリスト

	Vector3 mLedgeStartPosition;    //Ledgeの開始位置

	[SerializeField, Tooltip("Ledge"), EditOnPrefab]
	GameObject mLedge;

	[SerializeField, Tooltip("光らせるモデル")]
	GameObject mLightModel;

	[SerializeField, Tooltip("光らせるマテリアル"), EditOnPrefab]
	Material mLightMaterial;

	[SerializeField]
	Color mButtonOnColor;

	[SerializeField]
	float mButtonOnColorPower;

	[SerializeField]
	Color mButtonOffColor;

	[SerializeField]
	float mButtonOffColorPower;


	[SerializeField, Tooltip("Ledgeの移動開始位置"), EditOnPrefab]
	GameObject mLedgeMoveStart;

	[SerializeField, Tooltip("Ledgeの移動終了位置"), EditOnPrefab]
	GameObject mLedgeMoveEnd;

	[SerializeField, Tooltip("押されているオブジェクトを見つけるときに使うコライダー"), EditOnPrefab]
	GameObject mWeightCheckCollider;

}