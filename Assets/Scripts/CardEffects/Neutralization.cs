using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardEffects
{
    class Neutralization: PreEffect
    {
        public Neutralization()
        {
            this.isNeutralization = true;
            this.isExplosion = false;
        }
        public override void Execute()
        {
            GameManagerSrc.Instance.Neutralization();
        }
    }
}
