using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Gamers.EnemyState.Decorators;
using Assets.Scripts.Gamers.EnemyThinkingBlock;

namespace Assets.Scripts.Gamers.EnemyState.Decorators
{
    public class PreAttackDecoratedState : State
    {
        State state;
        public PreAttackDecoratedState(State state) : base(state.Self)
        {
            this.state = state;
            this.HeadOfThinkingChain = new ThinkingBlockOfChain(Deck.Deck.CardType.ATTACK, this.Self);
            this.HeadOfThinkingChain.NextBlock = state.Head;
        }
    }
}
