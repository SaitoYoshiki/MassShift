using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ChangeActiveInfoPanel : MonoBehaviour {
    [SerializeField]
    List<GameObject> mAreaTriggers;

    [SerializeField]
    List<GameObject> StageInfoPanel;

    [SerializeField]
    GameObject mPlayer;

    int mAreaIndex = 0;

    int oldActivePanel = 0;

    // Update is called once per frame
    void FixedUpdate() {
        //現在いるエリア位置を更新
        if (GetHitAreaIndex() != -1) {
            mAreaIndex = GetHitAreaIndex();
        }

        if (!StageInfoPanel[mAreaIndex].activeSelf) {
            StageInfoPanel[oldActivePanel].SetActive(false);
            StageInfoPanel[mAreaIndex].SetActive(true);

            oldActivePanel = mAreaIndex;
        }
    }

    int GetHitAreaIndex() {
        for (int i = 0; i < mAreaTriggers.Count; i++) {
            GameObject lAreaTrigger = mAreaTriggers[i];

            //各エリアのトリガーが持つ、全てのボックスコライダーと判定を行う
            foreach (var lC in lAreaTrigger.GetComponentsInChildren<BoxCollider>()) {

                //もしプレイヤーがヒットしていたら
                if (Physics.OverlapBox(lC.bounds.center, lC.bounds.size / 2.0f).Select(x => x.gameObject).Contains(mPlayer)) {
                    return i;
                }
            }
        }

        return -1;	//ヒットするエリアがなかった
    }
}
