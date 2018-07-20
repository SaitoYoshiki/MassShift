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
    GameObject Bar;

    [SerializeField]
    GameObject ExBar;

    public void ChangeStarColor(int _score) {
        Bar.SetActive(true);
        ExBar.SetActive(false);

        switch (_score) {
            case 1:
                Stars[0].sprite = starOffB;
                Stars[1].sprite = starOffY;
                Stars[2].sprite = starOnY;
                break;

            case 2:
                Stars[0].sprite = starOffB;
                Stars[1].sprite = starOnY;
                Stars[2].sprite = starOnY;
                break;

            case 3:
                Stars[0].sprite = starOnB;
                Stars[1].sprite = starOnY;
                Stars[2].sprite = starOnY;
                break;

            default:
                Stars[0].sprite = starOffB;
                Stars[1].sprite = starOffY;
                Stars[2].sprite = starOffY;
                break;
        }
    }

    public void ChangeStarColorEX(int _score) {
        Bar.SetActive(false);
        ExBar.SetActive(true);

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
                Stars[0].sprite = starOffY;
                Stars[1].sprite = starOffY;
                Stars[2].sprite = starOffY;
                break;
        }
    }
}
