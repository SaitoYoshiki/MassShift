using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisablePauseUI : MonoBehaviour {
    Result  rs;
    Pause   ps;

    [SerializeField]
    GameObject PauseUI;

	// Use this for initialization
	void Start () {
        rs = GetComponent<Result>();
        ps = GetComponent<Pause>();
	}
	
	// Update is called once per frame
	void Update () {
        if (rs.canGoal || ps.pauseFlg || !ps.canPause) {
            PauseUI.SetActive(false);
        }
        else {
            if (!PauseUI.activeSelf) {
                PauseUI.SetActive(true);
            }
        }
	}
}
