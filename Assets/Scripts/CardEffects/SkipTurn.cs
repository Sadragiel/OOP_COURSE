using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardEffects
{
    class SkipTurn : Effect
    {
        public SkipTurn() : base()
        {
            this.WillEndTheTurn = true;
        }
        public override void Execute()
        {
            GameManagerSrc.Instance.SkipTurn();
        }
    }
}
