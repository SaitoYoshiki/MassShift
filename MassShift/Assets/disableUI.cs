using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disableUI : MonoBehaviour {
    [SerializeField]
    List<GameObject> UIList;

    public bool flg = false;

    void Update() {
        if (flg) {
            DisableUI();
        }
    }

    public void DisableUI() {
        foreach (var ui in UIList) {
            ui.SetActive(false);
        }
    }
}
