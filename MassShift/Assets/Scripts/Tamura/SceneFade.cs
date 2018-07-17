﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneFade : MonoBehaviour {
    [SerializeField]
    Image fadeImage;

    [SerializeField]
    float fadeTime;
    float fadeStartTime;

    [SerializeField]
    Color fadeColor;

    bool isFadeIn = false;
    bool isFadeOut = false;

    // 初期化
    void Start() {
        fadeColor.a = 0.0f;
        fadeImage.color = fadeColor;
    }

    void Update() {
        if (isFadeIn) {
            // 単色フェードのコルーチンを開始
            StartCoroutine(SceneFadeIn());
        }
        else {
            StopCoroutine(SceneFadeIn());
        }

        if (isFadeOut) {
            // 単色フェードのコルーチンを開始
            StartCoroutine(SceneFadeOut());
        }
        else {
            StopCoroutine(SceneFadeOut());
        }
    }

    // フェードイン開始
    public void FadeInStart() {
        // エラー防止
        if (isFadeIn || isFadeOut) {
            return;
        }

        fadeStartTime = Time.realtimeSinceStartup;
        fadeColor.a = 0.0f;
        isFadeIn = true;
    }

    public void FadeInStart(float _fadeTime) {
        // エラー防止
        if (isFadeIn || isFadeOut) {
            return;
        }

        fadeTime = _fadeTime;
        fadeStartTime = Time.realtimeSinceStartup;
        fadeColor.a = 0.0f;
        isFadeIn = true;
    }

    // フェードアウト開始
    public void FadeOutStart() {
        // エラー防止
        if (isFadeOut || isFadeOut) {
            return;
        }

        fadeStartTime = Time.realtimeSinceStartup;
        fadeColor.a = 1.0f;
        isFadeOut = true;
    }

    // フェードアウト開始
    public void FadeOutStart(float _fadeTime) {
        // エラー防止
        if (isFadeOut || isFadeOut) {
            return;
        }

        fadeTime = _fadeTime;
        fadeStartTime = Time.realtimeSinceStartup;
        fadeColor.a = 1.0f;
        isFadeOut = true;
    }

    // フェードイン/アウト
    IEnumerator SceneFadeIn() {
        float nowTime = Time.realtimeSinceStartup - fadeStartTime;
        float timePer = nowTime / fadeTime;

        Debug.Log("rt" + Time.realtimeSinceStartup);
        Debug.Log("nt" + nowTime);
        Debug.Log("tp" + timePer);

        // フェードイン処理
        fadeColor.a = timePer;
        fadeImage.color = fadeColor;
        if (fadeColor.a >= 1.0f) {
            fadeColor.a = 1.0f;
            isFadeIn = false;
        }

        yield return null;
    }

    IEnumerator SceneFadeOut() {
        float nowTime = Time.realtimeSinceStartup - fadeStartTime;
        float timePer = nowTime / fadeTime;

        // フェードアウト処理
        fadeColor.a = 1.0f - timePer;
        fadeImage.color = fadeColor;
        if (fadeColor.a <= 0.0f) {
            fadeColor.a = 0.0f;
            isFadeOut = false;
        }

        yield return null;
    }

    public bool IsFadeIn() {
        return isFadeIn;
    }

    public bool IsFadeOut() {
        return isFadeOut;
    }
}
