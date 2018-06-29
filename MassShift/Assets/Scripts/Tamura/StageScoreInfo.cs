using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageScoreInfo : MonoBehaviour {
    public enum AREA {
        AREA1 = 1,
        AREA2,
        AREA3
    }
    
    [SerializeField]
    Text stageName;

    [SerializeField]
    Material score3mat;
    [SerializeField]
    Material score2mat;
    [SerializeField]
    Material score1mat;

    // 星３必要手数
    [SerializeField]
    Text score3text;

    // 星２必要手数
    [SerializeField]
    Text score2text;

    // 星１必要手数
    [SerializeField]
    Text score1text;

    [SerializeField]
    GameObject stageInfo;

    // 今この看板が置かれているエリア
    public AREA placedArea;

    StageSelectManager ssm;

    int oldStageNum = 0;

    int stageShiftTime;

	// Use this for initialization
	void Start () {
        ssm = FindObjectOfType<StageSelectManager>();
	}

    void Update() {
        int selectStageNum = (ssm.SelectStageNum % 5) + 1;

        if (selectStageNum != oldStageNum) {
            if (ssm.SelectStageNum != -1) {
                stageInfo.SetActive(true);

                stageShiftTime = ScoreManager.Instance.ShiftTimes((int)placedArea, selectStageNum);

                // そもそもステージをクリアしていない場合
                if (stageShiftTime == -1) {
                    Debug.Log("くりあしてない");
                    score3mat.color = Color.grey;
                    score2mat.color = Color.grey;
                    score1mat.color = Color.grey;
                }
                else {
                    Debug.Log("くりあしてる");
                    // ステージの評価が星1
                    score1mat.color = Color.white;

                    // ステージの評価が星2
                    if (stageShiftTime <= ScoreManager.Instance.Score2Times((int)placedArea, selectStageNum)) {
                        score2mat.color = Color.white;
                    }

                    // ステージの評価が星3
                    if (stageShiftTime <= ScoreManager.Instance.Score3Times((int)placedArea, selectStageNum)) {
                        score3mat.color = Color.white;
                    }
                }

                // ステージ名と必要手数を代入
                stageName.text = ((int)placedArea).ToString() + "-" + selectStageNum.ToString();
                score3text.text = "～" + (ScoreManager.Instance.Score3Times((int)placedArea, selectStageNum)).ToString();
                score2text.text = (ScoreManager.Instance.Score3Times((int)placedArea, selectStageNum) + 1).ToString() + "～" + (ScoreManager.Instance.Score2Times((int)placedArea, selectStageNum)).ToString();
                score1text.text = (ScoreManager.Instance.Score2Times((int)placedArea, selectStageNum) + 1).ToString() + "～";
            }
            else {
                // パネル上の表示物を消す
                stageInfo.SetActive(false);
            }
        }
        else {
            if (oldStageNum == 0) {
                // パネル上の表示物を消す
                stageInfo.SetActive(false);
            }
        }

        oldStageNum = selectStageNum;
    }
}
