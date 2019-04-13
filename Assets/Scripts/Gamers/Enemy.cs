using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Gamers
{
    public class Enemy : Gamer
    {
        public Enemy(Transform Hand, GameManagerSrc GameManager) : base(Hand, GameManager) { }
        public override IEnumerator GetCardToHand(Deck.Deck Deck)
        {
            if (!Deck.IsEmpty())
            {
                Card card = Deck.GetCard();

                //TODO EXPLOSION

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
    }
}
