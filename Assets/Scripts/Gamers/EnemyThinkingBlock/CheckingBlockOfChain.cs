using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Gamers.EnemyThinkingBlock
{
    //First Block of NormalState, that can change State of EnemyPlayer
    public class CheckingBlockOfChain : ThinkingBlockOfChain
    {
        public CheckingBlockOfChain(Deck.Deck.CardType type, Enemy self) : base(type, self)
        {
            if (type != Deck.Deck.CardType.CHECK)
                throw new System.Exception("Type Conflict! To Create CheckingBlockOfChain used " + type.ToString() + " type");
        }

        bool hasExplosion(int numOfCards)
        {
            for (int i = 0; i < numOfCards && i < this.self.NextCardTypes.Count; i++)
                if (this.self.NextCardTypes[i] == Deck.Deck.CardType.EXPLOSION)
                    return true;
            return false;
        }

        public override int? GetIndexOfCardToPlay()
        {
            if (this.self.NextCardTypes.Count != 0)
            {
                if (hasExplosion(GameManagerSrc.Instance.NumberOfCardToGetByMe))
                {
                    this.self.ChangeState(this.self.StateSet.GetAggresiveState(true));
                    return this.self.State.GetIndexOfCardToPlay();
                }
                else if (this.self.NextCardTypes.Count >= GameManagerSrc.Instance.NumberOfCardToGetByMe)
                {
                    //Signal to getCard
                    return null;
                }
            }
            int? index = self.State.TryToGetIndexOfCard(type);
            if (index != -1)
                return index;
            this.self.ChangeState(this.self.StateSet.GetAggresiveState(false));
            return this.self.State.GetIndexOfCardToPlay();
        }
    }
}
