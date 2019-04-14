using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Gamers.EnemyThinkingBlock
{
    public class ThinkingBlockOfChain
    {
        public Deck.Deck.CardType type;
        public Enemy self;
        public ThinkingBlockOfChain NextBlock;

        public virtual int GetIndexOfCardToPlay()
        {
            int index = self.State.TryToGetIndexOfCard(type);
            return index != -1 ? index : NextBlock.GetIndexOfCardToPlay();
        }
    }
}
