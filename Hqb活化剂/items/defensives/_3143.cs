﻿using System;
using LeagueSharp.Common;

namespace Activator.Items.Defensives
{
    class _3143 : item
    {
        internal override int Id
        {
            get { return 3143; }
        }

        internal override string Name
        {
            get { return "Randuins"; }
        }

        internal override string DisplayName
        {
            get { return "蓝盾"; }
        }

        internal override int Cooldown
        {
            get { return 60000; }
        }

        internal override float Range
        {
            get { return 500f; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.SelfLowHP, MenuType.SelfCount }; }
        }

        internal override int DefaultHP
        {
            get { return 35; }
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

            if (Player.CountEnemiesInRange(Range) >= Menu.Item("SelfCount" + Name).GetValue<Slider>().Value)
            {
                UseItem();
                RemoveItem(true);
            }
        }
    }
}
