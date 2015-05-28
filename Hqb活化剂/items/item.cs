using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Activator.Items
{
    public class item
    {
        internal virtual int Id { get; set; }
        internal virtual int Cooldown { get; set; }
        internal virtual string Name { get; set; }
        internal virtual string DisplayName { get; set; }
        internal virtual float Range { get; set; }
        internal virtual MenuType[] Category { get; set; }

        internal virtual int DefaultMP { get; set; }
        internal virtual int DefaultHP { get; set; }

        public Menu Menu { get; private set; }
        public Obj_AI_Hero Player { get { return ObjectManager.Player; } }

        public Obj_AI_Base Target
        {
            get
            {
                return
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(h => h.IsValidTarget(Range) && !h.IsZombie)
                        .OrderBy(h => h.Distance(Game.CursorPos))
                        .FirstOrDefault();
            }
        }

        public double ActiveReduction
        {
            get
            {
                var inc = new[] { 0.04, 0.07, 0.1 };
                return
                    (from mastery in Player.Masteries
                        where mastery.Id == 131 && mastery.Page > 0
                        select inc[mastery.Points - 1]).FirstOrDefault();
            }
        }

        public void UseItem(bool combo = false)
        {
            if (!combo || Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                LeagueSharp.Common.Items.UseItem(Id);
            }
        }

        public void UseItem(Obj_AI_Base target, bool combo = false)
        {
            if (!combo || Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                LeagueSharp.Common.Items.UseItem(Id, target);
            }
        }

        public void UseItem(Vector3 pos, bool combo = false)
        {
            if (!combo || Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                LeagueSharp.Common.Items.UseItem(Id, pos);
            }
        }

        public void RemoveItem(bool temporarily = false)     
        {
            if (temporarily)
            {
                if (LeagueSharp.Common.Items.HasItem(Id) && !LeagueSharp.Common.Items.CanUseItem(Id))
                {
                    Utility.DelayAction.Add((int)(Cooldown - (Cooldown * ActiveReduction) + Game.Ping),
                        delegate
                        {
                            if (LeagueSharp.Common.Items.HasItem(Id))
                                Game.OnUpdate += OnTick;
                        });

                    Game.OnUpdate -= OnTick;
                }                 
            }

            else
            {
                if (!LeagueSharp.Common.Items.HasItem(Id))
                {
                    Game.OnUpdate -= OnTick;
                }
            }
        }

        public item CreateMenu(Menu root)
        {
            var usefname = DisplayName ?? Name;

            Menu = new Menu(Name, "m" + Name);
            Menu.AddItem(new MenuItem("use" + Name, "使用 " + usefname)).SetValue(true);

            if (Category.Any(t => t == MenuType.EnemyLowHP))
                Menu.AddItem(new MenuItem("EnemyLowHP" + Name + "Pct", "敌人 HP % <="))
                    .SetValue(new Slider(DefaultHP));

            if (Category.Any(t => t == MenuType.SelfLowHP))
                Menu.AddItem(new MenuItem("SelfLowHP" + Name + "Pct", "自己 HP % <="))
                    .SetValue(new Slider(Name == "Botrk" ? 70 : 50));

            if (Category.Any(t => t == MenuType.SelfMuchHP))
                Menu.AddItem(new MenuItem("SelfMuchHP" + Name + "Pct", "造成伤害 % >="))
                    .SetValue(new Slider(40));

            if (Category.Any(t => t == MenuType.SelfLowMP))
                Menu.AddItem(new MenuItem("SelfLowMP" + Name + "Pct", "自己 Mp % <="))
                    .SetValue(new Slider(DefaultMP));

            if (Category.Any(t => t == MenuType.SelfCount))
                Menu.AddItem(new MenuItem("SelfCount" + Name, "敌人数 >="))
                    .SetValue(new Slider(3, 1, 5));

            if (Category.Any(t => t == MenuType.SelfMinMP))
                Menu.AddItem(new MenuItem("SelfMinMP" + Name + "Pct", "最低蓝量 %")).SetValue(new Slider(40));

            if (Category.Any(t => t == MenuType.SelfMinHP))
                Menu.AddItem(new MenuItem("SelfMinHP" + Name + "Pct", "最低Hp %")).SetValue(new Slider(40));

            if (Category.Any(t => t == MenuType.Zhonyas))
            {
                Menu.AddItem(new MenuItem("use" + Name + "Norm", "危险使用 (技能)")).SetValue(false);
                Menu.AddItem(new MenuItem("use" + Name + "Ulti", "危险使用 (仅大招)")).SetValue(true);
            }

            if (Category.Any(t => t == MenuType.Cleanse))
            {
                Menu.AddItem(new MenuItem("use" + Name + "Number", "最低使用法术数量")).SetValue(new Slider(DefaultHP/5, 1, 5));
                Menu.AddItem(new MenuItem("use" + Name + "Time", "最低持续时间")).SetValue(new Slider(2, 1, 5)); ;
                Menu.AddItem(new MenuItem("use" + Name + "Od", "仅使用来自英雄的负面状态")).SetValue(false);
            }

            if (Category.Any(t => t == MenuType.ActiveCheck))
                Menu.AddItem(new MenuItem("mode" + Name, "模式: "))
                    .SetValue(new StringList(new[] { "Always", "Combo" }));

            root.AddSubMenu(Menu);
            return this;
        }

        public virtual void OnTick(EventArgs args)
        {

        }
    }
}
