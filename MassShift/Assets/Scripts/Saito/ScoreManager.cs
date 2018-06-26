using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {

	MassShift mMassShift;
	
	//クリアに必要な手数
	[SerializeField]
	List<StageClearShiftTimes> mClearShiftTimes;

	private void Start() {
		mMassShift = FindObjectOfType<MassShift>();
	}


	//現在の星の数
	//
	public int Score() {
		int lShiftTimes = ShiftTimes;
		if(lShiftTimes <= Score3Times()) {
			return 3;
		}
		if (lShiftTimes <= Score2Times()) {
			return 2;
		}
		if (lShiftTimes <= Score1Times()) {
			return 1;
		}
		return 0;
	}

	//現在の手数
	//
	public int ShiftTimes {
		get {
			return mMassShift.ShiftTimes;
		}
	}

	//無効な値
	public const int cInvalidScoreTimes = -1;


	//この手数以下なら星１つ
	//
	public int Score1Times() {
		return cInvalidScoreTimes;
	}

	//この手数以下なら星２つ
	//
	public int Score2Times() {
		var s = GetClearShiftTimesData();
		if (s == null) {
			return cInvalidScoreTimes;
		}
		return s.Score2Times;
	}

	//この手数以下なら星３つ
	//
	public int Score3Times() {
		var s = GetClearShiftTimesData();
		if (s == null) {
			return cInvalidScoreTimes;
		}
		return s.Score3Times;
	}


	//この手数以下なら星１つ
	//
	public int Score1Times(int aAreaNumber, int aStageNumber) {
		return cInvalidScoreTimes;
	}

	//この手数以下なら星２つ
	//
	public int Score2Times(int aAreaNumber, int aStageNumber) {
		var s = GetClearShiftTimesData(aAreaNumber, aStageNumber);
		if (s == null) {
			return cInvalidScoreTimes;
		}
		return s.Score2Times;
	}

	//この手数以下なら星３つ
	//
	public int Score3Times(int aAreaNumber, int aStageNumber) {
		var s = GetClearShiftTimesData(aAreaNumber, aStageNumber);
		if (s == null) {
			return cInvalidScoreTimes;
		}
		return s.Score3Times;
	}



	StageClearShiftTimes.Data GetClearShiftTimesData() {
		if (Area.IsInStage() == false) {
			return null;
		}

		int lAreaNumber = Area.GetAreaNumber();
		int lStageNumber = Area.GetStageNumber();

		//チュートリアルなら
		if(lAreaNumber == 0) {
			return null;
		}

		return mClearShiftTimes[lAreaNumber - 1].mStages[lStageNumber - 1];
	}

	StageClearShiftTimes.Data GetClearShiftTimesData(int aAreaNumber, int aStageNumber) {
		
		//チュートリアルなら
		if (aAreaNumber == 0) {
			return null;
		}

		return mClearShiftTimes[aAreaNumber - 1].mStages[aStageNumber - 1];
	}
}
