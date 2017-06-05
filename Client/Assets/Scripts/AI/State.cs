using System;
using System.Collections.Generic;

namespace Nova
{
    public class State
    {
        // 状态名称
        public string Name { get; private set; }

        // 要执行的任务
        public Action<float> DoRun { get; private set; }

        // 进入状态时的额外动作
        public Action RunIn { get; private set; }

        // 离开状态时的额外动作
        public Action RunOut { get; private set; }

        // 是否是默认状态
        public bool IsDefault { get; set; }

        public State(string name)
        {
            Name = name;
        }

        public State Run(Action<float> doRun)
        {
            DoRun = doRun;
            return this;
        }

        public State OnRunIn(Action runIn)
        {
            RunIn = runIn;
            return this;
        }

        public State OnRunOut(Action runOut)
        {
            RunOut = runOut;
            return this;
        }

        public State AsDefault(bool b = true)
        {
            IsDefault = b;
            return this;
        }
    }
}
