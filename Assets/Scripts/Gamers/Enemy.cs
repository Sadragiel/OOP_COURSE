using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Gamers.EnemyThinkingBlock;

namespace Assets.Scripts.Gamers
{



    //First Block of NormalState, that can change State of EnemyPlayer
    public class CheckingBlockOfChain : ThinkingBlockOfChain
    {
        public CheckingBlockOfChain(Deck.Deck.CardType type, Enemy self) : base(type, self)
        {
            if (type != Deck.Deck.CardType.CHECK)
                throw new System.Exception("Type Conflict! To Create CheckingBlockOfChain used " + type.ToString() + " type");
        }

        bool hasExplosion(int numOfCards)
        {
            for (int i = 0; i < numOfCards && i < this.self.NextCardTypes.Count; i++)
                if (this.self.NextCardTypes[i] == Deck.Deck.CardType.EXPLOSION)
                    return true;
            return false;
        }

        public override int? GetIndexOfCardToPlay()
        {
            GameManagerSrc.Instance.Test("Begin of check");
            if (this.self.NextCardTypes.Count != 0)
            {
                GameManagerSrc.Instance.Test("I know some cards");
                if (hasExplosion(GameManagerSrc.Instance.NumberOfCardToGetByMe))
                {
                    GameManagerSrc.Instance.Test("I about bombs");
                    this.self.ChangeState(this.self.StateSet.GetAggresiveState(true));
                    return this.self.State.GetIndexOfCardToPlay();
                }
                else if (this.self.NextCardTypes.Count >= GameManagerSrc.Instance.NumberOfCardToGetByMe)
                {
                    //Signal to getCard
                    GameManagerSrc.Instance.Test("I know that there are not bombs");
                    return null;
                }
            }
            int? index = self.State.TryToGetIndexOfCard(type);
            GameManagerSrc.Instance.Test("Пытаюсь найти Чек");
            if (index != -1)
                return index;
            GameManagerSrc.Instance.Test("Чек не найден. Переходим в Агрессивное состояние");
            this.self.ChangeState(this.self.StateSet.GetAggresiveState(false));
            return this.self.State.GetIndexOfCardToPlay();
        }
    }

    public abstract class State
    {
        Enemy self;

        public Enemy Self
        {
            get => self;
        }

        protected ThinkingBlockOfChain HeadOfThinkingChain;

        public ThinkingBlockOfChain Head
        {
            get => HeadOfThinkingChain;
        }

        public State(Enemy self)
        {
            this.self = self;
        }

        public int? TryToGetIndexOfCard(Deck.Deck.CardType type)
        {
            foreach (Transform cardTransform in this.self.Hand)
            {
                if (GameManagerSrc.Instance.Deck.GetType(cardTransform.gameObject.GetComponent<CardControl>().Card) == type)
                    return cardTransform.GetSiblingIndex();
            }
            return -1;
        }

        public virtual int? GetIndexOfCardToPlay()
        {
            return this.HeadOfThinkingChain.GetIndexOfCardToPlay();
        }
    }

    public class NormalState : State
    {
        public NormalState(Enemy self) : base(self)
        {
            this.HeadOfThinkingChain = new CheckingBlockOfChain(Deck.Deck.CardType.CHECK, this.Self);
        }
    }

    public class AggresiveState : State
    {
        public AggresiveState(Enemy self) : base(self)
        {
            this.HeadOfThinkingChain = new ThinkingBlockOfChain(Deck.Deck.CardType.SKIP, this.Self);
            this.HeadOfThinkingChain.NextBlock = new ThinkingBlockOfChain(Deck.Deck.CardType.ATTACK, this.Self);
        }
    }

    public class PreShuffleDecoratedState : State
    {
        State state;
        public PreShuffleDecoratedState(State state) : base(state.Self)
        {
            this.state = state;
            this.HeadOfThinkingChain = new ThinkingBlockOfChain(Deck.Deck.CardType.SHUFFLE, this.Self);
            this.HeadOfThinkingChain.NextBlock = state.Head;
        }
    }

    public class PostShuffleDecoratedState : State
    {
        State state;
        public PostShuffleDecoratedState(State state) : base(state.Self)
        {
            this.state = state;
            this.HeadOfThinkingChain = new ThinkingBlockOfChain(Deck.Deck.CardType.SHUFFLE, this.Self);
            ThinkingBlockOfChain tail = state.Head;
            while (tail.NextBlock != null)
                tail = tail.NextBlock;
            tail.NextBlock = this.HeadOfThinkingChain;
        }
    }
    public class PreAttackDecoratedState : State
    {
        State state;
        public PreAttackDecoratedState(State state) : base(state.Self)
        {
            this.state = state;
            this.HeadOfThinkingChain = new ThinkingBlockOfChain(Deck.Deck.CardType.ATTACK, this.Self);
            this.HeadOfThinkingChain.NextBlock = state.Head;
        }
    }

