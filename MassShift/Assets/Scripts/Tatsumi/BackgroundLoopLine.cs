using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundLoopLine : MonoBehaviour {
	[SerializeField]
	GameObject linePrefab = null;
	[SerializeField]
	List<GameObject> instanceList = new List<GameObject>();
	[SerializeField]
	List<float> ratioList = new List<float>();
	[SerializeField]
	float spanTime = 1.0f;
	[SerializeField]
	float requiredTime = 1.2f;
	[SerializeField]
	Transform beginPoint, endPoint;
	[SerializeField]
	float firstTime = 0.0f;
	[SerializeField]
	int cnt = -1;

	void Start() {
		firstTime = Time.time;

		// 最初のオブジェクトを生成
		instanceList.Add(Instantiate(linePrefab));
		ratioList.Add(0.0f);
	}

	void Update() {
		// 既存のオブジェクトの更新
		float updateRatio = (Time.deltaTime / requiredTime);
		for (int idx = instanceList.Count - 1; idx >= 0; idx--) {
			// 進度の更新
			ratioList[idx] += updateRatio;

			// 終了
			if (ratioList[idx] >= 1.0f) {
				if (instanceList[idx]) {
					Destroy(instanceList[idx].gameObject);
				}
				instanceList.RemoveAt(idx);
				ratioList.RemoveAt(idx);
				continue;
			}

			// 位置の更新
			UpdatePosition(idx);
		}

		// 新たなオブジェクトの追加
		int nowCnt = (int)((Time.time - firstTime) / spanTime) + 1;
		while (nowCnt > cnt) {
			GameObject newInstance = Instantiate(linePrefab);
			newInstance.transform.position = beginPoint.position;
			instanceList.Add(newInstance);
			ratioList.Add((Time.time - (firstTime + spanTime * cnt)) / spanTime);
			UpdatePosition(instanceList.Count - 1);
			cnt++;
		}
	}

	void UpdatePosition(int _idx) {
		instanceList[_idx].transform.position = new Vector3(Mathf.Lerp(beginPoint.position.x, endPoint.position.x, ratioList[_idx]), Mathf.Lerp(beginPoint.position.y, endPoint.position.y, ratioList[_idx]), Mathf.Lerp(beginPoint.position.z, endPoint.position.z, ratioList[_idx]));
	}
}
