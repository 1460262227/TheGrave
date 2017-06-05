using System;
using System.Collections.Generic;

namespace Nova
{
    // 状态迁移条件
    public class StateTransition
    {
        // 迁移的状态两端
        public string FromState { get; private set; }
        public string ToState { get; private set; }
        public Action<float> RunTime { get; private set; }
        public Action Reset { get; private set; }

        // 迁移条件
        public Func<bool> TransitionCondition = null;

        public StateTransition From(string s)
        {
            FromState = s;
            return this;
        }

        public StateTransition To(string s)
        {
            ToState = s;
            return this;
        }

        public StateTransition When(Func<bool> condition)
        {
            TransitionCondition = condition;
            return this;
        }

        public StateTransition OnTimeElapsed(Action<float> runTime)
        {
            RunTime = runTime;
            return this;
        }

        public StateTransition OnReset(Action reset)
        {
            Reset = reset;
            return this;
        }
    }
}
