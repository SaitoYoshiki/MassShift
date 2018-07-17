using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SEChangeSound : MonoBehaviour {
    [SerializeField]
    GameObject PointerUpSEPrefab;

    public void PlaySE() {
        SoundManager.SPlay(PointerUpSEPrefab);
    }
}
