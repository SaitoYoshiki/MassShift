using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellChangeSphere : MonoBehaviour {
	[SerializeField]
	float minSize = 0.0f;
	[SerializeField]
	float maxSize = 100.0f;
	[SerializeField]
	float requiredTime = 1.0f;
	[SerializeField]
	Material afterMat;
	[SerializeField]
	float ratio = 0.0f;
	float beginTime = 0.0f;

	void Start() {
		beginTime = Time.time;
	}

	void Update() {
		if (ratio >= 1.0f) {
			Destroy(gameObject);
			return;
		}

		// 進行度合いを計算
		ratio = Mathf.Clamp(((Time.time - beginTime) / requiredTime), 0.0f, 1.0f);

		// 大きさを設定
		transform.localScale = (Vector3.one * Mathf.Lerp(minSize, maxSize, ratio));
	}

	void OnTriggerEnter(Collider _col) {
	Debug.Log("");
		BackgroundCell cell = _col.GetComponent<BackgroundCell>();
		if (!cell) return;
		_col.GetComponent<BackgroundCell>().PowerfulLighting(afterMat);
	}
}
