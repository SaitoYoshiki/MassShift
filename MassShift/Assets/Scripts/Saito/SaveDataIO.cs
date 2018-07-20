using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

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

		string json = JsonUtility.ToJson(SaveData.Instance);

		string iv;
		string base64;
		MyCryptography.Encrypt(json, out iv, out base64);

		byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
		byte[] base64Bytes = Encoding.UTF8.GetBytes(base64);

		using (FileStream fs = new FileStream(GetSaveFilePath(), FileMode.Create, FileAccess.Write)) {

			using (BinaryWriter bw = new BinaryWriter(fs)) {
				bw.Write(ivBytes.Length);
				bw.Write(ivBytes);
				bw.Write(base64Bytes.Length);
				bw.Write(base64Bytes);
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


		byte[] ivBytes = null;
		byte[] base64Bytes = null;
		using (FileStream fs = new FileStream(GetSaveFilePath(), FileMode.Open, FileAccess.Read)) {
			using (BinaryReader br = new BinaryReader(fs)) {
				int length = br.ReadInt32();
				ivBytes = br.ReadBytes(length);

				length = br.ReadInt32();
				base64Bytes = br.ReadBytes(length);
			}

			//復号化
			string json;
			string iv = Encoding.UTF8.GetString(ivBytes);
			string base64 = Encoding.UTF8.GetString(base64Bytes);

			try {
				MyCryptography.Decrypt(iv, base64, out json);
			}
			catch {
				//エラー発生時はロードしない
				SaveData.Instance = s;
				return;
			}


			SaveData sd = JsonUtility.FromJson<SaveData>(json);

			//エラーが起きたらロードしない
			if (sd == null) {
				SaveData.Instance = s;
				return;
			}


			//ステージ数が違っていたらロードしない
			if (!IsSameStageNum(s, sd)) {
				SaveData.Instance = s;
				return;
			}

			//ロードする
			SaveData.Instance = sd;
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
        s.mEventDoneFlag.mAlreadyVisitStageSelect = false;
		s.mEventDoneFlag.mArea2Open = false;
		s.mEventDoneFlag.mArea3Open = false;
        s.mEventDoneFlag.mArea4Open = false;

		return s;
	}


	static string GetSaveFilePath() {
		return Application.persistentDataPath + "/savedata";
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
