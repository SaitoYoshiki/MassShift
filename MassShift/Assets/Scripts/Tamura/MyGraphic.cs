using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyGraphic : Graphic {
    [SerializeField]
    Texture tex;

    [SerializeField]
    GameObject bgScaler;

    UIVertex lt;
    UIVertex rt;
    UIVertex lb;
    UIVertex rb;

    float time = 0.0f;

    void Start() {
        // 左上
        lt = UIVertex.simpleVert;
        lt.position = new Vector3(-600, 450, 0);
        lt.uv0 = new Vector2(0, 1);
        lt.color = Color.white;

        // 右上
        rt = UIVertex.simpleVert;
        rt.position = new Vector3(600, 450, 0);
        rt.uv0 = new Vector2(1, 1);
        rt.color = Color.white;

        // 右下
        rb = UIVertex.simpleVert;
        rb.position = new Vector3(660, -450, 0);
        rb.uv0 = new Vector2(1, 0);
        rb.color = Color.white;

        // 左下
        lb = UIVertex.simpleVert;
        lb.position = new Vector3(-660, -450, 0);
        lb.uv0 = new Vector2(0, 0);
        lb.color = Color.white;

        // toDo
        //台形の変形に合わせたUVの設定
    }

    // 頂点位置と頂点色を設定して生成、基本的に一度だけ呼ばれる()
    protected override void OnPopulateMesh(VertexHelper vh) {
        vh.Clear();
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

    void Update() {
        if (time <= 1.0f) {
            ResultAnim();
            time += 0.02f;
        }
    }

    public void ResultAnim() {
        float mul = time * time;

        rb.position = Vector3.Lerp(new Vector3(960, -450, 0), new Vector3(600, -450, 0), mul);
        lb.position = Vector3.Lerp(new Vector3(-960, -450, 0), new Vector3(-600, -450, 0), mul);

        byte col = (byte)(255 * mul);

        lt.color = new Color32(col, col, col, 255);
        rt.color = new Color32(col, col, col, 255);
        lb.color = new Color32(col, col, col, 255);
        rb.color = new Color32(col, col, col, 255);

        bgScaler.GetComponent<RectTransform>().transform.localScale = new Vector3(1.0f, mul, 1.0f);

        ReMakeGraphic();
    }

    // グラフィックを作成し直させる
    void ReMakeGraphic() {
        var graphics = base.GetComponent<Graphic>();
        if (graphics != null) {
            graphics.SetVerticesDirty();
        }
    }
}
