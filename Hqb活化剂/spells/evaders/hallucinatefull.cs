﻿using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Spells.Evaders
{
    class hallucinatefull : spell
    {
        internal override string Name
        {
            get { return "hallucinatefull"; }
        }

        internal override string DisplayName
        {
            get { return "Hallucinate | R"; }
        }

        internal override float Range
        {
            get { return float.MaxValue; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.Zhonyas }; }
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() ||
                Player.GetSpell(Slot).State != SpellState.Ready)
                return;

            foreach (var hero in champion.Heroes)
            {
                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (Menu.Item("use" + Name + "Norm").GetValue<bool>())
                    {
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                        {
                            UseSpell();
                            RemoveSpell();
                        }
                    }

                    if (Menu.Item("use" + Name + "Ulti").GetValue<bool>())
                    {
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                        {
                            UseSpell();
                            RemoveSpell();
                        }
                    }
                }
            }
        }
    }
}
