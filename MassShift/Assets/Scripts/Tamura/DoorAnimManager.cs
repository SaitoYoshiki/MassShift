using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimManager : MonoBehaviour {
    public List<GameObject> doorList = new List<GameObject>();
    public GameObject StageName;
    MonoColorFade monoFade;

    string sceneName;       // 現在のシーン名
    int openDoorCount;      // 開き終わったドアの数
    int closeDoorCount;     // 閉まり終わったドアの数
    bool isDoorAnimating;   // ドアがアニメーションしているか
    bool isDoorOpenEnd;     // ドアの開き演出が終わったかどうか
    bool isDoorCloseEnd;    // ドアの閉まり演出が終わったかどうか

    [SerializeField]
    GameObject doorOpenSEPrefab;
    [SerializeField]
    GameObject doorCloseSEPrefab;

	void Awake () {
        openDoorCount = 0;
        closeDoorCount = 0;

        monoFade = StageName.GetComponent<MonoColorFade>();
        sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        isDoorAnimating = false;
	}

    void Update() {
        if (!isDoorAnimating) {
            return;
        }
        else{
            CheckOpenEnd();
            CheckCloseEnd();
        }
    }

    // ドア開き開始(親から呼び出し)
    public void StartDoorOpen() {
        SoundManager.SPlay(doorOpenSEPrefab);
        foreach (GameObject door in doorList) {
            door.GetComponent<StageChangeScenematic>().StartOpening();
        }

        isDoorAnimating = true;
    }

    // ドア閉じ開始(親から呼び出し)
    public void StartDoorClose() {
        SoundManager.SPlay(doorCloseSEPrefab);
        foreach (GameObject door in doorList) {
            door.GetComponent<StageChangeScenematic>().StartClosing();
        }

        isDoorAnimating = true;
    }

    void CheckOpenEnd() {
        if (openDoorCount < doorList.Count || isDoorOpenEnd) {
            return;
        }
        // ドアが全て開き終わったら
        else {
            // ステージセレクトシーンでは
            if (sceneName == "StageSelect") {
                // ステージ名を出さず開き演出終了
                isDoorOpenEnd = true;
                isDoorAnimating = false;
            }
            // ステージセレクトシーン以外では
            else {
                // ステージ名フェードイン開始
                StageName.SetActive(true);
                if (!monoFade.IsFading()) {
                    // ステージ名フェードアウトが終了した
                    isDoorOpenEnd = true;
                    isDoorAnimating = false;
                }
            }
        }
    }

    void CheckCloseEnd() {
        if (closeDoorCount < doorList.Count || isDoorCloseEnd) {
            return;
        }
        else {
            // ドア閉まるアニメーションが全て終了した
            isDoorCloseEnd = true;
            isDoorAnimating = false;
        }
    }

    // 開き終わったか
    public bool isOpenEnd() {
        return isDoorOpenEnd;
    }

    // 閉じ終わったか
    public bool isCloseEnd() {
        return isDoorCloseEnd;
    }

    public void OpenCountPlus() {
        openDoorCount++;
    }

    public void CloseCountPlus() {
        closeDoorCount++;
    }
}
