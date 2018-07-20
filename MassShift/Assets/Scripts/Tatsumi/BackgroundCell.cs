using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundCell : MonoBehaviour {
	[SerializeField]
	Material befMat, aftMat;

	[SerializeField]
	float ratio = 0.0f;
	[SerializeField]
	float beginTime = 0.0f;
	[SerializeField]
	float ratioUpTime = 1.0f;
	[SerializeField]
	float holdTime = 2.0f;
	[SerializeField]
	float ratioDownTime = 1.0f;
	[SerializeField]
	float spanTimeMin = 0.5f;
	[SerializeField]
	float spanTimeMax = 2.0f;

	Material mat;

	[SerializeField]
	bool useCollisionTrigger = true;
	[SerializeField]
	bool useRandomTime = false;
	[SerializeField]
	bool isPowerfulLighting = false;

	void Start() {
		GetComponent<MeshRenderer>().material = mat = GetComponent<MeshRenderer>().material;

		if (!useRandomTime) return;
		beginTime = Time.time + Random.Range(spanTimeMin, spanTimeMax);
	}

	void Update() {
		if ((beginTime <= Time.time) || (ratio > 0.0f) || (isPowerfulLighting && (ratio < 1.0f))) {
			float elapsedTime = (Time.time - beginTime);
			// 点灯
			if (elapsedTime <= ratioUpTime) {
				ratio = (elapsedTime / ratioUpTime);
			}
			// 点灯保持
			else if ((elapsedTime - ratioUpTime) <= holdTime) {
				ratio = 1.0f;
			}
			// 滅灯
			else if (((elapsedTime - ratioUpTime - holdTime) <= ratioDownTime) && (!isPowerfulLighting)) {
				ratio = (1.0f - ((elapsedTime - ratioUpTime - holdTime) / ratioDownTime));
			}
			// 終了
			else if (!isPowerfulLighting) {
				ratio = 0.0f;

				if (useRandomTime) {
					// 次回開始時間を設定
					beginTime = Time.time + Random.Range(spanTimeMin, spanTimeMax);
				}
			}

			// マテリアルの変更
			mat.color = new Color(Mathf.Lerp(befMat.color.r, aftMat.color.r, ratio), Mathf.Lerp(befMat.color.g, aftMat.color.g, ratio), Mathf.Lerp(befMat.color.b, aftMat.color.b, ratio), Mathf.Lerp(befMat.color.a, aftMat.color.a, ratio));
			mat.SetColor("_EmissionColor", new Color(Mathf.Lerp(befMat.GetColor("_EmissionColor").r, aftMat.GetColor("_EmissionColor").r, ratio), Mathf.Lerp(befMat.GetColor("_EmissionColor").g, aftMat.GetColor("_EmissionColor").g, ratio), Mathf.Lerp(befMat.GetColor("_EmissionColor").b, aftMat.GetColor("_EmissionColor").b, ratio), Mathf.Lerp(befMat.GetColor("_EmissionColor").a, aftMat.GetColor("_EmissionColor").a, ratio)));
		}
	}

	void OnTriggerEnter(Collider _col) {
		if (_col.tag != "Area3BackgroundTrigger") return;
		if (!useCollisionTrigger) return;

		// 衝突時を開始時間として設定
		beginTime = Time.time;
	}

	public void PowerfulLighting(Material _newMat) {
		// 現在のマテリアルを変更前マテリアルに設定
		befMat = mat;

		// 新たなマテリアルを変更後マテリアルに設定
		aftMat = _newMat;

		// 現在時間を開始時間として設定
		beginTime = Time.time;

		// 強制的に点灯
		isPowerfulLighting = true;
	}
}
