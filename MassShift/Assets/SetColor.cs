using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetColor : MonoBehaviour {
    [SerializeField]
    List<Image> starImgList;
	
	public void SetGrayColor(){
        foreach (var img in starImgList) {
            if (img != null) {
                img.color = Color.gray;
            }
        }
    }

    public void SetWhiteColor() {
        foreach (var img in starImgList) {
            if (img != null) {
                img.color = Color.white;
            }
        }
    }
}
