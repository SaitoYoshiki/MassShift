using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MoveFloor)), CanEditMultipleObjects]
public class MoveFloorEditor : Editor {

	public override void OnInspectorGUI() {

		EditorGUI.BeginChangeCheck();

		base.OnInspectorGUI();

		if (EditorGUI.EndChangeCheck()) {
			Resize();
		}

		//ボタンが押されたら、サイズ変更を行う
		if (GUILayout.Button("サイズを適用")) {
			Resize();
		}
	}

	void Resize() {
		Undo.RecordObjects(targets, "Resize");
		foreach (var t in targets) {
			MoveFloor lMoveFloor = t as MoveFloor;
			lMoveFloor.Resize();
		}
	}
}
