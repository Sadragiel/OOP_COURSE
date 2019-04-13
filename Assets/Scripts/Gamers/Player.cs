using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Gamers
{
    public class Player : Gamer
    {
        public Player(Transform Hand, GameManagerSrc GameManager) : base(Hand, GameManager) { }
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
            cardGO.GetComponent<CardInfo>().ShowCardInfo();
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
