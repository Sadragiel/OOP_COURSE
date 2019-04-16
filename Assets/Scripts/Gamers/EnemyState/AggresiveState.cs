using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Gamers.EnemyThinkingBlock;

namespace Assets.Scripts.Gamers.EnemyState
{
    public class AggresiveState : State
    {
        public AggresiveState(Enemy self) : base(self)
        {
            this.HeadOfThinkingChain = new ThinkingBlockOfChain(Deck.Deck.CardType.SKIP, this.Self);
            this.HeadOfThinkingChain.NextBlock = new ThinkingBlockOfChain(Deck.Deck.CardType.ATTACK, this.Self);
        }
    }
}
