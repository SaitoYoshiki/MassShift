using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActivePlayer : MonoBehaviour {
    [SerializeField]
    GameObject celebratePlayer;
    [SerializeField]
    GameObject standPlayer;

    public void ActiveCeleblatePlayer() {
        celebratePlayer.SetActive(true);
        standPlayer.SetActive(false);
    }

    public void ActiveStandPlayer() {
        standPlayer.SetActive(true);
        celebratePlayer.SetActive(false);
    }
}
