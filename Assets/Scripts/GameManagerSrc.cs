using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Game
{
    public List<Card> FrontEnemyHand,
        PlayerHand,
        Deck,
        Discarded;
    public Game()
    {
        Deck = GetDeck();
        FrontEnemyHand = new List<Card>();
        PlayerHand = new List<Card>();
        Discarded = new List<Card>();
    }

    List<Card> GetDeck()
    {
        List<Card> list = new List<Card>();
        for (int i = 0; i < 20; i++)
            list.Add(CardStorage.AllCards[Random.Range(0, CardStorage.AllCards.Count)]);
        return list;
    }
}


public class GameManagerSrc : MonoBehaviour
{
    public Game CurrentGame;
    public Transform
        PlayerHand,
        FrontEnemyHand;
    public GameObject CardPref;
    public int StartCardCount;
    public int Turn;
    public Button EndFirstStepOfTurnBTN;
    public int TurnTime;
    public TextMeshProUGUI TimeCount;
    public bool IsPlayerTurn
    {
        get => Turn % 2 == 0;
    }
    void Start()
    {
        CurrentGame = new Game();
        Turn = 0;
        StartCardCount = 4;
        GetStartCards(CurrentGame.Deck, PlayerHand);
        GetStartCards(CurrentGame.Deck, FrontEnemyHand);
        StartCoroutine(TurnFunc());
    }

    void GetStartCards(List<Card> Deck, Transform Hand)
    {
        for (int i = 0; i < StartCardCount; i++)
            GetCardToHand(Deck, Hand);
    }

    void GetCardToHand(List<Card> Deck, Transform Hand)
    {
        if (Deck.Count == 0)
            return;
        Card card = Deck[0];
        GameObject cardGO = Instantiate(CardPref, Hand, false);
        cardGO.GetComponent<CardInfo>().ShowCardInfo(card, Hand == PlayerHand);
        Deck.RemoveAt(0);
    }

    IEnumerator TurnFunc()
    {
        print("Coroutine: Turn is " + Turn.ToString());
        TurnTime = 30;
        TimeCount.text = TurnTime.ToString();
        if (IsPlayerTurn)
        {
            while (TurnTime-- > 0)
            {
                TimeCount.text = TurnTime.ToString();
                yield return new WaitForSeconds(1);
            }
        }
        else
        {
            while (TurnTime-- > 27)
            {
                TimeCount.text = TurnTime.ToString();
                yield return new WaitForSeconds(1);
            }
        }
        EndFirstStepOfTurn();
    }

    public void EndFirstStepOfTurn()
    {
        StopAllCoroutines();
        GetCardToHand(CurrentGame.Deck, IsPlayerTurn ? PlayerHand : FrontEnemyHand);
        print("EndFirstStepOfTurn: Turn is " + Turn.ToString());
        Turn++;
        EndFirstStepOfTurnBTN.interactable = IsPlayerTurn;
        StartCoroutine(TurnFunc());
    }
}
