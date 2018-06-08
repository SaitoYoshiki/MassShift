using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class cameraMove : MonoBehaviour {

    private Vector3 cameraStartPoint = new Vector3(-41.0f, -0.0f, -15.0f);
    private Vector3 cameraZoomPoint = new Vector3(-43.0f, -0.5f, -5.0f);
    private Vector3 cameraEndPoint = new Vector3(0.0f, 1.0f, -50.0f);

    public float zoomInTime;
    public float zoomOutTime;

    [SerializeField]
    GameObject title;
    [SerializeField]
    GameObject text;
    [SerializeField]
    GameObject tutorial;
    [SerializeField]
    GameObject stageselect;

    StageTransition st;
    ChangeScene cs;

    float startZoomTime;
    float nowZoomTime;

    bool firstZoom = false;

    bool zoomInFlg = false;
    bool oldZoomInFlg;
    bool zoomOutFlg = false;
    bool oldZoomOutFlg;

    bool goTutorialFlg = false;

    //bool isAdditiveLoad = false;

    AsyncOperation TutorialActive;
    AsyncOperation StageSelectActive;

    public static bool fromTitle = false;

	void Start () {
        this.transform.position = cameraStartPoint;
        st = GameObject.Find("StageChangeCanvas").GetComponent<StageTransition>();
        cs = GameObject.Find("UIObject").GetComponent<ChangeScene>();

        RenderSettings.ambientSkyColor = new Color(0.0f, 0.0f, 0.0f);
	}
	
	void Update () {
        CheckFirstZoom();
        CheckZoomIn();
        //CheckZoomOut();
	}

    void CheckFirstZoom() {
        // ズームインし終わっていたら何もしない
        if (firstZoom) {
            return;
        }
        // ズームされていない初期状態なら
        else {
            if (Input.anyKeyDown) {
                // 「InputAnyKey」の表示を消す
                text.SetActive(false);
                firstZoom = true;
                zoomInFlg = true;
                startZoomTime = Time.realtimeSinceStartup;
            }
        }
    }

    void CheckZoomIn() {
        // ズームイン中でなくて
        if (!zoomInFlg) {
            // 前フレームでもズームインしていなければ何もしない
            if (oldZoomInFlg == zoomInFlg) {
                return;
            }
            // 前フレームでズームインが終わったなら
            else {
                // モード選択のボタンをActiveにする
                oldZoomInFlg = zoomInFlg;
                tutorial.SetActive(true);
                stageselect.SetActive(true);
            }
        }
        // ズームイン中なら
        else {
            oldZoomInFlg = zoomInFlg;
            Zoom(zoomInTime, ref zoomInFlg, cameraStartPoint, cameraZoomPoint);
        }
    }

    /*void CheckZoomOut() {
        // ズームアウト中でなくて
        if (!zoomOutFlg) {
            // 前フレームでもズームアウトしていなければ何もしない
            if (oldZoomOutFlg == zoomOutFlg) {
                if (goTutorialFlg) {
                    // チュートリアルへ飛ぶ
                    if (!isAdditiveLoad) {
                        return;
                    }
                    else {
                        isAdditiveLoad = false;
                        SceneManager.UnloadSceneAsync("Title");
                    }
                }
                else {
                    // ステージセレクトへ飛ぶ
                    if (!isAdditiveLoad) {
                        return;
                    }
                    else {
                        isAdditiveLoad = false;
                        SceneManager.UnloadSceneAsync("Title");
                    }
                }
            }
            // 前フレームでズームアウトが終わったなら
            else {
                oldZoomOutFlg = zoomOutFlg;
            }
        }
        // ズームアウト中なら
        else {
            oldZoomOutFlg = zoomOutFlg;
            //st.CloseDoorParent();
            Zoom(zoomOutTime, ref zoomOutFlg, cameraZoomPoint, cameraEndPoint);
        }
    }*/

    // タイトルでボタンが押されたらズームアウト
    public void OnButtonDown() {
        //Debug.Log("ズームアウト開始");
        //zoomOutFlg = true;
        //startZoomTime = Time.realtimeSinceStartup;
        title.SetActive(false);
        tutorial.SetActive(false);
        stageselect.SetActive(false);

        RenderSettings.ambientSkyColor = new Color(0.5019608f, 0.5019608f, 0.5019608f);

        //isAdditiveLoad = true;
        fromTitle = true;
    }

    // ズームイン/アウト
    void Zoom(float _zoomTime, ref bool _zoomFlg, Vector3 _startPos, Vector3 _endPos) {
        float zoomPer = 1.0f;
        nowZoomTime = Time.realtimeSinceStartup - startZoomTime;

        if (nowZoomTime < _zoomTime) {
            zoomPer = nowZoomTime / zoomInTime;
        }
        else {
            Debug.Log("ズーム終了");
            _zoomFlg = false;
        }
        this.transform.position = Vector3.Lerp(_startPos, _endPos, zoomPer);
    }

    public void OnTutorialSelected() {
        cameraEndPoint = new Vector3(-36.0f, 3.0f, -35.0f);
        goTutorialFlg = true;

        TutorialActive = SceneManager.LoadSceneAsync("Tutorial-1", LoadSceneMode.Single);
        SceneManager.UnloadSceneAsync("Title");
        //TutorialActive.allowSceneActivation = false;
    }

    public void OnStageSelectSelected() {
        cameraEndPoint = new Vector3(-32.0f, 1.0f, -50.0f);
        goTutorialFlg = false;

        StageSelectActive = SceneManager.LoadSceneAsync("StageSelect", LoadSceneMode.Single);
        SceneManager.UnloadSceneAsync("Title");
        //TutorialActive.allowSceneActivation = false;
    }

    // チュートリアル1の部屋と、ステージセレクト前の部屋を同じサイズにして、カメラ引きの位置は同じにする
}
