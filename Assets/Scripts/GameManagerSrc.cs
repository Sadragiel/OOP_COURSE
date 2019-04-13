using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.Gamers;
using Assets.Scripts.Deck;
using UnityEngine.SceneManagement;

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
    public GameObject InfoScreen;
    public Transform DeckStorage;

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

        StartCoroutine(StartManipulation());
    }

    IEnumerator StartManipulation()
    {
        EndFirstStepOfTurnBTN.interactable = false;
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
            for (int i = 0; i < Game.StartCardCount; i++)
                yield return gamer.GetCardToHand(Deck);

        }
        Deck.FillByExtraCards();
        foreach (Gamer gamer in Gamers)
        {
            yield return gamer.GetCardToHand(Deck.GetCard(Deck.CardType.NEUTRALIZATION));
        }
        EndFirstStepOfTurnBTN.interactable = true;

        CurrentPlayer.Turn();
    }

    public GameObject CreateCard(Card card, Transform Hand)
    {
        print("GameStorage length = " + DeckStorage.childCount);
        GameObject cardGO = GetFromDeckStorage(card);
        if (cardGO == null)
        {
            cardGO = Instantiate(CardPref, DeckPlace, false);
        }
        else
        {
            cardGO.transform.SetParent(DeckPlace);
        }
        CardControl cardC = cardGO.GetComponent<CardControl>();
        cardC.Init(card, true);
        cardC.Movement.GettingProcess(Hand);

        return cardGO;
    }

    public void AddToDeckStorage(GameObject CardGO)
    {
        CardGO.transform.SetParent(DeckStorage);
    }

    public GameObject GetFromDeckStorage(Card card)
    {
        foreach (Transform child in DeckStorage)
        {
            if (child.gameObject.GetComponent<CardControl>().Card.Name == card.Name)
            {
                return child.gameObject;
            }
        }
        return null;
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
        string res = "Next Cards: ";
        for (int i = 0; i < nextCards.Count; i++)
        {
            res += nextCards[i].ToString() + ", ";
        }
        print(res);
        if (IsPlayerTurn)
        {
            InfoScreen.SetActive(true);
            InfoScreen.GetComponent<InfoScr>().SinglMessage(res);
        }
        else
        {
            (CurrentPlayer as Enemy).Types = nextCards;
        }
    }

    public void Neutralization()
    {
        InfoScreen.SetActive(false);
        GameObject usedCard = CurrentPlayer.NeutralizationCards[0];
        CurrentPlayer.NeutralizationCards.RemoveAt(0);
        Deck.ReturnToTheDeck(usedCard);
        GameObject explosionCard = CurrentPlayer.ExplosionCard;
        CurrentPlayer.ExplosionCard = null;
        Deck.ReturnToTheDeck(explosionCard);
        ChangeTurn();
    }
    public void Explosion()
    {
        StopAllCoroutines();
        InfoScreen.SetActive(true);
        InfoScr infoScr = InfoScreen.GetComponent<InfoScr>();
        infoScr.ActionButtonActive(CurrentPlayer.HasNeutralization());
        infoScr.SetCard(CurrentPlayer.ExplosionCard);
        if (!IsPlayerTurn)
            infoScr.AcceptButtonActive();
        infoScr.SetMessage(
            "The bomb was pulled out of the deck!\n"
            +(IsPlayerTurn ? "You have " : "Your Opponent has ")
            +(CurrentPlayer.HasNeutralization() ? "" : "not ")
            + "Neutralization Card"
            );

    }

    public void CollapsInfoScreen()
    {
        InfoScreen.SetActive(false);
        if (CurrentPlayer.ExplosionCard != null)
        {
            if (CurrentPlayer.HasNeutralization())
                Neutralization();
            else
            {
                foreach (Transform child in CurrentPlayer.Hand)
                {
                    Deck.ReturnToTheDeck(child.gameObject);
                }
                Gamers.RemoveAt(CurrentPlayerIndex);
                if (Gamers.Count == 1)
                {
                    Win();
                    return;
                }
                ChangeTurn();
            }
        }
    }

    public void Win()
    {
        InfoScreen.SetActive(true);
        InfoScr infoScr = InfoScreen.GetComponent<InfoScr>();
        infoScr.SinglMessage("You Win!\nReturn to Menu");
        infoScr.ActionButtonActive(false);
    }

    private IEnumerator ChangingTurn()
    {
        while (NumberOfCardToGet-- > 0)
            yield return CurrentPlayer.GetCardToHand(CurrentGame.Deck);
        NumberOfCurrentPlayer++;
        NumberOfCardToGet = 1;
        SwitchBTN();
        CurrentPlayer.Turn();
    }

    public void ChangeTurn()
    {
        if (NumberOfCardToGet < 0)
            NumberOfCardToGet = 1;
        StopAllCoroutines();
        StartCoroutine(ChangingTurn());
    }


    //DebugLog
    public void Test(string s)
    {
        print(s);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
    }
}
