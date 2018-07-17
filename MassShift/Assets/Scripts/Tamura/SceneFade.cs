using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneFade : MonoBehaviour {
    /*[SerializeField]
    Image fadeImage;

    [SerializeField]
    float fadeTime;
    float fadeStartTime;

    [SerializeField]
    Color fadeColor;

    bool isFadeIn;
    bool isFadeOut;

    // 初期化
    void Start() {
        fadeColor.a = 0.0f;
        fadeImage.color = fadeColor;
    }

    void Update() {
        if (isFading) {
            // 単色フェードのコルーチンを開始
            StartCoroutine(SceneFadeIn());
        }
        else {
            StopCoroutine(SceneFadeIn());
        }
    }

    // フェード開始
    public override void FadeStart() {
        // エラー防止
        if (isFading || !isFadeEnd) {
            return;
        }

        isFading = true;
    }

    // フェードイン/アウト
    IEnumerator SceneFade() {
        if (!isFadeEnd) {
            // フェードイン処理
            fadeColor.a += 1.0f * (Time.deltaTime / fadeTime);
            fadeImage.color = fadeColor;
            if (fadeColor.a >= 1.0f) {
                fadeColor.a = 1.0f;
                isFadeEnd = true;
            }

            yield return null;
        }
        else {
            // フェードアウト処理
            fadeColor.a -= 1.0f * (Time.deltaTime / fadeTime);
            fadeImage.color = fadeColor;
            if (fadeColor.a <= 0.0f) {
                fadeColor.a = 0.0f;
                isFading = false;
            }

            yield return null;
        }
    }

    public bool IsFading() {
        return isFading;
    }*/
}
