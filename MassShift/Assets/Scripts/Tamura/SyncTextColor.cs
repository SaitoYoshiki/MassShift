using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SyncTextColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public UnityEngine.UI.Button button;
    public UnityEngine.UI.Text text;

    float colorChangeTime;
    bool colorChangeFlg;
    Color startColor;
    Color endColor;

    float startColorChangeTime;

    // Use this for initialization
    void Start() {
        colorChangeTime = button.colors.fadeDuration;
    }

    // マウスカーソルが範囲内に入った
    public void OnPointerEnter(PointerEventData eventData) {

        startColor = text.color;
        //endColor = button.colors.highlightedColor;
        endColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);

        startColorChangeTime = Time.unscaledTime;
        colorChangeFlg = true;
    }

    // マウスカーソルが範囲外に出た
    public void OnPointerExit(PointerEventData eventData) {
        startColor = text.color;
        endColor = Color.white;

        startColorChangeTime = Time.unscaledTime;
        colorChangeFlg = true;
    }

    // Update is called once per frame
    void Update() {
        if (!colorChangeFlg) {
            return;
        }
        else {
            ColorChange();
        }
    }

    void ColorChange() {
        float nowColorChangeTime = Time.unscaledTime - startColorChangeTime;
        float colorChangePer = Mathf.Clamp((nowColorChangeTime / colorChangeTime), 0.0f, 1.0f);

        text.color = Color.Lerp(startColor, endColor, colorChangePer);
        if (colorChangePer >= 1.0f) {
            colorChangeFlg = false;
        }
    }

    public void DeactiveButton() {
        text.color = Color.white;
    }
}
