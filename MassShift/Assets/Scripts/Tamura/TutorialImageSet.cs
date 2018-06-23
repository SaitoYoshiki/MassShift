using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// button.csからisPlayerPushでプレイヤーがボタンを押しているかどうか判定する

public class TutorialImageSet : MonoBehaviour {
    public UnityEngine.UI.Image tutorialLight;
    
    // 最初に再生する動画
    public UnityEngine.UI.Image tutorialVideo1;
    UnityEngine.Video.VideoPlayer vp1;

    // ボタンが押された時に再生する動画
    public UnityEngine.UI.Image tutorialVideo2;
    UnityEngine.Video.VideoPlayer vp2;

    public BoxCollider boxCol;

    [SerializeField]
    Button button;

    // 電源ON/OFFのアニメーション時間
    [SerializeField]
    float animTime;
    // アニメーション開始時のDeltaTime
    float animStartTime;
    // 電源ON/OFFのアニメーション中かどうか
    bool monitorAnimFlg = false;
    public bool isMonitorON = false;

    void Start() {
        vp1 = tutorialVideo1.GetComponent<UnityEngine.Video.VideoPlayer>();
        if (tutorialVideo2 != null) {
            vp2 = tutorialVideo2.GetComponent<UnityEngine.Video.VideoPlayer>();
        }
    }

    public void StartAnimation() {
        monitorAnimFlg = true;
        animStartTime = Time.fixedUnscaledTime;
    }

    public void MonitorAnimation() {
        float nowAnimTime = Time.fixedUnscaledTime - animStartTime;
        float animPer = Mathf.Clamp((nowAnimTime / animTime), 0.0f, 1.0f);

        UnityEngine.UI.Image video;
        UnityEngine.Video.VideoPlayer vp;

        if (button != null && button.IsButtonOn && !button.IsPlayerPush && tutorialVideo2 != null) {
            vp1.time = 0.0f;
            vp1.Pause();

            video = tutorialVideo2;
            vp = vp2;
        }
        else {
            if (tutorialVideo2 != null) {
                vp2.time = 0.0f;
                vp2.Pause();
            }

            video = tutorialVideo1;
            vp = vp1;
        }

        if (isMonitorON) {
            // 横広がり
            if (video.transform.localScale.x < 1.0f) {
                video.transform.localScale = Vector3.Lerp(new Vector3(0.0f, 0.0f, 1.0f), new Vector3(1.0f, 0.1f, 1.0f), animPer);
                if (animPer >= 1.0f) {
                    animStartTime = Time.fixedUnscaledTime;
                    vp.Play();
                }
            }
            // 縦広がり
            else {
                video.transform.localScale = Vector3.Lerp(new Vector3(1.0f, 0.1f, 1.0f), Vector3.one, animPer);
                if (animPer >= 1.0f) {
                    monitorAnimFlg = false;
                }
            }
        }
        else {
            // 縦縮み
            if (video.transform.localScale.y > 0.1f) {
                video.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(1.0f, 0.1f, 1.0f), animPer);
                if (animPer >= 1.0f) {
                    animStartTime = Time.fixedUnscaledTime;
                }
            }
            // 横縮み
            else {
                video.transform.localScale = Vector3.Lerp(new Vector3(1.0f, 0.1f, 1.0f), new Vector3(0.0f, 0.0f, 1.0f), animPer);
                if (animPer >= 1.0f) {
                    monitorAnimFlg = false;
                    vp.time = 0.0f;
                    vp.Pause();
                }
            }
        }
    }
}
