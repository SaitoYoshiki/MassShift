using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SyncChangeColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public Image img;
    public UnityEngine.UI.Button button;
    public UnityEngine.UI.Text text;

    float colorChangeTime;
    bool colorChangeFlg;
    Color startColor;
    Color endColor;
    Color startColor2;
    Color endColor2;

    float startColorChangeTime;

	// Use this for initialization
	void Start () {
        colorChangeTime = button.colors.fadeDuration;
        img.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        text.color = new Color(0.2f, 0.8f, 0.8f, 0.5f);
	}

    // マウスカーソルが範囲内に入った
    public void OnPointerEnter(PointerEventData eventData) {
        startColor = img.color;
        endColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        startColor2 = text.color;
        endColor2 = new Color(0.2f, 0.8f, 0.8f, 1.0f);

        startColorChangeTime = Time.unscaledTime;
        colorChangeFlg = true;
    }

    // マウスカーソルが範囲外に出た
    public void OnPointerExit(PointerEventData eventData) {
        startColor = img.color;
        endColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);

        startColor2 = text.color;
        endColor2 = new Color(0.2f, 0.8f, 0.8f, 0.5f);

        startColorChangeTime = Time.unscaledTime;
        colorChangeFlg = true;
    }

	// Update is called once per frame
	void Update () {
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

        img.color = Color.Lerp(startColor, endColor, colorChangePer);
        text.color = Color.Lerp(startColor2, endColor2, colorChangePer);
        if (colorChangePer >= 1.0f) {
            colorChangeFlg = false;
        }
    }

    public void DeactiveButton() {
        img.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        text.color = new Color(0.2f, 0.8f, 0.8f, 0.5f);
    }
}
