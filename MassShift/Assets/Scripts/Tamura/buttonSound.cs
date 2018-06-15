using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonSound : MonoBehaviour {
    [SerializeField]
    GameObject buttonPushSound;

    public void OnPush() {
        SoundManager.SPlay(buttonPushSound);
    }
}
