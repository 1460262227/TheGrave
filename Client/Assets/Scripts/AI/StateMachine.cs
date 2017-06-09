using System;
using System.Collections.Generic;

namespace Nova
{
    // AI 状态机
    public class StateMachine
    {
        public string Name { get; private set; }

        // 状态集
        Dictionary<string, State> states = new Dictionary<string, State>();
        List<StateTransition> allTrans = new List<StateTransition>();

        public Action<string> DebugInfo = null;

        // 迁移集
        Dictionary<string, List<StateTransition>> trans = new Dictionary<string, List<StateTransition>>();

        // 初始状态
        string StartState { get; set; }

        public StateMachine(string name)
        {
            Name = name;
        }

        // 添加状态
        public State NewState(string stateName)
        {            
            var s = new State(stateName);
            states[s.Name] = s;

            return s;
        }

        // 添加迁移条件
        public StateTransition Trans()
        {
            var t = new StateTransition();
            allTrans.Add(t);
            return t;
        }

        bool running = false;
        string curState = null;

        // 启动状态机，只能启动一次
        public void StartAI()
        {
            Prepare();
            curState = StartState;
            running = true;

            DebugInfo.SC(this.GetHashCode() + " starts at: " + curState);
        }

        // 销毁状态机，不能再次启动
        public void Destroy()
        {
            running = false;
            allTrans.Clear();
            curState = null;

            DebugInfo.SC(this.GetHashCode() + " destroy");
        }

        // prepare all transitions
        void Prepare()
        {
            trans.Clear();

            // 形如 "a|b" 的 FromState 要拆分一下
            foreach (var st in allTrans.ToArray())
            {
                if (st.FromState.Contains("|"))
                {
                    var fs = st.FromState.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (var s in fs)
                    {
                        var t = new StateTransition();
                        t.From(s).To(st.ToState).OnReset(st.Reset).When(st.TransitionCondition).OnTimeElapsed(st.RunTime);
                        allTrans.Add(t);
                    }

                    allTrans.Remove(st);
                }
            }

            foreach (var st in allTrans)
            {
                // 状态迁移的两端不能为空，也不能是同一状态，且状态表中必须存在
                if (st.FromState == null || st.ToState == null || st.FromState == st.ToState
                    || !states.ContainsKey(st.ToState) || !states.ContainsKey(st.FromState))
                    throw new Exception("invalid state transition: " + (st.FromState == null ? "null" : st.FromState) + " => " + (st.ToState == null ? "null" : st.ToState));

                if (!trans.ContainsKey(st.FromState))
                    trans[st.FromState] = new List<StateTransition>();

                trans[st.FromState].Add(st);
            }

            foreach (var s in states.Values)
            {
                if (s.IsDefault)
                {
                    // 后面的替代前面的
                    if (StartState != null)
                        states[StartState].AsDefault(false);

                    StartState = s.Name;
                }
            }

            if (StartState == null)
                throw new Exception("StartState is null since it's not set or the StateMachine has been destroyed.");
        }

        public void Pause()
        {
            running = false;
            DebugInfo.SC(this.GetHashCode() + " paused");
        }

        public void Resume()
        {
            running = true;
            DebugInfo.SC(this.GetHashCode() + " resume");
        }

        public void Run(float te)
        {
            if (!running)
                return;

            CheckTransition(te);
            if (CurSt.DoRun != null)
                CurSt.DoRun(te);
        }

        // 当前状态
        State CurSt { get { return states[curState]; } }

        // 检查状态迁移
        void CheckTransition(float te)
        {
            if (!trans.ContainsKey(curState))
                return;

            // 要生效的迁移条件
            StateTransition tToWork = null;

            // 当前状态下的迁移条件
            foreach (var t in trans[curState])
            {
                if (t.RunTime != null)
                    t.RunTime(te);

                if (t.TransitionCondition())
                {
                    tToWork = t;
                    break;
                }
            }

            // 没有满足条件的就算了
            if (tToWork == null)
                return;

            DebugInfo.SC(this.GetHashCode() + " change state from: " + tToWork.FromState + " to: " + tToWork.ToState);

            // 执行迁移操作

            if (CurSt.RunOut != null)
                CurSt.RunOut();

            curState = tToWork.ToState;

            if (CurSt.RunIn != null)
                CurSt.RunIn();

            // 新状态下的所有迁移条件重置一次
            foreach (var nt in trans[curState])
            {
                if (nt.Reset != null)
                    nt.Reset();
            }
        }
    }
}
