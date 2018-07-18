using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageChangeScenematic : MonoBehaviour {
    public enum DOOR {
        UP = 0,
        DOWN,
        RIGHT,
        LEFT
    }

    public DoorAnimManager daManager;

    bool isOpening = false;
    bool isClosing = false;
    
    public float doorAnimTime;
    
    [SerializeField, Range(0.0f, 1.0f)]
    public float doorAnimPer;
    [SerializeField, Range(0.0f, 1.0f)]
    public float doorStopPer;

    float startTime;
    float stopTime;
    
    public Vector3 openPos;
    public Vector3 stopPos;
    public Vector3 closePos;

    int area;
    int stage;

    void Start() {
        area = Area.GetAreaNumber();
        stage = Area.GetStageNumber();
        openPos = this.transform.localPosition;

        // Active時に開く
        //StartOpening();
    }

    void Update() {
        CheckOpening();
        CheckClosing();
    }

    void CheckOpening() {
        if (!isOpening) {
            return;
        }
        else {
            AnimDoor(closePos, openPos, ref isOpening, true);
        }
    }

    void CheckClosing() {
        if (!isClosing) {
            return;
        }
        else {
            AnimDoor(openPos, closePos, ref isClosing, false);
        }
    }

    public void StartOpening() {
        isOpening = true;
        startTime = Time.realtimeSinceStartup;
    }

    public void StartOpening(float _animTime) {
        isOpening = true;
        startTime = Time.realtimeSinceStartup;
        doorAnimTime = _animTime;
    }

    public void StartClosing(){
        isClosing = true;
        startTime = Time.realtimeSinceStartup;
    }

    public void StartClosing(float _animTime) {
        isClosing = true;
        startTime = Time.realtimeSinceStartup;
        doorAnimTime = _animTime;
    }
    
    // ドア開閉演出
    void AnimDoor(Vector3 _startPos, Vector3 _endPos, ref bool _flg, bool openFlg) {
        float nowTime = Time.realtimeSinceStartup - startTime;

        float timePer = nowTime / doorAnimTime;

        // アニメーション一段階目
        if (timePer <= doorAnimPer) {
            this.transform.localPosition = Vector3.Lerp(_startPos, _endPos, timePer / doorAnimPer);
        }
        // アニメーション終了
        else {
            this.transform.localPosition = _endPos;
            _flg = false;
            if (openFlg) {
                daManager.OpenCountPlus();
            }
            else {
                daManager.CloseCountPlus();
            }
        }
    }
}
