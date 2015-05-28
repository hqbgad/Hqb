using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Summoners
{
    public class summoner
    {
        internal virtual string Name { get; set; }
        internal virtual string DisplayName { get; set; }
        internal virtual string[] ExtraNames { get; set; }
        internal virtual float Range { get; set; }
        internal virtual int Cooldown { get; set; }
        internal virtual int DefaultMP { get; set; }
        internal virtual int DefaultHP { get; set; }

        public Menu Menu { get; private set; }
        public SpellSlot Slot { get { return Player.GetSpellSlot(Name); } }
        public Obj_AI_Hero Player { get { return ObjectManager.Player; } }

        public void UseSpell(bool combo = false)
        {
            if (!combo || Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                if (Player.GetSpell(Slot).State == SpellState.Ready)
                {
                    Player.Spellbook.CastSpell(Slot);
                }
            }
        }

        public void UseSpellOn(Obj_AI_Base target, bool combo = false)
        {
            if (!combo || Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                if (Player.GetSpell(Slot).State == SpellState.Ready)
                {
                    Player.Spellbook.CastSpell(Slot, target);
                }
            }
        }

        public double SummonerReduction
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

        public void RemoveSpell()
        {
            if (Player.GetSpell(Slot).State != SpellState.Ready)
            {
                Game.OnUpdate -= OnTick;
                Utility.DelayAction.Add((int) (Cooldown - (Cooldown*SummonerReduction) + Game.Ping),
                    () => Game.OnUpdate += OnTick);

                Console.WriteLine("STOPPED");
            }
        }

        public summoner CreateMenu(Menu root)
        {
            Menu = new Menu(DisplayName, "m" + Name);

            if (!Name.Contains("smite"))
                Menu.AddItem(new MenuItem("use" + Name, "使用 " + DisplayName)).SetValue(true);

            if (Name == "summonerheal")
            {
                Menu.AddItem(new MenuItem("SelfLowHP" + Name + "Pct", "自己 HP % <="))
                    .SetValue(new Slider(20));
                Menu.AddItem(new MenuItem("SelfMuchHP" + Name + "Pct", "自己 伤害 % >="))
                    .SetValue(new Slider(45));
            }

            if (Name == "summonerboost")
            {
                Menu.AddItem(new MenuItem("use" + Name + "Number", "最低技能数")).SetValue(new Slider(2, 1, 5));
                Menu.AddItem(new MenuItem("use" + Name + "Time", "最低持续时间")).SetValue(new Slider(2, 1, 5));
                Menu.AddItem(new MenuItem("use" + Name + "Od", "仅来自英雄的负面状态")).SetValue(false);
                Menu.AddItem(new MenuItem("mode" + Name, "模式: ")).SetValue(new StringList(new[] { "Always", "Combo" }));
            }

            if (Name == "summonerdot")
                Menu.AddItem(new MenuItem("mode" + Name, "模式: "))
                    .SetValue(new StringList(new[] { "Killsteal", "Killsteal" }, 1));

            if (Name == "summonermana")
                Menu.AddItem(new MenuItem("SelfLowMP" + Name + "Pct", "最低蓝量 % <=")).SetValue(new Slider(40));

            if (Name == "summonerbarrier")
            {
                Menu.AddItem(new MenuItem("SelfLowHP" + Name + "Pct", "自己 HP % <=")).SetValue(new Slider(20));
                Menu.AddItem(new MenuItem("SelfMuchHP" + Name + "Pct", "自己 伤害 % >=")).SetValue(new Slider(45));
                Menu.AddItem(new MenuItem("use" + Name + "Ulti", "危险技能(R)")).SetValue(true);
                Menu.AddItem(new MenuItem("use" + Name + "Dot", "被点燃")).SetValue(true);
            }

            if (Name == "summonerexhaust")
            {
                Menu.AddItem(new MenuItem("a" + Name + "Pct", "友军 HP %")).SetValue(new Slider(35));
                Menu.AddItem(new MenuItem("e" + Name + "Pct", "敌人 HP %")).SetValue(new Slider(35));
                Menu.AddItem(new MenuItem("use" + Name + "Ulti", "危险使用")).SetValue(true);
                Menu.AddItem(new MenuItem("mode" + Name, "模式: ")).SetValue(new StringList(new[] { "Always", "Combo" }));
            }

            if (Activator.SmiteInGame && Name == "summonersmite")
            {
                Menu.AddItem(new MenuItem("usesmite", "按键")).SetValue(new KeyBind('M', KeyBindType.Toggle, true));
                Menu.AddItem(new MenuItem("smitesmall", "小野怪")).SetValue(true);
                Menu.AddItem(new MenuItem("smitelarge", "大野怪")).SetValue(true);
                Menu.AddItem(new MenuItem("smitesuper", "史诗野怪")).SetValue(true);
                Menu.AddItem(new MenuItem("smitemode", "惩戒敌人: "))
                    .SetValue(new StringList(new[] { "Killsteal", "Combo", "Nope" }));
                Menu.AddItem(new MenuItem("savesmite", "保存惩戒").SetValue(true));    
            }

            root.AddSubMenu(Menu);
            return this;
        }

        public virtual void OnDraw(EventArgs args)
        {

        }

        public virtual void OnTick(EventArgs args)
        {

        }
    }
}
