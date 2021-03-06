﻿using System.Collections;
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
    public Game(int numOfPlayers)
    {
         Deck = new Deck(numOfPlayers);
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

    public int NumberOfCardToGetByMe
    {
        get => NumberOfCardToGet;
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
        PhraseEnvoicer.InitSystem();
        PhraseEnvoicer.PlayText("Игра начинается! Раздаем Карты");
        StartCoroutine(StartManipulation());
    }

    IEnumerator StartManipulation()
    {
        EndFirstStepOfTurnBTN.interactable = false;
        if (Hands.Length < 2)
            throw new System.Exception("No players exception");
        CurrentGame = new Game(Hands.Length);
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
        PhraseEnvoicer.PlayText("Раздаем карты нейтрализации");
        foreach (Gamer gamer in Gamers)
        {
            yield return gamer.GetCardToHand(Deck.GetCard(Deck.CardType.NEUTRALIZATION));
        }
        EndFirstStepOfTurnBTN.interactable = true;

        CurrentPlayer.Turn();
    }

    public GameObject CreateCard(Card card, Transform Hand)
    {
        GameObject cardGO = CreateCardForPlace(card, DeckStorage);
        cardGO.GetComponent<CardControl>().Movement.GettingProcess(Hand);
        return cardGO;
    }

    private GameObject CreateCardForPlace(Card card, Transform Place)
    {
        GameObject cardGO = GetFromDeckStorage(card);
        if (cardGO == null)
        {
            cardGO = Instantiate(CardPref, Place, false);
        }
        else
        {
            cardGO.transform.SetParent(Place);
        }
        CardControl cardC = cardGO.GetComponent<CardControl>();
        cardC.Init(card, true);
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
        EndFirstStepOfTurnBTN.interactable = IsPlayerTurn;
    }

    public void SetTimerValue(int value)
    {
        Timer.text = value.ToString();
    }

    //Comands
    public void SkipTurn()
    {
        PhraseEnvoicer.PlayText("Пропуск хода");
        NumberOfCardToGet--;
        ChangeTurn();
    }
    public void Attack()
    {
        PhraseEnvoicer.PlayText("Атака");
        NumberOfCardToGet = 0;
        ChangeTurn();
        NumberOfCardToGet = 2;
    }
    public void Shuffle()
    {
        PhraseEnvoicer.PlayText("Перетасовка");
        foreach (Gamer gamer in Gamers)
        {
            try
            {
                (gamer as Enemy).NextCardTypes = new List<Deck.CardType>();
            }
            catch (System.Exception e) { } //NullPointer or FailedCast
        }
        Deck.Shuffle();
    }
    public void Check()
    {
        List<Deck.CardType> nextCards = new List<Deck.CardType>(Deck.CheckForNextCards(3));
        if (IsPlayerTurn)
        {
            string res = "Следующие карты: ";
            for (int i = 0; i < nextCards.Count; i++)
            {
                res += Deck.GetRusName(nextCards[i]) + ", ";
            }
            InfoScr info = InfoScreen.GetComponent<InfoScr>();
            info.SinglMessage(res);
            foreach (Deck.CardType type in nextCards)
                info.SetCard(CreateCardForPlace(Deck.GetCardWithoutLosing(type), info.CardPlace));
            InfoScreen.SetActive(true);
            PhraseEnvoicer.PlayText(res);
        }
        else
        {
            PhraseEnvoicer.PlayText("Проверка");
            (CurrentPlayer as Enemy).NextCardTypes = nextCards;
        }
    }

    public void Neutralization()
    {
        
        InfoScreen.SetActive(false);
        InfoScreen.GetComponent<InfoScr>().SinglMessage("");
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
        
        InfoScr infoScr = InfoScreen.GetComponent<InfoScr>();
        infoScr.ActionButtonActive(CurrentPlayer.HasNeutralization());
        infoScr.SetCard(CurrentPlayer.ExplosionCard);
        if (!IsPlayerTurn)
            infoScr.AcceptButtonActive();
        string res = "Вытянутая карта оказалась взрывом!\n"
            + (IsPlayerTurn ? "У вас " : "У вашего опонента ")
            + (CurrentPlayer.HasNeutralization() ? "есть " : "нет ")
            + "Карты Нейтрализации!";
        infoScr.SetMessage(res);
        InfoScreen.SetActive(true);
        PhraseEnvoicer.PlayText("Взрыв " + (CurrentPlayer.HasNeutralization() ? "" : "не ") + "будет нейтрализован");
    }

    private int NumOfPlayerRemained()
    {
        int res = 0;
        foreach (Gamer gamer in Gamers)
            if (gamer != null)
                res++;
        return res;
    }

    public void CollapsInfoScreen()
    {
        InfoScreen.SetActive(false);
        InfoScreen.GetComponent<InfoScr>().SinglMessage("");
        if (CurrentPlayer.ExplosionCard != null)
        {
            if (CurrentPlayer.HasNeutralization())
                Neutralization();
            else
            {
                Deck.WasExplosion();
                while (CurrentPlayer.Hand.childCount != 0)
                {
                    Deck.ReturnToTheDeck(CurrentPlayer.Hand.GetChild(0).gameObject);
                }
                NumberOfCardToGet = 0;
                Gamers[CurrentPlayerIndex] = null;
                if (NumOfPlayerRemained() == 1)
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
        infoScr.SinglMessage("Победа!\nВозвращайтесь в главное меню чтоб начать новую игру!");
        PhraseEnvoicer.PlayText("Победа!\nВозвращайтесь в главное меню чтоб начать новую игру!");
        infoScr.ActionButtonActive(false);
    }

    private IEnumerator ChangingTurn()
    {
        while (NumberOfCardToGet-- > 0)
        {
            foreach (Gamer gamer in Gamers)
            {
                try
                {
                    (gamer as Enemy).NoticeGettingCard();
                }
                catch (System.Exception e) { } //NullPointer or FailedCast
            }
            yield return CurrentPlayer.GetCardToHand(CurrentGame.Deck);
        }
          
        do
        {
            NumberOfCurrentPlayer++;
        } while (CurrentPlayer == null);
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


    ////DebugLog
    //public void Test(string s)
    //{
    //    print(s);
    //}

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
