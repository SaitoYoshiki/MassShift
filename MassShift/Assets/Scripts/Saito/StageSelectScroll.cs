using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StageSelectScroll : MonoBehaviour {

	[SerializeField]
	List<GameObject> mAreaTriggers;

	public List<GameObject> mAreaCameraPosition;

	[SerializeField]
	GameObject mPlayer;

	[SerializeField]
	GameObject mCamera;

	[SerializeField]
	float mCameraMoveSpeed = 10.0f;

	[SerializeField]
	GameObject mLeftWall;   //エリア1で、左側に戻れないようにする壁

	//スクロールするかどうか
	public bool mIsScroll = false;

	int mAreaIndex = -1;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if(mIsScroll == false) {
			return;
		}

		//現在いるエリア位置を更新
		if (GetHitAreaIndex() != -1) {
			mAreaIndex = GetHitAreaIndex();
		}

		//カメラを移動
		if (mAreaIndex != -1) {
			mLeftWall.SetActive(true);
			MoveCameraTo(mAreaCameraPosition[mAreaIndex].transform.position);
		}
	}

	int GetHitAreaIndex() {

		for(int i = 0; i < mAreaTriggers.Count; i++) {
			GameObject lAreaTrigger = mAreaTriggers[i];

			//各エリアのトリガーが持つ、全てのボックスコライダーと判定を行う
			foreach(var lC in lAreaTrigger.GetComponentsInChildren<BoxCollider>()) {

				//もしプレイヤーがヒットしていたら
				if(Physics.OverlapBox(lC.bounds.center, lC.bounds.size / 2.0f).Select(x => x.gameObject).Contains(mPlayer)) {
					return i;
				}
			}
		}

		return -1;	//ヒットするエリアがなかった
	}

	void MoveCameraTo(Vector3 aPosition) {

		Vector3 lDir = aPosition - mCamera.transform.position;

		float lMoveDistance = Mathf.Clamp(mCameraMoveSpeed * Time.fixedDeltaTime, 0.0f, lDir.magnitude);
		Vector3 lMove = lDir.normalized * lMoveDistance;

		mCamera.transform.position += lMove;
	}

}
