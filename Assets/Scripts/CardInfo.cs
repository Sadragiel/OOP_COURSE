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

    //TODO Bad function
    public void ShowCardInfo(Card card, bool flag)
    {
        this.SelfCard = card;
        this.Logo.sprite = flag ? card.Logo : null;
        this.Logo.preserveAspect = flag;
        this.Name.text = flag ? card.Name : null;
    }
}
