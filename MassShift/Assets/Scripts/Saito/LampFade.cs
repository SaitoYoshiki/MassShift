using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampFade : MonoBehaviour {

	[SerializeField, EditOnPrefab, Tooltip("オフの時のエミッションカラー")]
	Color mOffEmissionColor;

	[SerializeField, EditOnPrefab, Tooltip("オフの時のエミッションカラーの強さ")]
	float mOffEmissionColorPower;

	[SerializeField, EditOnPrefab, Tooltip("オンの時のエミッションカラー")]
	Color mOnEmissionColor;

	[SerializeField, EditOnPrefab, Tooltip("オンの時のエミッションカラーの強さ")]
	float mOnEmissionColorPower;
	
	[SerializeField, EditOnPrefab, Tooltip("何秒で切り替わるか")]
	float mFadeTime = 1.0f;

	[SerializeField, EditOnPrefab, Tooltip("エミッションカラーを変更するマテリアル")]
	Material mLightMaterial;

	float mRate;    //オフの時とオンの時の割合
	bool mIsOn;	//オンかどうか

	// Use this for initialization
	void Start () {
		mRate = 0.0f;
		mIsOn = false;
		Utility.ChangeMaterialColor(gameObject, mLightMaterial, "_EmissionColor", mOffEmissionColor * mOffEmissionColorPower);
	}
	
	// Update is called once per frame
	void Update () {
		float lBeforeRate = mRate;
		
		if(mIsOn) {
			mRate += 1.0f / mFadeTime * Time.deltaTime;
		}
		else {
			mRate -= 1.0f / mFadeTime * Time.deltaTime;
		}
		mRate = Mathf.Clamp01(mRate);

		//光る割合が変化していたら
		if(Mathf.Approximately(lBeforeRate, mRate) == false) {
			Utility.ChangeMaterialColor(gameObject, mLightMaterial, "_EmissionColor", Color.Lerp(mOffEmissionColor * mOffEmissionColorPower, mOnEmissionColor * mOnEmissionColorPower, mRate));
		}
	}
	
	//オンにする
	//
	public void TurnOn() {
		mIsOn = true;
	}

	//オフにする
	//
	public void TurnOff() {
		mIsOn = false;
	}

	//オン・オフにする
	//
	public void Turn(bool aIsOn) {
		mIsOn = aIsOn;
	}
}
