using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scripts/StageClearShiftTimes")]
public class StageClearShiftTimes : ScriptableObject {
	
	[System.Serializable]
	public class Data {
		[SerializeField]
		int mScore2Times = -1;	//この手数以下なら、星２つ
		[SerializeField]
		int mScore3Times = -1;	//この手数以下なら、星３つ

		public int Score2Times {
			get {
				return mScore2Times;
			}
		}
		public int Score3Times {
			get {
				return mScore3Times;
			}
		}
	}

	[SerializeField]
	public List<Data> mStages;
}
