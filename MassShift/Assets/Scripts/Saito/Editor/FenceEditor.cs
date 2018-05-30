using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Fence))]
public class FenceEditor : Editor {

	public override void OnInspectorGUI() {

		//ボタンが押されたら、サイズ変更を行う
		if (GUILayout.Button("サイズ変更")) {
			Fence lFence = (Fence)target;
			lFence.Resize();
		}

		base.OnInspectorGUI();
	}
}