    public class StateSet
    {
        public enum StateType
        {
            NORMAL, AGGRESIVE
        }
        private State normal;
        private State aggresive;
        private Enemy self;

        public State Normal
        {
            get => this.normal;
        }
        public State Aggresive
        {
            get => this.aggresive;
        }
        public StateSet(Enemy self)
        {
            this.self = self;
            this.normal = new NormalState(self);
            this.aggresive = new AggresiveState(self);
        }

        public State GetAggresiveState(bool bombs)
        {
            State resultState = this.aggresive;
            if (GameManagerSrc.Instance.NumberOfCardToGetByMe > 1)
                resultState = new PreAttackDecoratedState(this.aggresive);
            if (bombs && this.self.HasNeutralization())
                resultState = new PreShuffleDecoratedState(this.aggresive);
            if (bombs && !this.self.HasNeutralization())
                resultState = new PostShuffleDecoratedState(this.aggresive);
            return resultState;
        }
    }

    public class Enemy : Gamer
    {
        public State State;
        public StateSet StateSet;

        public Enemy(Transform Hand, GameManagerSrc GameManager) : base(Hand, GameManager) {
            this.StateSet = new StateSet(this);
            this.State = this.StateSet.Normal;
            this.NextCardTypes = new List<Deck.Deck.CardType>();
        }

        public List<Deck.Deck.CardType> NextCardTypes;

        public override IEnumerator GetCardToHand(Deck.Deck Deck)
        {
            if (!Deck.IsEmpty())
            {
                Card card = Deck.GetCard();
                yield return GetCardToHand(card);
            }
        }

        public override IEnumerator GetCardToHand(Card Card)
        {
            GameObject cardGO = GameManager.CreateCard(Card, Hand);
            cardGO.GetComponent<CardInfo>().HideCardInfo();
            yield return new WaitForSeconds(.9f);
            cardGO.GetComponent<CardMovement>().SetAsGotten(Hand);
            Card card = cardGO.GetComponent<CardControl>().Card;
            if (card.PreEffect != null)
            {
                if (card.PreEffect.isNeutralization)
                {
                    this.NeutralizationCards.Add(cardGO);
                }
                else if (card.PreEffect.isExplosion)
                {
                    this.ExplosionCard = cardGO;
                    card.PreEffect.Execute();
                }
                else
                {
                    card.PreEffect.Execute();
                }
            }

        }

        IEnumerator ClockControl()
        {
            int TurnTime = Game.TurnTime;
            GameManager.SetTimerValue(TurnTime);
            while (TurnTime-- > 0)
            {
                GameManager.SetTimerValue(TurnTime);
                yield return new WaitForSeconds(1);
            }
            EndTurn();
        }

        protected override IEnumerator PlayngCards()
        {
            GameManager.Test("Begin of playing");
            GameManager.StartCoroutine(ClockControl());
            yield return new WaitForSeconds(1);

            do
            {
                this.ChangeState(this.StateSet.Normal);
                int? index = this.State.GetIndexOfCardToPlay();
                if (index == null)
                {
                    //end turn
                    GameManager.Test("I am going to get card");
                    yield return new WaitForSeconds(2);
                    EndTurn();
                    break;
                }

                GameObject cardGO = Hand.gameObject.transform.GetChild(index.Value).gameObject;
                CardControl cardControl = cardGO.GetComponent<CardControl>();
                GameManager.Test("Играю карту номер " + index.ToString() + ", " + cardControl.Card.Name);
                cardControl.Movement.Discard();
                yield return new WaitForSeconds(1);
                cardControl.Movement.SetAsDiscarded();
                yield return new WaitForSeconds(1);
                if (cardControl.Card.Effect.WillEndTheTurn)
                {
                    cardControl.Card.Effect.Execute();
                    break;
                }
                else
                {
                    cardControl.Card.Effect.Execute();

                }
            } while (this.Hand.childCount != 0);


        }

        public void ChangeState(State newState)
        {
            this.State = newState;
        }

        public void NoticeGettingCard()
        {
            if (this.NextCardTypes.Count > 0)
                this.NextCardTypes.RemoveAt(0);
        }
    }
}
