using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Nova
{
    // 状态机管理器，驱动管理所有状态机
    public class StateMachineManager
    {
        // 所有状态机
        Dictionary<string, StateMachine> sms = new Dictionary<string, StateMachine>();

        public void OnTimeElapsed(float te)
        {
            foreach (var sm in sms.Values.ToArray())
                sm.Run(te);
        }

        // 获取已有状态机
        public StateMachine Get(string name)
        {
            return sms.ContainsKey(name) ? sms[name] : null;
        }

        // 创建新的状态机
        public StateMachine Create(string name)
        {
            if (sms.ContainsKey(name))
                throw new Exception("state machine name conflict: " + name);

            var sm = new StateMachine(name);
            sms[name] = sm;
            return sm;
        }

        // 删除状态机
        public void Del(string name)
        {
            sms.Remove(name);
        }
    }
}
