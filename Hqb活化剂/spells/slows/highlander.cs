﻿using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Spells.Slows
{
    class highlander : spell
    {
        internal override string Name
        {
            get { return "highlander"; }
        }

        internal override string DisplayName
        {
            get { return "Highlander | R"; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.SlowRemoval, MenuType.ActiveCheck }; }
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() ||
                Player.GetSpell(Slot).State != SpellState.Ready)
                return;

            if (Player.HasBuffOfType(BuffType.Slow) && Menu.Item("use" + Name + "SSR").GetValue<bool>())
            {
                UseSpell(Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
            }
        }
    }
}
