using Assets.Scripts.CardEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Deck
{
    public class Deck
    {
        //Constances
        const string AttackName = "ATTACK";
        const string SkipName = "SKIP";
        const string ShuffleName = "SHUFFLE";
        const string CheckName = "CHECK";
        const string ExplosionName = "EXPLOSION";
        const string NeutralizationName = "NEUTRALIZATION";

        int NumOfPlayers;
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


        public Deck(int NumOfPlayers)
        {
            this.NumOfPlayers = NumOfPlayers;
            NextCards = new List<CardType>();
            Creator = new CardCreator();
            CardLists = new Dictionary<CardType, CardListInfo>() {
                { CardType.ATTACK,
                    new CardListInfo(Creator.Create(AttackName, "Sprites/Jeanne", true, new Attack()), 5)
                },
                { CardType.SKIP,
                    new CardListInfo(Creator.Create(SkipName, "Sprites/Atalanta", true, new SkipTurn()), 5)
                },
                { CardType.SHUFFLE,
                    new CardListInfo(Creator.Create(ShuffleName, "Sprites/Kiyohime", true, new Shuffle()), 5)
                },
                { CardType.CHECK,
                    new CardListInfo(Creator.Create(CheckName, "Sprites/Mash", true, new Check()), 5)
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
            for (int i = NextCards.Count; i < Math.Min(numOfCards, Remained()); i++)
            {
                bool wasFound = false;
                while (!wasFound)
                {
                    CardType type = GetRandomType();
                    int numOfCardsOfThatType = 0;
                    foreach (CardType t in NextCards)
                    {
                        if (t == type)
                            numOfCardsOfThatType++;
                    }
                    if (CardLists[type].CardRemained > numOfCardsOfThatType)
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
                    new CardListInfo(Creator.Create(ExplosionName, "Sprites/Hassaan", false, null, new Explosion()), NumOfPlayers - 1));
            CardLists.Add(CardType.NEUTRALIZATION,
                    new CardListInfo(Creator.Create(NeutralizationName, "Sprites/Merlin", false, null, new Neutralization()), 8));
        }

        CardType GetRandomType()
        {
            System.Random _Random = new System.Random(Environment.TickCount);
            Array enumValues = Enum.GetValues(typeof(CardType));
            return (CardType)enumValues.GetValue(_Random.Next(enumValues.Length));
        }

        public Card GetCard()
        {

            if (this.Remained() == 0)
            {
                return null;
            }
            Card card = null;
            if (NextCards.Count != 0)
            {
                card = GetCard(NextCards[0]);
                NextCards.RemoveAt(0);
            }
            while (card == null)
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
                    if (type != CardType.EXPLOSION)
                        CardList.CardRemained--;
                    Card resultCard = CardList.ExistincCards.Count != 0 ? CardList.ExistincCards.Pop() : CardList.CardTemplates.Clone() as Card;
                    CardLists[type] = CardList; // CardList это просто копия CardLists[type], поэтому синхронизируем изменения
                    return resultCard;
                }
            }
            catch (Exception) { }
            return null;
        }

        public Card GetCardWithoutLosing(CardType type)
        {
            try
            {
                CardListInfo CardList = CardLists[type]; // До добавляния карт Взрыва и Обезвреживания может возникнуть исключение

                if (CardList.CardRemained > 0)
                {
                    return CardList.ExistincCards.Count != 0 ? CardList.ExistincCards.Pop() : CardList.CardTemplates.Clone() as Card;
                }
            }
            catch (Exception) { }
            return null;
        }

        public CardType GetType(Card card)
        {
            return card.Name == AttackName ? CardType.ATTACK
                : card.Name == SkipName ? CardType.SKIP
                : card.Name == ShuffleName ? CardType.SHUFFLE
                : card.Name == CheckName ? CardType.CHECK
                : card.Name == ExplosionName ? CardType.EXPLOSION
                : CardType.NEUTRALIZATION;
        }

        public string GetRusName(CardType type)
        {
            switch (type)
            {
                case CardType.ATTACK:
                    {
                        return "Атака";
                    }
                case CardType.CHECK:
                    {
                        return "Проверка Верхних Карт";
                    }
                case CardType.EXPLOSION:
                    {
                        return "Взрыв";
                    }
                case CardType.NEUTRALIZATION:
                    {
                        return "Нейтрализация";
                    }
                case CardType.SHUFFLE:
                    {
                        return "Перемешивание Колоды";
                    }
                case CardType.SKIP:
                    {
                        return "Пропуск Хода";
                    }
            }
            return "";
        }

        public void ReturnToTheDeck(GameObject cardGO)
        {
            Card card = cardGO.GetComponent<CardControl>().Card;
            GameManagerSrc.Instance.AddToDeckStorage(cardGO);
            CardType type = GetType(card);
            CardListInfo cl = CardLists[type];
            cl.ExistincCards.Push(card);
            CardLists[type] = cl;
        }

        public void WasExplosion()
        {
            CardListInfo cl = this.CardLists[CardType.EXPLOSION];
            cl.CardRemained--;
            this.CardLists[CardType.EXPLOSION] = cl;
        }
    }
}
