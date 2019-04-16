using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Gamers.EnemyThinkingBlock;
using UnityEngine;

namespace Assets.Scripts.Gamers.EnemyState
{
    public abstract class State
    {
        Enemy self;

        public Enemy Self
        {
            get => self;
        }

        protected ThinkingBlockOfChain HeadOfThinkingChain;

        public ThinkingBlockOfChain Head
        {
            get => HeadOfThinkingChain;
        }

        public State(Enemy self)
        {
            this.self = self;
        }

        public int? TryToGetIndexOfCard(Deck.Deck.CardType type)
        {
            foreach (Transform cardTransform in this.self.Hand)
            {
                if (GameManagerSrc.Instance.Deck.GetType(cardTransform.gameObject.GetComponent<CardControl>().Card) == type)
                    return cardTransform.GetSiblingIndex();
            }
            return -1;
        }

        public virtual int? GetIndexOfCardToPlay()
        {
            return this.HeadOfThinkingChain.GetIndexOfCardToPlay();
        }
    }
}
