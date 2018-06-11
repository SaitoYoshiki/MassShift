using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialImageSet : MonoBehaviour {
    public UnityEngine.UI.Image tutorialLight;
    public UnityEngine.UI.Image tutorialVideo;
    public BoxCollider boxCol;

    // 電源ON/OFFのアニメーション時間
    [SerializeField]
    float animTime;
    // アニメーション開始時のDeltaTime
    float animStartTime;
    // 電源ON/OFFのアニメーション中かどうか
    bool monitorAnimFlg = false;
    public bool isMonitorON = false;

    public void StartAnimation() {
        monitorAnimFlg = true;
        animStartTime = Time.fixedUnscaledTime;
    }

    public void MonitorAnimation() {
        float nowAnimTime = Time.fixedUnscaledTime - animStartTime;
        float animPer = Mathf.Clamp((nowAnimTime / animTime), 0.0f, 1.0f);

        if (isMonitorON) {
            // 横広がり
            if (tutorialVideo.transform.localScale.x < 1.0f) {
                tutorialVideo.transform.localScale = Vector3.Lerp(new Vector3(0.0f, 0.0f, 1.0f), new Vector3(1.0f, 0.1f, 1.0f), animPer);
                if (animPer >= 1.0f) {
                    animStartTime = Time.fixedUnscaledTime;
                }
            }
            // 縦広がり
            else {
                tutorialVideo.transform.localScale = Vector3.Lerp(new Vector3(1.0f, 0.1f, 1.0f), Vector3.one, animPer);
                if (animPer >= 1.0f) {
                    monitorAnimFlg = false;
                }
            }
        }
        else {
            // 縮み
            tutorialVideo.transform.localScale = Vector3.Lerp(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(0.0f, 0.0f, 1.0f), animPer);
            if (animPer >= 1.0f) {
                monitorAnimFlg = false;
            }
        }
    }
}
