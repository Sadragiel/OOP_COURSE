using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Gamers.EnemyState.Decorators;

namespace Assets.Scripts.Gamers.EnemyState
{
    public class StateSet
    {
        public enum StateType
        {
            NORMAL, AGGRESIVE
        }
        private State normal;
        private State aggresive;
        private Enemy self;

        public State Normal
        {
            get => this.normal;
        }
        public State Aggresive
        {
            get => this.aggresive;
        }
        public StateSet(Enemy self)
        {
            this.self = self;
            this.normal = new NormalState(self);
            this.aggresive = new AggresiveState(self);
        }

        public State GetAggresiveState(bool bombs)
        {
            State resultState = this.aggresive;
            if (GameManagerSrc.Instance.NumberOfCardToGetByMe > 1)
                resultState = new PreAttackDecoratedState(this.aggresive);
            if (bombs && this.self.HasNeutralization())
                resultState = new PreShuffleDecoratedState(this.aggresive);
            if (bombs && !this.self.HasNeutralization())
                resultState = new PostShuffleDecoratedState(this.aggresive);
            return resultState;
        }
    }
}
