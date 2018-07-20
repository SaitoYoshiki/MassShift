using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GeneratePlayerJumpJet : MonoBehaviour {

	[SerializeField, Tooltip("ジャンプのエフェクトのプレハブ"), EditOnPrefab]
	GameObject mJumpEffectPrefab;

	[SerializeField, Tooltip("追従するトランスフォーム")]
	GameObject mFollowTransform;

	GameObject mJumpEffect; //実体化した、ジャンプのエフェクト
	float mTakeTime = 0.0f; //経過時間
	bool mIsPlayingEffect = false;

	[SerializeField, Tooltip("エフェクトを止める時間")]
	float mEffectStopTime = 0.2f;


	void Update() {
		///*
		if(Input.GetKeyDown(KeyCode.R)) {
			GenerateEffect();
		}
		//*/

		if(mIsPlayingEffect) {
			mTakeTime += Time.deltaTime;
			if(mEffectStopTime <= mTakeTime) {
				StopEffect();
				mIsPlayingEffect = false;
			}
		}
	}

	public void GenerateInWaterEffect() {
		GenerateEffect();
	}

	public void GenerateInWaterSurfaceEffect() {
		GenerateEffect();
	}

	public void GenerateInGroundEffect() {
		GenerateEffect();
	}

	void GenerateEffect() {

		if(mJumpEffect) {
			Destroy(mJumpEffect);
		}

		//エフェクトの実体化
		mJumpEffect = Instantiate(mJumpEffectPrefab);
		mJumpEffect.GetComponent<PlayerJumpEffect>().mFollowTransform = mFollowTransform;

		mIsPlayingEffect = true;
		mTakeTime = 0.0f;
	}

	void StopEffect() {
		foreach(var p in mJumpEffect.GetComponentsInChildren<ParticleSystem>()) {
			p.Stop();
		}
	}
}
