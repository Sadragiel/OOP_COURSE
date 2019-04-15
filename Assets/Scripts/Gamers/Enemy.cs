using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Gamers.EnemyThinkingBlock;

namespace Assets.Scripts.Gamers
{



    //First Block of NormalState, that can change State of EnemyPlayer
    public class CheckingBlockOfChain : ThinkingBlockOfChain
    {
        public CheckingBlockOfChain(Deck.Deck.CardType type) : base(type)
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
            if (this.self.NextCardTypes.Count != 0)
            {
                if (hasExplosion(GameManagerSrc.Instance.NumberOfCardToGetByMe))
                {
                    //Change state to Aggression or Gambling;
                    return null;
                }
                else if (this.self.NextCardTypes.Count >= GameManagerSrc.Instance.NumberOfCardToGetByMe)
                {
                    //Signal to getCard
                    return null;
                }
            }
            int? index = self.State.TryToGetIndexOfCard(type);
            return index != -1 ? index : null;
        }
    }



    public abstract class State
    {
        Enemy self;

        protected ThinkingBlockOfChain HeadOfThinkingChain;

        public ThinkingBlockOfChain Head
        {
            get => HeadOfThinkingChain;
        }

        public int TryToGetIndexOfCard(Deck.Deck.CardType type)
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
        public NormalState()
        {
            this.HeadOfThinkingChain = new CheckingBlockOfChain(Deck.Deck.CardType.CHECK);
        }
    }

    public class AggresiveState : State
    {
        public AggresiveState()
        {
            this.HeadOfThinkingChain = new ThinkingBlockOfChain(Deck.Deck.CardType.SKIP);
            this.HeadOfThinkingChain.NextBlock = new ThinkingBlockOfChain(Deck.Deck.CardType.ATTACK);
        }
    }

    public class PreShuffleDecoratedState : State
    {
        State state;
        public PreShuffleDecoratedState(State state)
        {
            this.state = state;
            this.HeadOfThinkingChain = new ThinkingBlockOfChain(Deck.Deck.CardType.SHUFFLE);
            this.HeadOfThinkingChain.NextBlock = state.Head;
        }
    }

    public class PostShuffleDecoratedState : State
    {
        State state;
        public PostShuffleDecoratedState(State state)
        {
            this.state = state;
            this.HeadOfThinkingChain = new ThinkingBlockOfChain(Deck.Deck.CardType.SHUFFLE);
            ThinkingBlockOfChain tail = state.Head;
            while (tail.NextBlock != null)
                tail = tail.NextBlock;
            tail.NextBlock = this.HeadOfThinkingChain;
        }
    }
    public class PreAttackDecoratedState : State
    {
        State state;
        public PreAttackDecoratedState(State state)
        {
            this.state = state;
            this.HeadOfThinkingChain = new ThinkingBlockOfChain(Deck.Deck.CardType.ATTACK);
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

        public State Normal
        {
            get => this.normal;
        }
        public State Aggresive
        {
            get => this.aggresive;
        }
        public StateSet(Enemy enemy)
        {
            //this.overflowed = new OverflowedState(server);
        }
    }

    public class Enemy : Gamer
    {
        public State State;
        public StateSet StateSet;

        public Enemy(Transform Hand, GameManagerSrc GameManager) : base(Hand, GameManager) { }

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
            GameManager.StartCoroutine(ClockControl());
            yield return new WaitForSeconds(1);
            for (int i = 0; i < Hand.transform.childCount; i++)
            {
                GameObject cardGO = Hand.gameObject.transform.GetChild(i).gameObject;
                CardControl cardControl = cardGO.GetComponent<CardControl>();
                if (!cardControl.Card.isPlayable)
                    continue;
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
                    EndTurn();
                    break;
                }
            }
            yield return new WaitForSeconds(2);
            EndTurn();
        }

        public void ChangeState(StateSet.StateType newState)
        {
            switch (newState)
            {
                case StateSet.StateType.NORMAL:
                    {
                        this.State = this.StateSet.Normal;
                    }
                    break;
                case StateSet.StateType.AGGRESIVE:
                    {
                        this.State = this.StateSet.Aggresive;
                    }
                    break;

            }
        }
    }
}
