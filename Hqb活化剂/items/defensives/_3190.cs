﻿using System;
using LeagueSharp.Common;

namespace Activator.Items.Defensives
{
    class _3190 : item
    {
        internal override int Id
        {
            get { return 3190; }
        }

        internal override string Name
        {
            get { return "Locket"; }
        }

        internal override string DisplayName
        {
            get { return "鸟盾"; }
        }

        internal override int Cooldown
        {
            get { return 60000; }
        }

        internal override float Range
        {
            get { return 600f; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.SelfLowHP, MenuType.SelfMuchHP, MenuType.Zhonyas }; }
        }

        internal override int DefaultHP
        {
            get { return 50; }
        }

        internal override int DefaultMP
        {
            get { return 0; }
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>())
            {
                return;
            }

            foreach (var hero in champion.Heroes)
            {
                if (hero.Player.Distance(Player.ServerPosition) > Range)
                    return;

                if (Menu.Item("use" + Name + "Norm").GetValue<bool>())
                {
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                    {
                        UseItem();
                        RemoveItem(true);
                    }
                }

                if (Menu.Item("use" + Name + "Ulti").GetValue<bool>())
                {
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                    {
                        UseItem();
                        RemoveItem(true);
                    }
                }

                if (hero.Player.Health/hero.Player.MaxHealth*100 <=
                    Menu.Item("SelfLowHP" + Name + "Pct").GetValue<Slider>().Value)
                {
                    if (hero.IncomeDamage > 0)
                    {
                        UseItem();
                        RemoveItem(true);
                    }
                }

                if (hero.IncomeDamage / hero.Player.MaxHealth * 100 >=
                    Menu.Item("SelfMuchHP" + Name + "Pct").GetValue<Slider>().Value)
                {
                    UseItem();
                    RemoveItem(true);
                }
            }
        }
    }
}
