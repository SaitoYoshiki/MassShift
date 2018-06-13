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
        foreach (var i in vp) {

        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
