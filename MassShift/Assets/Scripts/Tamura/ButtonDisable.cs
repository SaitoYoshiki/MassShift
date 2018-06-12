using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonDisable : MonoBehaviour {
    [SerializeField]
    GameObject stageSelect;

    [SerializeField]
    GameObject retry;

    [SerializeField]
    GameObject nextStage;

    public void DisablePauseMenu() {
        stageSelect.SetActive(false);
        retry.SetActive(false);
    }

    public void DisableResultMenu() {
        nextStage.SetActive(false);
    }
}
