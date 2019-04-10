using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardInfo : MonoBehaviour
{
    public Card SelfCard;
    public Image Logo;
    public TextMeshProUGUI Name;
    public GameObject HideObj;

    public void HideCardInfo(bool f)
    {
        HideObj.SetActive(f);
    }
    public void ShowCardInfo(Card card)
    {
        HideObj.SetActive(false);

        Logo.sprite = card.Logo;
        Logo.preserveAspect = true;
        Name.text = card.Name;
    }
}
