using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MovieStopper : MonoBehaviour {
    [SerializeField]
    List<VideoPlayer> vp;

    Pause pause;

	// Use this for initialization
	void Start () {
        pause = FindObjectOfType<Pause>();
        pause.pauseEvent.AddListener(PauseVideo);
	}
	
	// Update is called once per frame
	void Update () {

	}

    void PauseVideo() {
        foreach (VideoPlayer video in vp) {
            if (pause.pauseFlg) {
                video.Pause();
            }
            else {
                video.Play();
            }
        }
    }
}
