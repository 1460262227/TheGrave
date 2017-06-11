using System;
using System.Collections.Generic;
using AStar;
using UnityEngine;

namespace Nova
{
    public static class AIExt
    {
        public static StateMachineManager SMMgr = null;
        public static AStarPathFinder PathFinder = null;
        public static Ground Ground = null;
        public static Func<Pos, Actor[]> GetActorAtPos = null;
        public static event Action<Actor, Actor> OnActorCollid = null;
        public static BloodyNum BloodyNum = null;

        public static StateMachine CreateAI(this Actor a, string aiType)
        {
            switch (aiType)
            {
                case "WalkOrAttack":
                    return a.WalkOrAttack();
                case "StayAndCollid":
                    return a.StayAndCollid();
                case "PatrolInLayer":
                    return a.PatrolInLayer();
            }

            return null;
        }

        public static Actor[] GetActorsInRange(this Actor a, int range)
        {
            var actors = new List<Actor>();
            Utils.For(-range, range, -range, range, (dx, dy) =>
            {
                actors.AddRange(Ground.GetActorsAtPos(a.Pos + new Pos(dx, dy)));
            });

            return actors.ToArray();
        }

        public static void Move2(this Actor a, float fx, float fy)
        {
            a.transform.localPosition = Ground.ToWorldPos(fx, fy);
        }

        public static void MoveOnPath(this Actor a, List<Pos> path)
        {
            if (a.MovePath != null && a.MovePath.Count > 0)
                path.Insert(0, a.MovePath[0]);

            a.MovePath = path;
        }

        public static void Attack(this Actor a, Actor target)
        {
            if (!target.IsDead())
            {
                target.Hp--;
                BloodyNum.PlayNumAt(-1, target.Pos.x, target.Pos.y);
            }
        }

        public static StateMachine GetStateMachine(this Actor a)
        {
            return SMMgr.Get(a.ID);
        }

        public static List<Pos> FindPath(this Actor a, Pos dst)
        {
            var path = new List<Pos>();
            PathFinder.KeepInLayer = a.KeepInLayer;
            var pts = PathFinder.FindPath(a.Pos.x, a.Pos.y, dst.x, dst.y);
            Utils.For(pts.Length / 2, (n) =>
            {
                var x = pts[n * 2];
                var y = pts[n * 2 + 1];
                path.Add(new Pos(x, y));
            });

            return path;
        }

        #region 各种独立行为

        // 原地待命，什么都不干
        static Action<int> Idle(this Actor a)
        {
            return null;
        }

        // 沿着给定路径移动
        static Action<float> MakeMoveOnPath(this Actor a, float speed, Action onMoving = null)
        {
            var interval = 1 / speed; // 每走一各的时间间隔
            var t = 0f; // 累计结余时间
            a.MovePath = null;

            return (te) =>
            {
                // 行动间隔时间
                t += te;

                // 路径走完了
                var path = a.MovePath;
                if (path == null || path.Count == 0)
                    return;

                var div = t / interval;
                if (div < 1)
                {
                    var dp = path[0] - a.Pos;
                    var v2 = new Vector2(dp.x, dp.y);
                    var pt = v2 * div + new Vector2(a.Pos.x, a.Pos.y);
                    a.Move2(pt.x, pt.y);
                }
                else
                {
                    // 移动一格
                    a.Pos = path[0];
                    path.RemoveAt(0);
                    t -= interval;
                    a.Move2(a.Pos.x, a.Pos.y);
                    onMoving.SC();
                }
            };
        }

        static Actor FindTarget(this Actor a)
        {
            var actors = a.GetActorsInRange(a.SightRange);
            foreach (var tar in actors)
            {
                // 检查攻击距离
                if (tar != null && !tar.IsDead() && a != tar && a.IsEnemy(tar))
                    return tar;
            }

            return null;
        }

        // 搜索攻击目标
        static Action<float> MakeFindingTarget(this Actor a, Action<Actor> cbTarget)
        {
            return (te) =>
            {
                var tar = a.FindTarget();
                if (tar != null)
                    cbTarget(tar);
            };
        }

