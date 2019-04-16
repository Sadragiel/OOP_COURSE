using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Gamers.EnemyThinkingBlock;

namespace Assets.Scripts.Gamers.EnemyState
{
    public class NormalState : State
    {
        public NormalState(Enemy self) : base(self)
        {
            this.HeadOfThinkingChain = new CheckingBlockOfChain(Deck.Deck.CardType.CHECK, this.Self);
        }
    }
}
