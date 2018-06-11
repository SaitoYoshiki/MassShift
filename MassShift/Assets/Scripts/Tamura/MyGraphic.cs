using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MyGraphic : Graphic {
    // 頂点位置と頂点色を設定して生成
    protected override void OnPopulateMesh(VertexHelper vh) {
        vh.Clear();

        // 左上
        UIVertex lt = UIVertex.simpleVert;
        lt.position = new Vector3(-50, 100, 0);
        lt.color = Color.green;

        // 右上
        UIVertex rt = UIVertex.simpleVert;
        rt.position = new Vector3(50, 100, 0);
        rt.color = Color.red;

        // 右下
        UIVertex rb = UIVertex.simpleVert;
        rb.position = new Vector3(100, 0, 0);
        rb.color = Color.yellow;

        // 左下
        UIVertex lb = UIVertex.simpleVert;
        lb.position = new Vector3(-100, 0, 0);
        lb.color = Color.white;

        vh.AddUIVertexQuad(new UIVertex[] {
            lb, rb, rt, lt
        });
    }
}