        // 以一定时间间隔攻击目标
        static Action<float> MakeAttacking(this Actor a, Func<Actor> getTarget, float attackSpeed)
        {
            float attackInterval = 1 / attackSpeed;
            float t = attackInterval;

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
            a.MovePath = null;

            return (te) =>
            {
                // 行动间隔时间
                t += te;

                // 路径走完了
                var tar = getTarget();
                if (tar == null)
                    return;

                var path = a.FindPath(tar.Pos);
                if (path == null || path.Count < 2) // 第一个节点是现在所在位置
                    return;

                path.RemoveAt(0);
                var div = t / interval;
                if (div < 1)
                {
                    var dp = path[0] - a.Pos;
                    var v2 = new Vector2(dp.x, dp.y);
                    var pt = v2 * div + new Vector2(a.Pos.x, a.Pos.y);
                    a.Move2(pt.x, pt.y);
                }
                else
                {
                    // 移动一格
                    a.Pos = path[0];
                    path.RemoveAt(0);
                    t -= interval;
                    a.Move2(a.Pos.x, a.Pos.y);
                }
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
        public static StateMachine GetAIStateMachine(this Actor a)
        {
            return SMMgr.Get(a.ID);
        }

        // 创建新的状态机
        public static StateMachine CreateNewStateMachine(this Actor a)
        {
            SMMgr.Del(a.ID);
            return SMMgr.Get(a.ID);
        }

        // 启动状态机
        public static void StartAI(this Actor a, string aiType)
        {
            var sm = a.GetAIStateMachine();
            if (sm == null && aiType != null)
                sm = a.CreateAI(aiType);

            if (sm != null)
                sm.StartAI();
        }

        static StateMachine MakeSureSM(this Actor a)
        {
            var sm = a.GetAIStateMachine();
            if (sm == null)
                sm = SMMgr.Create(a.ID);

            return sm;
        }

        // 停止状态机
        public static void StopAI(this Actor a)
        {
            var sm = a.GetAIStateMachine();
            if (sm != null)
                sm.Destroy();

            SMMgr.Del(a.ID);
        }

        // 有行动路径就行走，没有就寻找攻击目标，找到目标攻击。这个 AI 给玩家控制的角色用的
        static StateMachine WalkOrAttack(this Actor a)
        {
            var sm = a.MakeSureSM();

            // 行走、搜索目标和攻击三个状态
            Actor target = null;
            sm.NewState("idle").AsDefault().Run(a.MakeFindingTarget((tar) => { target = tar; }));
            sm.NewState("walking").Run(a.MakeMoveOnPath(5));
            sm.NewState("attacking").Run(a.MakeAttacking(() => target, 1));

            // 状态迁移
            sm.Trans().From("idle|attacking").To("walking").When(() => a.MovePath != null && a.MovePath.Count > 0);
            sm.Trans().From("attacking").To("idle").When(() => target == null || target.IsDead() || !a.InAttackRange(target));
            sm.Trans().From("walking").To("idle").When(() => a.MovePath == null || a.MovePath.Count == 0);
            sm.Trans().From("idle").To("attacking").When(() => target != null && !target.IsDead() && a.InAttackRange(target));

            // sm.DebugInfo = Debug.LogWarning;
            return sm;
        }

        // 原地待机，并检查与其它 Actor 的碰撞情况
        static StateMachine StayAndCollid(this Actor a)
        {
            var sm = a.MakeSureSM();
            sm.NewState("idle").Run((te) =>
            {
                var actors = GetActorAtPos(a.Pos);
                if (actors.Length == 0)
                    return;

                foreach (var toA in actors)
                    OnActorCollid.SC(a, toA);
            }).AsDefault();

            return sm;
        }

        // 同层巡逻
        static StateMachine PatrolInLayer(this Actor a)
        {
            var sm = a.MakeSureSM();

            // 原地搜索目标、巡逻到指定地点、追击目标、攻击目标三种状态
            Actor target = null;
            sm.NewState("findingTarget").Run(a.MakeFindingTarget((tar) => { target = tar; })).AsDefault();
            Func<bool> exitLoop = () => a.MovePath != null && a.MovePath.Count > 0;
            sm.NewState("patrol2Pos").Run(a.MakeMoveOnPath(2, () => { target = a.FindTarget(); })).OnRunIn(() =>
            {
                var findRs = Utils.Range(1, 5).Disorder();
                foreach (var r in findRs)
                {
                    Utils.For(-r, r, (i) =>
                    {
                        var dps = new Pos[] { new Pos(i, -r), new Pos(i, r), new Pos(-r, i), new Pos(r, i) };
                        dps.Disorder();
                        foreach (var dp in dps)
                        {
                            var d = a.Pos + dp;
                            var path = a.FindPath(d);
                            if (path != null && path.Count > 0)
                            {
                                a.MovePath = path;
                                break;
                            }
                        }
                    }, exitLoop);

                    if (exitLoop())
                        break;
                }
            });
            sm.NewState("chasing").Run(a.MakeChasing(() => target, 3));
            sm.NewState("attacking").Run(a.MakeAttacking(() => target, 1));

            // 状态切换
            sm.Trans().From("findingTarget").To("patrol2Pos").Wait4Sec(1);
            sm.Trans().From("findingTarget|patrol2Pos").To("chasing").When(() => target != null && !target.IsDead() && a.InSightRange(target));
            sm.Trans().From("chasing").To("findingTarget").When(() => target == null || target.IsDead() || !a.InSightRange(target));
            sm.Trans().From("patrol2Pos").To("findingTarget").When(() => a.MovePath == null || a.MovePath.Count == 0);
            sm.Trans().From("chasing").To("attacking").When(() => a.InAttackRange(target));
            sm.Trans().From("attacking").To("chasing").When(() => target != null && !a.InAttackRange(target));
            sm.Trans().From("attacking|chasing").To("findingTarget").When(() => target != null && target.IsDead());

            // sm.DebugInfo = Debug.Log;
            return sm;
        }

        #endregion
    }
}
