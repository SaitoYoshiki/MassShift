using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class Area {

	readonly int mTutorialNum = 3;	//チュートリアルのステージ数

	readonly List<int> mStageNum = new List<int>() {
		5,	//エリア１
		5,	//エリア２
		5	//エリア３
	};

	const string cTutorialSceneName = "Tutorial-{0}";
	const string cStageSceneName = "Stage{0}-{1}";


	//シングルトン
	//
	static Area sInstance = null;
	
	static Area Instance {
		get {
			if (sInstance == null) {
				sInstance = new Area();
			}
			return sInstance;
		}
	}


	//現在のシーン名を返す
	//
	static string GetSceneName() {
		return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
	}


	//ステージのシーン名を返す
	//
	public static string GetStageSceneName(int aAreaNumber, int aStageNumber) {

		//チュートリアルなら
		if (aAreaNumber == 0) {
			return string.Format(cTutorialSceneName, aStageNumber);
		}

		int lAreaIndex = GetAreaIndex(aAreaNumber);

		if (0 <= lAreaIndex && lAreaIndex < GetAreaCount()) {
			return string.Format(cStageSceneName, aAreaNumber, aStageNumber);
		}
		return "";
	}


	//現在のエリア番号を返す（ステージ中なら、０・１・２・３．それ以外は-1）
	//
	public static int GetAreaNumber() {
		string lSceneName = GetSceneName();

		//チュートリアルかどうかの判定
		Regex r = new Regex(string.Format(cTutorialSceneName, @"(\d+?)"));

		if (r.IsMatch(lSceneName)) {
			return 0;
		}


		//ステージかどうかの判定
		r = new Regex(string.Format(cStageSceneName, @"(\d+?)", @"(\d+?)"));

		if(r.IsMatch(lSceneName)) {
			var m = r.Match(lSceneName);
			int lRes = 0;
			int.TryParse(m.Groups[1].Value, out lRes);
			return lRes;
		}

		return -1;
	}


	//現在のステージ番号を返す（ステージ中なら、０・１・２・３…。それ以外は-1）
	//
	public static int GetStageNumber() {
		string lSceneName = GetSceneName();

		//チュートリアルかどうかの判定
		Regex r = new Regex(string.Format(cTutorialSceneName, @"(\d+?)"));

		if (r.IsMatch(lSceneName)) {
			var m = r.Match(lSceneName);
			int lRes = 0;
			int.TryParse(m.Groups[1].Value, out lRes);
			return lRes;
		}

		//エリア１・２・３かどうかの判定
		r = new Regex(string.Format(cStageSceneName, @"(\d+?)", @"(\d+?)"));

		if (r.IsMatch(lSceneName)) {
			var m = r.Match(lSceneName);
			int lRes = 0;
			int.TryParse(m.Groups[2].Value, out lRes);
			return lRes;
		}

		return -1;
	}


	//チュートリアルを除いたエリア数を返す
	//
	public static int GetAreaCount() {
		return Instance.mStageNum.Count;
	}

	//ステージ数を返す（エリア番号は１・２・３）
	//
	public static int GetStageCount(int aAreaNumber) {

		//チュートリアルなら
		if(aAreaNumber == 0) {
			return Instance.mTutorialNum;
		}

		int lAreaIndex = GetAreaIndex(aAreaNumber);

		if (GetAreaCount() <= lAreaIndex) return -1;
		if (lAreaIndex < 0) return -1;
		return Instance.mStageNum[lAreaIndex];
	}


	//次のステージのシーン名を返す
	//
	public static string GetNextStageSceneName(int aAreaNumber, int aStageNumber) {

		//次のシーンが存在しないなら
		if (!ExistNextStage(aAreaNumber, aStageNumber)) return "";

		//次のシーンが同じエリアに存在しないなら
		if(!ExistNextStageSameArea(aAreaNumber, aStageNumber)) {
			//次のエリアの最初のステージへ
			aAreaNumber += 1;
			aStageNumber = 1;
		}
		//次のシーンが同じエリアに存在するなら
		else {
			//次のステージへ
			aStageNumber += 1;
		}
		return GetStageSceneName(aAreaNumber, aStageNumber);
	}


	//ゲーム中に、次のステージが存在しない
	//
	public static bool ExistNextStage(int aAreaNumber, int aStageNumber) {

		//チュートリアルなら
		if(aAreaNumber == 0) {
			return true;	//必ず存在する
		}

		int lAreaIndex = GetAreaIndex(aAreaNumber);
		int lStageIndex = GetStageIndex(aStageNumber);

		//最終ステージだけ、次のステージが存在しない
		if (GetAreaCount() - 1 == lAreaIndex) {
			if (GetStageCount(aAreaNumber) - 1 == lStageIndex) {
				return false;
			}
		}

		return true;
	}


	//同じエリアに、次のステージが存在しない
	//
	public static bool ExistNextStageSameArea(int aAreaNumber, int aStageNumber) {
		
		//チュートリアルの場合
		if(aAreaNumber == 0) {
			//範囲外チェック
			if (aStageNumber <= 0) return false;
			if (Instance.mTutorialNum < aStageNumber) return false;
			//チュートリアルの最終ステージには次のステージがない
			if (Instance.mTutorialNum == aStageNumber) return false;
			return true;
		}

		int lAreaIndex = GetAreaIndex(aAreaNumber);
		int lStageIndex = GetStageIndex(aStageNumber);

		//範囲外チェック
		if (GetAreaCount() <= lAreaIndex) return false;
		if (lAreaIndex < 0) return false;

		//範囲外チェック
		if (GetStageCount(aAreaNumber) <= lStageIndex) return false;
		if (lStageIndex < 0) return false;

		//エリアの最後のステージには、次のステージが存在しない
		if (lStageIndex == GetStageCount(aAreaNumber) - 1) return false;

		return true;
	}


	//そのシーンがステージ中か
	//
	public static bool IsInStage() {

		int lAreaNum = GetAreaNumber();
		if (0 <= lAreaNum && lAreaNum <= GetAreaCount()) {
			return true;
		}

		return false;
	}


	//１・２・３のエリア番号を、配列のインデックスに対応させる
	//
	static int GetAreaIndex(int aAreaNumber) {
		return aAreaNumber - 1;
	}

	//１・２・３のステージ番号を、配列のインデックスに対応させる
	//
	static int GetStageIndex(int aStageNumber) {
		return aStageNumber - 1;
	}


	//そのエリアに行くことが可能か
	//
	public static bool CanGoArea(int aAreaNumber) {

		//チュートリアルとエリア1には、絶対に行くことができる
		if(aAreaNumber == 0 || aAreaNumber == 1) {
			return true;
		}

		//エリア2は、エリア1のステージを全てクリアしていると行ける
		if(aAreaNumber == 2) {
			foreach(var s in SaveData.Instance.GetAreaData(1).mStagesData) {
				if(s.mShiftTimesOnClear == SaveData.StageData.cInitTimes) {
					return false;
				}
			}
			return true;
		}

		//エリア3は、エリア1とエリア2のステージを全てクリアしていると行ける
		if (aAreaNumber == 3) {
			foreach (var s in SaveData.Instance.GetAreaData(1).mStagesData) {
				if (s.mShiftTimesOnClear == SaveData.StageData.cInitTimes) {
					return false;
				}
			}
			foreach (var s in SaveData.Instance.GetAreaData(2).mStagesData) {
				if (s.mShiftTimesOnClear == SaveData.StageData.cInitTimes) {
					return false;
				}
			}
			return true;
		}

		return false;
	}


	//そのエリア番号が有効か
	public static bool IsValidArea(int aAreaNumber) {
		if (aAreaNumber < 0) return false;
		if (aAreaNumber > GetAreaCount()) return false;
		return true;
	}
	//そのステージ番号が有効か
	public static bool IsValidStage(int aAreaNumber, int aStageNumber) {
		if (!IsValidArea(aAreaNumber)) return false;

		if (aStageNumber <= 0) return false;
		if (aStageNumber > GetStageCount(aAreaNumber)) return false;
		return true;
	}


	public static int sBeforeAreaNumber = 1;
	public static int sBeforeStageNumber = 1;
}
