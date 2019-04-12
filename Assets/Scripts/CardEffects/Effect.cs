﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardEffects
{
    public abstract class Effect
    {
        public bool WillEndTheTurn;
        public abstract void Execute();
    }
}
