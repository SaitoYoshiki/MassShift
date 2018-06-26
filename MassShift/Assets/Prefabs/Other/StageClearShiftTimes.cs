using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scripts/StageClearShiftTimes")]
public class StageClearShiftTimes : ScriptableObject {
	
	[System.Serializable]
	class StageData {
		[SerializeField]
		int mGrade1 = -1;
		[SerializeField]
		int mGrade2 = -1;

		public int Grade1 {
			get {
				return mGrade1;
			}
		}
		public int Grade2 {
			get {
				return mGrade2;
			}
		}
	}

	[SerializeField]
	List<StageData> mStages;
}
