using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using SharpDX;

namespace ScaryKalista
{
    class Modes
    {
        private static Vector2 _baron = new Vector2(5007.124f, 10471.45f);
        private static Vector2 _dragon = new Vector2(9866.148f, 4414.014f);

        public static void Combo()
        {
            var target = TargetSelector.GetTarget(Spells.Q.Range, DamageType.Physical);

            if (target == null || !target.IsValidTarget()) return;

            if (Config.ComboMenu.IsChecked("combo.useQ")
                && Spells.Q.IsReady()
                && !Player.Instance.IsDashing())
            {
                if (!(Player.Instance.ManaPercent < Config.ComboMenu.GetValue("combo.minManaQ")))
                {
                    Spells.Q.Cast(target);
                }
            }

            if (Config.ComboMenu.IsChecked("combo.useE") 
                && !Config.MiscMenu.IsChecked("misc.killstealE")
                && Spells.E.IsReady())
            {
                if (EntityManager.Heroes.Enemies.Any(x => x.IsRendKillable()))
                {
                    Spells.E.Cast();
                }
            }

            if (Config.ComboMenu.IsChecked("combo.gapClose"))
            {
                var gapCloseTarget = 
                    EntityManager.MinionsAndMonsters.CombinedAttackable
                        .FirstOrDefault(x => x.IsValidTarget(Player.Instance.GetAutoAttackRange()));

                Orbwalker.ForcedTarget = EntityManager.Heroes.Enemies.Any(x => Player.Instance.IsInAutoAttackRange(x)) ? null : gapCloseTarget;
            }
        }

        public static void Harass()
        {
            var target = TargetSelector.GetTarget(Spells.Q.Range, DamageType.Physical);

            if (target == null || !target.IsValidTarget()) return;

            if (Config.HarassMenu.IsChecked("harass.useQ") 
                && Spells.Q.IsReady() 
                && !Player.Instance.IsDashing()
                && !(Player.Instance.ManaPercent < Config.HarassMenu.GetValue("harass.minManaQ")))
            {
                Spells.Q.Cast(target);
            }
        }

        public static void LaneClear()
        {
            if (Player.HasBuff("summonerexhaust")
                || Player.Instance.ManaPercent < Config.LaneMenu.GetValue("laneclear.minMana"))
            {
                return;
            }

            var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(
                EntityManager.UnitTeam.Enemy, Player.Instance.Position, Spells.Q.Range).ToArray();

            if (!minions.Any()) return;

            if (Config.LaneMenu.IsChecked("laneclear.useQ"))
            {
                var qKillableMinions = minions.Where(x => x.GetTotalHealth() < Damages.GetQDamage(x)).ToArray();
                if (!qKillableMinions.Any()) return;

                var predictionResult =
                    (from minion in qKillableMinions
                     let pred = Spells.Q.GetPrediction(minion)
                     let count = pred.GetCollisionObjects<Obj_AI_Minion>().Count(x =>
                                    x.GetTotalHealth() < Damages.GetQDamage(x) 
                                    && x.IsValidTarget() && x.IsEnemy)
                     where count >= Config.LaneMenu.GetValue("laneclear.minQ")
                     where !Player.Instance.IsDashing()
                     select pred).FirstOrDefault();

                if (Spells.Q.IsReady() && predictionResult != null)
                {
                    Spells.Q.Cast(predictionResult.CastPosition);
                }
            }

            if (Config.LaneMenu.IsChecked("laneclear.useE") && Spells.E.IsReady())
            {
                var count = minions.Count(x => x.IsRendKillable() && x.Health > 10);
                if (count >= Config.LaneMenu.GetValue("laneclear.minE"))
                {
                    Spells.E.Cast();
                }
            }
        }

        public static void JungleClear()
        {
            if (Config.JungleMenu.IsChecked("jungleclear.useE")
                && !Config.MiscMenu.IsChecked("misc.junglestealE")
                && Spells.E.IsReady())
            {
                var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Spells.E.Range).ToArray();

                if (!Config.JungleMenu.IsChecked("jungleclear.miniE"))
                {
                    if (minions.Any(x => x.IsRendKillable() && !x.Name.Contains("Mini")))
                    {
                        Spells.E.Cast();
                    }
                }

                else if (minions.Any(x => x.IsRendKillable()))
                {
                    Spells.E.Cast();
                }
            }
        }

