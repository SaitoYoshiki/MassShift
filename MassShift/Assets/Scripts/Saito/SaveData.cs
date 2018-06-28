using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData {

	//起動時に、セーブされたデータをロードする
	//
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void OnLoad() {
		Instance.LoadData();
	}

	//シングルトン
	//
	static SaveData sInstance;

	public static SaveData Instance {
		get {
			if(sInstance == null) {
				sInstance = new SaveData();
			}
			return sInstance;
		}
	}


	[System.Serializable]
	public class StageData {
		public int mShiftTimesOnClear;
	}

	public List<List<StageData>> mStageData;	//そのステージに関するデータ


	public class LastPlayStage {
		public int mAreaNumber = -1;
		public int mStageNumber = -1;
	}
	public LastPlayStage mLastPlayStage;	//最後に遊んだステージ番号
	
	
	public class EventDoneFlag {
		public bool mArea2Open;   //エリア2に行けるよう
		public bool mArea3Open;   //エリア3に行けるよう
	}
	public EventDoneFlag mEventDoneFlag;	//イベントを行ったかどうかのフラグ

	


	//現在のクリアデータを、外部にセーブする
	//
	void Save() {

		//ToDo
		//
	}


	//外部にセーブされているデータを、ロードしてくる
	//
	void LoadData() {

		//Listのリサイズ
		mStageData = new List<List<StageData>>(Area.GetAreaCount() + 1);
		
		for(int i = 0; i < Area.GetAreaCount() + 1; i++) {
			mStageData.Add(new List<StageData>(Area.GetStageCount(i)));

			for(int j = 0; j < Area.GetStageCount(i); j++) {
				var lStageData = new StageData();
				lStageData.mShiftTimesOnClear = -1;
				mStageData[i].Add(lStageData);
			}
		}


		mLastPlayStage = new LastPlayStage();
		mLastPlayStage.mAreaNumber = -1;
		mLastPlayStage.mStageNumber = -1;

		mEventDoneFlag = new EventDoneFlag();
		mEventDoneFlag.mArea2Open = false;
		mEventDoneFlag.mArea3Open = false;


		//ToDo
		//テキストからデータを読んでくる
		//
	}


	//対象のエリアに行けるかどうか
	//
	public bool CanGoArea(int aAreaNumber) {
		return true;
	}


	//ステージのクリアデータを取得（書き込み、読み込み可能）
	//
	public StageData Data(int aAreaNumber, int aStageNumber) {
		return mStageData[aAreaNumber][aStageNumber - 1];
	}
}
