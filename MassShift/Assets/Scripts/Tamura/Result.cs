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

    [SerializeField]
    GameObject ClearJingleSEPrefab;

    // ゴールしたかどうか、GameManager側から変更
    public bool canGoal;

    public float animTime;
    float animStartTime;
    bool resultAnimFlg;

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
                    return;
                }
                // リザルト画面が表示されていなければ
                else {
                    // ポーズ機能を無効に
                    GetComponent<Pause>().enabled = false;

                    // リザルト画面を表示
                    ResultCanvas.SetActive(true);

                    // クリアしたのが各エリアの最終ステージならば
                    if (!Area.ExistNextStageSameArea(Area.GetAreaNumber(), Area.GetStageNumber())) {
                        // エリアクリア時のリザルト画面を表示
                        ResultCanvas_AC.SetActive(true);
                    }
                    else {
                        // ステージクリア時のリザルト画面を表示
                        ResultCanvas.SetActive(true);
                    }
                }
            }
            // チュートリアルなら
            else {
                GetComponent<ChangeScene>().OnNextButtonDown();
            }
        }
	}

    public bool IsResultCanvasActive() {
        return ResultCanvas.activeSelf;
    }

    public void SetResultCanvasActive(bool _active) {
        ResultCanvas.SetActive(_active);
    }
}
