using EloBuddy;
using EloBuddy.SDK;

namespace ScaryKalista
{
    using System;
    using System.Linq;
    using Color = System.Drawing.Color;

    public class DamageIndicator
    {
        private static int _height;
        private static int _width;
        private static int _xOffset;
        private static int _yOffset;

        public static Color EnemyColor = Color.Lime;
        public static Color JungleColor = Color.White;

        private static DamageToUnitDelegate _damageToUnit;
        public delegate float DamageToUnitDelegate(Obj_AI_Base minion);

        public static DamageToUnitDelegate DamageToUnit
        {
            get
            {
                return _damageToUnit;
            }

            set
            {
                if (_damageToUnit == null)
                {
                    Drawing.OnEndScene += OnEndScene;
                }

                _damageToUnit = value;
            }
        }

        private static void OnEndScene(EventArgs args)
        {
            if (_damageToUnit == null) return;

            if (Config.DrawMenu.IsChecked("draw.enemyE"))
            {
                foreach (var hero in EntityManager.Heroes.Enemies
                    .Where(x => x.IsValidTarget()
                                && x.IsHPBarRendered
                                && x.HasRendBuff()))
                {
                    _height = 9;
                    _width = 104;
                    _xOffset = 2;
                    _yOffset = 9;

                    if (hero.ChampionName == "Jhin" || hero.ChampionName == "Annie")
                    {
                        _xOffset = -10;
                        _yOffset = -3;
                    }

                    DrawLine(hero);
                    DrawText(hero);
                }
            }
        
            if (Config.DrawMenu.IsChecked("draw.jungleE"))
            {
                foreach (
                var unit in
                EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Spells.E.Range)
                        .Where(x => x.IsValidTarget()
                                    && x.IsHPBarRendered
                                    && x.HasRendBuff()))
                {
                    if (unit.Name.Contains("Blue") || unit.Name.Contains("Red"))
                    {
                        _height = 9;
                        _width = 142;
                        _xOffset = -4;
                        _yOffset = 7;
                    }
                    else if (unit.Name.Contains("Dragon"))
                    {
                        _height = 10;
                        _width = 143;
                        _xOffset = -4;
                        _yOffset = 8;
                    }
                    else if (unit.Name.Contains("Baron"))
                    {
                        _height = 12;
                        _width = 191;
                        _xOffset = -29;
                        _yOffset = -320;
                    }
                    else if (unit.Name.Contains("Herald"))
                    {
                        _height = 10;
                        _width = 142;
                        _xOffset = -4;
                        _yOffset = 7;
                    }
                    else if ((unit.Name.Contains("Razorbeak") 
                        || unit.Name.Contains("Gromp") 
                        || unit.Name.Contains("Murkwolf") 
                        || unit.Name.Contains("Krug")) 
                        && !unit.Name.Contains("Mini"))
                    {
                        _width = 91;
                        _height = 3;
                        _xOffset = 21;
                        _yOffset = 7;
                    }
                    else if (unit.Name.Contains("Crab"))
                    {
                        _width = 61;
                        _height = 2;
                        _xOffset = 36;
                        _yOffset = 21;
                    }
                    else if (unit.Name.Contains("RazorbeakMini"))
                    {
                        _height = 3;
                        _width = 60;
                        _xOffset = 36;
                        _yOffset = 6;
                    }
                    else if (unit.Name.Contains("MurkwolfMini"))
                    {
                        _height = 3;
                        _width = 59;
                        _xOffset = 37;
                        _yOffset = 5;
                    }
                    else if (unit.Name.Contains("KrugMini"))
                    {
                        _height = 3;
                        _width = 59;
                        _xOffset = 37;
                        _yOffset = 62;
                    }
                    else
                    {
                        continue;
                    }

                    DrawLine(unit);
                    DrawText(unit);
                }
            }
        }

        private static void DrawLine(Obj_AI_Base unit)
        {
            var damage = _damageToUnit(unit);
            if (damage <= 0) return;

            var barPos = unit.HPBarPosition;

            //Get remaining HP after damage applied in percent and the current percent of health
            var percentHealthAfterDamage = Math.Max(0, unit.TotalShieldHealth() - damage) / unit.MaxHealth;
            var currentHealthPercentage = unit.TotalShieldHealth() / unit.MaxHealth;

            //Calculate start and end point of the bar indicator
            var startPoint = barPos.X + _xOffset + (percentHealthAfterDamage * _width);
            var endPoint = barPos.X + _xOffset + (currentHealthPercentage * _width);
            var yPos = barPos.Y + _yOffset;

            //Create a new transparent color based on the type of unit
            var color = unit is AIHeroClient ? EnemyColor : JungleColor;
            var transparentColor = Color.FromArgb(175, color);

            //Draw the line
            Drawing.DrawLine(startPoint, yPos, endPoint, yPos, _height, transparentColor);

            //Debug
            //Drawing.DrawLine(barPos.X + _xOffset, yPos, barPos.X + _xOffset + _width, yPos, _height, transparentColor);
        }

        private static void DrawText(Obj_AI_Base unit)
        {
            var damage = _damageToUnit(unit);
            if (damage <= 0) return;

            //Draw damage percentage
            if (Config.DrawMenu.IsChecked("draw.percentage")
                && (unit is AIHeroClient 
                || (unit.Name.Contains("Baron") || unit.Name.Contains("Dragon") || unit.Name.Contains("Herald") || unit.Name.Contains("Blue") || unit.Name.Contains("Red"))))
            {
                var textOffsetX = 40;
                var textOffsetY = 4;

                if (!(unit is AIHeroClient))
                {
                    textOffsetX = 20;
                    textOffsetY = 8;
                }

                var percent = Math.Floor((damage / unit.GetTotalHealth()) * 100);
                if (percent >= 100 && !unit.IsRendKillable())
                {
                    Drawing.DrawText(unit.HPBarPosition.X + _xOffset + _width + textOffsetX, unit.HPBarPosition.Y + _yOffset - textOffsetY,
                        Color.Red, "Can't kill!", 20);
                }
                else
                {
                    Drawing.DrawText(unit.HPBarPosition.X + _xOffset + _width + textOffsetX, unit.HPBarPosition.Y + _yOffset - textOffsetY,
                        Color.White, unit.IsRendKillable() ? "Killable!" : percent + "%", 20);
                }
            }
        }
    }
}
