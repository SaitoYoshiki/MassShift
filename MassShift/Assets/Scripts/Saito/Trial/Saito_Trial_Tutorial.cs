using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Saito_Trial_Tutorial : MonoBehaviour {
    [SerializeField]
    List<TutorialImageSet> imgSet;
    
    /*
	[SerializeField]
	List<BoxCollider> mArea;

	[SerializeField]
	List<UnityEngine.UI.Image> mImage;
    */
	[SerializeField]
	GameObject mPlayer;

	int mTargetIndex = -1;

	[SerializeField]
	float mFadeTime = 0.2f;

    Pause pause;

    MoveTransform mt;

	// Use this for initialization
	void Start () {
		/*foreach(var i in mImage) {
			SetFade(i, 0.0f);
		}*/
        // リストに登録したチュートリアルセットすべての
        foreach (var iset in imgSet) {
            // 枠のライト部分を暗く
            SetFade(iset.tutorialLight, 0.0f);
        }

        pause = FindObjectOfType<Pause>();
        mt = FindObjectOfType<MoveTransform>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (mt.IsMove) {
            foreach (var iset in imgSet) {
                iset.SetVideoScaleZero();
            }
            return;
        }
        else {
            mTargetIndex = GetHitIndex();
        }

		/*for (int i = 0; i < mArea.Count; i++) {
			float lFadeDelta = -1.0f / mFadeTime * Time.fixedDeltaTime;
			if(mTargetIndex == i) {
				lFadeDelta *= -1.0f;
			}
			SetFade(mImage[i], GetFade(mImage[i]) + lFadeDelta);
		}*/

        for (int i = 0; i < imgSet.Count; i++) {
            float lFadeDelta = -1.0f / mFadeTime * Time.fixedDeltaTime;

            if (mTargetIndex == i) {
                lFadeDelta *= -1.0f;
                if (!imgSet[i].isMonitorON && pause.canPause) {
                    imgSet[i].isMonitorON = true;
                    imgSet[i].StartAnimation();
                }
            }
            else {
                if (imgSet[i].isMonitorON) {
                    imgSet[i].isMonitorON = false;
                    imgSet[i].StartAnimation();
                }
            }
            SetFade(imgSet[i].tutorialLight, GetFade(imgSet[i].tutorialLight) + lFadeDelta);
            imgSet[i].MonitorAnimation();
        }
	}

	void SetFade(UnityEngine.UI.Image aImage, float aFade) {
		Color c = aImage.color;
		c.a = Mathf.Clamp01(aFade);
		aImage.color = c;
	}

	float GetFade(UnityEngine.UI.Image aImage) {
		return aImage.color.a;
	}

	int GetHitIndex() {

		/*for(int i = 0; i < mArea.Count; i++) {
			BoxCollider lArea = mArea[i];
			if(Physics.OverlapBox(lArea.bounds.center, lArea.bounds.size / 2.0f).Select(x => x.gameObject).Contains(mPlayer)) {
				return i;
			}
		}*/

        for (int i = 0; i < imgSet.Count; i++) {
            BoxCollider lArea = imgSet[i].boxCol;
            if (Physics.OverlapBox(lArea.bounds.center, lArea.bounds.size / 2.0f).Select(x => x.gameObject).Contains(mPlayer)) {
                return i;
            }
        }

		return -1;

	}
}
