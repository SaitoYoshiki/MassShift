using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalBlackCurtain : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	[SerializeField]
	SpriteRenderer mSprite;

	//
	public void StartFade(float aStartAlpha, float aEndAlpha, float aStartTime, float aTime) {
		if(mFadeCoroutine != null) {
			StopCoroutine(mFadeCoroutine);
		}
		mFadeCoroutine = StartCoroutine(FadeCoroutine(aStartAlpha, aEndAlpha, aStartTime, aTime));
	}

	Coroutine mFadeCoroutine;

	IEnumerator FadeCoroutine(float aStartAlpha, float aEndAlpha, float aStartTime, float aTime) {

		SetAlpha(aStartAlpha);

		float lTime = 0.0f;

		//開始時間まで待つ
		while(true) {
			lTime += Time.deltaTime;
			if(lTime >= aStartTime) {
				break;
			}
			yield return null;
		}


		//終了時までフェード
		while (true) {
			lTime += Time.deltaTime;

			float lDeltaTime = lTime - aStartTime;
			SetAlpha(Mathf.Lerp(aStartAlpha, aEndAlpha, Mathf.Clamp01(lDeltaTime / aTime)));

			if (lTime >= aStartTime + aTime) {
				break;
			}
			yield return null;
		}

		SetAlpha(aEndAlpha);

		yield return null;
	}

	public void SetAlpha(float aAlpha) {
		Color c = mSprite.color;
		c.a = aAlpha;
		mSprite.color = c;
	}
}
