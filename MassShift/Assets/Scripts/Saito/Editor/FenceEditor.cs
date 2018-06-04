using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Fence)), CanEditMultipleObjects]
public class FenceEditor : Editor {

	public override void OnInspectorGUI() {
		
		EditorGUI.BeginChangeCheck();

		base.OnInspectorGUI();

		if (EditorGUI.EndChangeCheck()) {
			Resize();
		}

		//ボタンが押されたら、サイズの適用を行う
		if (GUILayout.Button("サイズを適用")) {
			Resize();
		}
	}

	void Resize() {
		Undo.RecordObjects(targets, "Resize");
		foreach (var t in targets) {
			Fence lFence = t as Fence;
			lFence.Resize();
		}
	}
}
