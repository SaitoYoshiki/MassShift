using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class swapMatByScore : MonoBehaviour {
    [SerializeField]
    GameObject score;

    [SerializeField]
    Material scoreMat1;
    [SerializeField]
    Material scoreMat2;
    [SerializeField]
    Material scoreMat3;

    int oldScore = 0;
	
	void Update () {
        if (ScoreManager.Instance.Score() != oldScore) {
            switch (ScoreManager.Instance.Score()) {
                case 1:
                    score.GetComponent<Image>().material = scoreMat1;
                    break;
                case 2:
                    score.GetComponent<Image>().material = scoreMat2;
                    break;
                case 3:
                    score.GetComponent<Image>().material = scoreMat3;
                    break;

                default:
                    break;
            }
        }

        oldScore = ScoreManager.Instance.Score();
	}
}
