using System;
using System.Collections.Generic;
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
            if (Game.MapId == GameMapId.SummonersRift) WallJump.InitSpots();
            InitEvents();
        }

        private static void InitEvents()
        {
            DamageIndicator.DamageToUnit = Damages.GetActualDamage;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnBuffLose += Obj_AI_Base_OnBuffLose;
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffGain;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            
            //Orbwalker.OnUnkillableMinion += Modes.OnUnkillableMinion;
        }

        static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (target == null || target.IsDead || !target.IsValid) return;
            if (!RendTargets.ContainsKey(target.NetworkId))
                RendTargets[target.NetworkId] = 0;
            RendTargets[target.NetworkId] += 1;
        }
        internal static Dictionary<int, int> RendTargets = new Dictionary<int, int>();
        private static void Obj_AI_Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (sender == null || sender.IsDead || !sender.IsValid) return;
            if (args.Buff.Name == "kalistaexpungemarker")
            {
                if (!RendTargets.ContainsKey(sender.NetworkId))
                    RendTargets[sender.NetworkId] = 0;
                RendTargets[sender.NetworkId] += 1;
            }
        }


        private static void Obj_AI_Base_OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (sender == null || sender.IsDead || !sender.IsValid) return;
            if (args.Buff.Name == "kalistaexpungemarker")
            {
                RendTargets.Remove(sender.NetworkId);
            }
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
                Orbwalker.ForcedTarget = null;
            }
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

            if (Config.DrawMenu.IsChecked("draw.percentage"))
            {

                foreach (var enemy in
                    EntityManager.Heroes.Enemies
                    .Where(x => Player.Instance.Distance(x) <= 2000f && !x.IsDead && x.IsVisible && x.HasRendBuff()))
                {
                    var percent = Math.Floor((Damages.GetActualDamage(enemy) / enemy.GetTotalHealth()) * 100);

                    if (percent >= 100 && !enemy.IsRendKillable())
                    {
                        Drawing.DrawText(enemy.HPBarPosition.X + 140, enemy.HPBarPosition.Y + 5, System.Drawing.Color.Red, "Spellshield/undying buff!", 20);
                    }
                    else
                    {
                        Drawing.DrawText(enemy.HPBarPosition.X + 140, enemy.HPBarPosition.Y + 5, System.Drawing.Color.White, 
                        enemy.IsRendKillable() ? "Killable!" : percent + "%", 20);
                    }
                }
            }

            if (Config.DrawMenu.IsChecked("draw.stacks"))
            {
                foreach (var enemy in
                    EntityManager.Heroes.Enemies
                    .Where(x => Player.Instance.Distance(x) <= 2000f && !x.IsDead && x.IsVisible && x.HasRendBuff()))
                {
                    var stacks = enemy.GetRendBuff().Count;
                    Drawing.DrawText(enemy.Position.X, enemy.Position.Y, System.Drawing.Color.White, "Stacks: " + stacks);
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
