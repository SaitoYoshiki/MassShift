using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBGMManager : MonoBehaviour {
    [SerializeField]
    GameObject TitleBGMPrefab;

    void Start() {
        GameObject tBgm = SoundManager.SPlay(TitleBGMPrefab);
        SoundManager.SFade(tBgm, 0.0f, SoundManager.SVolume(TitleBGMPrefab), 1.0f, false);
    }
}
