using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEventReceive : MonoBehaviour {
	[SerializeField]
	bool canPlaySound = true;
	[SerializeField]
	WeightManager weightMng = null;
	[SerializeField]
	GameObject walkLightSE;
	[SerializeField]
	GameObject walkHeavySE;

	void PlayWalkSE() {
		if (canPlaySound) {
			Debug.Log("PlayWalkSE");
			if (weightMng.WeightLv <= WeightManager.Weight.light) {
				SoundManager.SPlay(walkLightSE);
			} else {
				SoundManager.SPlay(walkHeavySE);
			}
		}
	}
}
