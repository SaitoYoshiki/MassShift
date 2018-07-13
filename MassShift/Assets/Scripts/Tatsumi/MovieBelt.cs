using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovieBelt : MonoBehaviour {
	[SerializeField]
	Transform topBelt = null, bottomBelt = null;

	[SerializeField]
	bool isDisplay = false;
	public bool IsDisplay {
		get {
			return isDisplay;
		}
		set {
			isDisplay = value;
		}
	}

	[SerializeField]
	float move = 0.025f;
	public float Move {
		get {
			return move;
		}
		set {
			move = value;
		}
	}

	[SerializeField]
	float ratioSpd = 0.05f;
	public float RatioSpd {
		get {
			return ratioSpd;
		}
		set {
			ratioSpd = value;
		}
	}

	[SerializeField]
	float ratio = 0.0f;
	public float Ratio {
		get {
			return ratio;
		}
	}

	Vector3 defTopLocalPos = Vector3.zero, defBottomLocalPos = Vector3.zero;

	void Awake() {
		defTopLocalPos = topBelt.localPosition;
		defBottomLocalPos = bottomBelt.localPosition;

		topBelt.localPosition = new Vector3(topBelt.localPosition.x, Mathf.Lerp(defTopLocalPos.y, (defTopLocalPos.y - Move), ratio), topBelt.localPosition.z);
		bottomBelt.localPosition = new Vector3(bottomBelt.localPosition.x, Mathf.Lerp(defBottomLocalPos.y, (defBottomLocalPos.y + Move), ratio), bottomBelt.localPosition.z);
	}

	void FixedUpdate () {
		// 進度を変更
		if (IsDisplay) {
			ratio += RatioSpd;
		} else {
			ratio -= RatioSpd;
		}
		ratio = Mathf.Clamp(ratio, 0.0f, 1.0f);

		// 進度に応じて位置を変更
		topBelt.localPosition = new Vector3(topBelt.localPosition.x, Mathf.Lerp(defTopLocalPos.y, (defTopLocalPos.y - Move), ratio), topBelt.localPosition.z);
		bottomBelt.localPosition = new Vector3(bottomBelt.localPosition.x, Mathf.Lerp(defBottomLocalPos.y, (defBottomLocalPos.y + Move), ratio), bottomBelt.localPosition.z);
	}
}