        public static void Flee()
        {
            if (Config.FleeMenu.IsChecked("flee.useJump") && Game.MapId == GameMapId.SummonersRift)
            {
                var spot = WallJump.GetJumpSpot();
                if (Spells.Q.IsReady() && spot != null)
                {
                    Orbwalker.DisableAttacking = true;
                    Orbwalker.DisableMovement = true;

                    WallJump.JumpWall();
                    return;
                }
            }

            if(Config.FleeMenu.IsChecked("flee.attack"))
            {
                Orbwalker.DisableAttacking = false;
                Orbwalker.DisableMovement = false;

                var target =
                    ObjectManager.Get<Obj_AI_Base>()
                    .FirstOrDefault(
                        x => 
                            x.IsValidTarget(Player.Instance.GetAutoAttackRange())
                            && !x.IsMe 
                            && !x.IsAlly);

                Orbwalker.ForcedTarget = target;
            }
        }

        public static void PermaActive()
        {
            if (Config.MiscMenu.IsChecked("misc.killstealE") && Spells.E.IsReady())
            {
                if (EntityManager.Heroes.Enemies.Any(x => x.IsRendKillable()))
                {
                    Spells.E.Cast();
                }
            }

            if (Config.MiscMenu.IsChecked("misc.junglestealE") && Spells.E.IsReady())
            {
                var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Spells.E.Range).ToArray();

                if (!Config.JungleMenu.IsChecked("jungleclear.miniE"))
                {
                    if (minions.Any(x => x.IsRendKillable() && !x.Name.Contains("Mini")))
                    {
                        Spells.E.Cast();
                    }
                }

                else if (minions.Any(x => x.IsRendKillable()))
                {
                    Spells.E.Cast();
                }
            }

            if (Config.MiscMenu.IsChecked("misc.harassEnemyE") && Spells.E.IsReady())
            {
                if (Player.HasBuff("summonerexhaust") || (Player.Instance.Mana - 40) < 40) return;

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) 
                    || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)
                    || (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && Config.MiscMenu.IsChecked("misc.harassEnemyECombo")))
                {
                    var minion =
                        EntityManager.MinionsAndMonsters.GetLaneMinions(
                            EntityManager.UnitTeam.Enemy, Player.Instance.Position, Spells.E.Range)
                            .Any(x => x.IsRendKillable() && x.Health > 10);

                    var hero =
                        EntityManager.Heroes.Enemies
                        .Any(x =>
                                x.HasRendBuff()
                                && !x.HasSpellShield()
                                && !x.HasUndyingBuff());

                    if (minion && hero)
                    {
                        Spells.E.Cast();
                    }
                }
            }

            if (Config.MiscMenu.IsChecked("misc.useR") && Spells.R.IsReady())
            {
                if (Kalista.Soulbound == null) return;

                if (Kalista.Soulbound.HealthPercent <= Config.MiscMenu.GetValue("misc.healthR")
                    && Kalista.Soulbound.IsValidTarget(Spells.R.Range)
                    && Kalista.Soulbound.CountEnemiesInRange(500) > 0)
                {
                    Spells.R.Cast();
                }
            }

            if (Config.SentinelMenu.IsActive("sentinel.castDragon") && Spells.W.IsReady())
            {
                if (Player.Instance.Distance(_dragon) <= Spells.W.Range)
                {
                    Spells.W.Cast(_dragon.To3DWorld());
                }
            }

            if (Config.SentinelMenu.IsActive("sentinel.castBaron") && Spells.W.IsReady())
            {
                if (Player.Instance.Distance(_baron) <= Spells.W.Range)
                {
                    Spells.W.Cast(_baron.To3DWorld());
                }
            }
            //Auto E before death
            if (Config.MiscMenu.IsChecked("misc.beforeDeath") )
            {
                if (Player.Instance.HealthPercent < Config.MiscMenu.GetValue("misc.beforedeathE") && EntityManager.Heroes.Enemies.Any(o => o.IsValidTarget() && o.HasRendBuff() && Spells.E.IsInRange(o)) && Spells.E.Cast())
                {
                    return;
                }
            }
        }

        public static void OnUnkillableMinion(Obj_AI_Base unit, Orbwalker.UnkillableMinionArgs args)
        {
            if (Player.HasBuff("summonerexhaust") 
                || (Player.Instance.Mana - 40) < 40 
                || !Spells.E.IsReady())
            {
                return;
            }

            if (Config.MiscMenu.IsChecked("misc.unkillableE") && unit.IsRendKillable())
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)
                    || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                {
                    Spells.E.Cast();
                }
            }
        }
    }
}
