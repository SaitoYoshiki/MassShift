using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowAndZoom : MonoBehaviour {

	bool mFollow;

	Vector3 mCameraStartPos;
	float mCameraPosZ;

	Vector3 mCameraOffset;

	const float cCameraMoveSpeed = 4.0f;

	Player mPlayer;

	// Use this for initialization
	void Awake() {
		mFollow = false;
		mCameraStartPos = transform.position;
		mCameraPosZ = mCameraStartPos.z;

		mPlayer = FindObjectOfType<Player>();
	}

	// Update is called once per frame
	void Update() {

		//Lキーでモード切替
		if (Input.GetKeyDown(KeyCode.L)) {
			mFollow = !mFollow;

			//フォローモードでなくなるなら
			if (mFollow == false) {
				transform.position = mCameraStartPos;
			}
		}

		//OPキーで、ズーム切り替え
		if (Input.GetKeyDown(KeyCode.O)) {
			mCameraPosZ -= 5.0f;
		}
		if (Input.GetKeyDown(KeyCode.P)) {
			mCameraPosZ += 5.0f;
		}

		//UHJKキーで、オフセット位置を移動
		if (Input.GetKey(KeyCode.U)) {
			mCameraOffset.y += cCameraMoveSpeed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.J)) {
			mCameraOffset.y -= cCameraMoveSpeed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.H)) {
			mCameraOffset.x -= cCameraMoveSpeed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.K)) {
			mCameraOffset.x += cCameraMoveSpeed * Time.deltaTime;
		}

		if (mFollow) {
			transform.position = GetFollowCameraPosition();
		}
	}

	Vector3 GetFollowCameraPosition() {
		//Vector3 lPos = mPlayer.transform.position;
		Vector3 lPos = Vector3.zero;

		lPos.z = mCameraPosZ;
		lPos += mCameraOffset;

		return lPos;
	}
}
