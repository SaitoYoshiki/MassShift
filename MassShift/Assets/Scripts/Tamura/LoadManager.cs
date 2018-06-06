using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour {
    [SerializeField]
    Camera gameCamera;
    GameObject gameDirectionalLight;
    GameObject uiObject;

    void Start() {
        gameDirectionalLight = GameObject.Find("GameDirectionalLight");

        gameCamera.enabled = false;
        gameDirectionalLight.SetActive(false);

        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void OnSceneUnloaded(Scene i_unloadedScene) {
        Debug.Log("ActiveSceneName:" + SceneManager.GetActiveScene().name);
        gameCamera.enabled = true;
        gameDirectionalLight.SetActive(true);
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
}
