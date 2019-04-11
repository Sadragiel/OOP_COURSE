using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.Gamers;


public class Game
{
    //Static information
    public static int StartCardCount;
    public static int TurnTime;
    static Game()
    {
        StartCardCount = 4;
        TurnTime = 30;
    }
    //Non-static
    public List<Card> Deck;
    public Game()
    {
        Deck = GetDeck();
    }
    //TODO Create valid deck
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
    //Data from inspector
    public Transform[] Hands;
    public Transform DiscerdedPlace;
    public Transform DeckPlace;
    public GameObject CardPref;
    public Button EndFirstStepOfTurnBTN;
    public TextMeshProUGUI Timer;

    //LocalData
    public Game CurrentGame;
    public List<Gamer> Gamers;

    int NumberOfCurrentPlayer;
    int NumberOfCardToGet = 1;

    //Properties
    public int CurrentPlayerIndex
    {
        get => NumberOfCurrentPlayer % Gamers.Count;
    }
    public bool IsPlayerTurn
    {
        get => CurrentPlayerIndex == 0;
    }
    public Gamer CurrentPlayer
    {
        get => Gamers[CurrentPlayerIndex];
    }
    public List<Card> Deck
    {
        get => CurrentGame.Deck;
    }

    //Singltone
    public static GameManagerSrc Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        if (Hands.Length < 2)
            throw new System.Exception("No players exception");
        CurrentGame = new Game();
        NumberOfCurrentPlayer = 0;
        //Players
        Gamers = new List<Gamer>()
        {
            new Player(Hands[0], this)
        };
        for (int i = 1; i < Hands.Length; i++)
        {
            Gamers.Add(new Enemy(Hands[i], this));
        }
        foreach (Gamer gamer in Gamers)
        {
            GetStartCards(gamer);
        }
        EndFirstStepOfTurnBTN.interactable = true;

        CurrentPlayer.Turn();
    }

    void GetStartCards(Gamer gamer)
    {
        for (int i = 0; i < Game.StartCardCount; i++)
            gamer.GetCardToHand(Deck);
    }

    public GameObject CreateCard(Card card, Transform Hand)
    {
        GameObject cardGO = Instantiate(CardPref, Hand, false);
        CardControl cardC = cardGO.GetComponent<CardControl>();
        cardC.Init(card, true);
        return cardGO;
    }

    public void SwitchBTN()
    {
        EndFirstStepOfTurnBTN.interactable = !EndFirstStepOfTurnBTN.interactable;
    }

    public void SetTimerValue(int value)
    {
        Timer.text = value.ToString();
    }

    //Comands
    public void SkipTurn()
    {
        print("SkipTurn");
        NumberOfCardToGet--;
        ChangeTurn();
    }
    public void Attack()
    {
        print("Attack");
        NumberOfCardToGet = 0;
        ChangeTurn();
        NumberOfCardToGet = 2;
    }
    public void Shuffle()
    {
        print("Shuffle");
    }
    public void Check()
    {
        print("Check");
    }


    public void ChangeTurn()
    {
        if (NumberOfCardToGet < 0)
            NumberOfCardToGet = 1;
        print(string.Format("GameManager report: Ходил Игрок номер {0}; Количество карт к взятию {1}", CurrentPlayerIndex, NumberOfCardToGet));
        StopAllCoroutines();
        while(NumberOfCardToGet-- > 0)
            CurrentPlayer.GetCardToHand(CurrentGame.Deck);
        NumberOfCurrentPlayer++;
        NumberOfCardToGet = 1;
        SwitchBTN();
        CurrentPlayer.Turn();
    }
}
