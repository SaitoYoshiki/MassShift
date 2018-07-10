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
    List<Sprite> stagePreview;
    
    // 各スコアの星画像
    [SerializeField]
    GameObject score3star;
    [SerializeField]
    GameObject score2star;
    [SerializeField]
    GameObject score1star;
    [SerializeField]
    GameObject score0star;

    [SerializeField]
    Image stagePic;

    // スコアのポーズ用キャラ
    [SerializeField]
    SetActivePlayer scorePlayer;

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
                        score3star.SetActive(false);
                        score2star.SetActive(false);
                        score1star.SetActive(false);

                        score0star.SetActive(true);

                        scorePlayer.ActiveStandPlayer();
                    }
                    else {
                        Debug.Log("くりあした");
                        // 万歳キャラを表示
                        scorePlayer.ActiveCeleblatePlayer();

                        // ステージの評価が星3
                        if (stageShiftTime <= ScoreManager.Instance.Score3Times((int)placedArea, selectStageNum)) {
                            score3star.SetActive(true);

                            score2star.SetActive(false);
                            score1star.SetActive(false);
                        }
                        // 星3ではない
                        else {
                            // ステージの評価が星2
                            if (stageShiftTime <= ScoreManager.Instance.Score2Times((int)placedArea, selectStageNum)) {
                                score2star.SetActive(true);

                                score3star.SetActive(false);
                                score1star.SetActive(false);
                            }
                            else {
                                // ステージの評価が星1
                                score1star.SetActive(true);

                                score3star.SetActive(false);
                                score2star.SetActive(false);
                            }
                        }

                        score0star.SetActive(false);
                    }

                    // ステージ画像を変更
                    stagePic.sprite = stagePreview[ssm.SelectStageNum % 5];

                    // ステージ名と必要手数を代入
                    stageName.text = ((int)placedArea).ToString() + " - " + selectStageNum.ToString();
                    score3text.text = "～" + (ScoreManager.Instance.Score3Times((int)placedArea, selectStageNum)).ToString();
                    score2text.text = (ScoreManager.Instance.Score3Times((int)placedArea, selectStageNum) + 1).ToString() + "～" + (ScoreManager.Instance.Score2Times((int)placedArea, selectStageNum)).ToString();
                    score1text.text = (ScoreManager.Instance.Score2Times((int)placedArea, selectStageNum) + 1).ToString() + "～";

                    if (ScoreManager.Instance.ShiftTimes((int)placedArea, selectStageNum) != -1) {
                        bestScoretext.text = "Best Score : " + ScoreManager.Instance.ShiftTimes((int)placedArea, selectStageNum).ToString();
                    }
                    else {
                        bestScoretext.text = "Best Score : -";
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
