using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardEffects
{
    public abstract class Effect
    {
        protected GameManagerSrc GameManager;
        public bool WillEndTheTurn;
        public Effect()
        {

        }

        public abstract void Execute();
    }
}
