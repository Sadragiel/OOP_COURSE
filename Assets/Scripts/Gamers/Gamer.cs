using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Deck;
namespace Assets.Scripts.Gamers
{
    public abstract class Gamer
    {
        public List<GameObject> NeutralizationCards;
        public GameObject ExplosionCard = null;
        public Gamer(Transform Hand, GameManagerSrc GameManager)
        {
            this.Hand = Hand;
            this.GameManager = GameManager;
            this.NeutralizationCards = new List<GameObject>();
        }
        protected GameManagerSrc GameManager;
        public Transform Hand;
        public abstract IEnumerator GetCardToHand(Deck.Deck Deck);
        public abstract IEnumerator GetCardToHand(Card Card);
        protected abstract IEnumerator PlayngCards();               //Coroutine
        protected void EndTurn()
        {
            GameManager.ChangeTurn();
        }

        //Start of Coroutine
        public void Turn()
        {
            GameManager.StartCoroutine(PlayngCards());
        }

        public bool HasNeutralization()
        {
            return this.NeutralizationCards.Count != 0;
        }

    }
}
