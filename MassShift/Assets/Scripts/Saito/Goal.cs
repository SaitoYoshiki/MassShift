using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Goal : MonoBehaviour {

	// Use this for initialization
	void Awake() {

		Resize();

		/*
		mLampList = new List<GameObject>();
		for(int i = 0; i < mLampModel.transform.childCount; i++) {
			if(mLampModel.transform.GetChild(i).name == mLampPrefab.name) {
				mLampList.Add(mLampModel.transform.GetChild(i).gameObject);
			}
		}
		mLampList = mLampList.OrderByDescending(x => x.transform.localPosition.y).ToList();
		*/

		TurnLamp();

		mAllPlayerList = FindObjectsOfType<Player>().ToList();

		//ステージ中なら
		if(Area.IsInStage()) {
			Utility.ChangeMaterialColor(mModel, mInsideMaterial, "_EmissionColor", (Color.white * 1.1f));
		}
		else {
			Utility.ChangeMaterialColor(mModel, mInsideMaterial, "_EmissionColor", Color.black);
		}
	}
	
	// Update is called once per frame
	void Update () {
		UpdateOpenRate();
		ModelAnimation();

		UpdateLamp();
	}
	private void FixedUpdate() {
		UpdateInPlayer();
	}

	//開いている割合を更新
	void UpdateOpenRate() {

		if (IsAllButtonOn) {
			mOpenRate += 1.0f / mOpenTakeTime * Time.deltaTime;
		}
		else {
			mOpenRate -= 1.0f / mCloseTakeTime * Time.deltaTime;
		}
		mOpenRate = Mathf.Clamp01(mOpenRate);
	}

	//ゴールのアニメーションの再生
	void ModelAnimation() {

		//このフレームで開いたなら
		if(mBeforeAllButtonOn == false) {
			if(IsAllButtonOn == true) {
				SetAnimation(true);
				if(mPlayOpenSE) {
					SoundManager.SPlay(mOpenSE);    //音を鳴らす
				}
			}
		}

		//このフレームで閉まったなら
		if (mBeforeAllButtonOn == true) {
			if (IsAllButtonOn == false) {
				SetAnimation(false);
			}
		}

		mBeforeAllButtonOn = IsAllButtonOn;
	}

	//ゴールエリアに含まれているプレイヤーを更新
	void UpdateInPlayer() {
		mInPlayerList = GetInPlayer();
	}

	//ランプを点灯させる
	void TurnLamp() {

		for (int i = 0; i < ButtonOnCount(); i++) {
			mLampList[i].GetComponent<LampFade>().TurnOn();
		}

		for (int i = ButtonOnCount(); i < mButtonList.Count; i++) {
			mLampList[i].GetComponent<LampFade>().TurnOff();
		}
	}

	void UpdateLamp()
	{
		int lButtonOnCount = ButtonOnCount();

		//ボタンの点灯数に変化があったら
		if (mBeforeButtonOnCount != lButtonOnCount) {
			TurnLamp();
		}

		mBeforeButtonOnCount = lButtonOnCount;
	}


	//扉が開いたり、閉まったりするアニメーションを再生する
	void SetAnimation(bool lOpen) {

		Animator a = mModel.GetComponentInChildren<Animator>();
		float lAnimLength = a.GetCurrentAnimatorClipInfo(0)[0].clip.length;

		a.Play("Open", 0, mOpenRate);

		if (lOpen) {
			a.SetFloat("Speed", 1.0f / mOpenTakeTime * lAnimLength);
		}
		else {
			a.SetFloat("Speed", -1.0f / mCloseTakeTime * lAnimLength);
		}
	}

	//ボタンが全てオンかどうか
	public bool IsAllButtonOn {
		get {
			//強制的に開くフラグがtrueなら
			if (mOpenForce) return true;

			//ボタンの数が0ではなくて
			if (ButtonCount() != 0) {
				//全てのボタンが点灯していたら
				if (ButtonOnCount() == ButtonCount()) {
					return true;
				}
			}
			return false;
		}
	}


	List<Player> GetInPlayer() {
		var pl = new List<Player>();
		foreach(var p in mAllPlayerList) {
			if(IsCollisionComplete(mGoalTrigger.GetComponent<BoxCollider>(), p.GetComponent<Collider>())) {
				pl.Add(p);
			}
		}
		return pl;
	}

	public bool IsInPlayer(Player p) {
		return mInPlayerList.Contains(p);
	}

	//オンになっているボタンの数を取得する
	int ButtonOnCount() {

		int lTotal = 0;

		foreach (var g in mButtonList) {
			if (g == null) {
				Debug.LogWarning("Goal's Button is null");
				return -1;
			}
			if (g.IsButtonOn == true) {
				lTotal += 1;
			}
		}

		return lTotal;
	}

	//オンにする必要のあるボタンの数を取得する
	int ButtonCount() {
		return mButtonList.Count;
	}

	//ゴールが完全に開いているか
	bool IsOpen {
		get { return mOpenRate >= 1.0f; }
	}


	//コライダーが完全にエリアに入っているかどうか
	static bool IsCollisionComplete(BoxCollider aArea, Collider aObject) {

		//エリアに入っていなかったら、完全に入ることはない
		bool res = Physics.OverlapBox(aArea.bounds.center, aArea.bounds.size / 2.0f).Contains(aObject);
		if (res == false) return false;


		//隣り合うエリアに入っていたら、完全には入っていない
		//
		res = Physics.OverlapBox(GetPosition(aArea, Vector3.up), aArea.bounds.size / 2.0f).Contains(aObject);
		if (res == true) return false;

		res = Physics.OverlapBox(GetPosition(aArea, Vector3.down), aArea.bounds.size / 2.0f).Contains(aObject);
		if (res == true) return false;

		res = Physics.OverlapBox(GetPosition(aArea, Vector3.right), aArea.bounds.size / 2.0f).Contains(aObject);
		if (res == true) return false;

		res = Physics.OverlapBox(GetPosition(aArea, Vector3.left), aArea.bounds.size / 2.0f).Contains(aObject);
		if (res == true) return false;

		//ここから先はZ方向のチェックなので、とりあえずは必要ない
		return true;


		res = Physics.OverlapBox(GetPosition(aArea, Vector3.up), aArea.bounds.size / 2.0f).Contains(aObject);
		if (res == true) return false;

		res = Physics.OverlapBox(GetPosition(aArea, Vector3.up), aArea.bounds.size / 2.0f).Contains(aObject);
		if (res == true) return false;

		return true;
	}

	static Vector3 GetPosition(BoxCollider aCollider, Vector3 aOffset) {
		Vector3 aPositionOffset = aCollider.transform.rotation * new Vector3(aCollider.bounds.size.x * aOffset.x, aCollider.bounds.size.y * aOffset.y, aCollider.bounds.size.z * aOffset.z);
		return aCollider.transform.position + aPositionOffset;
	}


	void Resize() {

		mButtonList = FindObjectsOfType<Button>().ToList();

		//現在のモデルの削除
		for (int i = mLampModel.transform.childCount - 1; i >= 0; i--) {
			Destroy(mLampModel.transform.GetChild(i).gameObject);
		}

		//ボタンの数が0なら、ランプ部分を作成しない
		if (mButtonList.Count == 0) return;

		//モデルの配置

		mLampList = new List<GameObject>();
		//ランプ
		for (int i = 0; i < mButtonList.Count; i++) {
			GameObject lLamp = Instantiate(mLampPrefab, mLampModel.transform);
			lLamp.transform.localPosition = mLampBasePosition + Vector3.down * mLampInterval * i;
			mLampList.Add(lLamp);
		}

		//土台
		Vector3 lBase = mLampBasePosition;

		//上端
		GameObject lTop = Instantiate(mLampTopPrefab, mLampModel.transform);
		lTop.transform.localPosition = lBase;

		//真ん中
		for (int i = 0; i < mButtonList.Count - 1; i++) {
			lBase += Vector3.down * mLampInterval;
			GameObject lMid = Instantiate(mLampMidPrefab, mLampModel.transform);
			lMid.transform.localPosition = lBase - Vector3.down * mLampInterval * 0.5f;
		}

		//下端
		GameObject lBottom = Instantiate(mLampBottomPrefab, mLampModel.transform);
		lBottom.transform.localPosition = lBase;
	}

#if UNITY_EDITOR

	[ContextMenu("Resize")]
	public void ResizeOnEditor() {

		if (this == null) return;
		if (EditorUtility.IsPrefab(gameObject)) return;
		if (UnityEditor.EditorApplication.isPlaying) return;

		//現在のモデルの削除
		for (int i = mLampModel.transform.childCount - 1; i >= 0; i--) {
			if (EditorUtility.IsInPrefab(mLampModel.transform.GetChild(i).gameObject, EditorUtility.GetPrefab(gameObject))) continue;
			EditorUtility.DestroyGameObject(mLampModel.transform.GetChild(i).gameObject);
		}

		//モデルの配置


		//ランプ
		for (int i = 0; i < mButtonList.Count; i++) {
			GameObject lLamp = EditorUtility.InstantiatePrefab(mLampPrefab, mLampModel);
			lLamp.transform.localPosition = mLampBasePosition + Vector3.down * mLampInterval * i;
		}

		//土台
		Vector3 lBase = mLampBasePosition;

		//上端
		GameObject lTop = EditorUtility.InstantiatePrefab(mLampTopPrefab, mLampModel);
		lTop.transform.localPosition = lBase;

		//真ん中
		for (int i = 0; i < mButtonList.Count - 1; i++) {
			lBase += Vector3.down * mLampInterval;
			GameObject lMid = EditorUtility.InstantiatePrefab(mLampMidPrefab, mLampModel);
			lMid.transform.localPosition = lBase - Vector3.down * mLampInterval * 0.5f;
		}

		//下端
		GameObject lBottom = EditorUtility.InstantiatePrefab(mLampBottomPrefab, mLampModel);
		lBottom.transform.localPosition = lBase;
	}


	private void OnValidate() {
		//UnityEditor.EditorApplication.delayCall += ResizeOnEditor;
	}

#endif

	//[SerializeField]
	List<Button> mButtonList;

	List<Player> mAllPlayerList;

	[SerializeField, Tooltip("扉を強制的に開かせる")]
	public bool mOpenForce = false;

	bool mBeforeAllButtonOn = false;
	int mBeforeButtonOnCount = 0;

	float mOpenRate = 0.0f; //扉が開いている割合

	[HideInInspector]
	public bool mPlayOpenSE = true;

	[SerializeField, Tooltip("扉が開くのに何秒かかるか"), EditOnPrefab]
	float mOpenTakeTime = 1.0f;

	[SerializeField, Tooltip("扉が閉まるのに何秒かかるか"), EditOnPrefab]
	float mCloseTakeTime = 1.0f;

	[SerializeField, Tooltip("扉が開くときに鳴るSE"), EditOnPrefab]
	GameObject mOpenSE;

	[SerializeField, Disable]
	List<Player> mInPlayerList = new List<Player>();


	[SerializeField, PrefabOnly,EditOnPrefab, Tooltip("ランプの上端のモデル")]
	GameObject mLampTopPrefab;

	[SerializeField, PrefabOnly, EditOnPrefab, Tooltip("ランプの真ん中のモデル")]
	GameObject mLampMidPrefab;

	[SerializeField, PrefabOnly, EditOnPrefab, Tooltip("ランプの下端のモデル")]
	GameObject mLampBottomPrefab;

	[SerializeField, PrefabOnly, EditOnPrefab, Tooltip("ランプのモデル")]
	GameObject mLampPrefab;

	[SerializeField, EditOnPrefab, Tooltip("ランプを配置する間隔")]
	float mLampInterval = 1.0f;

	List<GameObject> mLampList;	//ランプのインスタンス。０から順に、上から

	[SerializeField, EditOnPrefab, Tooltip("ゴールのモデル")]
	GameObject mModel;

	[SerializeField, EditOnPrefab, Tooltip("プレイヤーが完全に入っているとゴール")]
	GameObject mGoalTrigger;

	[SerializeField, EditOnPrefab, Tooltip("ランプを配置する基準となる")]
	Vector3 mLampBasePosition;

	[SerializeField, EditOnPrefab, Tooltip("ランプ全てのモデルの親")]
	GameObject mLampModel;

	[SerializeField, EditOnPrefab, Tooltip("扉の内側のマテリアル")]
	Material mInsideMaterial;
}
