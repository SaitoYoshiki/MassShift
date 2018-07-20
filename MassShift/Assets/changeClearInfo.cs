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
                StageNames[1].text = "EX - 1";
                StageNames[2].text = "EX - 2";
                StageNames[3].text = "EX - 3";
                StageNames[4].text = "";

                if (ScoreManager.Instance.ShiftTimes(_selectStageNum, 1) != -1) {
                    StageNames[1].color = Color.green;
                }
                else {
                    StageNames[1].color = Color.red;
                }

                if (ScoreManager.Instance.ShiftTimes(_selectStageNum, 2) != -1) {
                    StageNames[2].color = Color.green;
                }
                else {
                    StageNames[2].color = Color.red;
                }

                if (ScoreManager.Instance.ShiftTimes(_selectStageNum, 3) != -1) {
                    StageNames[3].color = Color.green;
                }
                else {
                    StageNames[3].color = Color.red;
                }

                StarsNormal[0].SetActive(false);
                StarsNormal[1].GetComponent<ChangeStar>().ChangeStarColorEX(ScoreManager.Instance.Score(4, 1, ScoreManager.Instance.ShiftTimes(4, 1)));
                StarsNormal[2].GetComponent<ChangeStar>().ChangeStarColorEX(ScoreManager.Instance.Score(4, 2, ScoreManager.Instance.ShiftTimes(4, 2)));
                StarsNormal[3].GetComponent<ChangeStar>().ChangeStarColorEX(ScoreManager.Instance.Score(4, 3, ScoreManager.Instance.ShiftTimes(4, 3)));
                StarsNormal[4].SetActive(false);

                break;

            default:
                StarsNormal[0].SetActive(true);
                StarsNormal[4].SetActive(true);

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

                    StarsNormal[count - 1].GetComponent<ChangeStar>().ChangeStarColor(score);

                    count++;
                }

                break;
        }
    } 
}
