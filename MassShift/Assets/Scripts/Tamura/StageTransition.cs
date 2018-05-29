using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTransition : MonoBehaviour {
    [SerializeField]
    List<GameObject> door = new List<GameObject>();

    int area;
    int stage;

    void Awake() {
        area = Area.GetAreaNumber();
        stage = Area.GetStageNumber();

        // 現在のエリアに対応したドアをActiveに変更
        ActivateDoor();
    }

    // シーン開始時にエリアに対応したドアキャンバスをActivateする
    public void ActivateDoor() {
        // 対応したエリアが存在するなら
        if (area > 0) {
            door[area - 1].SetActive(true);
        }
        else {
            door[0].SetActive(true);
        }
    }

    // エリアに対応したドアを開ける
    public void OpenDoorParent() {
        // 対応したエリアが存在するなら
        if (area > 0) {
            door[area - 1].GetComponent<DoorAnimManager>().StartDoorOpen();
        }
        else {
            door[0].GetComponent<DoorAnimManager>().StartDoorOpen();
        }
    }

    // エリアに対応したドアを閉じる
    public void CloseDoorParent() {
        // 対応したエリアが存在するなら
        if (area > 0) {
            Debug.Log("Big"+door[area - 1].name);
            door[area - 1].GetComponent<DoorAnimManager>().StartDoorClose();
        }
        else {
            Debug.Log("Small"+door[0].name);
            door[0].GetComponent<DoorAnimManager>().StartDoorClose();
        }
    }

    // ドア開き演出が終了したかどうか取得
    public bool GetOpenEnd() {
        if (area > 0) {
            return door[area - 1].GetComponent<DoorAnimManager>().isOpenEnd();
        }
        else {
            return door[0].GetComponent<DoorAnimManager>().isOpenEnd();
        }
    }

    // ドア閉め演出が終了したかどうか取得
    public bool GetCloseEnd() {
        if (area > 0) {
            return false;
        }
        else {
            return false;
        }
    }
}
