using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageClearManager : MonoBehaviour {

	//起動時に、セーブされたデータをロードする
	//
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void Load() {
		Instance.LoadData();
	}

	//シングルトン
	//
	static StageClearManager sInstance;

	public static StageClearManager Instance {
		get {
			if(sInstance == null) {
				sInstance = new StageClearManager();
			}
			return sInstance;
		}
	}


	[System.Serializable]
	public class StageClearData {
		public bool mIsClear;
	}

	List<List<StageClearData>> mStageClearData;	//そのステージのクリアに関するデータ
	bool mDoneArea2Event;   //エリア2に行けるようになるイベントをすでにやったか
	bool mDoneArea3Event;   //エリア3に行けるようになるイベントをすでにやったか


	//現在のクリアデータを、外部にセーブする
	//
	void SaveData() {

		//ToDo
		//
	}


	//外部にセーブされているデータを、ロードしてくる
	//
	void LoadData() {

		//ToDo
		//テキストからデータを読んでくる
		//

		//Listのリサイズ
		mStageClearData = new List<List<StageClearData>>(Area.GetAreaCount());
		for(int i = 0; i < Area.GetAreaCount(); i++) {
			mStageClearData[i] = new List<StageClearData>(Area.GetStageCount(i));

			for(int j = 0; j < Area.GetStageCount(i); j++) {
				var lStageClearData = new StageClearData();
				lStageClearData.mIsClear = false;
				mStageClearData[i][j] = lStageClearData;
			}
		}


		mDoneArea2Event = false;
		mDoneArea3Event = false;
	}


	//対象のエリアに行けるかどうか
	//
	public bool CanGoArea(int aAreaNumber) {
		return true;
	}


	//ステージのクリアデータを取得（書き込み、読み込み可能）
	//
	public StageClearData ClearData(int aAreaNumber, int aStageNumber) {
		return mStageClearData[aAreaNumber][aStageNumber];
	}
}
