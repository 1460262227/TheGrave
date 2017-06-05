using System;
using System.Collections.Generic;
using AStar;

namespace Nova
{
    public static class AIExt
    {
        public static StateMachineManager SMMgr = null;
        public static AStarPathFinder PathFinder = null;
        public static Ground Ground = null;

        public static StateMachine CreateAI(string name, string aiType)
        {
            return null;
        }

        static Actor[] GetActorsInRange(this Actor a, int range)
        {
            return null;
        }

        static void Move2(this Actor a, int x, int y)
        {

        }

        static void Attack(this Actor a, Actor target)
        {

        }

        static List<Pos> FindPath(this Actor a, Pos dst)
        {
            return null;
        }

        #region 各种独立行为

        // 原地待命，什么都不干
        static Action<int> Idle(this Actor a)
        {
            return null;
        }

        // 沿着给定路径移动
        static Action<float> MakeMoveOnPath(this Actor a, List<Pos> path, float speed)
        {
            var t = 0f; // 累计结余时间
            var interval = 1 / speed; // 每走一各的时间间隔

            return (te) =>
            {
                // 行动间隔时间
                t += te;
                if (t < interval)
                    return;

                t -= interval;

                // 路径走完了
                if (path.Count == 0)
                    return;

                // 移动一格
                var nextPos = path[0];
                path.RemoveAt(0);
                a.Move2(nextPos.x, nextPos.y);
            };
        }

        // 搜索攻击目标
        static Action<float> MakeFindingTarget(this Actor a, Action<Actor> cbTarget)
        {
            return (te) =>
            {
                var actors = a.GetActorsInRange(a.SightRange);
                foreach (var tar in actors)
                {
                    // 检查攻击距离
                    if (tar != null)
                    {
                        cbTarget(tar);
                        return;
                    }
                }
            };
        }

        // 以一定时间间隔攻击目标
        static Action<float> MakeAttack(this Actor a, Func<Actor> getTarget, float attackInterval)
        {
            float t = 0;

            return (te) =>
            {
                t += te;

                // 检查攻击间隔
                if (t < attackInterval)
                    return;

                t -= attackInterval;

                // 检查攻击目标
                var target = getTarget();
                if (target == null)
                    return;

                // 检查攻击距离和目标存活状态
                if (!a.InAttackRange(target))
                    return;

                // 攻击
                a.Attack(target);
            };
        }

        // 追赶目标
        static Action<float> MakeChasing(this Actor a, Func<Actor> getTarget, float speed)
        {
            var t = 0f; // 累计结余时间
            var interval = 1 / speed; // 每走一各的时间间隔

            return (te) =>
            {
                // 行动间隔时间
                t += te;
                if (t < interval)
                    return;

                t -= interval;

                // 寻路
                var target = getTarget();
                var path = a.FindPath(target.Pos);
                if (path.Count < 2) // 第一格是自己现在位置
                    return;

                // 移动一格
                var nextPos = path[1];
                a.Move2(nextPos.x, nextPos.y);
            };
        }

        #endregion

        #region 状态迁移条件

        // 等待指定时间
        public static StateTransition Wait4Sec(this StateTransition st, float sec)
        {
            float t = sec;
            st.When(() => { return t <= 0; })
              .OnReset(() => { t = sec; })
              .OnTimeElapsed((te) => { t -= te; });

            return st;
        }

        #endregion

        #region 创建常用的 AI

        // 获取角色当前状态机
        public static StateMachine GetAIStateMachine(string name)
        {
            return SMMgr.Get(name);
        }

        // 创建新的状态机
        public static StateMachine CreateNewStateMachine(string name)
        {
            SMMgr.Del(name);
            return SMMgr.Get(name);
        }

        // 启动状态机
        public static void StartAIAt(string name, string aiType)
        {
            var sm = GetAIStateMachine(name);
            if (sm == null && aiType != null)
                sm = CreateAI(name, aiType);

            if (sm != null)
                sm.StartAt();
        }

