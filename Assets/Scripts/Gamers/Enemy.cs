using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Gamers.EnemyThinkingBlock;

namespace Assets.Scripts.Gamers
{

    

    //First Block, that can change State of EnemyPlayer
    public class CheckingBlockOfChain : ThinkingBlockOfChain
    {

        public override int GetIndexOfCardToPlay()
        {
            if (this.self.NextCardTypes.Count != 0)
            {
                if (this.self.NextCardTypes[0] == Deck.Deck.CardType.EXPLOSION)
                {
                    //Change state to Aggression;
                }
                else
                {
                    //Change state to Normal;
                }
            }
            else
            {
                
            }
            int index = self.State.TryToGetIndexOfCard(type);
            return index != -1 ? index : NextBlock.GetIndexOfCardToPlay();
        }
    }

    public abstract class State
    {
        Enemy self;
        

        public int TryToGetIndexOfCard(Deck.Deck.CardType type)
        {
            foreach (Transform cardTransform in this.self.Hand)
            {
                if (GameManagerSrc.Instance.Deck.GetType(cardTransform.gameObject.GetComponent<CardControl>().Card) == type)
                    return cardTransform.GetSiblingIndex();
            }
            return -1;
        }


    }


    public class StateSet
    {
        public enum StateType
        {
            NORMAL, OVERFLOWED
        }
        private State normal;
        private State overflowed;
        public State Normal
        {
            get => this.normal;
        }
        public State Overflowed
        {
            get => this.overflowed;
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
                case StateSet.StateType.OVERFLOWED:
                    {
                        this.State = this.StateSet.Overflowed;
                    }
                    break;

            }
        }
    }
}
