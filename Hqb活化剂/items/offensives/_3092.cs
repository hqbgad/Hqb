﻿using System;
using LeagueSharp.Common;

namespace Activator.Items.Offensives
{
    class _3092 : item
    {
        internal override int Id 
        {
            get { return 3092; }
        }

        internal override string Name 
        {
            get { return "Queens"; }
        }

        internal override string DisplayName
        {
            get { return "冰霜女王的指令"; }
        }

        internal override float Range
        {
            get { return 850f; }
        }

        internal override int Cooldown
        {
            get { return 60000; }
        }

        internal override int DefaultHP
        {
            get { return 90; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.ActiveCheck, MenuType.SelfLowHP, MenuType.EnemyLowHP }; }
        }

        public override void OnTick(EventArgs args)
        {
            if (Menu.Item("use" + Name).GetValue<bool>() && Target != null)
            {
                if (Target.Health / Target.MaxHealth * 100 <= Menu.Item("EnemyLowHP" + Name + "Pct").GetValue<Slider>().Value)
                {
                    UseItem(Target.ServerPosition, Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
                    RemoveItem(true);
                }

                if (Player.Health / Player.MaxHealth * 100 <= Menu.Item("SelfLowHP" + Name + "Pct").GetValue<Slider>().Value)
                {
                    UseItem(Target.ServerPosition, Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
                    RemoveItem(true);
                }
            }
        }
    }
}
