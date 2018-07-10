using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeLoadingImage : MonoBehaviour {
    public Sprite stop;
    public Sprite left;
    public Sprite right;

    Image loadingImage;
    public Text text;
	
    void Start(){
        loadingImage = GetComponent<Image>();
    }

	public void ChangeImage(int _timeStage){
        switch (_timeStage) {
            case 0:
                loadingImage.sprite = stop;
                break;
            case 1:
                loadingImage.sprite = left;
                break;
            case 2:
                loadingImage.sprite = stop;
                break;
            case 3:
                loadingImage.sprite = right;
                break;

            default:
                break;
        }
    }

    public void ChangeText(float _progless) {
        text.text = _progless.ToString();
    }
}
