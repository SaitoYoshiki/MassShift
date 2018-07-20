using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeStar : MonoBehaviour {
    [SerializeField]
    Sprite starOnB;

    [SerializeField]
    Sprite starOffB;

    [SerializeField]
    Sprite starOnY;

    [SerializeField]
    Sprite starOffY;

    [SerializeField]
    List<Image> Stars;

    [SerializeField]
    Image Bar;

    public void ChangeStarColor(int _score) {
        Bar.enabled = true;

        switch (_score) {
            case 1:
                Stars[0].sprite = starOffY;
                Stars[1].sprite = starOffY;
                Stars[2].sprite = starOnB;
                break;

            case 2:
                Stars[0].sprite = starOffY;
                Stars[1].sprite = starOnY;
                Stars[2].sprite = starOnB;
                break;

            case 3:
                Stars[0].sprite = starOnY;
                Stars[1].sprite = starOnY;
                Stars[2].sprite = starOnB;
                break;

            default:
                Stars[0].sprite = starOffY;
                Stars[1].sprite = starOffY;
                Stars[2].sprite = starOffB;
                break;
        }
    }

    public void ChangeStarColorEX(int _score) {
        Bar.enabled = false;

        switch (_score) {
            case 1:
                Stars[0].sprite = starOffY;
                Stars[1].sprite = starOffY;
                Stars[2].sprite = starOnY;
                break;

            case 2:
                Stars[0].sprite = starOffY;
                Stars[1].sprite = starOnY;
                Stars[2].sprite = starOnY;
                break;

            case 3:
                Stars[0].sprite = starOnY;
                Stars[1].sprite = starOnY;
                Stars[2].sprite = starOnY;
                break;

            default:
                break;
        }
    }
}
