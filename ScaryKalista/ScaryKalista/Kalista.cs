using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace ScaryKalista
{
    public static class Kalista
    {
        private static bool _fleeActivated;
        public static void OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Kalista") return;

            Config.Initialize();
            Spells.InitSpells();
            WallJump.InitSpots();
            InitEvents();
        }

        private static void InitEvents()
        {
            DamageIndicator.DamageToUnit = Damages.GetActualDamage;

            Game.OnUpdate += OnUpdate;
            Orbwalker.OnUnkillableMinion += Modes.OnUnkillableMinion;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.Instance.IsDead) return;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Modes.Combo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Modes.Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Modes.LaneClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Modes.JungleClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Modes.Flee();
                _fleeActivated = true;
            }

            Modes.PermaActive();

            if (_fleeActivated && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                _fleeActivated = false;
                Orbwalker.DisableAttacking = false;
                Orbwalker.DisableMovement = false;
            }

            DamageIndicator.EnemyEnabled = Config.DrawMenu.IsChecked("draw.enemyE");
            DamageIndicator.JungleEnabled = Config.DrawMenu.IsChecked("draw.jungleE");
        }

        private static void OnDraw(EventArgs args)
        {
            if (Config.DrawMenu.IsChecked("draw.Q"))
            {
                Circle.Draw(Color.DarkRed, Spells.Q.Range, Player.Instance.Position);
            }

            if (Config.DrawMenu.IsChecked("draw.W"))
            {
                Circle.Draw(Color.DarkRed, Spells.W.Range, Player.Instance.Position);
            }

            if (Config.DrawMenu.IsChecked("draw.E"))
            {
                Circle.Draw(Color.DarkRed, Spells.E.Range, Player.Instance.Position);
            }

            if (Config.DrawMenu.IsChecked("draw.R"))
            {
                Circle.Draw(Color.DarkRed, Spells.R.Range, Player.Instance.Position);
            }

            if (Config.DrawMenu.IsChecked("draw.killableMinions"))
            {
                foreach (var killableMinion in 
                    EntityManager.MinionsAndMonsters.GetLaneMinions(
                        EntityManager.UnitTeam.Enemy, Player.Instance.Position, Spells.E.Range)
                        .Where(x => x.IsRendKillable()))
                {
                    Circle.Draw(Color.GreenYellow, 25f, 2f, killableMinion.Position);
                }
            }

            if (Config.DrawMenu.IsChecked("draw.stacks"))
            {
                foreach (var enemy in
                    EntityManager.Heroes.Enemies
                    .Where(x => Player.Instance.Distance(x) <= 2000f && !x.IsDead && x.IsVisible))
                {
                    var stacks = enemy.HasRendBuff() ? enemy.GetRendBuff().Count : 0;
                    if (stacks > 0)
                    {
                        Drawing.DrawText(enemy.Position.X, enemy.Position.Y, System.Drawing.Color.White, "Stacks: " + stacks);
                    }
                }
            }

            if (Config.DrawMenu.IsChecked("draw.jumpSpots"))
            {
                foreach (var spot in WallJump.JumpSpots.Where(s => Player.Instance.Distance(s[0]) <= 2000))
                {
                    Circle.Draw(Color.DarkGray, 30f, spot[0]);
                }
            }
        }
    }
}
