using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardEffects
{
    class Check : Effect
    {
        public Check() : base()
        {
            this.WillEndTheTurn = false;
        }
        public override void Execute()
        {
            GameManagerSrc.Instance.Check();
        }
    }
}
