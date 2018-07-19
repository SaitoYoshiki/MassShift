using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTitleUI : MonoBehaviour {
    [SerializeField]
    GameObject FirstTitleUI;

    [SerializeField]
    GameObject SecondTitleUI;

	void Awake () {
        GameObject TitleUI;

        // このプレイデータでステージセレクトを訪れたことがあれば
        if (SaveData.Instance.mEventDoneFlag.mAlreadyVisitStageSelect) {
            // ステセレへ行けるほうのタイトルを表示
            TitleUI = SecondTitleUI;
        }
        else {
            // ステセレへ行けるほうのタイトルを表示
            TitleUI = FirstTitleUI;
        }
	}
}
