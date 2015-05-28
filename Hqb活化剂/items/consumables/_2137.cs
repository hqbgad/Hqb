﻿using System;
using LeagueSharp.Common;

namespace Activator.Items.Consumables
{
    class _2137 : item
    {
        internal override int Id
        {
            get { return 2137; }
        }

        internal override string Name
        {
            get { return "Elixir of Ruin"; }
        }

        internal override float Range
        {
            get { return float.MaxValue; }
        }
        internal override MenuType[] Category
        {
            get { return new[] { MenuType.SelfLowHP, MenuType.SelfMuchHP }; }
        }

        internal override int DefaultHP
        {
            get { return 10; }
        }

        internal override int DefaultMP
        {
            get { return 0; }
        }

        public override void OnTick(EventArgs args)
        {
            foreach (var hero in champion.Heroes)
            {
                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (!Menu.Item("use" + Name).GetValue<bool>())
                        return;

                    if (hero.Player.IsRecalling() || hero.Player.InFountain())
                        return;

                    if (hero.Player.Health / hero.Player.MaxHealth*100 <= Menu.Item("SelfLowHP" + Name + "Pct").GetValue<Slider>().Value)
                    {
                        if (hero.IncomeDamage > 0)
                        {
                            UseItem();
                            RemoveItem();
                        }
                    }

                    if (hero.IncomeDamage / hero.Player.MaxHealth * 100 >=
                        Menu.Item("SelfMuchHP" + Name + "Pct").GetValue<Slider>().Value)
                    {
                        UseItem();
                        RemoveItem();
                    }
                }
            }
        }
    }
}
