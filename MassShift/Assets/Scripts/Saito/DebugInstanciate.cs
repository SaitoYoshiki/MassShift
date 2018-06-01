using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugInstanciate {

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void Init() {
		Instantiate();
		//DisableLog();
	}

	static void Instantiate() {
		var g = Resources.Load("DebugInstatiate") as GameObject;
		var gi = GameObject.Instantiate(g);
		GameObject.DontDestroyOnLoad(gi);
	}

	static void DisableLog() {
		Debug.logger.logEnabled = false;
	}
}
