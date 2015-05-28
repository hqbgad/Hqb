using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Activator.Spells
{
    public class spell
    {
        internal virtual string Name { get; set; }
        internal virtual string DisplayName { get; set; }
        internal virtual float Range { get; set; }
        internal virtual MenuType[] Category { get; set; }
        internal virtual int DefaultMP { get; set; }
        internal virtual int DefaultHP { get; set; }

        public Menu Menu { get; private set; }
        public SpellSlot Slot { get { return Player.GetSpellSlot(Name); } }
        public Spell Spell { get { return new Spell(Player.GetSpellSlot(Name)); } }
        public Obj_AI_Hero Player { get { return ObjectManager.Player; } }

        public Obj_AI_Hero LowTarget
        {
            get
            {
                return ObjectManager.Get<Obj_AI_Hero>()
                    .Where(x => x.IsValidTarget(Range))
                    .OrderBy(ene => ene.Health/ene.MaxHealth*100).First();
            }
        }

        public spell CreateMenu(Menu root)
        {
            if (Player.GetSpellSlot(Name) == SpellSlot.Unknown)
                return null;

            Menu = new Menu(DisplayName, "m" + Name);
            Menu.AddItem(new MenuItem("use" + Name, "使用 " + DisplayName)).SetValue(true);

            if (Category.Any(t => t == MenuType.Stealth))
                Menu.AddItem(new MenuItem("Stealth" + Name + "Pct", "保护!")).SetValue(true);

            if (Category.Any(t => t == MenuType.SlowRemoval))
                Menu.AddItem(new MenuItem("use" + Name + "SSR", "减速!")).SetValue(true);

            if (Category.Any(t => t == MenuType.EnemyLowHP))
                Menu.AddItem(new MenuItem("EnemyLowHP" + Name + "Pct", "敌人 HP % <="))
                    .SetValue(new Slider(DefaultHP));

            if (Category.Any(t => t == MenuType.SelfLowHP))
                Menu.AddItem(new MenuItem("SelfLowHP" + Name + "Pct", "英雄 HP % <="))
                    .SetValue(new Slider(DefaultHP));

            if (Category.Any(t => t == MenuType.SelfMuchHP))
                Menu.AddItem(new MenuItem("SelfMuchHP" + Name + "Pct", "英雄 伤害 % >="))
                    .SetValue(new Slider(55));

            if (Category.Any(t => t == MenuType.SelfLowMP))
                Menu.AddItem(new MenuItem("SelfLowMP" + Name + "Pct", "英雄 MP % <="))
                    .SetValue(new Slider(DefaultMP));

            if (Category.Any(t => t == MenuType.SelfCount))
                Menu.AddItem(new MenuItem("SelfCount" + Name, "附近数 >="))
                    .SetValue(new Slider(4, 1, 5));

            if (Category.Any(t => t == MenuType.SelfMinMP))
                Menu.AddItem(new MenuItem("SelfMinMP" + Name + "Pct", "最低蓝量 %")).SetValue(new Slider(40));

            if (Category.Any(t => t == MenuType.SelfMinHP))
                Menu.AddItem(new MenuItem("SelfMinHP" + Name + "Pct", "最低Hp %")).SetValue(new Slider(40));

            if (Category.Any(t => t == MenuType.SpellShield))
            {
                Menu.AddItem(new MenuItem("ss" + Name + "All", "友军技能")).SetValue(false);
                Menu.AddItem(new MenuItem("ss" + Name + "CC", "人数控制")).SetValue(true);
            }

            if (Category.Any(t => t == MenuType.Zhonyas))
            {
                Menu.AddItem(new MenuItem("use" + Name + "Norm", "使用在危险（技能）")).SetValue(false);
                Menu.AddItem(new MenuItem("use" + Name + "Ulti", "使用在危险（只有大招）")).SetValue(true);
            }

            if (Category.Any(t => t == MenuType.ActiveCheck))
                Menu.AddItem(new MenuItem("mode" + Name, "模式: "))
                    .SetValue(new StringList(new[] { "Always", "Combo" }));

            root.AddSubMenu(Menu);
            return this;
        }

        public void CastOnBestTarget(Obj_AI_Hero primary, bool nonhero = false)
        {
            foreach (var hero in champion.Heroes)
            {
                if (TargetSelector.GetPriority(primary) >= 2)
                {
                    UseSpellOn(hero.Attacker);
                    RemoveSpell();
                }

                else if (LowTarget != null)
                {
                    UseSpellOn(LowTarget);
                    RemoveSpell();
                }
            }
        }

        public void UseSpell(bool combo = false)
        {
            if (!combo || Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                if (Spell.IsReady())
                {
                    Spell.Cast();
                }
            }
        }

        public void UseSpellTowards(Vector3 targetpos, bool combo = false)
        {
            if (!combo || Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                if (Spell.IsReady())
                {
                    if (Spell.IsReady())
                    {
                        Spell.Cast(targetpos);
                    }
                }
            }
        }

        public void UseSpellOn(Obj_AI_Base target, bool combo = false)
        {
            if (!combo || Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                if (Spell.IsReady())
                {
                    if (Spell.IsReady())
                    {
                        Spell.CastOnUnit(target);
                    }
                }
            }
        }

        public void RemoveSpell()
        {
            //if (!Spell.IsReady())
            //{
            //    //Game.OnUpdate -= OnTick;
            //    //Console.WriteLine("STOPPED");
            //}
        }


        public virtual void OnTick(EventArgs args)
        {
     
        }
    }
}
