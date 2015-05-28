﻿using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator.Summoners
{
    internal class smite : summoner
    {
        internal override string Name
        {
            get { return "summonersmite"; }
        }

        internal override string DisplayName
        {
            get { return "惩戒"; }
        }

        internal override float Range
        {
            get { return 500f; }
        }

        internal static readonly string[] SmallMinions =
        {
            "SRU_Murkwolf",
            "SRU_Razorbeak",
            "SRU_Krug",
            "SRU_Gromp"
        };

        internal static readonly string[] EpicMinions =
        {
            "TT_Spiderboss",
            "SRU_Baron",
            "SRU_Dragon"
        };


        internal static readonly string[] LargeMinions =
        {
            "SRU_Blue",
            "SRU_Red",
            "TT_NWraith",
            "TT_NGolem",
            "TT_NWolf"
        };

        internal override string[] ExtraNames
        {
            get
            {
                return new[]
                {
                    "s5_summonersmiteplayerganker", "s5_summonersmiteduel",
                    "s5_summonersmitequick", "itemsmiteaoe"
                };
            }
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("usesmite").GetValue<KeyBind>().Active)
                return;

            // smite hero blu/red
            if (Player.GetSpell(Activator.Smite).Name == "s5_summonersmiteduel" ||
                Player.GetSpell(Activator.Smite).Name == "s5_summonersmiteplayerganker")
            {
                if (!Menu.Item("savesmite").GetValue<bool>() ||
                     Menu.Item("savesmite").GetValue<bool>() && Player.GetSpell(Activator.Smite).Ammo > 1)
                {
                    // KS Smite
                    if (Menu.Item("smitemode").GetValue<StringList>().SelectedIndex == 0 &&
                        Player.GetSpell(Activator.Smite).Name == "S5_SummonerSmitePlayerGanker")
                    {
                        var firsthero =
                            ObjectManager.Get<Obj_AI_Hero>()
                                .First(h => h.IsValidTarget(500) && !h.IsZombie && h.Health <= 20 + 8*Player.Level);

                        Player.Spellbook.CastSpell(Activator.Smite, firsthero);
                    }

                    // Combo Smite
                    if (Menu.Item("smitemode").GetValue<StringList>().SelectedIndex == 1 ||
                        Player.GetSpell(Activator.Smite).Name == "S5_SummonerSmiteDuel")
                    {
                        if (Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
                        {
                            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>()
                                .Where(h => h.IsValidTarget(500) && !h.IsZombie)
                                .OrderBy(h => h.Distance(Game.CursorPos)))
                            {
                                Player.Spellbook.CastSpell(Activator.Smite, hero);
                            }
                        }
                    }
                }
            }

            // smite minion
            foreach (var minion in MinionManager.GetMinions(Player.Position, 500f, MinionTypes.All, MinionTeam.Neutral))
            {
                if (!minion.IsValidTarget(500))
                    return;

                var damage = (float) Player.GetSummonerSpellDamage(minion, Damage.SummonerSpell.Smite);

                if (LargeMinions.Any(name => minion.Name.StartsWith(name) && !minion.Name.Contains("Mini")))
                {
                    if (Menu.Item("smitelarge").GetValue<bool>() && minion.Health <= damage)
                    {
                        Player.Spellbook.CastSpell(Activator.Smite, minion);
                    }
                }

                else if (SmallMinions.Any(name => minion.Name.StartsWith(name) && !minion.Name.Contains("Mini")))
                {
                    if (Menu.Item("smitesmall").GetValue<bool>() && minion.Health <= damage)
                    {
                        Player.Spellbook.CastSpell(Activator.Smite, minion);
                    }
                }

                else if (EpicMinions.Any(name => minion.Name.StartsWith(name)))
                {
                    if (Menu.Item("smitesuper").GetValue<bool>() && minion.Health <= damage)
                    {
                        Player.Spellbook.CastSpell(Activator.Smite, minion);
                    }
                }
            }
        }


        public override void OnDraw(EventArgs args)
        {
            
        }
    }
}
