using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardInfo : MonoBehaviour
{
    public CardControl CC;
    public Card SelfCard;
    public Image Logo;
    public TextMeshProUGUI Name;
    public GameObject HideObj;

    public void HideCardInfo()
    {
        HideObj.SetActive(true);
    }
    public void ShowCardInfo()
    {
        HideObj.SetActive(false);
        Logo.sprite = CC.Card.Logo;
        Logo.preserveAspect = true;
        Name.text = CC.Card.Name;
    }
}
