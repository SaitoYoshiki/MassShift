﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeightEffect))]
public class WeightParticle : MonoBehaviour {

	[SerializeField]
	GameObject mFlyingParticle;

	[SerializeField]
	GameObject mLightParticle;

	[SerializeField]
	GameObject mHeavyParticle;

	// Use this for initialization
	void Awake () {
		GetComponent<WeightEffect>().OnWeightChange += ChangeParticle;
	}
	
	void ChangeParticle(WeightManager.Weight aWeight) {

		if (aWeight == WeightManager.Weight.flying) {
			PlayParticle(mFlyingParticle);
			StopParticle(mHeavyParticle);
			StopParticle(mLightParticle);
		}

		if (aWeight == WeightManager.Weight.light) {
			PlayParticle(mLightParticle);
			StopParticle(mHeavyParticle);
			StopParticle(mFlyingParticle);
		}

		if (aWeight == WeightManager.Weight.heavy) {
			PlayParticle(mHeavyParticle);
			StopParticle(mLightParticle);
			StopParticle(mFlyingParticle);
		}
	}

	void PlayParticle(GameObject aWeightParticle) {

		foreach (var p in aWeightParticle.GetComponentsInChildren<ParticleSystem>()) {
			p.Play();
		}
		foreach (var p in aWeightParticle.GetComponentsInChildren<MeshRenderer>()) {
			p.enabled = true;
		}
	}
	void StopParticle(GameObject aWeightParticle) {

		foreach (var p in aWeightParticle.GetComponentsInChildren<ParticleSystem>()) {
			p.Stop();
		}
		foreach (var p in aWeightParticle.GetComponentsInChildren<MeshRenderer>()) {
			p.enabled = false;
		}
	}
}
