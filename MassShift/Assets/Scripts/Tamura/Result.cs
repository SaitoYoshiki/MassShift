using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Result : MonoBehaviour {
    // 通常時のリザルト画面
    [SerializeField]
    GameObject ResultCanvas;

    // エリア内の全ステージクリア時のリザルト画面
    [SerializeField]
    GameObject ResultCanvas_AC;

    // ゲーム(ファイナルステージ)クリア時のリザルト画面
    [SerializeField]
    GameObject ResultCanvas_GC;

    GameObject ResultUI;
    GameObject clearImage;
    GameObject bgLightImage;

    [SerializeField]
    GameObject ClearJingleSEPrefab;

    [SerializeField]
    StageTransition st;

    // ゴールしたかどうか、GameManager側から変更
    public bool canGoal;

    public float animTime;
    float animStartTime;
    bool resultAnimFlg;

    float alpha = 0.0f;

    bool isStartCloseDoor = false;

    void Start() {
        if (!Area.ExistNextStageSameArea(Area.GetAreaNumber(), Area.GetStageNumber())) {
            if (Area.GetAreaNumber() != 4) {
                ResultCanvas = ResultCanvas_AC;
            }
            else {
                ResultCanvas = ResultCanvas_GC;
            }
        }

        ResultUI = ResultCanvas.transform.Find("ResultUI").gameObject;
        clearImage = ResultUI.transform.Find("GameClear/ClearText").gameObject;
        bgLightImage = ResultUI.transform.Find("BG_light").gameObject;

        st = FindObjectOfType<StageTransition>();
    }

	void Update () {
        // ゴールしていないなら何もしない
        if (!canGoal) {
            return;
        }
        // ゴールしたなら
        else {
            // チュートリアル以外なら
            if (Area.GetAreaNumber() != 0) {
                // リザルト画面が表示されているなら何もしない
                if (IsResultCanvasActive()) {
                    if (resultAnimFlg) {
                        ResultAnim();
                    }
                    else {
                        if (alpha == 0.0f) {
                            SoundManager.SPlay(ClearJingleSEPrefab);
                        }
                        alpha += 0.05f;
                        clearImage.GetComponent<UnityEngine.UI.Image>().color = new Color(1.0f, 1.0f, 1.0f, alpha);
                        bgLightImage.GetComponent<UnityEngine.UI.Image>().color = new Color(1.0f, 1.0f, 1.0f, alpha);
                    }
                    return;
                }
                // リザルト画面が表示されていなければ
                else {
                    // ポーズ機能を無効に
                    GetComponent<Pause>().enabled = false;

                    // ステージクリア時のリザルト画面を表示
                    ResultCanvas.SetActive(true);
                    StartResultAnim();
                }
            }
            // チュートリアルなら
            else {
                if (!isStartCloseDoor) {
                    st.CloseDoorParent();
                    isStartCloseDoor = true;
                }

                if (st.GetCloseEnd()) {
                    GetComponent<ChangeScene>().OnNextButtonDown();
                }
            }
        }
	}

    public bool IsResultCanvasActive() {
        return ResultCanvas.activeSelf;
    }

    public void SetResultCanvasActive(bool _active) {
        ResultCanvas.SetActive(_active);
    }

    void StartResultAnim() {
        animStartTime = Time.fixedTime;
        resultAnimFlg = true;
    }

    void ResultAnim() {
        float nowAnimTime = Time.fixedTime - animStartTime;
        float animPer = Mathf.Clamp((nowAnimTime / animTime), 0.0f, 1.0f);

        /*if (animPer <= 0.7f) {
            animPer = animPer / 0.7f;
            animPer = animPer * animPer;
            ResultUI.GetComponent<RectTransform>().localPosition = Vector3.Lerp(new Vector3(0.0f, 1000.0f, 0.0f), new Vector3(0.0f, -100.0f, 0.0f), animPer);
        }
        else {
            animPer = (animPer - 0.7f) / 0.3f;
            animPer = animPer * animPer;
            ResultUI.GetComponent<RectTransform>().localPosition = Vector3.Lerp(new Vector3(0.0f, -100.0f, 0.0f), Vector3.zero, animPer);
            if (animPer >= 1.0f) {
                resultAnimFlg = false;
            }
        }*/

        animPer = animPer * animPer;
        ResultUI.GetComponent<RectTransform>().localPosition = Vector3.Lerp(new Vector3(0.0f, 1000.0f, 0.0f), Vector3.zero, animPer);
        if (animPer >= 1.0f) {
            resultAnimFlg = false;
        }
    }

    public void DisableGraphicRaycaster() {
        ResultCanvas.GetComponent<UnityEngine.UI.GraphicRaycaster>().enabled = false;
    }
}
