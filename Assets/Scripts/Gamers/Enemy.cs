using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Gamers.EnemyThinkingBlock;
using Assets.Scripts.Gamers.EnemyState;

namespace Assets.Scripts.Gamers
{
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
            GameManager.StartCoroutine(ClockControl());
            yield return new WaitForSeconds(1);

            do
            {
                this.ChangeState(this.StateSet.Normal);
                int? index = this.State.GetIndexOfCardToPlay();
                if (index == null)
                {
                    //end turn
                    yield return new WaitForSeconds(2);
                    EndTurn();
                    break;
                }

                GameObject cardGO = Hand.gameObject.transform.GetChild(index.Value).gameObject;
                CardControl cardControl = cardGO.GetComponent<CardControl>();
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
