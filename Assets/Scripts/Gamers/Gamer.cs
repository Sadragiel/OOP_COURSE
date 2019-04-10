using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Gamers
{
    public abstract class Gamer
    {
        public List<Card> Cards;
        public Gamer(Transform Hand, GameManagerSrc GameManager)
        {
            this.Hand = Hand;
            this.GameManager = GameManager;
        }
        protected GameManagerSrc GameManager;
        public Transform Hand;
        public abstract void GetCardToHand(List<Card> Deck);
        protected abstract IEnumerator PlayngCards();               //Coroutine
        protected void EndTurn()
        {
            GameManager.ChangeTurn();
        }

        //Start oc Coroutine
        public void Turn()
        {
            GameManager.StartCoroutine(PlayngCards());
        }
    }
}
