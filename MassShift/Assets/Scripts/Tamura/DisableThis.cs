using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableThis : MonoBehaviour {
    [SerializeField]
    bool isNotInTutorial;

    [SerializeField]
    bool isNotInStage;


	void Start () {
        // 現在いるのがチュートリアルかつ
        if (Area.GetAreaNumber() == 0) {
            // このスクリプトがアタッチされているオブジェクトがチュートリアル用でないなら
            if (isNotInTutorial) {
                // チュートリアル用ポーズ画面を表示しない
                this.gameObject.SetActive(false);
            }
        }
        // 現在いるのがステージで
        else{
            // このスクリプトがアタッチされているオブジェクトがステージ用でないなら
            if (isNotInStage) {
                // ステージ用ポーズ画面を表示しない
                this.gameObject.SetActive(false);
            }
        }
	}
}
