using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangePauseText : MonoBehaviour {
    [SerializeField]
    Text StageName;

    [SerializeField]
    Text CurrentShiftCount;

    [SerializeField]
    Text TargetShiftCount;

    void Start() {
        // チュートリアル以外なら
        if (Area.GetAreaNumber() != 0) {
            if (Area.GetAreaNumber() == 4) {
                if (Area.GetStageNumber() == 4) {
                    StageName.text = "Stage FINAL";
                }
                else {
                    StageName.text = "STAGE EX - " + Area.GetStageNumber();
                }
            }
            else {
                StageName.text = "STAGE " + Area.GetAreaNumber() + " - " + Area.GetStageNumber();
            }
        }
        // チュートリアルなら
        else {
            StageName.text = "TUTORIAL - " + Area.GetStageNumber();
        }
        CurrentShiftCount.text = "CURRENT SCORE : 0";

        if (Area.GetAreaNumber() != 0) {
            TargetShiftCount.text = "TARGET SCORE : " + (ScoreManager.Instance.Score3Times() - 2) + " - " + ScoreManager.Instance.Score3Times();
        }
    }

	// Update is called once per frame
	void Update () {
        CurrentShiftCount.text = "CURRENT SCORE : " + ScoreManager.Instance.ShiftTimes();

        // チュートリアルなら
        if (Area.GetAreaNumber() != 0) {
            // 現在の評価に応じて目標手数範囲のテキスト変更
            switch (ScoreManager.Instance.Score()) {
                case 1:
                    TargetShiftCount.text = "TARGET SCORE : " + (ScoreManager.Instance.Score2Times() + 1) + " ～ ";
                    break;
                case 2:
                    TargetShiftCount.text = "TARGET SCORE : " + (ScoreManager.Instance.Score3Times() + 1) + " ～ " + ScoreManager.Instance.Score2Times();
                    break;
                case 3:
                    TargetShiftCount.text = "TARGET SCORE :  ～ " + ScoreManager.Instance.Score3Times();
                    break;

                default:
                    break;
            }
        }
	}
}
