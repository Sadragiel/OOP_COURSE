﻿using Assets.Scripts.CardEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Deck
{
    public class Deck
    {
        private struct CardListInfo
        {
            public Stack<Card> ExistincCards;    //Flywaight: To Get Card From Discard;
            public int CardRemained;             //To know how many card remained;
            public Card CardTemplates;           //To get new Card by cloning a template;
            public CardListInfo(Card template, int numOfCards)
            {
                ExistincCards = new Stack<Card>();
                CardRemained = numOfCards;
                CardTemplates = template;
            }
        }
        public enum CardType
        {
            EXPLOSION, NEUTRALIZATION, SKIP, ATTACK, SHUFFLE, CHECK
        }
        Dictionary<CardType, CardListInfo> CardLists;
        List<CardType> NextCards;
        CardCreator Creator;
        Random _Random = new Random(Environment.TickCount);

        public Deck(int numOfPlayers)
        {
            NextCards = new List<CardType>();
            Creator = new CardCreator();
            CardLists = new Dictionary<CardType, CardListInfo>() {
                { CardType.ATTACK,
                    new CardListInfo(Creator.Create("ATTACK", "Sprites/Jeanne", true, new Attack()), 4)
                },
                { CardType.SKIP,
                    new CardListInfo(Creator.Create("SKIP", "Sprites/Atalanta", true, new SkipTurn()), 4)
                },
                { CardType.SHUFFLE,
                    new CardListInfo(Creator.Create("SHUFFLE", "Sprites/Kiyohime", true, new Shuffle()), 4)
                },
                { CardType.CHECK,
                    new CardListInfo(Creator.Create("CHECK", "Sprites/Mash", true, new Check()), 4)
                },
            };
        }

        public bool IsEmpty()
        {
            bool res = true;
            foreach (KeyValuePair<CardType, CardListInfo> entry in CardLists)
            {
                res = res && entry.Value.CardRemained <= 0;
                if (!res)
                    return res;
            }
            return res;
        }

        int Remained()
        {
            int res = 0;
            foreach (KeyValuePair<CardType, CardListInfo> entry in CardLists)
            {
                res += entry.Value.CardRemained;
            }
            return res;
        }

        public List<CardType> CheckForNextCards(int numOfCards)
        {
            for (int i = 0; i < Math.Min(numOfCards, Remained()); i++)
            {
                bool wasFound = false;
                while (!wasFound)
                {
                    CardType type = GetRandomType();
                    if (CardLists[type].CardRemained > 0)
                    {
                        NextCards.Add(type);
                        wasFound = true;
                    } 
                }
            }
            return NextCards;
        }

        public void Shuffle()
        {
            while (NextCards.Count != 0)
            {
                NextCards.RemoveAt(0);
            }
        }

        public void FillByExtraCards()
        {
            CardLists.Add(CardType.EXPLOSION,
                    new CardListInfo(Creator.Create("EXPLOSION", "Sprites/Hassaan", false, null, new Explosion()), 4));
            CardLists.Add(CardType.NEUTRALIZATION,
                    new CardListInfo(Creator.Create("NEUTRALIZATION", "Sprites/Merlin", false, null, new Neutralization()), 4));
        }

        CardType GetRandomType()
        {
            Array enumValues = Enum.GetValues(typeof(CardType));
            return (CardType)enumValues.GetValue(_Random.Next(enumValues.Length));
        }

        public Card GetCard()
        {
            Card card = null;
            if (NextCards.Count != 0)
            {
                card = GetCard(NextCards[0]);
                NextCards.RemoveAt(0);
            }
            while(card == null)
                card = GetCard(GetRandomType());
            return card;
        }
        public Card GetCard(CardType type)
        {
            try
            {
                CardListInfo CardList = CardLists[type]; // До добавляния карт Взрыва и Обезвреживания может возникнуть исключение
                
                if (CardList.CardRemained > 0)
                {
                    CardList.CardRemained--;
                    return CardList.ExistincCards.Count != 0 ? CardList.ExistincCards.Pop() : CardList.CardTemplates.Clone() as Card;
                } 
            }
            catch (Exception) {  }
            return null;
        }
    }
}
