using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeLoadingImage : MonoBehaviour {
    public Sprite anim1;
    public Sprite anim2;
    public Sprite anim3;
    public Sprite anim4;

    Image loadingImage;
    public Text text;
	
    void Awake(){
        loadingImage = GetComponent<Image>();
    }

	public void ChangeImage(int _timeStage){
        switch (_timeStage) {
            case 0:
                loadingImage.sprite = anim1;
                break;
            case 1:
                loadingImage.sprite = anim2;
                break;
            case 2:
                loadingImage.sprite = anim3;
                break;
            case 3:
                loadingImage.sprite = anim4;
                break;

            default:
                break;
        }
    }

    public void ChangeText(float _progless) {
        //text.text = _progless.ToString();
    }
}
