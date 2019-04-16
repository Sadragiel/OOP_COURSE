using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Gamers.EnemyState.Decorators;
using Assets.Scripts.Gamers.EnemyThinkingBlock;

namespace Assets.Scripts.Gamers.EnemyState.Decorators
{
    public class PostShuffleDecoratedState : State
    {
        State state;
        public PostShuffleDecoratedState(State state) : base(state.Self)
        {
            this.state = state;
            this.HeadOfThinkingChain = new ThinkingBlockOfChain(Deck.Deck.CardType.SHUFFLE, this.Self);
            ThinkingBlockOfChain tail = state.Head;
            while (tail.NextBlock != null)
                tail = tail.NextBlock;
            tail.NextBlock = this.HeadOfThinkingChain;
        }
    }
}
