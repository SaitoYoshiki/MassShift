using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManagerData : MonoBehaviour {

	//クリアに必要な手数のデータ（アセットで設定）
	[SerializeField]
	public List<StageClearShiftTimes> mClearShiftTimes;
}
