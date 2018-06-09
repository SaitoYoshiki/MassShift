using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LandImpact))]
public class LandImpactShake : MonoBehaviour {

	[SerializeField, Tooltip("揺れる時間"), EditOnPrefab]
	float mShakeTime = 0.2f;

	[SerializeField, Tooltip("揺れる大きさ"), EditOnPrefab]
	float mShakeMagnitude = 0.2f;

	[SerializeField, Tooltip("この高さ以下なら、揺れを半分に抑える"), EditOnPrefab]
	float mShakeDisHeight = 1.5f;

	// Use this for initialization
	void Awake () {
		GetComponent<LandImpact>().OnLand += OnLand;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnLand(WeightManager.Weight aWeight, LandImpact.CEnviroment aEnviroment, float aFallDistance) {

		//重さが2で
		if(aWeight == WeightManager.Weight.heavy) {
			//水中か、地上に着地したら
			if(aEnviroment == LandImpact.CEnviroment.cWater || aEnviroment == LandImpact.CEnviroment.cGround) {
				if (mShakeCoroutine != null) {
					StopCoroutine(mShakeCoroutine);
				}
				
				if (mShakeDisHeight >= aFallDistance) {
					mShakeCoroutine = StartCoroutine(Shake(mShakeTime, mShakeMagnitude / 2.0f, 0.25f));	//カメラを揺らす（弱い）
				}
				else {
					mShakeCoroutine = StartCoroutine(Shake(mShakeTime, mShakeMagnitude * 3.0f, 0.25f)); //カメラを揺らす（強い）
				}
			}
		}
	}

	Coroutine mShakeCoroutine;
	IEnumerator Shake(float aShakeTime, float aShakeMagnitude, float aShakeDelay) {
		//待機
		yield return new WaitForSeconds(aShakeDelay);
		ShakeCamera.ShakeAll(aShakeTime, aShakeMagnitude); //カメラを揺らす
	}
}
