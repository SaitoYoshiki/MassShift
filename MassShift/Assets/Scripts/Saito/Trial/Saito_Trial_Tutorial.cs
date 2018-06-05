using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Saito_Trial_Tutorial : MonoBehaviour {

	[SerializeField]
	List<BoxCollider> mArea;

	[SerializeField]
	List<UnityEngine.UI.Image> mImage;

	[SerializeField]
	GameObject mPlayer;

	int mTargetIndex = -1;

	[SerializeField]
	float mFadeTime = 1.0f;


	// Use this for initialization
	void Start () {
		foreach(var i in mImage) {
			SetFade(i, 0.0f);
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		mTargetIndex = GetHitIndex();

		for (int i = 0; i < mArea.Count; i++) {
			float lFadeDelta = -1.0f / mFadeTime * Time.fixedDeltaTime;
			if(mTargetIndex == i) {
				lFadeDelta *= -1.0f;
			}
			SetFade(mImage[i], GetFade(mImage[i]) + lFadeDelta);
		}
	}

	void SetFade(UnityEngine.UI.Image aImage, float aFade) {
		Color c = aImage.color;
		c.a = Mathf.Clamp01(aFade);
		aImage.color = c;
	}
	float GetFade(UnityEngine.UI.Image aImage) {
		return aImage.color.a;
	}

	int GetHitIndex() {

		for(int i = 0; i < mArea.Count; i++) {
			BoxCollider lArea = mArea[i];
			if(Physics.OverlapBox(lArea.bounds.center, lArea.bounds.size / 2.0f).Select(x => x.gameObject).Contains(mPlayer)) {
				return i;
			}
		}
		return -1;

	}
}
