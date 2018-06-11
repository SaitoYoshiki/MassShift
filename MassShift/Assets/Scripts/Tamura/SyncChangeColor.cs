using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SyncChangeColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public Image img;
    public UnityEngine.UI.Button button;


    float colorChangeTime;
    bool colorChangeFlg;
    Color startColor;
    Color endColor;

    float startColorChangeTime;

	// Use this for initialization
	void Start () {
        colorChangeTime = button.colors.fadeDuration;
        img.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
	}

    // マウスカーソルが範囲内に入った
    public void OnPointerEnter(PointerEventData eventData) {
        startColor = img.color;
        endColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        startColorChangeTime = Time.unscaledTime;
        colorChangeFlg = true;
    }

    // マウスカーソルが範囲外に出た
    public void OnPointerExit(PointerEventData eventData) {
        startColor = img.color;
        endColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);
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
        if (colorChangePer >= 1.0f) {
            colorChangeFlg = false;
        }
    }
}
