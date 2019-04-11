using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Gamers
{
    public class Enemy : Gamer
    {
        public Enemy(Transform Hand, GameManagerSrc GameManager) : base(Hand, GameManager) { }
        public override void GetCardToHand(Deck.Deck Deck)
        {
            if (Deck.IsEmpty())
                return;
            Card card = Deck.GetCard();

            //TODO EXPLOSION

            //Cards.Add(card);
            GameObject cardGO = GameManager.CreateCard(card, Hand);
            cardGO.GetComponent<CardInfo>().HideCardInfo();

            
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
            GameObject cardGO = Hand.gameObject.transform.GetChild(0).gameObject;
            CardControl cardConеrol = cardGO.GetComponent<CardControl>();
            cardConеrol.Movement.Discard();
            yield return new WaitForSeconds(1);
            cardConеrol.Movement.SetAsDiscarded();
            yield return new WaitForSeconds(1);
            if (cardConеrol.Card.Effect.WillEndTheTurn)
                cardConеrol.Card.Effect.Execute();
            else
            {
                cardConеrol.Card.Effect.Execute();
                EndTurn();
            }
                
        }
    }
}
