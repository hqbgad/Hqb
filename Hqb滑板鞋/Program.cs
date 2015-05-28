using System;
using System.Linq;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using Collision = LeagueSharp.Common.Collision;


namespace Kalista
{
    class Program
    {
        public const string ChampionName = "Kalista";
        static Spell Q;
        static Spell W;
        static Spell E;
        static Spell R;
        public static Menu Menu;

        internal static Orbwalking.Orbwalker Orbwalker;
        internal static float getManaPer { get { return Player.Mana / Player.MaxMana * 100; } }
        static Obj_AI_Hero Player { get { return ObjectManager.Player; } }
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != ChampionName)
            {
                return;
            }

            Menu = new Menu("Hqb滑板鞋", "Kalista", true);

            var targetSelectorMenu = new Menu("目标 选择", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Menu.AddSubMenu(targetSelectorMenu);

            Orbwalker = new Orbwalking.Orbwalker(Menu.AddSubMenu(new Menu("走砍 设置", "Orbwalker")));

            Menu.AddSubMenu(new Menu("连招 设置", "lianzhao"));
            Menu.SubMenu("lianzhao").AddItem(new MenuItem("lzp", "使用 Q", true).SetValue(true));
            Menu.SubMenu("lianzhao").AddItem(new MenuItem("lze", "使用 E", true).SetValue(true));
            Menu.SubMenu("lianzhao").AddItem(new MenuItem("lzeee", "使用E 抢人头", true).SetValue(true));
            Menu.SubMenu("lianzhao").AddItem(new MenuItem("lzeeeeee", "连招时 E 堆叠数", true).SetValue(new Slider(5, 1, 20)));
            Menu.SubMenu("lianzhao").AddItem(new MenuItem("lzmp", "连招丨最低蓝量比", true).SetValue(new Slider(50, 0, 100)));

            Menu.AddSubMenu(new Menu("骚扰 设置", "saorao"));
            Menu.SubMenu("saorao").AddItem(new MenuItem("srq", "使用 Q", true).SetValue(true));
            Menu.SubMenu("saorao").AddItem(new MenuItem("srmp", "骚扰丨最低蓝量比", true).SetValue(new Slider(50, 0, 100)));

            Menu.AddSubMenu(new Menu("清线 设置", "qingxian"));
            Menu.SubMenu("qingxian").AddItem(new MenuItem("qxq", "使用 Q", true).SetValue(true));
            Menu.SubMenu("qingxian").AddItem(new MenuItem("qxqqq", "使用 Q丨最少清兵数", true).SetValue(new Slider(3, 1, 5)));
            Menu.SubMenu("qingxian").AddItem(new MenuItem("qxe", "使用 E", true).SetValue(true));
            Menu.SubMenu("qingxian").AddItem(new MenuItem("qxeee", "使用 E丨最少清兵数", true).SetValue(new Slider(3, 1, 5)));
            Menu.SubMenu("qingxian").AddItem(new MenuItem("qxmp", "清线丨最低蓝量比", true).SetValue(new Slider(60, 0, 100)));

            Menu.AddSubMenu(new Menu("清野 设置", "qingye"));
            Menu.SubMenu("qingye").AddItem(new MenuItem("qyq", "使用 Q", true).SetValue(true));
            Menu.SubMenu("qingye").AddItem(new MenuItem("qye", "使用 E", true).SetValue(true));
            Menu.SubMenu("qingye").AddItem(new MenuItem("eqiangyeguai", "使用 E丨抢野怪", true).SetValue(true));
            Menu.SubMenu("qingye").AddItem(new MenuItem("qymp", "清野丨最低蓝量比", true).SetValue(new Slider(20, 0, 100)));

            Menu.AddSubMenu(new Menu("其他 设置", "qitashezhi"));
            Menu.SubMenu("qitashezhi").AddItem(new MenuItem("DrawEDamage", "显示 E 伤害").SetValue(true));
            Menu.SubMenu("qitashezhi").AddItem(new MenuItem("DamageExxx", "自动E丨小兵准备死亡并且英雄身上有E的buff").SetValue(true));
            Menu.SubMenu("qitashezhi").AddItem(new MenuItem("jilao", "拯救你的基佬").SetValue(true));
            Menu.SubMenu("qitashezhi").AddItem(new MenuItem("jilaohp", "基佬最低Hp百分比").SetValue(new Slider(20, 100, 0)));
            Menu.SubMenu("qitashezhi").AddItem(new MenuItem("AAmb", "AA 目标").SetValue(true));
            Menu.SubMenu("qitashezhi").AddItem(new MenuItem("bdxb", "可补刀小兵").SetValue(new Circle(true, Color.GreenYellow)));
            Menu.SubMenu("qitashezhi").AddItem(new MenuItem("fjkjs", "附近可击杀小兵").SetValue(new Circle(true, Color.Gray)));
            Menu.SubMenu("qitashezhi").AddItem(new MenuItem("wushangdaye", "无伤打野点").SetValue(true));

            Menu.AddSubMenu(new Menu("技能 范围", "jinengxianshi"));
            Menu.SubMenu("jinengxianshi").AddItem(new MenuItem("drawingAA", "显示 AA 范围", true).SetValue(new Circle(true, Color.FromArgb(0, 230, 255))));
            Menu.SubMenu("jinengxianshi").AddItem(new MenuItem("drawingQ", "显示 Q 范围", true).SetValue(new Circle(true, Color.FromArgb(138, 101, 255))));
            Menu.SubMenu("jinengxianshi").AddItem(new MenuItem("drawingW", "显示 W 范围", true).SetValue(new Circle(false, Color.FromArgb(202, 170, 255))));
            Menu.SubMenu("jinengxianshi").AddItem(new MenuItem("drawingE", "显示 E 范围", true).SetValue(new Circle(true, Color.FromArgb(255, 0, 0))));
            Menu.SubMenu("jinengxianshi").AddItem(new MenuItem("drawingR", "显示 R 范围", true).SetValue(new Circle(false, Color.FromArgb(0, 255, 0))));

            Menu.AddItem(new MenuItem("MSG", "我滑啊滑啊滑 然后就摔倒了"));

            Menu.AddItem(new MenuItem("Credit", "作者 : Hqb "));

            Menu.AddItem(new MenuItem("Version", "QQ :1326272899"));

            Menu.AddToMainMenu();
            Drawing.OnDraw += fanweixianquanxianshi;
            DamageIndicator.DamageToUnit = xianshilzshanghai;
            Game.OnUpdate += Huabian;
            Obj_AI_Hero.OnProcessSpellCast += 拯救基佬;

            Game.PrintChat("Flowers-Kalista Loaded!~~~ Version : 1.2.0.0 Thanks for your use!");
            Q = new Spell(SpellSlot.Q, 1160f);
            W = new Spell(SpellSlot.W, 5000f);
            E = new Spell(SpellSlot.E, 1000f);
            R = new Spell(SpellSlot.R, 1450f);

            Q.SetSkillshot(0.25f, 35f, 1600f, true, SkillshotType.SkillshotLine);

        }

