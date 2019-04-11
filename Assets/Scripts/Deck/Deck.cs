using Assets.Scripts.CardEffects;
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
            Creator = new CardCreator();
            CardLists = new Dictionary<CardType, CardListInfo>() {
                { CardType.ATTACK,
                    new CardListInfo(Creator.Create("ATTACK", "Sprites/Jeanne", new Attack()), 4)
                },
                { CardType.SKIP,
                    new CardListInfo(Creator.Create("SKIP", "Sprites/Atalanta", new SkipTurn()), 4)
                },
                { CardType.SHUFFLE,
                    new CardListInfo(Creator.Create("SHUFFLE", "Sprites/Kiyohime", new Shuffle()), 4)
                },
                { CardType.CHECK,
                    new CardListInfo(Creator.Create("CHECK", "Sprites/Mash", new Check()), 4)
                },
            };
        }

        public bool IsEmpty()
        {
            bool res = true;
            foreach (KeyValuePair<CardType, CardListInfo> entry in CardLists)
            {
                GameManagerSrc.Instance.Test("res is " + res.ToString());
                res = res && entry.Value.CardRemained <= 0;
                if (!res)
                    return res;
            }

            return res;
        }

        public void FillByExtraCards()
        {
            CardLists.Add(CardType.EXPLOSION,
                    new CardListInfo(Creator.Create("EXPLOSION", "Sprites/Hassaan", new Explosion()), 4));
            CardLists.Add(CardType.NEUTRALIZATION,
                    new CardListInfo(Creator.Create("NEUTRALIZATION", "Sprites/Merlin", new Neutralization()), 4));
        }

        CardType GetRandomType()
        {
            Array enumValues = Enum.GetValues(typeof(CardType));
            return (CardType)enumValues.GetValue(_Random.Next(enumValues.Length));
        }

        void print(CardType type)
        {
            switch (type)
            {
                case CardType.ATTACK: GameManagerSrc.Instance.Test("Type is ATTACK"); break;
                case CardType.EXPLOSION:
                    GameManagerSrc.Instance.Test("Type is EXPLOSION");
                    break;
                case CardType.NEUTRALIZATION:
                    GameManagerSrc.Instance.Test("Type is NEUTRALIZATION");
                    break;
                case CardType.SHUFFLE:
                    GameManagerSrc.Instance.Test("Type is SHUFFLE");
                    break;
                case CardType.SKIP:
                    GameManagerSrc.Instance.Test("Type is SKIP");
                    break;
                case CardType.CHECK:
                    GameManagerSrc.Instance.Test("Type is CHECK");
                    break;
            }
        }

        public Card GetCard()
        {
            Card card = null;
            while(card == null)
                card = GetCard(GetRandomType());
            return card;
        }
        public Card GetCard(CardType type)
        {
            print(type);
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
