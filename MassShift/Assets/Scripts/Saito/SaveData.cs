using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveData {

	//シングルトン
	//
	static SaveData sInstance;

	public static SaveData Instance {
		get {
			return sInstance;
		}
		set {
			if (sInstance == null) {
				sInstance = value;
			}
		}
	}


	[System.Serializable]
	public class StageData {
		public const int cInitTimes = -1;
		public int mShiftTimesOnClear;
	}

	[System.Serializable]
	public class AreaData {

		public AreaData(int aCount) {
			mStagesData = new List<StageData>(aCount);
		}

		public List<StageData> mStagesData;
	}

	public List<AreaData> mStageData;    //そのステージに関するデータ

	[System.Serializable]
	public class LastPlayStage {
		public int mAreaNumber = -1;
		public int mStageNumber = -1;
	}
	public LastPlayStage mLastPlayStage;    //最後に遊んだステージ番号

	[System.Serializable]
	public class EventDoneFlag {
		public bool mArea2Open;   //エリア2に行けるよう
		public bool mArea3Open;   //エリア3に行けるよう
		public bool mArea4Open;   //エリア4に行けるよう
	}
	public EventDoneFlag mEventDoneFlag;    //イベントを行ったかどうかのフラグ

	public static string Version {
		get {
			return "ver1.0";
		}
	}


	//ステージのクリアデータを取得（書き込み、読み込み可能）
	//
	public StageData Data(int aAreaNumber, int aStageNumber) {
		return mStageData[aAreaNumber].mStagesData[aStageNumber - 1];
	}
	public AreaData GetAreaData(int aAreaNumber) {
		return mStageData[aAreaNumber];
	}

	public void Save() {
		SaveDataIO.Save();
	}
}
