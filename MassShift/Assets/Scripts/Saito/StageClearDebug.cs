using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageClearDebug : MonoBehaviour {

	[SerializeField]
	UnityEngine.UI.Toggle mToggle;

	[SerializeField]
	GameObject mExtend;


	[SerializeField]
	UnityEngine.UI.InputField mAreaInput;
	[SerializeField]
	UnityEngine.UI.InputField mStageInput;
	[SerializeField]
	UnityEngine.UI.InputField mTimesInput;

	[SerializeField]
	UnityEngine.UI.Button mClearButton;
	[SerializeField]
	UnityEngine.UI.Button mNotClearButton;
	[SerializeField]
	UnityEngine.UI.Button mClearAllButton;
	[SerializeField]
	UnityEngine.UI.Button mNotClearAllButton;

	// Use this for initialization
	void Start () {
		mToggle.onValueChanged.AddListener(ToggleValueChanged);
		ToggleValueChanged(false);

		mClearButton.onClick.AddListener(Clear);
		mNotClearButton.onClick.AddListener(NotClear);
		mClearAllButton.onClick.AddListener(AllClear);
		mNotClearAllButton.onClick.AddListener(NotAllClear);
	}

	// Update is called once per frame
	void Update () {
		
	}

	void ToggleValueChanged(bool aOn) {
		if(aOn) {
			mToggle.GetComponentInChildren<UnityEngine.UI.Text>().text = "閉じる";
			mExtend.SetActive(true);
		}
		else {
			mToggle.GetComponentInChildren<UnityEngine.UI.Text>().text = "開く";
			mExtend.SetActive(false);
		}
	}


	bool GetInputData(out int aAreaNumber, out int aStageNumber, out int aTimes) {

		aAreaNumber = 0;
		aStageNumber = 0;
		aTimes = 0;

		aAreaNumber = AreaNumber();
		if (Area.IsValidArea(aAreaNumber) == false) return false;

		aStageNumber = StageNumber();
		if (Area.IsValidStage(aAreaNumber, aStageNumber) == false) return false;

		aTimes = Times();

		return true;
	}

	int AreaNumber() {
		int o;
		if (int.TryParse(mAreaInput.text, out o)) {
			return o;
		}
		return -1;
	}
	int StageNumber() {
		int o;
		if(int.TryParse(mStageInput.text, out o)) {
			return o;
		}
		return -1;
	}
	int Times() {
		int o;
		if (int.TryParse(mTimesInput.text, out o)) {
			return o;
		}
		return -1;
	}


	void Clear() {
		int lAreaNumber;
		int lStageNumber;
		int lTimes;
		if(GetInputData(out lAreaNumber, out lStageNumber, out lTimes)) {
			SaveData.Instance.Data(lAreaNumber, lStageNumber).mShiftTimesOnClear = lTimes;
			SaveData.Instance.Save();
		}
	}
	void NotClear() {
		int lAreaNumber;
		int lStageNumber;
		int lTimes;
		if (GetInputData(out lAreaNumber, out lStageNumber, out lTimes)) {
			SaveData.Instance.Data(lAreaNumber, lStageNumber).mShiftTimesOnClear = SaveData.StageData.cInitTimes;
			SaveData.Instance.Save();
		}
	}

	//全ステージをクリア済みにする
	//
	void AllClear() {
		for (int a = 0; a < SaveData.Instance.mStageData.Count; a++) {
			for (int s = 0; s < SaveData.Instance.mStageData[a].mStagesData.Count; s++) {
				SaveData.Instance.mStageData[a].mStagesData[s].mShiftTimesOnClear = ScoreManager.Instance.Score3Times(a, s + 1);
			}
		}
		SaveData.Instance.Save();
	}

	//クリアしていない状態にする
	//
	void NotAllClear() {
		foreach (var a in SaveData.Instance.mStageData) {
			foreach (var s in a.mStagesData) {
				s.mShiftTimesOnClear = SaveData.StageData.cInitTimes;
			}
		}
		SaveData.Instance.Save();
	}
}
