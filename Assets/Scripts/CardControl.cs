using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardControl : MonoBehaviour
{
    public Card Card;
    public CardInfo Info;
    public CardMovement Movement;
    GameManagerSrc gameManager = GameManagerSrc.Instance;
    bool IsPlayerCard;

    public void Init(Card card, bool isPlayerCard)
    {
        if (card == null)
            print("NULL");
        Card = card;
        IsPlayerCard = isPlayerCard;

        if (isPlayerCard)
        {
            Info.ShowCardInfo();
        }
        else
            Info.HideCardInfo();
    }

}
