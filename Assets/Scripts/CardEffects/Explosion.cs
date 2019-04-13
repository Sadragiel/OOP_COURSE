using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardEffects
{
    class Explosion : PreEffect
    {
        public Explosion()
        {
            this.isNeutralization = false;
            this.isExplosion = true;
        }
        public override void Execute()
        {
            GameManagerSrc.Instance.Explosion();
        }
    }
}
