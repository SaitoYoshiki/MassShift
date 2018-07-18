using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SaveDataIO {

	//起動時に、セーブされたデータをロードする
	//
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void OnLoad() {
		Load();
	}



	//現在のクリアデータを、外部にセーブする
	//
	public static void Save() {
		using (FileStream fs = new FileStream(GetSaveFilePath(), FileMode.Create, FileAccess.Write)) {
			using (StreamWriter sw = new StreamWriter(fs)) {
				sw.WriteLine(SaveData.Version);
				string json = JsonUtility.ToJson(SaveData.Instance);
				sw.WriteLine(json);
			}
		}
	}


	//外部にセーブされているデータを、ロードしてくる
	//
	static void Load() {

		//セーブデータの初期化
		SaveData s = GetInitSaveData();
		

		//
		//テキストからデータを読んでくる
		//

		//ファイルが存在しなければ、何もロードしない
		if (!File.Exists(GetSaveFilePath())) {
			Debug.Log("SaveData Not Exist");
			SaveData.Instance = s;
			return;
		}


		//ファイルを開いてロードする
		//
		using (FileStream fs = new FileStream(GetSaveFilePath(), FileMode.Open, FileAccess.Read)) {

			using (StreamReader sr = new StreamReader(fs)) {

				//バージョンが違うならロードしない
				string lVersion = sr.ReadLine();
				if (lVersion != SaveData.Version) {
					SaveData.Instance = s;
					return;
				}

				string json = sr.ReadToEnd();
				SaveData sd = JsonUtility.FromJson<SaveData>(json);

				//エラーが起きたらロードしない
				if (sd == null) {
					SaveData.Instance = s;
					return;
				}

				
				//ステージ数が違っていたらロードしない
				if(!IsSameStageNum(s, sd)) {
					SaveData.Instance = s;
					return;
				}

				//ロードする
				SaveData.Instance = sd;
			}
		}
	}

	static bool IsSameStageNum(SaveData lNow, SaveData lSave) {
		if (lNow.mStageData.Count != lSave.mStageData.Count) return false;
		for(int i = 0; i < lNow.mStageData.Count; i++) {
			if (lNow.mStageData[i].mStagesData.Count != lSave.mStageData[i].mStagesData.Count) return false;
		}
		return true;
	}

	static SaveData GetInitSaveData() {

		SaveData s = new SaveData();

		//Listのリサイズ
		s.mStageData = new List<SaveData.AreaData>(Area.GetAreaCount() + 1);

		for (int i = 0; i < Area.GetAreaCount() + 1; i++) {
			s.mStageData.Add(new SaveData.AreaData(Area.GetStageCount(i)));

			for (int j = 0; j < Area.GetStageCount(i); j++) {
				var lStageData = new SaveData.StageData();
				lStageData.mShiftTimesOnClear = SaveData.StageData.cInitTimes;
				s.mStageData[i].mStagesData.Add(lStageData);
			}
		}


		s.mLastPlayStage = new SaveData.LastPlayStage();
		s.mLastPlayStage.mAreaNumber = -1;
		s.mLastPlayStage.mStageNumber = -1;

		s.mEventDoneFlag = new SaveData.EventDoneFlag();
		s.mEventDoneFlag.mArea2Open = false;
		s.mEventDoneFlag.mArea3Open = false;

		return s;
	}


	static string GetSaveFilePath() {
		return Application.persistentDataPath + "/savedata.json";
	}

	public static void Delete() {
		if (File.Exists(GetSaveFilePath())) {
			File.Delete(GetSaveFilePath());
		}
	}


#if UNITY_EDITOR

	//セーブデータを消去する
	[MenuItem("Edit/DeleteSaveData")]
	static void DeleteSaveData() {
		Delete();
	}

#endif
}
