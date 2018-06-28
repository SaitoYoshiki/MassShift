using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeResultText : MonoBehaviour {
    [SerializeField]
    Text TotalShiftCount;

	// Use this for initialization
	void Start () {
        TotalShiftCount.text = "TOTAL SHIFT : " + ScoreManager.Instance.ShiftTimes();
	}
}