        static StateMachine MakeSureSM(string name)
        {
            var sm = GetAIStateMachine(name);
            if (sm == null)
                sm = SMMgr.Create(name);

            return sm;
        }

        // 停止状态机
        public static void StopAI(string name)
        {
            var sm = GetAIStateMachine(name);
            if (sm != null)
                sm.Destroy();

            SMMgr.Del(name);
        }

        //// 在给定位置的附近游荡：中心位置，半径、行动时间间隔
        //public static StateMachine WalkAround(this Actor a, int r, float idleTime)
        //{
        //    var sm = a.MakeSureSM();
        //    var center = a.Pos;

        //    // 行走和待命两个状态
        //    var path = new List<Pos>();
        //    sm.NewState("idle").AsDefault();
        //    sm.NewState("walking").OnRunIn(() =>
        //    {
        //        var dx = Utils.RandomNext(-r, r);
        //        var dy = Utils.RandomNext(-r, r);
        //        var dst = center + new Pos(dx, dy);
        //        path.AddRange(a.FindPath(dst));
        //    }).Run(a.MakeMoveOnPath(path, 1));

        //    // 状态迁移
        //    sm.Trans().From("idle").To("walking").Wait4Sec(idleTime); // 待命一定时间后开始走动
        //    sm.Trans().From("walking").To("idle").When(() => path.Count == 0); // 走到目的地后开始待机

        //    return sm;
        //}

        //// 原地攻击
        //public static StateMachine HoldAndAttack(this Actor a)
        //{
        //    var sm = a.MakeSureSM();

        //    // 搜寻目标和攻击两个状态
        //    Actor target = null;
        //    sm.NewState("findingTarget").Run(a.MakeFindingTarget((targetActor) => { target = targetActor; })).AsDefault();
        //    sm.NewState("attacking").Run(a.MakeAttack(() => target, 1));

        //    // 状态迁移
        //    sm.Trans().From("findingTarget").To("attacking").When(() => target != null); // 有目标就攻击
        //    sm.Trans().From("attacking").To("findingTarget").When(() => target != null && target.IsDead() || !a.IsInRange(target, 1)); // 目标死亡或超出范围就重新找目标

        //    return sm;
        //}

        //// 巡逻
        //public static StateMachine Patrol(this Actor a, int r, float idleTime)
        //{
        //    var sm = a.MakeSureSM();
        //    var center = a.Pos;

        //    // 行走和待命两个状态
        //    Actor target = null;
        //    var path = new List<Pos>();
        //    sm.NewState("idle").AsDefault();
        //    sm.NewState("walking").OnRunIn(() =>
        //    {
        //        var dx = Utils.RandomNext(-r, r);
        //        var dy = Utils.RandomNext(-r, r);
        //        var dst = center + new Pos(dx, dy);
        //        path.AddRange(a.FindPath(dst));
        //    }).Run(a.MakeMoveOnPath(path, 1, (tar) => { target = tar; }));
        //    sm.NewState("chasing").Run(a.MakeChasing(() => target, 1));
        //    sm.NewState("attacking").Run(a.MakeAttack(() => target, 1));

        //    // 状态迁移
        //    sm.Trans().From("idle").To("walking").Wait4Sec(idleTime); // 待命一定时间后开始游荡
        //    sm.Trans().From("walking").To("idle").When(() => path.Count == 0); // 游荡到目的地后开始待命

        //    sm.Trans().From("walking").To("chasing").When(() => target != null); // 游荡过程中有目标就追赶
        //    sm.Trans().From("chasing").To("attacking").When(() => a.IsInRange(target, 1)); // 追到就攻击
        //    sm.Trans().From("attacking").To("chasing").When(() => !a.IsInRange(target, 1)); // 目标逃出攻击范围就继续追

        //    // 目标死亡或跑太远就回去
        //    sm.Trans().From("chasing|attacking").To("walking").When(() => target.IsDead() || a.Pos.Distance(center) >= 5);

        //    return sm;
        //}

        #endregion
    }
}
