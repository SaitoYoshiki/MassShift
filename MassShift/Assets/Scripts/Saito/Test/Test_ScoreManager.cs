using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_ScoreManager : MonoBehaviour {

	ScoreManager mScoreManager;

	// Use this for initialization
	void Start () {

		mScoreManager = FindObjectOfType<ScoreManager>();

		Debug.Log("Score1Times:" + mScoreManager.Score1Times());
		Debug.Log("Score2Times:" + mScoreManager.Score2Times());
		Debug.Log("Score3Times:" + mScoreManager.Score3Times());

		Debug.Log("TutorialScore3Times:" + mScoreManager.Score3Times(0, 1));
		Debug.Log("1-1Score3Times:" + mScoreManager.Score3Times(1, 1));
		Debug.Log("1-5Score3Times:" + mScoreManager.Score3Times(1, 5));
		Debug.Log("2-1Score3Times:" + mScoreManager.Score3Times(2, 1));
		Debug.Log("2-5Score3Times:" + mScoreManager.Score3Times(2, 5));
		Debug.Log("3-1Score3Times:" + mScoreManager.Score3Times(3, 1));
		Debug.Log("3-5Score3Times:" + mScoreManager.Score3Times(3, 5));
	}

	private void Update() {
		if(Input.GetKeyDown(KeyCode.Q)) {
			Debug.Log("Score1Times:" + mScoreManager.Score1Times());
			Debug.Log("Score2Times:" + mScoreManager.Score2Times());
			Debug.Log("Score3Times:" + mScoreManager.Score3Times());
			Debug.Log("Score:" + mScoreManager.Score());
		}
	}
}
