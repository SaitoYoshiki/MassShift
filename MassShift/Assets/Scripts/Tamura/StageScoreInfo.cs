using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageScoreInfo : MonoBehaviour {
    public enum AREA {
        AREA1 = 1,
        AREA2,
        AREA3,
        AREA4
    }
    // Area4だったりAreaExだったりする！しない。
    
    [SerializeField]
    Text stageName;

    [SerializeField]
    Text areaName;

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

    // 今入れるステージの情報表示オブジェクト
    [SerializeField]
    GameObject stageInfo;

    // Exステージ用の各エリアの進捗表示オブジェクト
    [SerializeField]
    GameObject clearInfo;

    // 今この看板が置かれているエリア
    public AREA placedArea;

    StageSelectManager ssm;

    ChangeActiveInfoPanel caip;

    changeClearInfo cci;

    int oldStageNum = 0;

    int stageShiftTime;

	// Use this for initialization
	void Start () {
        ssm = FindObjectOfType<StageSelectManager>();
        caip = FindObjectOfType<ChangeActiveInfoPanel>();
        cci = FindObjectOfType<changeClearInfo>();
	}

    void Update() {
        // ドア前ならステージ番号、それ以外なら0になる
        int selectStageNum = (ssm.SelectStageNum % 5) + 1;

        // 前のステージ番号から変更があった場合
        if (selectStageNum != oldStageNum) {
            // キャラがドア前にいて、かつエリア移動の扉でないなら
            if (ssm.SelectStageNum != -1 && ssm.SelectStageNum < 20) {
                // かつパネルの置いてあるエリアが今キャラのいるエリアと同じなら
                if (caip.AreaIndex == (int)placedArea) {
                    // Exエリア内かつまだそのステージに入れない場合
                    if (placedArea == AREA.AREA4 && !canGoArea4Stage(selectStageNum)) {
                        // 各エリアのクリア状況を表示
                        clearInfo.SetActive(true);

                        if (selectStageNum == 4) {
                            // 最終ステージの前に立った場合
                            areaName.text = "EX";
                            cci.ChangeStageInfo(selectStageNum);
                        }
                        else {
                            // Ex-1～3の前に立った場合
                            areaName.text = selectStageNum.ToString();
                            cci.ChangeStageInfo(selectStageNum);
                        }
                        // 少なくともクリアしてないステージはない
                        // 全ステージ星2以上ならOK
                    }
                    // それ以外(Area1～3、AreaExでそのステージに入れる場合)
                    else {
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
                        // Exステージの場合
                        if (placedArea == AREA.AREA4) {
                            if (selectStageNum < 4) {
                                stageName.text = "EX - " + selectStageNum.ToString();
                            }
                            else {
                                stageName.text = "Final";
                            }

                            score3text.text = "～" + (ScoreManager.Instance.Score3Times((int)placedArea, selectStageNum)).ToString();
                            score2text.text = (ScoreManager.Instance.Score3Times((int)placedArea, selectStageNum) + 1).ToString() + "～" + (ScoreManager.Instance.Score2Times((int)placedArea, selectStageNum)).ToString();
                            score1text.text = (ScoreManager.Instance.Score2Times((int)placedArea, selectStageNum) + 1).ToString() + "～";
                        }
                        // それ以外
                        else {
                            stageName.text = ((int)placedArea).ToString() + " - " + selectStageNum.ToString();
                            score3text.text = "～" + (ScoreManager.Instance.Score3Times((int)placedArea, selectStageNum)).ToString();
                            score2text.text = (ScoreManager.Instance.Score3Times((int)placedArea, selectStageNum) + 1).ToString() + "～" + (ScoreManager.Instance.Score2Times((int)placedArea, selectStageNum)).ToString();
                            score1text.text = (ScoreManager.Instance.Score2Times((int)placedArea, selectStageNum) + 1).ToString() + "～";
                        }

                        if (ScoreManager.Instance.ShiftTimes((int)placedArea, selectStageNum) != -1) {
                            bestScoretext.text = "Best Score : " + ScoreManager.Instance.ShiftTimes((int)placedArea, selectStageNum).ToString();
                        }
                        else {
                            bestScoretext.text = "Best Score : -";
                        }
                    }//Exエリア入れる判定
                }//エリア番号判定
            }//ssm.stageSelectNumがカラかどうか
            else {
                // パネル上の表示物を消す
                stageInfo.SetActive(false);

                if (clearInfo != null) {
                    clearInfo.SetActive(false);
                }
            }
        }
        // 今いるステージ番号に変わりがなく
        else {
            // oldStageNumが初期状態 or キャラがドア前にいない状態なら
            if (oldStageNum == 0) {
                // パネル上の表示物を消す
                stageInfo.SetActive(false);
                
                if (clearInfo != null) {
                    clearInfo.SetActive(false);
                }
            }
        }
        oldStageNum = selectStageNum;
    }

    bool canGoArea4Stage(int _stageNum) {
        switch (_stageNum) {
            case 1:
                Debug.Log("Area1 : " + Area.CanGoStage4_1());
                return Area.CanGoStage4_1();
            case 2:
                Debug.Log("Area2 : " + Area.CanGoStage4_2());
                return Area.CanGoStage4_2();
            case 3:
                Debug.Log("Area3 : " + Area.CanGoStage4_3());
                return Area.CanGoStage4_3();
            case 4:
                Debug.Log("AreaEx : " + Area.CanGoFinalStage());
                return Area.CanGoFinalStage();

            default:
                Debug.Log("default");
                return false;
        }
    }
}
