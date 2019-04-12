using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.Gamers;
using Assets.Scripts.Deck;

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
    public Deck Deck;
    public Game()
    {
         Deck = new Deck(2);
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
    public Deck Deck
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

        StartCardManipulation();
        
        EndFirstStepOfTurnBTN.interactable = true;

        CurrentPlayer.Turn();
    }

    void StartCardManipulation()
    {
        foreach (Gamer gamer in Gamers)
        {
            for (int i = 0; i < Game.StartCardCount; i++)
                gamer.GetCardToHand(Deck);
        }
        Deck.FillByExtraCards();
        foreach (Gamer gamer in Gamers)
        {
            gamer.GetCardToHand(Deck.GetCard(Deck.CardType.NEUTRALIZATION));
        }
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
        Deck.Shuffle();
    }
    public void Check()
    {
        List<Deck.CardType> nextCards = Deck.CheckForNextCards(3);
        string res = "Слудующие карты: ";
        for (int i = 0; i < nextCards.Count; i++)
        {
            res += nextCards[i].ToString() + ", ";
        }
        print(res);
    }
    public void Neutralization()
    {
        print("Neutralization");
    }
    public void Explosion()
    {
        print("Explosion");
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


    //DebugLog
    public void Test(string s)
    {
        print(s);
    }
}
