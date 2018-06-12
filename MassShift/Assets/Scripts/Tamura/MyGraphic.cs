using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyGraphic : Graphic {
    [SerializeField]
    private Texture tex;

    // 頂点位置と頂点色を設定して生成、基本的に一度だけ呼ばれる()
    protected override void OnPopulateMesh(VertexHelper vh) {
        vh.Clear();

        Debug.Log("mesh");

        // 左上
        UIVertex lt = UIVertex.simpleVert;
        lt.position = new Vector3(-600, 450, 0);
        lt.uv0 = new Vector2(0, 1);
        lt.color = Color.white;

        // 右上
        UIVertex rt = UIVertex.simpleVert;
        rt.position = new Vector3(600, 450, 0);
        rt.uv0 = new Vector2(1, 1);
        rt.color = Color.white;

        // 右下
        UIVertex rb = UIVertex.simpleVert;
        rb.position = new Vector3(660, -450, 0);
        rb.uv0 = new Vector2(1, 0);
        rb.color = Color.white;

        // 左下
        UIVertex lb = UIVertex.simpleVert;
        lb.position = new Vector3(-660, -450, 0);
        lb.uv0 = new Vector2(0, 0);
        lb.color = Color.white;

        vh.AddUIVertexQuad(new UIVertex[] {
            lb, rb, rt, lt
        });
    }

    // テクスチャの設定
    public override Texture mainTexture {
        get {
            // ここで設定したいテクスチャを返すようにする
            return tex;
        }
    }

    // グラフィックを作成し直させる
    void ReMakeGraphic() {
        var graphics = base.GetComponent<Graphic>();
        if (graphics != null) {
            graphics.SetVerticesDirty();
        }
    }
}
