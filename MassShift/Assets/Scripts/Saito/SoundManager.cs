﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	[SerializeField, EditOnPrefab]
	GameObject mSounds;

	private void Start() {
		//ゲーム起動中、複数のインスタンスが作られてはならない
		var sms = FindObjectsOfType<SoundManager>();
		if(sms.Length >= 2) {
			Debug.LogError("<color=#ff0000>SoundManagerが複数作成されています</color>", this);
			Destroy(this);
		}
	}

	public GameObject Play(GameObject aSoundPrefab, float aDelay) {
		if (aSoundPrefab == null) return null;
		var lSoundInstance = Instantiate(aSoundPrefab, mSounds.transform);
		lSoundInstance.GetComponent<AudioSource>().PlayDelayed(aDelay);
		return lSoundInstance;
	}
	public GameObject Play(GameObject aSoundPrefab) {
		return Play(aSoundPrefab, 0.0f);
	}

	public void Stop(GameObject aSoundInstance) {
		if (aSoundInstance == null) return;
		aSoundInstance.GetComponent<AudioSource>().Stop();
		Destroy(aSoundInstance);
	}

	public void Pause(GameObject aSoundInstance, bool aPause) {
		if (aSoundInstance == null) return;
		if (aPause) {
			aSoundInstance.GetComponent<AudioSource>().Pause();
		}
		else {
			aSoundInstance.GetComponent<AudioSource>().UnPause();
		}
	}


	//シングルトン的
	static SoundManager sInstance = null;
	public static SoundManager Instance {
		get {
			if (sInstance == null) {
				sInstance = FindObjectOfType<SoundManager>();
				if (sInstance == null) {
					Debug.LogError("<color=#ff0000>SoundManagerが存在しません</color>");
				}
			}
			return sInstance;
		}
	}
}
