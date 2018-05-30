using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorActivate : MonoBehaviour {
    // タイトルシーンに移ったらカーソルを表示する
	void Start () {
        Cursor.visible = true;
	}
}
