using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class changeClearInfo : MonoBehaviour {
    [SerializeField]
    List<Text> StageNames;

    [SerializeField]
    List<GameObject> StarsNormal;

    public void ChangeStageInfo(int _selectStageNum) {
        switch (_selectStageNum) {
            case 4:
                StageNames[0].text = "";
                StageNames[1].text = "";
                StageNames[2].text = "";
                StageNames[3].text = "";
                StageNames[4].text = "";

                StageNames[5].text = "EX - 1";
                StageNames[6].text = "EX - 2";
                StageNames[7].text = "EX - 3";

                if (ScoreManager.Instance.ShiftTimes(_selectStageNum, 1) != -1) {
                    StageNames[5].color = Color.green;
                }
                else {
                    StageNames[5].color = Color.red;
                }

                if (ScoreManager.Instance.ShiftTimes(_selectStageNum, 2) != -1) {
                    StageNames[6].color = Color.green;
                }
                else {
                    StageNames[6].color = Color.red;
                }

                if (ScoreManager.Instance.ShiftTimes(_selectStageNum, 3) != -1) {
                    StageNames[7].color = Color.green;
                }
                else {
                    StageNames[7].color = Color.red;
                }

                StarsNormal[0].SetActive(false);
                StarsNormal[1].SetActive(false);
                StarsNormal[2].SetActive(false);
                StarsNormal[3].SetActive(false);
                StarsNormal[4].SetActive(false);

                StarsNormal[5].SetActive(true);
                StarsNormal[6].SetActive(true);
                StarsNormal[7].SetActive(true);

                StarsNormal[5].GetComponent<ChangeStar>().ChangeStarColorEX(ScoreManager.Instance.Score(4, 1, ScoreManager.Instance.ShiftTimes(4, 1)));
                StarsNormal[6].GetComponent<ChangeStar>().ChangeStarColorEX(ScoreManager.Instance.Score(4, 2, ScoreManager.Instance.ShiftTimes(4, 2)));
                StarsNormal[7].GetComponent<ChangeStar>().ChangeStarColorEX(ScoreManager.Instance.Score(4, 3, ScoreManager.Instance.ShiftTimes(4, 3)));

                break;

            default:
                StarsNormal[0].SetActive(true);
                StarsNormal[1].SetActive(true);
                StarsNormal[2].SetActive(true);
                StarsNormal[3].SetActive(true);
                StarsNormal[4].SetActive(true);

                StarsNormal[5].SetActive(false);
                StarsNormal[6].SetActive(false);
                StarsNormal[7].SetActive(false);

                int count = 1;
                foreach(var text in StageNames){
                    int score = ScoreManager.Instance.Score(_selectStageNum, count, ScoreManager.Instance.ShiftTimes(_selectStageNum, count));
                    text.text = _selectStageNum.ToString() + " - " + count.ToString();
                    if (score <= 1) {
                        text.color = Color.red;
                    }
                    else {
                        text.color = Color.green;
                    }

                    StageNames[5].text = "";
                    StageNames[6].text = "";
                    StageNames[7].text = "";

                    StarsNormal[count - 1].GetComponent<ChangeStar>().ChangeStarColor(score);

                    count++;
                }

                break;
        }
    } 
}
