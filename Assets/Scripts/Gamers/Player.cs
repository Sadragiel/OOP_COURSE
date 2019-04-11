using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Gamers
{
    public class Player : Gamer
    {
        public Player(Transform Hand, GameManagerSrc GameManager) : base(Hand, GameManager) { }
        public override void GetCardToHand(List<Card> Deck)
        {
            if (Deck.Count == 0)
                return;
            Card card = Deck[0];
            Deck.RemoveAt(0);

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
