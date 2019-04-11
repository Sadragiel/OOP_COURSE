using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Gamers
{
    public class Player : Gamer
    {
        public Player(Transform Hand, GameManagerSrc GameManager) : base(Hand, GameManager) { }
        public override void GetCardToHand(Deck.Deck Deck)
        {
            if (Deck.IsEmpty())
                return;
            Card card = Deck.GetCard();

            //TODO EXPLOSION

            //Cards.Add(card);
            GameObject cardGO = GameManager.CreateCard(card, Hand);
            cardGO.GetComponent<CardInfo>().ShowCardInfo();

        }

        protected override IEnumerator PlayngCards()
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
    }
}