        static void fanweixianquanxianshi(EventArgs args)
        {

            if (Player.IsDead)
                return;

            var drawingAA = Menu.Item("drawingAA", true).GetValue<Circle>();
            var drawingQ = Menu.Item("drawingQ", true).GetValue<Circle>();
            var drawingW = Menu.Item("drawingW", true).GetValue<Circle>();
            var drawingE = Menu.Item("drawingE", true).GetValue<Circle>();
            var drawingR = Menu.Item("drawingR", true).GetValue<Circle>();
            var drawMinionLastHit = Program.Menu.Item("bdxb").GetValue<Circle>();
            var drawMinionNearKill = Program.Menu.Item("fjkjs").GetValue<Circle>();

            if (drawingAA.Active)
                Render.Circle.DrawCircle(Player.Position, Orbwalking.GetRealAutoAttackRange(Player), drawingAA.Color);

            if (Q.IsReady() && drawingQ.Active)
                Render.Circle.DrawCircle(Player.Position, Q.Range, drawingQ.Color);

            if (W.IsReady() && drawingW.Active)
                Render.Circle.DrawCircle(Player.Position, W.Range, drawingW.Color);

            if (E.IsReady() && drawingE.Active)
                Render.Circle.DrawCircle(Player.Position, E.Range, drawingE.Color);

            if (R.IsReady() && drawingR.Active)
                Render.Circle.DrawCircle(Player.Position, R.Range, drawingR.Color);

            if (drawMinionLastHit.Active || drawMinionNearKill.Active)
            {
                var xMinions =
                    MinionManager.GetMinions(Player.Position, Player.AttackRange + Player.BoundingRadius + 300, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);

                foreach (var xMinion in xMinions)
                {
                    if (drawMinionLastHit.Active && Player.GetAutoAttackDamage(xMinion, true) >= xMinion.Health)
                        Render.Circle.DrawCircle(xMinion.Position, xMinion.BoundingRadius, drawMinionLastHit.Color, 5);
                    else if (drawMinionNearKill.Active && Player.GetAutoAttackDamage(xMinion, true) * 2 >= xMinion.Health)
                        Render.Circle.DrawCircle(xMinion.Position, xMinion.BoundingRadius, drawMinionNearKill.Color, 5);
                }
            }

            if (Game.MapId == (GameMapId)11 && Menu.Item("wushangdaye").GetValue<Boolean>())
            {
                const float circleRange = 100f;

                Render.Circle.DrawCircle(new Vector3(7461.018f, 3253.575f, 52.57141f), circleRange, Color.Blue, 5); // blue team :red
                Render.Circle.DrawCircle(new Vector3(3511.601f, 8745.617f, 52.57141f), circleRange, Color.Blue, 5); // blue team :blue
                Render.Circle.DrawCircle(new Vector3(7462.053f, 2489.813f, 52.57141f), circleRange, Color.Blue, 5); // blue team :golems
                Render.Circle.DrawCircle(new Vector3(3144.897f, 7106.449f, 51.89026f), circleRange, Color.Blue, 5); // blue team :wolfs
                Render.Circle.DrawCircle(new Vector3(7770.341f, 5061.238f, 49.26587f), circleRange, Color.Blue, 5); // blue team :wariaths

                Render.Circle.DrawCircle(new Vector3(10930.93f, 5405.83f, -68.72192f), circleRange, Color.Yellow, 5); // Dragon

                Render.Circle.DrawCircle(new Vector3(7326.056f, 11643.01f, 50.21985f), circleRange, Color.Red, 5); // red team :red
                Render.Circle.DrawCircle(new Vector3(11417.6f, 6216.028f, 51.00244f), circleRange, Color.Red, 5); // red team :blue
                Render.Circle.DrawCircle(new Vector3(7368.408f, 12488.37f, 56.47668f), circleRange, Color.Red, 5); // red team :golems
                Render.Circle.DrawCircle(new Vector3(10342.77f, 8896.083f, 51.72742f), circleRange, Color.Red, 5); // red team :wolfs
                Render.Circle.DrawCircle(new Vector3(7001.741f, 9915.717f, 54.02466f), circleRange, Color.Red, 5); // red team :wariaths                    
            }

            if (Program.Menu.Item("AAmb").GetValue<Boolean>())
            {
                var target = Orbwalker.GetTarget();

                if (target != null)
                    Render.Circle.DrawCircle(target.Position, target.BoundingRadius + 15, Color.Red, 6);
            }

        }
        static float xianshilzshanghai(Obj_AI_Base enemy)
        {
            float damage = 0;

            if (E.IsReady())
                damage += E.GetDamage(enemy);

            return damage;
        }
        static void Huabian(EventArgs args)
        {
            if (Player.IsDead)
                return;

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    lianzhao();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    saorao();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    qingxian();
                    qingye();
                    break;
            }

