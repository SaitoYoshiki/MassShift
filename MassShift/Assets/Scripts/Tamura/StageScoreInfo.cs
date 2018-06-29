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

    // 各スコアの星画像
    [SerializeField]
    SetColor score3star;
    [SerializeField]
    SetColor score2star;
    [SerializeField]
    SetColor score1star;

    // 各スコアのポーズ用キャラ
    [SerializeField]
    SetActivePlayer score3player;
    [SerializeField]
    SetActivePlayer score2player;
    [SerializeField]
    SetActivePlayer score1player;

    // 星３必要手数
    [SerializeField]
    Text score3text;

    // 星２必要手数
    [SerializeField]
    Text score2text;

    // 星１必要手数
    [SerializeField]
    Text score1text;

    // プレイヤーのこれまでの最短手
    [SerializeField]
    Text bestScoretext;

    [SerializeField]
    GameObject stageInfo;

    // 今この看板が置かれているエリア
    public AREA placedArea;

    StageSelectManager ssm;

    ChangeActiveInfoPanel caip;

    int oldStageNum = 0;

    int stageShiftTime;

	// Use this for initialization
	void Start () {
        ssm = FindObjectOfType<StageSelectManager>();
        caip = FindObjectOfType<ChangeActiveInfoPanel>();
	}

    void Update() {
        int selectStageNum = (ssm.SelectStageNum % 5) + 1;

        if (selectStageNum != oldStageNum) {
            if (ssm.SelectStageNum != -1) {

                if (caip.AreaIndex == (int)placedArea) {

                    stageInfo.SetActive(true);

                    stageShiftTime = ScoreManager.Instance.ShiftTimes((int)placedArea, selectStageNum);

                    // そもそもステージをクリアしていない場合
                    if (stageShiftTime == -1) {
                        Debug.Log("くりあしてない");
                        // マテリアルカラーは無効化した
                        score3star.SetGrayColor();
                        score2star.SetGrayColor();
                        score1star.SetGrayColor();

                        score3player.ActiveStandPlayer();
                        score2player.ActiveStandPlayer();
                        score1player.ActiveStandPlayer();
                    }
                    else {
                        Debug.Log("くりあした");
                        // ステージの評価が星1
                        score1star.SetWhiteColor();
                        score1player.ActiveCeleblatePlayer();

                        // ステージの評価が星2
                        if (stageShiftTime <= ScoreManager.Instance.Score2Times((int)placedArea, selectStageNum)) {
                            score2star.SetWhiteColor();
                            score2player.ActiveCeleblatePlayer();
                        }
                        else {
                            score2star.SetGrayColor();
                            score2player.ActiveStandPlayer();
                        }

                        // ステージの評価が星3
                        if (stageShiftTime <= ScoreManager.Instance.Score3Times((int)placedArea, selectStageNum)) {
                            score3star.SetWhiteColor();
                            score3player.ActiveCeleblatePlayer();
                        }
                        else {
                            score3star.SetGrayColor();
                            score3player.ActiveStandPlayer();
                        }
                    }

                    // ステージ名と必要手数を代入
                    stageName.text = ((int)placedArea).ToString() + " - " + selectStageNum.ToString();
                    score3text.text = "～" + (ScoreManager.Instance.Score3Times((int)placedArea, selectStageNum)).ToString();
                    score2text.text = (ScoreManager.Instance.Score3Times((int)placedArea, selectStageNum) + 1).ToString() + "～" + (ScoreManager.Instance.Score2Times((int)placedArea, selectStageNum)).ToString();
                    score1text.text = (ScoreManager.Instance.Score2Times((int)placedArea, selectStageNum) + 1).ToString() + "～";

                    if (ScoreManager.Instance.ShiftTimes((int)placedArea, selectStageNum) != -1) {
                        bestScoretext.text = "Best Score : " + ScoreManager.Instance.ShiftTimes((int)placedArea, selectStageNum).ToString();
                    }
                    else {
                        bestScoretext.text = "Best Score : N/A";
                    }
                }
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
