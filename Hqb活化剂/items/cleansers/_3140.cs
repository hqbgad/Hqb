﻿using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Items.Cleansers
{
    class _3140 : item
    {
        internal override int Id
        {
            get { return 3140; }
        }

        internal override string Name
        {
            get { return "Quicksilver"; }
        }

        internal override string DisplayName
        {
            get { return "水银腰带"; }
        }

        internal override int Cooldown
        {
            get { return 90000; }
        }

        internal override float Range
        {
            get { return float.MaxValue; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.Cleanse, MenuType.ActiveCheck }; }
        }

        internal override int DefaultHP
        {
            get { return 5; }
        }

        internal override int DefaultMP
        {
            get { return 0; }
        }

        public override void OnTick(EventArgs args)
        {
            foreach (var hero in champion.Heroes)
            {
                if (!Menu.Item("use" + Name).GetValue<bool>())
                    return;

                if (hero.Player.NetworkId == Player.NetworkId && hero.Player.Distance(Player.ServerPosition) <= Range)
                {
                    if (hero.ForceQSS)
                    {
                        UseItem();
                        RemoveItem(true);
                    }

                    if (hero.QSSBuffCount >= Menu.Item("use" + Name + "Number").GetValue<Slider>().Value &&
                        hero.QSSHighestBuffTime >= Menu.Item("use" + Name + "Time").GetValue<Slider>().Value)
                    {
                        if (!Menu.Item("use" + Name + "Od").GetValue<bool>())
                        {
                            Utility.DelayAction.Add(Game.Ping + 80, delegate
                            {
                                UseItem(Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
                                if (!LeagueSharp.Common.Items.CanUseItem(Id))
                                {
                                    hero.QSSBuffCount = 0;
                                    hero.QSSHighestBuffTime = 0;
                                    RemoveItem(true);
                                }
                            });
                        }
                    }
                }
            }
        }
    }
}
