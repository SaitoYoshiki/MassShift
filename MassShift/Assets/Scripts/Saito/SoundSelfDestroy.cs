using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundSelfDestroy : MonoBehaviour {

	AudioSource mAudioSource;

	[HideInInspector]
	public bool mIsPause = false;

	float mDestroyNowTime = 0.0f;

	const float cDestroyTime = 10.0f;	//再生が止まってから、削除するまでの秒数

	// Use this for initialization
	void Start() {
		mAudioSource = GetComponent<AudioSource>();
		mDestroyNowTime = 0.0f;
	}

	// Update is called once per frame
	void Update() {
		//削除される条件を満たしたら
		if(DestroyCondition()) {
			mDestroyNowTime += Time.deltaTime;
			if(mDestroyNowTime >= cDestroyTime) {
				Destroy(gameObject);
			}
		}
		else {
			mDestroyNowTime = 0.0f;	//削除されるまでの時間をリセット
		}
	}

	bool DestroyCondition() {
		if (mAudioSource.isPlaying == true) return false;
		if (mIsPause == true) return false;   //ポーズ中なら破棄しない
		return true;
	}
}