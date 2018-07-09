﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class cameraMove : MonoBehaviour {

    private Vector3 cameraStartPoint = new Vector3(-15.0f, -0.0f, -15.0f);
    private Vector3 cameraZoomPoint = new Vector3(-17.0f, -0.5f, -5.0f);
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

    [SerializeField]
    PlayerAnimation pa;

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
    bool titleEndFlg = false;

    AsyncOperation TutorialActive;
    AsyncOperation StageSelectActive;

    public static bool fromTitle = false;

    //Color startLightColor = new Color(0.0f, 0.0f, 0.0f);
    //Color endLightColor = new Color(0.5019608f, 0.5019608f, 0.5019608f);
    //float colorPer = 0.0f;

    float loadStartTime;
    ChangeLoadingImage cli;

	void Start () {
        Time.timeScale = 1.0f;

        this.transform.position = cameraStartPoint;
        st = GameObject.Find("StageChangeCanvas").GetComponent<StageTransition>();
        st.gameObject.SetActive(false);
        cs = GameObject.Find("UIObject").GetComponent<ChangeScene>();

        cli = FindObjectOfType<ChangeLoadingImage>();
        //RenderSettings.ambientSkyColor = startLightColor;
	}

    void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode) {
        SceneManager.SetActiveScene(scene);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
	
	void Update () {
        CheckFirstZoom();
        CheckZoomIn();

        /*if (colorPer > 0.0f && colorPer < 1.0f) {
            colorPer += 0.01f;
            RenderSettings.ambientSkyColor = Color.Lerp(startLightColor, endLightColor, colorPer);
            if (colorPer >= 1.0f) {
                //StageSelectActive.allowSceneActivation = true;
            }
        }*/

        if (!titleEndFlg) {
            return;
        }
        else {
            if (pa.IsStandByAnimationFinish()) {
                if (goTutorialFlg) {
                    TutorialActive.allowSceneActivation = true;
                }
                else {
                    StageSelectActive.allowSceneActivation = true;
                }
            }
        }
	}

    void CheckFirstZoom() {
        // ズームインし終わっていたら何もしない
        if (firstZoom) {
            return;
        }
        // ズームされていない初期状態なら
        else {
            if (Input.GetMouseButtonDown(0)) {
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

        //RenderSettings.ambientSkyColor = endLightColor;

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
            _zoomFlg = false;
        }
        this.transform.position = Vector3.Lerp(_startPos, _endPos, zoomPer);
    }

    public void OnTutorialSelected() {
        cameraEndPoint = new Vector3(-36.0f, 3.0f, -35.0f);
        goTutorialFlg = true;
        titleEndFlg = true;
        TutorialActive = SceneManager.LoadSceneAsync("Tutorial-1", LoadSceneMode.Single);
        TutorialActive.allowSceneActivation = false;

        loadStartTime = Time.realtimeSinceStartup;
        StartCoroutine(CheckProgress(TutorialActive));
    }

    public void OnStageSelectSelected() {
        cameraEndPoint = new Vector3(-32.0f, 1.0f, -50.0f);
        goTutorialFlg = false;
        titleEndFlg = true;
        StageSelectActive = SceneManager.LoadSceneAsync("StageSelect", LoadSceneMode.Single);
        StageSelectActive.allowSceneActivation = false;

        loadStartTime = Time.realtimeSinceStartup;
        StartCoroutine(CheckProgress(StageSelectActive));
    }

    // ロードの進捗(0～0.9)を出力
    IEnumerator CheckProgress(AsyncOperation _aop) {
        while (!_aop.isDone) {
            //Debug.Log("読み込み進捗:" + _aop.progress);
            float loadtime = Time.realtimeSinceStartup - loadStartTime;
            //Debug.Log("ロード開始からの経過時間:" + loadtime);

            SwitchLoadingImage(loadtime);

            yield return null;
        }
    }

    void SwitchLoadingImage(float _loadtime) {
        int timeStage = (int)((_loadtime % 2.0f) * 2);
        Debug.Log("経過時間段階:" + timeStage);
        cli.ChangeImage(timeStage);
    }
}
