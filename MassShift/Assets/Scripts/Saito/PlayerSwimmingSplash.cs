using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwimmingSplash : MonoBehaviour {

	[SerializeField, EditOnPrefab, Tooltip("このゲームオブジェクトに追従する")]
	GameObject mSwimmingSplashEffectPrefab;

	GameObject mSwimmingSplash;

	[SerializeField, Tooltip("このゲームオブジェクトに追従する")]
	GameObject mFollowTransform;

	public GameObject FollowTransform {
		get {
			return mFollowTransform;
		}
	}

	Player mPlayer;

	bool mIsPlaying;

	// Use this for initialization
	void Start () {
		mPlayer = GetComponent<Player>();
		mSwimmingSplash = Instantiate(mSwimmingSplashEffectPrefab, transform);

		//最初はエフェクトを止める
		PlayEffect(false);
	}
	
	// Update is called once per frame
	void Update () {

		bool lIsPlaying = IsPlayerSwimming();

		if(mIsPlaying != lIsPlaying) {
			PlayEffect(lIsPlaying);
			mIsPlaying = lIsPlaying;
		}

		Vector3 lPosition = mFollowTransform.transform.position;
		lPosition.z = 0.0f;
		mSwimmingSplash.transform.position = lPosition;
		mSwimmingSplash.transform.rotation = mFollowTransform.transform.rotation;
	}

	bool IsPlayerSwimming() {
		return mPlayer.IsWaterSurfaceSwiming;
	}


	//パーティクルを止めたり再生したりする
	//
	void PlayEffect(bool aIsPlay) {
		foreach(var p in mSwimmingSplash.GetComponentsInChildren<ParticleSystem>()) {
			if(aIsPlay) {
				p.Play();
			}
			else {
				p.Stop();
			}
		}
	}
}
