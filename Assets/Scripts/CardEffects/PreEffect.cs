using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardEffects
{
    public abstract class PreEffect
    {
        public bool isNeutralization;
        public bool isExplosion;
        public abstract void Execute();
    }
}
