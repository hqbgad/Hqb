﻿using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Activator
{
    public class gametroyhandler
    {
        public static void Load()
        {
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
        }

        private static void GameObject_OnCreate(GameObject obj, EventArgs args)
        {
            // if no troys dont itterate
            if (!Activator.TroysInGame)
                return;

            foreach (var troy in gametroy.Troys)
            {
                if (troy.Included)
                    return;

                // include the troy and start ticking 
                if (obj.Name.Contains(troy.Name))
                {
                    troy.Included = true;
                    troy.Obj = obj;
                    troy.Start = Environment.TickCount;
                    Game.OnUpdate += Game_OnUpdate;
                }
            }
        }

        private static void GameObject_OnDelete(GameObject obj, EventArgs args)
        {
            foreach (var hero in champion.Heroes)
            {
                // if no troys dont itterate
                if (!Activator.TroysInGame)
                    return;

                foreach (var troy in gametroy.Troys)
                {
                    if (!troy.Included)
                        return;

                    // delete the troy and stop ticking
                    if (obj.Name.Contains(troy.Name))
                    {
                        troy.Included = false;
                        troy.Start = 0;
                        hero.Attacker = null;
                        hero.IncomeDamage = 0f;
                        hero.HitTypes.Clear();
                        Game.OnUpdate -= Game_OnUpdate;
                    }
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            foreach (var hero in champion.Heroes)
            {
                // if no troys dont tick
                if (!Activator.TroysInGame)
                    return;

                foreach (var troy in gametroy.Troys)
                {
                    // check if troy is included and is enemy
                    if (!troy.Included || !troy.Owner.IsEnemy)
                        continue;

                    if (troy.Obj.IsValid && hero.Player.Distance(troy.Obj.Position) <= troy.Obj.BoundingRadius)
                    {
                        hero.Attacker = troy.Owner;
                        hero.IncomeDamage = (float) troy.Owner.GetSpellDamage(hero.Player, troy.Slot);

                        // detect danger/cc/ultimates from our db
                        foreach (var item in spelldata.troydata)
                        {
                            if (troy.Name != item.Name)
                                continue;

                            // spell is important or lethal!
                            if (item.HitType.Contains(HitType.Ultimate))
                                hero.HitTypes.Add(HitType.Ultimate);

                            // spell is important but not as fatal
                            if (item.HitType.Contains(HitType.Danger))
                                hero.HitTypes.Add(HitType.Danger);

                            // spell has a crowd control effect
                            if (item.HitType.Contains(HitType.CrowdControl))
                                hero.HitTypes.Add(HitType.CrowdControl);
                        }
                    }
                }
            }
        }
    }
}