            lzeee();
            eqiangyeguai();
            DamageExxx();
        }

        private static void DamageExxx()
        {
            if (!Menu.Item("DamageExxx", true).GetValue<Boolean>() || !E.IsReady())
                {
            var enemy = HeroManager.Enemies.Where(o => o.HasBuffOfType(BuffType.SpellShield)).OrderBy(o => o.Distance(Player, true)).FirstOrDefault();
            if (enemy != null)
            {
                if (enemy.Distance(Player, true) < Math.Pow(E.Range + 200, 2))
                {
                    if (ObjectManager.Get<Obj_AI_Minion>().Any(o => o.IsRendKillable() && E.IsInRange(o)))
                    {
                        E.Cast();
                    }
					return ;
                }
				return ;
            }
                }
        }

       private static void eqiangyeguai()
       {
 	            if (!Menu.Item("eqiangyeguai", true).GetValue<Boolean>() || !E.IsReady())
                return;

            var Mob = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault(x => x.Health + (x.HPRegenRate / 2) <= E.GetDamage(x));

            if (E.CanCast(Mob))
                E.Cast();

            var Minion = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth).FirstOrDefault(x => x.Health <= E.GetDamage(x) && (x.SkinName.ToLower().Contains("siege") || x.SkinName.ToLower().Contains("super")));

            if (E.CanCast(Minion))
                E.Cast();
       }

       private static void lzeee()
       {
 	            if (!Menu.Item("lzeee", true).GetValue<Boolean>() || !E.IsReady())
                return;

            var target = HeroManager.Enemies.FirstOrDefault(x => !x.HasBuffOfType(BuffType.Invulnerability) && !x.HasBuffOfType(BuffType.SpellShield) && E.CanCast(x) && (x.Health + (x.HPRegenRate / 2)) <= E.GetDamage(x));

            if (E.CanCast(target))
                E.Cast();
       }

       private static void qingye()
       {
 	            if (!Orbwalking.CanMove(1) || !(getManaPer > Menu.Item("qymp", true).GetValue<Slider>().Value))
                return;

            var Mobs = MinionManager.GetMinions(Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (Mobs.Count <= 0)
                return;

            if (Menu.Item("qyq", true).GetValue<Boolean>() && Q.CanCast(Mobs[0]))
                Q.Cast(Mobs[0]);

            if (Menu.Item("qye", true).GetValue<Boolean>() && E.CanCast(Mobs[0]))
            {
                if (Mobs[0].Health + (Mobs[0].HPRegenRate / 2) <= E.GetDamage(Mobs[0]))
                    E.Cast();
            }
       }

       private static void qingxian()
       {
 	            if (!Orbwalking.CanMove(1) || !(getManaPer > Menu.Item("qxmp", true).GetValue<Slider>().Value))
                return;

            var Minions = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Enemy);

            if (Minions.Count <= 0)
                return;

            if (Menu.Item("qxq", true).GetValue<Boolean>() && Q.IsReady())
            {
                foreach (var minion in Minions.Where(x => x.Health <= Q.GetDamage(x)))
                {
                    var killcount = 0;

                    foreach (var colminion in Q_GetCollisionMinions(Player, Player.ServerPosition.Extend(minion.ServerPosition, Q.Range)))
                    {
                        if (colminion.Health <= Q.GetDamage(colminion))
                            killcount++;
                        else
                            break;
                    }

                    if (killcount >= Menu.Item("qxqqq", true).GetValue<Slider>().Value)
                    {
                        if (!Player.IsWindingUp && !Player.IsDashing())
                        {
                            Q.Cast(minion);
                            break;
                        }
                    }
                    if (Menu.Item("qxe", true).GetValue<Boolean>() && E.IsReady())
                    {
                        var minionkillcount = 0;
                        foreach (var Minion in Minions.Where(x => E.CanCast(x) && x.Health <= E.GetDamage(x))) { minionkillcount++; }
                        if (minionkillcount >= Menu.Item("qxeee", true).GetValue<Slider>().Value)
                            E.Cast();
                    }
                }
            }
       }

       private static void saorao()
       {
 	            if (!Orbwalking.CanMove(1) || !(getManaPer > Menu.Item("srmp", true).GetValue<Slider>().Value))
                return;

            if (Menu.Item("srq", true).GetValue<Boolean>())
            {
                var Qtarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical, true);

                if (Q.CanCast(Qtarget) && Q.GetPrediction(Qtarget).Hitchance >= HitChance.VeryHigh && !Player.IsWindingUp && !Player.IsDashing())
                    Q.Cast(Qtarget);
            }
       }

        static List<Obj_AI_Base> Q_GetCollisionMinions(Obj_AI_Hero source, Vector3 targetposition)
        {
            var input = new PredictionInput
            {
                Unit = source,
                Radius = Q.Width,
                Delay = Q.Delay,
                Speed = Q.Speed,
            };

            input.CollisionObjects[0] = CollisionableObjects.Minions;

            return Collision.GetCollision(new List<Vector3> { targetposition }, input).OrderBy(obj => obj.Distance(source, false)).ToList();
        }
        static void lianzhao()
        {
            if (!Orbwalking.CanMove(1) || !(getManaPer > Menu.Item("lzmp", true).GetValue<Slider>().Value))
                return;

            if (Menu.Item("lzp", true).GetValue<Boolean>())
            {
                var Qtarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical, true);

                if (Q.CanCast(Qtarget) && Q.GetPrediction(Qtarget).Hitchance >= HitChance.VeryHigh && !Player.IsWindingUp && !Player.IsDashing())
                    Q.Cast(Qtarget);
            }

            var Minion = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Enemy).Where(x => x.Health <= E.GetDamage(x)).OrderBy(x => x.Health).FirstOrDefault();
            var Target = HeroManager.Enemies.Where(x => E.CanCast(x) && E.GetDamage(x) >= 1 && !x.HasBuffOfType(BuffType.Invulnerability) && !x.HasBuffOfType(BuffType.SpellShield)).OrderByDescending(x => E.GetDamage(x)).FirstOrDefault();
            var target = Target;
            if (Menu.Item("lze", true).GetValue<Boolean>() && (E.Instance.State == SpellState.Ready || E.Instance.State == SpellState.Surpressed) && target.HasBuffOfType(BuffType.SpellShield))
            {
                    if (Player.Distance(target, true) > Math.Pow(Orbwalking.GetRealAutoAttackRange(target), 2))
                    {
                        var minions = ObjectManager.Get<Obj_AI_Minion>().Where(m => m.IsValidTarget(Orbwalking.GetRealAutoAttackRange(m)));
                        if (minions.Any(m => m.IsRendKillable()))
                        {
                            E.Cast(true);
                        }
                        else
                        {
                            var minion = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Enemy).Find(m => m.Health > Player.GetAutoAttackDamage(m) && m.Health < Player.GetAutoAttackDamage(m) + E.GetDamage(m, (m.HasBuffOfType(BuffType.SpellShield) ? m.GetRendBuff().Count + 1 : 1)));
                            if (minion != null)
                            {
                                Orbwalker.ForceTarget(minion);
                            }
                        }
                    }
                    else if (E.IsInRange(target))
                    {
                        if (target.IsRendKillable())
                        {
                            E.Cast(true);
                        }
                        else if (target.GetRendBuff().Count >= Menu.Item("lzeeeeee").GetValue<Slider>().Value)
                        {
                            if (target.ServerPosition.Distance(Player.ServerPosition, true) > Math.Pow(E.Range * 0.8, 2) ||
                                target.GetRendBuff().EndTime - Game.Time < 0.3)
                            {
                                E.Cast(true);
                            }
                        }
                    }
                }
                if (Target.Health <= E.GetDamage(Target) || (E.CanCast(Minion) && E.CanCast(Target)))
                    E.Cast();
            }
        private static void 拯救基佬(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "KalistaExpungeWrapper")
                Utility.DelayAction.Add(250, Orbwalking.ResetAutoAttackTimer);

            if (Menu.Item("jilao").GetValue<Boolean>() && R.IsReady())
            {
                if (sender.Type == GameObjectType.obj_AI_Hero && sender.IsEnemy)
                {

                    var 判断是否拯救基佬 = HeroManager.Allies.FirstOrDefault(hero => hero.HasBuff("kalistacoopstrikeally", true) && args.Target.NetworkId == hero.NetworkId && hero.Health / hero.MaxHealth * 100 <= Menu.Item("jilaohp").GetValue<Slider>().Value);

                    if (R.CanCast(判断是否拯救基佬))
                        R.Cast();
                }
            }
        }

        }
}