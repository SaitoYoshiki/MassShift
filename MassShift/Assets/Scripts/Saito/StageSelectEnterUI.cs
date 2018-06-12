using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectEnterUI : MonoBehaviour {

	[SerializeField]
	UnityEngine.UI.Image mImage;

	[SerializeField]
	Animator mAnimator;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetPosition(Vector3 aWorldPosition) {
		RectTransform r = GetComponent<RectTransform>();

		Vector2 lPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, aWorldPosition);
		lPoint.x *= 1920.0f/ Camera.main.pixelWidth;
		lPoint.y *= 1080.0f / Camera.main.pixelHeight;

		r.anchoredPosition = lPoint;
	}

	public void SetRotation(Quaternion aRotation) {
		RectTransform r = GetComponent<RectTransform>();
		r.rotation = aRotation;
		mImage.GetComponent<RectTransform>().localRotation = Quaternion.Inverse(aRotation);
	}

	public void StartAnimation() {
		//透明度のアニメーションの設定
		mAnimator.SetFloat("Fade", 1.0f);

		mAnimator.Play("Pop", 0, 0.0f);
		mAnimator.Play("Fade", 1, 0.0f);
	}
	public void StopAnimation() {
		//透明度のアニメーションの設定
		mAnimator.SetFloat("Fade", -1.0f);

		float lNowFadeTime = Mathf.Clamp01(mAnimator.GetCurrentAnimatorStateInfo(1).normalizedTime);
		if (mAnimator.GetCurrentAnimatorStateInfo(1).IsName("Empty")) {
			lNowFadeTime = 0.0f;
		}
		mAnimator.Play("Fade", 1, lNowFadeTime);
	}
}
