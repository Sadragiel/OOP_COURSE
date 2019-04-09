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

            GameObject cardGO = GameManager.CreateCard(Hand);
            cardGO.GetComponent<CardInfo>().ShowCardInfo(card, false);

            GameManager.SwitchBTN();
        }

        protected override IEnumerator PlayngCards()
        {
            int TurnTime = Game.TurnTime;
            GameManager.SetTimerValue(TurnTime);
            while (TurnTime-- > 27)
            {
                GameManager.SetTimerValue(TurnTime);
                yield return new WaitForSeconds(1);
            }
            EndTurn();
        }
    }
}
