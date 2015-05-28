﻿using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Spells.Evaders
{
    class blackshield : spell
    {
        internal override string Name
        {
            get { return "blackshield"; }
        }

        internal override string DisplayName
        {
            get { return "Black Shield | E"; }
        }

        internal override float Range
        {
            get { return 750f; }
        }

        internal override MenuType[] Category
        {
            get { return new[] { MenuType.Zhonyas, MenuType.SpellShield }; }
        }

        internal override int DefaultHP
        {
            get { return 0; }
        }

        internal override int DefaultMP
        {
            get { return 0; }
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() ||
                Player.GetSpell(Slot).State != SpellState.Ready)
                return;

            foreach (var hero in champion.Heroes)
            {
                if (hero.Player.Distance(Player.ServerPosition) > Range)
                    return;


                if (Menu.Item("ss" + Name + "All").GetValue<bool>())
                {
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Spell))
                    {
                        UseSpellOn(hero.Player);
                        RemoveSpell();
                    }
                }

                if (Menu.Item("ss" + Name + "CC").GetValue<bool>())
                {
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.CrowdControl))
                    {
                        UseSpellOn(hero.Player);
                        RemoveSpell();
                    }
                }

                if (Menu.Item("use" + Name + "Norm").GetValue<bool>())
                {
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                    {
                        UseSpellOn(hero.Player);
                        RemoveSpell();
                    }
                }

                if (Menu.Item("use" + Name + "Ulti").GetValue<bool>())
                {
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                    {
                        UseSpellOn(hero.Player);
                        RemoveSpell();
                    }
                }
            }
        }
    }
}
