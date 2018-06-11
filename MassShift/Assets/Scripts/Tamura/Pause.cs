﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pause : MonoBehaviour {
    [SerializeField]
    GameObject pauseCanvas;
    [SerializeField]
    GameObject optionCanvas;
    [SerializeField]
    GameObject quitCanvas;

    Blur blur;

    [SerializeField]
    GameObject PauseStartSEPrefab;
    [SerializeField]
    GameObject PauseEndSEPrefab;
    [SerializeField]
    GameObject ButtonSelectSEPrefab;

    // GameMain側から変更される、ポーズ可能かどうか
    public bool canPause = true;

    public bool pauseFlg = false;   // ポーズ中かどうか
    bool optionFlg = false;         // オプション画面を開いているかどうか

    float intencity = 0.0f;

    // ぼかし処理終了までの時間
    public float blurTime;

    // ポーズ画面開き/閉じのアニメーション時間
    public float animTime;
    // アニメーション開始時のDeltaTime
    float animStartTime;
    // ポーズのアニメーション中かどうか
    [SerializeField]
    bool pauseAnimFlg = false;

    GameObject pauseUI;

    // ポーズイベント
    public UnityEvent pauseEvent = new UnityEvent();

    void Start() {
        blur = Camera.main.GetComponent<Blur>();
        pauseUI = pauseCanvas.transform.Find("PauseUI").gameObject;
        pauseUI.transform.localScale = Vector3.zero;
    }

    void Update() {
        var deltaTime = Time.unscaledDeltaTime;

        // Escキーでポーズ / ポーズ解除
        if (Input.GetKeyDown(KeyCode.Escape) && canPause && !pauseAnimFlg) {
            if (!optionFlg) {
                PauseFunc();
            }
            else {
                // オプション画面が出ているときにEscキーを押すとポーズメニューに戻る
                OnOptionButtonDown();
            }
        }

        if (pauseFlg) {
            intencity += deltaTime / blurTime;
        }
        else {
            intencity -= deltaTime / blurTime;
        }

        // 0～1の範囲の値を返す
        intencity = Mathf.Clamp01(intencity);

        // intensityをintに変換
        blur.Resolution = (int)(intencity * 10);

        if (pauseAnimFlg) {
            PauseAnim();
        }
    }

    public void PauseFunc() {
        pauseFlg = !pauseFlg;

        if (pauseFlg) {
            Time.timeScale = 0.0f;
            pauseCanvas.SetActive(true);
            SoundManager.SPlay(PauseStartSEPrefab);
        }
        // ポーズ解除
        else {
            Time.timeScale = 1.0f;
            pauseCanvas.SetActive(false);
            //SoundManager.SPlay(PauseEndSEPrefab);
        }

        StartPauseAnim();

        // 登録された関数を実行
        pauseEvent.Invoke();
    }

    void StartPauseAnim() {
        animStartTime = Time.unscaledTime;
        pauseAnimFlg = true;
    }

    void PauseAnim() {
        float nowAnimTime = Time.unscaledTime - animStartTime;
        float animPer = Mathf.Clamp((nowAnimTime / animTime), 0.0f, 1.0f);

        if (pauseFlg) {
            if (pauseUI.transform.localScale.x < 1.0f) {
                pauseUI.transform.localScale = Vector3.Lerp(new Vector3(0.0f, 0.1f, 1.0f), new Vector3(1.0f, 0.1f, 1.0f), animPer);
                if (animPer >= 1.0f) {
                    animStartTime = Time.unscaledTime;
                }
            }
            else {
                pauseUI.transform.localScale = Vector3.Lerp(new Vector3(1.0f, 0.1f, 1.0f), Vector3.one, animPer);
                if (animPer >= 1.0f) {
                    pauseAnimFlg = false;
                }
            }
        }
        else {
            pauseUI.transform.localScale = Vector3.Lerp(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(0.1f, 0.1f, 1.0f), animPer);
            if (animPer >= 1.0f) {
                pauseAnimFlg = false;
                //Time.timeScale = 1.0f;
                //pauseCanvas.SetActive(false);
            }
        }
    }

    public void OnOptionButtonDown() {
        optionFlg = !optionFlg;

        if (optionFlg) {
            // ポーズ画面を閉じてオプション画面を開く
            pauseCanvas.SetActive(false);
            optionCanvas.SetActive(true);
        }
        else {
            // オプション画面を閉じる
            optionCanvas.SetActive(false);
            pauseCanvas.SetActive(true);

            GetComponent<SetPlayerPrefs>().SaveOptionSetting();
        }
    }

    public void OnGameExitButtonDown() {
        // 本当に終了してもええかウィンドウを出す
        quitCanvas.SetActive(true);

        // exeの終了
        Application.Quit();
    }
}
