using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(OnewayFloor)), CanEditMultipleObjects]
public class OnewayFloorEditor : Editor {

	public override void OnInspectorGUI() {

		EditorGUI.BeginChangeCheck();

		base.OnInspectorGUI();

		if (EditorGUI.EndChangeCheck()) {
			Resize();
		}

		//ボタンが押されたら、サイズ変更を行う
		if (GUILayout.Button("サイズ・上下の向きを適用")) {
			Resize();
		}
	}

	void Resize() {
		Undo.RecordObjects(targets, "Resize");
		foreach (var t in targets) {
			OnewayFloor lOnewayFloor = t as OnewayFloor;
			lOnewayFloor.Resize();
		}
	}
}
