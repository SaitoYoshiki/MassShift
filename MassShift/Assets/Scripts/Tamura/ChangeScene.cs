﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ChangeScene : MonoBehaviour {
    private enum CHANGE_SCENE_MODE{
        NEXT,
        RETRY,
        TUTORIAL,
        STAGESELECT,
        TITLE,
        SELECTSCENE,
        ESCAPE
    }

    public SceneObject titleScene;
    public SceneObject stageSelectScene;

    private bool changeSceneFlg;
    private CHANGE_SCENE_MODE changeSceneMode;
    private bool endGameFlg;
    private bool pauseFlg;

    bool flg;

    [SerializeField]
    private StageTransition st;
    SceneFade sf;
    Result result;

    void Start() {
        sf = FindObjectOfType<SceneFade>();
        changeSceneFlg = false;
        endGameFlg = false;
        pauseFlg = false;

        if (titleScene == null) {
            Debug.LogError("タイトルシーンが指定されていません");
        }

        if (stageSelectScene == null) {
            Debug.LogError("ステージセレクトシーンが指定されていません");
        }

        result = GetComponent<Result>();
    }

    void Update() {
        if (!changeSceneFlg) {
            if (!st.GetCloseEnd()){
                // これだとステセレとかでタイトル戻れない
            //if (!st.GetCloseEnd() && (!sf.IsFadeEnd() || SceneManager.GetActiveScene().name == "StageSelect")) {
                // これだとオープニング流れない、ステセレでタイトル戻れない
            //if (!st.GetCloseEnd() && !sf.IsFadeEnd()){
                // これだとオープニング流れない、ステセレでEXエリア行けない
                if (flg && sf.IsFadeEnd()) {
                    changeSceneFlg = true;
                }
                return;
            }
            else {
                changeSceneFlg = true;
            }
        }
        else {
            switch (changeSceneMode) {
                // 次のステージへ
                case CHANGE_SCENE_MODE.NEXT:
                    {
                        int area, stage;
                        area = Area.GetAreaNumber();
                        stage = Area.GetStageNumber();
                        /*stage++;

                        if (stage == (int)StageInfo.STAGE.STAGE_MAX) {
                            area++;
                            stage = 1;
                            if (area == (int)StageInfo.AREA.AREA_MAX) {
                                endGameFlg = true;
                            }
                        }

                        string loadSceneName = "Stage" + area.ToString() + "-" + stage.ToString();

                        if (!endGameFlg) {
                            // 次のステージを読み込む
                            // 後で修正
                            SceneManager.LoadScene(loadSceneName, LoadSceneMode.Single);
                        }
                        else {
                            // 最終ステージクリア後の処理
                        }*/

                        // 次のステージがない場合はResult.cs側で「次のステージへ」ボタンを非アクティブにするのでここでの判定はいらない
                        string nextSceneName = Area.GetNextStageSceneName(area, stage);
                        // 次のステージを読み込む
                        SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
                    }
                    break;

                // リトライ
                case CHANGE_SCENE_MODE.RETRY:
                    // 現在のシーンを再読込
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
                    break;

                // チュートリアル
                case CHANGE_SCENE_MODE.TUTORIAL:
                    // チュートリアルシーンを読み込み
                    SceneManager.LoadScene("Tutorial-1", LoadSceneMode.Single);
                    break;

                // ステージセレクト
                case CHANGE_SCENE_MODE.STAGESELECT:
                    // ステセレシーンを読み込み
                    SceneManager.LoadScene(stageSelectScene, LoadSceneMode.Single);
                    break;

                // タイトル
                case CHANGE_SCENE_MODE.TITLE:
                    // タイトルシーンを読み込み
                    SceneManager.LoadScene(titleScene, LoadSceneMode.Single);
                    break;

                // エンディング
                case CHANGE_SCENE_MODE.ESCAPE:
                    // エンディングを読み込み
                    SceneManager.LoadScene("Ending", LoadSceneMode.Single);
                    break;

                default:
                    break;
            }
        }
    }

    public void OnNextButtonDown() {
        // ポーズを解除してシーン変更フラグを立てる
        if (pauseFlg) {
            pauseFlg = false;
        }
        GetComponent<Pause>().DisableGraphicRaycaster();
        GetComponent<Pause>().canPause = false;
        GetComponent<Result>().DisableGraphicRaycaster();
        changeSceneMode = CHANGE_SCENE_MODE.NEXT;

        // リザルト画面を消す
        /*if (result.IsResultCanvasActive()) {
            result.SetResultCanvasActive(false);
            result.canGoal = false;
        }*/

        // チュートリアル以外なら
        if (Area.GetAreaNumber() != 0) {
            // ドア閉め演出
            st.CloseDoorParent();
        }
        // チュートリアルなら
        else {
            // 最終ステージでないなら
            if (Area.ExistNextStageSameArea(Area.GetAreaNumber(), Area.GetStageNumber())) {
                // 演出せず次のステージへ
                changeSceneFlg = true;
            }
            // チュートリアルの最終面なら
            else {
                // ステージセレクトへ飛ぶ
                changeSceneMode = CHANGE_SCENE_MODE.STAGESELECT;
                changeSceneFlg = true;

            }
        }
    }

    public void OnRetryButtonDown() {
        // ポーズを解除してシーン変更フラグを立てる
        if (pauseFlg) {
            pauseFlg = false;
        }
        GetComponent<Pause>().DisableGraphicRaycaster();
        GetComponent<Pause>().canPause = false;
        GetComponent<Result>().DisableGraphicRaycaster();
        changeSceneMode = CHANGE_SCENE_MODE.RETRY;

        sf.FadeInStart();
        //changeSceneFlg = true;
        flg = true;

        // リザルト画面を消す
        /*if (result.IsResultCanvasActive()) {
            result.SetResultCanvasActive(false);
            result.canGoal = false;
        }*/
    }

    public void OnTutorialButtonDown() {
        // ポーズを解除してシーン変更フラグを立てる
        if (pauseFlg) {
            pauseFlg = false;
        }
        changeSceneMode = CHANGE_SCENE_MODE.TUTORIAL;
        changeSceneFlg = true;
    }

    public void OnStageSelectButtonDown() {
        // ポーズを解除してシーン変更フラグを立てる
        if (pauseFlg) {
            pauseFlg = false;
        }
        GetComponent<Pause>().DisableGraphicRaycaster();
        GetComponent<Pause>().canPause = false;
        GetComponent<Result>().DisableGraphicRaycaster();
        changeSceneMode = CHANGE_SCENE_MODE.STAGESELECT;

        // リザルト画面を消す
        /*if (result.IsResultCanvasActive()) {
            result.SetResultCanvasActive(false);
            result.canGoal = false;
        }*/

        if (SceneManager.GetActiveScene().name == "Title") {
            //changeSceneFlg = true;
            flg = true;
            sf.FadeInStart();
        }
        else {
            // ドア閉め演出
            st.CloseDoorParent();
        }
    }

    public void OnTitleButtonDown() {
        // ポーズを解除してシーン変更フラグを立てる
        if (pauseFlg) {
            pauseFlg = false;
        }
        GetComponent<Pause>().DisableGraphicRaycaster();
        GetComponent<Pause>().canPause = false;
        changeSceneMode = CHANGE_SCENE_MODE.TITLE;
        //changeSceneFlg = true;
        flg = true;

        if (SceneManager.GetActiveScene().name != "Ending") {
            sf.FadeInStart();
            SceneFade.isGoTitleWithBlackFade = true;
        }
        else {
            sf.FadeInStart(Color.white);
            SceneFade.isGoTitleWithWhiteFade = true;
        }
    }

    public void OnEscapeButtonDown() {
        // ポーズを解除してシーン変更フラグを立てる
        if (pauseFlg) {
            pauseFlg = false;
        }
        GetComponent<Pause>().DisableGraphicRaycaster();
        GetComponent<Pause>().canPause = false;
        GetComponent<Result>().DisableGraphicRaycaster();
        changeSceneMode = CHANGE_SCENE_MODE.ESCAPE;

        // ドア閉め演出
        st.CloseDoorParent();
    }

    public void OnStageSelected() {
        changeSceneMode = CHANGE_SCENE_MODE.SELECTSCENE;
        changeSceneFlg = true;
    }

    // 必要なもの
    /* 5/4
     *変数
     * E現在のシーン┐
     * E次のシーン　├シーンのリストを作ることで一つにまとめられる？
     * E前のシーン　┘
     * Eポーズ中かどうか
     * Eオプション画面かどうか
     * E各エリアの最終ステージかどうか
     * 
     *関数
     * シーンロード/アンロード
     * スタートへ戻る
     * ポーズキャンバスのON/OFF
     * リザルトキャンバスのON/OFF
     * 
     *シーン
     * スタート
     * チュートリアル
     * ステージセレクト
     * 各ステージ
     * 
     *最終ステージ判別方法
     * シーン名
     * 最終ステージ内にのみ判別用オブジェクトを置いておく
     * 
     * 5/7
     *--シーン-----------------------------------------------------------------------
     *タイトルシーン
     * タイトルUI(チュートリアル・ステージセレクト)
     * カメラ制御スクリプト
     * ブルーム等の演出
     * 
     *チュートリアルシーン
     * 操作説明UI(デザイナーが全て描く)
     * ポーズ機能(ステージセレクト、タイトルへ戻る、オプション)
     * 
     *ステージセレクトシーン
     * ポーズ機能(タイトルへ戻る、オプション)
     *
     *ステージ
     * ステージ開始演出(デザイナー作成)
     * ポーズ機能(リトライ、ステージセレクト、タイトルへ戻る、オプション)
     * リザルト機能(次のステージ、もう一度遊ぶ、ステージセレクト)
     * StageInfoから現在のエリアとステージを取得して次のステージへ飛べるかどうか判断
     *-------------------------------------------------------------------------------
     *
     *--機能-------------------------------------------------------------------------
     * ポーズ機能(Escキーで開く・閉じる)
     * 　チュートリアル・ステージセレクト・ステージで使用可
     * 　[オプション]ボタンからオプション画面が開く、音量と解像度を調節可能
     * 
     * リザルト機能
     * 　ステージクリア時に自動的に開く
     * 　各エリアの最終ステージ以外の場合、[次のステージへ]ボタンが表示される
     *-------------------------------------------------------------------------------
     *
     * ステージのオブジェクト配置はプログラマー側で行うこと
     */
}