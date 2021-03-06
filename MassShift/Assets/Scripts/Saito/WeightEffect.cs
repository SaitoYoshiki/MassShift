﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeightManager))]
public class WeightEffect : MonoBehaviour {

	public delegate void OnWeightChangeEvent(WeightManager.Weight aWeight);

	public event OnWeightChangeEvent OnWeightChange;

	WeightManager mWeightManager;

	[SerializeField, Disable]
	WeightManager.Weight mBeforeWeight;

	// Use this for initialization
	void OnEnable () {
		Init();
	}

	private void Start() {
		Init();
	}

	void Init() {
		mWeightManager = GetComponent<WeightManager>();
		mBeforeWeight = mWeightManager.WeightLvSeem;    //見かけの重さでエフェクトを出す
		if(OnWeightChange != null) {
			OnWeightChange(mBeforeWeight);
		}
	}
	
	// Update is called once per frame
	void Update () {

		//見かけの重さが変更されていたら、イベントを呼び出す
		WeightManager.Weight lWeight = mWeightManager.WeightLvSeem;
		if(mBeforeWeight != lWeight){
			OnWeightChange(lWeight);
			mBeforeWeight = lWeight;
		}
	}
}
