using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailControl : MonoBehaviour {
	// Fanの位置
	[SerializeField]
	Transform FanTra;

	// WindStopの位置
	[SerializeField]
	Transform WindStopTra;

	// 風のパーティクルシステム
	[SerializeField]
	ParticleSystem WindPss;
	ParticleSystem.TrailModule trails;

	// trailsが制御される距離
	[SerializeField] float ControlDis;

	// 修正値
	[SerializeField, Range(0.0f, 1.0f)]
	float CorrectVal;

	[SerializeField, Range(0.0f, 1.0f)]
	float multRatio = 1.0f;

	// 今の距離
	float NowDis;


	// Use this for initialization
	void Start () {
		trails = WindPss.trails;
	}

	// Update is called once per frame
	void Update () {
		//trails.dieWithParticles = false;

//		float windStopPointX;
//		float vec = 1;
//		if (WindStopTra.position.x > FanTra.position.x)
//		{
//			vec = -1;
//		}
//		windStopPointX = (WindStopTra.position.x + WindStopTra.lossyScale.x * 0.5f * vec);

		// FanからWindStopまでの距離

		NowDis = Mathf.Abs(FanTra.position.x - WindStopTra.position.x) - (WindStopTra.lossyScale.x * 0.5f);
		if (NowDis < ControlDis) {
			//trails.dieWithParticles = true ;
			//trails.lifetime = trails.lifetime.curveMultiplier * 1f;
			//Debug.LogError("test"  + trails.lifetimeMultiplier);
			multRatio = ((CorrectVal * (NowDis)) / ControlDis);
		}else {
			multRatio = 1.0f;
		}

		trails.lifetimeMultiplier = multRatio;
	}
}
