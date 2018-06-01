using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeStageScene : MonoBehaviour {

	[SerializeField, SceneName]
	List<string> mSceneNameList;

	// Use this for initialization
	void Start() {
		
	}

	// Update is called once per frame
	void Update() {

	}


	private void OnGUI() {


		//縦方向のレイアウト開始
		GUILayout.BeginVertical();

		float lTotalWidth = 0.0f;   //その行での横幅の合計値
		const float cWidth = 90.0f; //ボタンの横幅
		const float cHeight = 50.0f; //ボタンの縦幅

		for (int i = 0; i < mSceneNameList.Count; i++) {
			var s = mSceneNameList[i];

			//行のはじめ
			if (lTotalWidth == 0.0f) {
				GUILayout.BeginHorizontal();
			}

			lTotalWidth += cWidth;

			//ボタンが押されたら、シーン移動
			if (GUILayout.Button(s, GUILayout.MinWidth(cWidth), GUILayout.MaxWidth(cWidth), GUILayout.MinHeight(cHeight), GUILayout.MaxHeight(cHeight))) {
				UnityEngine.SceneManagement.SceneManager.LoadScene(s);
			}

			//次のボタンが画面外にはみ出るなら
			if (lTotalWidth + cWidth >= Screen.width) {
				//次の行へ行く
				GUILayout.EndHorizontal();
				lTotalWidth = 0.0f;
			}
		}

		//縦方向のレイアウト終了
		GUILayout.EndVertical();
	}
}
