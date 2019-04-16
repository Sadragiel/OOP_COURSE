using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Gamers.EnemyThinkingBlock;

namespace Assets.Scripts.Gamers.EnemyState.Decorators
{
    public class PreShuffleDecoratedState : State
    {
        State state;
        public PreShuffleDecoratedState(State state) : base(state.Self)
        {
            this.state = state;
            this.HeadOfThinkingChain = new ThinkingBlockOfChain(Deck.Deck.CardType.SHUFFLE, this.Self);
            this.HeadOfThinkingChain.NextBlock = state.Head;
        }
    }
}
