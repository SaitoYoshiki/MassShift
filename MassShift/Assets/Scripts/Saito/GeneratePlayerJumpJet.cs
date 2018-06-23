using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GeneratePlayerJumpJet : MonoBehaviour {

	[SerializeField, Tooltip("ジャンプのエフェクトのプレハブ"), EditOnPrefab]
	GameObject mJumpEffectPrefab;

	[SerializeField, Tooltip("追従するトランスフォーム")]
	GameObject mFollowTransform;

	GameObject mJumpEffect;

	void Update() {
		/*
		if(Input.GetKeyDown(KeyCode.R)) {
			GenerateEffect();
		}
		//*/
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
		//エフェクトの実体化
		mJumpEffect = Instantiate(mJumpEffectPrefab);
		mJumpEffect.GetComponent<PlayerJumpEffect>().mFollowTransform = mFollowTransform;
	}
}
