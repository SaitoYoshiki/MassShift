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
            StageName.text = "STAGE " + Area.GetAreaNumber() + " - " + Area.GetStageNumber();
        }
        // チュートリアルなら
        else {
            StageName.text = "TUTORIAL-" + Area.GetStageNumber();
        }
        CurrentShiftCount.text = "CURRENT SHIFT : 0";


        TargetShiftCount.text = "TARGET SHIFT : " + (ScoreManager.Instance.Score3Times() - 2) + " - " + ScoreManager.Instance.Score3Times();
    }

	// Update is called once per frame
	void Update () {
        CurrentShiftCount.text = "CURRENT SHIFT : " + ScoreManager.Instance.ShiftTimes();

        switch (ScoreManager.Instance.Score()) {
            case 1:
                TargetShiftCount.text = "TARGET SHIFT : " + (ScoreManager.Instance.Score2Times() + 1) + " - ";
                break;
            case 2:
                TargetShiftCount.text = "TARGET SHIFT : " + (ScoreManager.Instance.Score2Times() - 2) + " - " + ScoreManager.Instance.Score2Times();
                break;
            case 3:
                TargetShiftCount.text = "TARGET SHIFT : " + (ScoreManager.Instance.Score3Times() - 2) + " - " + ScoreManager.Instance.Score3Times();
                break;

            default:
                break;
        }
	}
}
