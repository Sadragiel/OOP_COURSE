using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Gamers
{
    public class Enemy : Gamer
    {
        public Enemy(Transform Hand, GameManagerSrc GameManager) : base(Hand, GameManager) { }
        public override void GetCardToHand(List<Card> Deck)
        {
            if (Deck.Count == 0)
                return;
            Card card = Deck[0];
            Deck.RemoveAt(0);

            //TODO EXPLOSION

            //Cards.Add(card);
            GameObject cardGO = GameManager.CreateCard(Hand);
            cardGO.GetComponent<CardInfo>().HideCardInfo(true);

            GameManager.SwitchBTN();
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
            cardGO.GetComponent<CardMovement>().Discard();
            yield return new WaitForSeconds(1);
            cardGO.GetComponent<CardMovement>().SetAsDiscarded();
            cardGO.GetComponent<CardInfo>().HideCardInfo(false);
            yield return new WaitForSeconds(1);
            EndTurn();
        }
    }
}
