using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InfoScr : MonoBehaviour
{
    public TextMeshProUGUI Message;
    public TextMeshProUGUI BTN_Text;
    public TextMeshProUGUI ABTN_Text;
    public Button Action;
    public Button AlternativeAction;
    public Button Accept;
    public Transform CardPlace;

    public void SetCard(GameObject CardGO)
    {
        
        CardGO.transform.SetPositionAndRotation(CardGO.transform.position, CardPlace.rotation);
        CardGO.GetComponent<CardInfo>().ShowCardInfo();
        CardGO.transform.SetParent(CardPlace);
    }

    public void SetMessage(string message)
    {
        this.Message.text = message;
    }

    public void ActionButtonActive(bool flag)
    {
        Action.gameObject.SetActive(flag);
        AlternativeAction.gameObject.SetActive(!flag);
        Accept.gameObject.SetActive(false);

    }

    public void AcceptButtonActive()
    {
        Action.gameObject.SetActive(false);
        AlternativeAction.gameObject.SetActive(false);
        Accept.gameObject.SetActive(true);
    }

    public void SinglMessage(string message)
    {
        while (CardPlace.childCount != 0)
            GameManagerSrc.Instance.Deck.ReturnToTheDeck(CardPlace.GetChild(0).gameObject);
        AcceptButtonActive();
        SetMessage(message);
    }
}

