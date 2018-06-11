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

		r.anchoredPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, aWorldPosition);
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
		mAnimator.Play("Fade", 1, 1.0f);
	}
}
