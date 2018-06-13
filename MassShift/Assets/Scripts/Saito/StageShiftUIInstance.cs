using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageShiftUIInstance : MonoBehaviour {

	[SerializeField]
	GameObject mUpUI;

	[SerializeField]
	GameObject mDownUI;

	[SerializeField]
	GameObject mFailUI;

	// Use this for initialization
	void Awake () {
		NotShow();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ShowDown() {
		NotShow();
		mDownUI.SetActive(true);
	}
	public void ShowUp() {
		NotShow();
		mUpUI.SetActive(true);
	}
	public void ShowFail() {
		NotShow();
		mFailUI.SetActive(true);
	}

	public void NotShow() {
		mUpUI.SetActive(false);
		mDownUI.SetActive(false);
		mFailUI.SetActive(false);
	}


	public void SetPosition(Vector3 aWorldPosition) {
		RectTransform r = GetComponent<RectTransform>();

		Vector2 lPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, aWorldPosition);
		lPoint.x *= 1920.0f / Camera.main.pixelWidth;
		lPoint.y *= 1080.0f / Camera.main.pixelHeight;

		r.anchoredPosition = lPoint;
	}
}
