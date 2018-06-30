using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionCheck : MonoBehaviour {
    public enum Resolution {
        LARGE,
        MIDDLE,
        SMALL,
        OTHER
    }

    [SerializeField]
    GameObject LargeResolutionButton;
    [SerializeField]
    GameObject MiddleResolutionButton;
    [SerializeField]
    GameObject SmallResolutionButton;

    UnityEngine.UI.Button largeButton;
    UnityEngine.UI.Button middleButton;
    UnityEngine.UI.Button smallButton;

    Resolution res = Resolution.OTHER;
    int oldWidth;
    UnityEngine.UI.Button deactiveButton;
    SyncChangeColor deactiveSync;

	// Use this for initialization
	void Start () {
        largeButton = LargeResolutionButton.GetComponent<UnityEngine.UI.Button>();
        middleButton = MiddleResolutionButton.GetComponent<UnityEngine.UI.Button>();
        smallButton = SmallResolutionButton.GetComponent<UnityEngine.UI.Button>();

        ChangeDeactive();
	}
	
	// Update is called once per frame
	void Update () {
        if (Screen.width == oldWidth) {
            return;
        }
        else {
            deactiveButton.enabled = true;
            deactiveSync.enabled = true;
            ChangeDeactive();
        }
	}

    void ChangeDeactive() {
        switch (Screen.width) {
            case 1920:
                res = Resolution.LARGE;
                
                deactiveButton = largeButton;
                largeButton.enabled = false;

                deactiveSync = LargeResolutionButton.GetComponent<SyncChangeColor>();
                deactiveSync.enabled = false;
                break;

            case 1600:
                res = Resolution.MIDDLE;
                
                deactiveButton = middleButton;
                middleButton.enabled = false;
                
                deactiveSync = MiddleResolutionButton.GetComponent<SyncChangeColor>();
                deactiveSync.enabled = false;
                break;

            case 1280:
                res = Resolution.SMALL;
                
                deactiveButton = smallButton;
                smallButton.enabled = false;
                
                deactiveSync = SmallResolutionButton.GetComponent<SyncChangeColor>();
                deactiveSync.enabled = false;
                break;

            default:
                break;
        }

        oldWidth = Screen.width;
    }
}
