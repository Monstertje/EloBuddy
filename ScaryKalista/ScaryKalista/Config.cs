using System.Linq;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace ScaryKalista
{
    public class Config
    {
        public static Menu Menu { get; private set; }
        public static Menu ComboMenu { get; private set; }
        public static Menu HarassMenu { get; private set; }
        public static Menu LaneMenu { get; private set; }
        public static Menu JungleMenu { get; private set; }
        public static Menu FleeMenu { get; private set; }
        public static Menu MiscMenu { get; private set; }
        public static Menu DrawMenu { get; private set; }
        public static Menu BalistaMenu { get; private set; }
        public static Menu ItemMenu { get; private set; }

        public static void Initialize()
        {
            var blitzcrank = EntityManager.Heroes.Allies.Any(x => x.ChampionName == "Blitzcrank");

            //Initialize the menu
            Menu = MainMenu.AddMenu("Scary Kalista", "ScaryKalista");
            Menu.AddGroupLabel("Welcome to Scary Kalista!");

            //Hellsing init
            // Misc
            Misc.Initialize();

            //Combo
            ComboMenu = Menu.AddSubMenu("Combo");
            {
                ComboMenu.Add("combo.useQ", new CheckBox("Use Q"));
                ComboMenu.Add("combo.minManaQ", new Slider("Mininum {0}% mana to use Q", 40));

                ComboMenu.Add("combo.sep1", new Separator());
                ComboMenu.Add("combo.useE", new CheckBox("Kill with E"));
                ComboMenu.Add("combo.gapClose", new CheckBox("Use minions/jungle to gap close"));
            }

            //Harass
            HarassMenu = Menu.AddSubMenu("Harass");
            {
                HarassMenu.Add("harass.useQ", new CheckBox("Use Q"));
                HarassMenu.Add("harass.minManaQ", new Slider("Mininum {0}% mana for Q", 60));
            }

            //LaneClear
            LaneMenu = Menu.AddSubMenu("LaneClear");
            {
                LaneMenu.Add("laneclear.useQ", new CheckBox("Use Q"));
                LaneMenu.Add("laneclear.minQ", new Slider("Mininum {0} minions for Q", 3, 2, 10));

                LaneMenu.Add("laneclear.sep1", new Separator());
                LaneMenu.Add("laneclear.useE", new CheckBox("Kill with E"));
                LaneMenu.Add("laneclear.minE", new Slider("Mininum {0} minions for E", 3, 2, 10));

                LaneMenu.Add("laneclear.sep2", new Separator());
                LaneMenu.Add("laneclear.minMana", new Slider("Mininum {0}% mana to LaneClear", 30));
            }

            //JungleClear
            JungleMenu = Menu.AddSubMenu("JungleClear");
            {
                JungleMenu.Add("jungleclear.useE", new CheckBox("Kill jungle camps with E"));
                JungleMenu.Add("jungleclear.miniE", new CheckBox("Kill mini jungle monsters with E", false));
            }

            //Flee
            FleeMenu = Menu.AddSubMenu("Flee");
            {
                FleeMenu.Add("flee.attack", new CheckBox("Attack champions/minions/monsters"));
                FleeMenu.Add("flee.useJump", new CheckBox("Jump walls with Q on jump spots"));
            }

            //Misc
            MiscMenu = Menu.AddSubMenu("Misc");
            {
                MiscMenu.Add("misc.labelSteal", new Label("Stealing: you don't have to hold any button"));
                MiscMenu.Add("misc.killstealE", new CheckBox("Killsteal with E"));
                MiscMenu.Add("misc.junglestealE", new CheckBox("Junglesteal with E"));

                MiscMenu.Add("misc.sep1", new Separator());
                MiscMenu.Add("misc.unkillableE", new CheckBox("Kill unkillable minions with E"));

                MiscMenu.Add("misc.sep2", new Separator());
                MiscMenu.Add("misc.harassEnemyE", new CheckBox("Harass enemy with E when minion can die"));
                MiscMenu.Add("misc.harassEnemyECombo", new CheckBox("Do this also in combo", false));

                MiscMenu.Add("misc.sep3", new Separator());
                MiscMenu.Add("misc.castDragonW", new KeyBind("Send W to Dragon", false, KeyBind.BindTypes.HoldActive, 'U'));
                MiscMenu.Add("misc.castBaronW", new KeyBind("Send W to Baron/Rift Herald", false, KeyBind.BindTypes.HoldActive, 'I'));

                MiscMenu.Add("misc.sep4", new Separator());
                MiscMenu.Add("misc.useR", new CheckBox("Use R to save ally"));
                MiscMenu.Add("misc.healthR", new Slider("{0}% Health to save ally", 15, 5, 25));
                ////Hellsing Sentinel Usage
                //MiscMenu.Add("misc.sep5", new Separator());
                //MiscMenu.Add("misc.labelBETASentinel", new Label("Sentinel: Send your sentinal automatically to places, Thanks Hellsing!"));
                //MiscMenu.Add("misc.enableAutoSentinel", new CheckBox("Enable AutoSentinel"));
                //MiscMenu.Add("misc.noModeSentinal", new CheckBox("Only cast when not in a mode"));
                //MiscMenu.Add("misc.alertIfDamage", new CheckBox("Alert if sentinel takes damage"));
                ////AutoSentinel Locations
                //MiscMenu.Add("misc.sep6", new Separator());
                //MiscMenu.Add("misc.betaSentinel", new Label("Sentinel: Send your sentinal automatically to places, Thanks Hellsing!"));
                //MiscMenu.Add("misc.sendToBaron", new CheckBox("Send to baron"));
                //MiscMenu.Add("misc.sendToDragon", new CheckBox("Send to Dragon"));
                //MiscMenu.Add("misc.sendToMid", new CheckBox("Send to Mid"));
                //MiscMenu.Add("misc.sendToBlue", new CheckBox("Send to Blue"));
                //MiscMenu.Add("misc.sendToRed", new CheckBox("Send to Red"));
                //SentinelManager.RecalculateOpenLocations();



            }

            //Items
            ItemMenu = Menu.AddSubMenu("Items");
            {
                var cutlass = Items.BilgewaterCutlass;
                ItemMenu.Add("item." + cutlass.ItemInfo.Name, new CheckBox("Use " + cutlass.ItemInfo.Name));
                ItemMenu.Add("item." + cutlass.ItemInfo.Name + "MyHp", new Slider("Your HP lower than {0}%", 80));
                ItemMenu.Add("item." + cutlass.ItemInfo.Name + "EnemyHp", new Slider("Enemy HP lower than {0}%", 80));
                ItemMenu.Add("item.sep", new Separator());

                var bork = Items.BladeOfTheRuinedKing;
                ItemMenu.Add("item." + bork.ItemInfo.Name, new CheckBox("Use " + bork.ItemInfo.Name));
                ItemMenu.Add("item." + bork.ItemInfo.Name + "MyHp", new Slider("Your HP lower than {0}%", 80));
                ItemMenu.Add("item." + bork.ItemInfo.Name + "EnemyHp", new Slider("Enemy HP lower than {0}%", 80));
            }

            //Balista
            if (blitzcrank)
            {
                BalistaMenu = Menu.AddSubMenu("Balista");
                {
                    BalistaMenu.Add("balista.comboOnly", new CheckBox("Only use Balista in combo mode"));
                    BalistaMenu.Add("balista.distance", new Slider("Minimum distance between you and Blitzcrank: {0}", 400, 0, 1200));
                    BalistaMenu.Add("balista.sep", new Separator());
                    BalistaMenu.Add("balista.label", new Label("Use Balista for:"));
                    foreach (var enemy in EntityManager.Heroes.Enemies)
                    {
                        BalistaMenu.Add("balista." + enemy.ChampionName, new CheckBox(enemy.ChampionName));
                    }
                }
            }

            //Drawings
            DrawMenu = Menu.AddSubMenu("Drawings");
            {
                DrawMenu.Add("draw.Q", new CheckBox("Draw Q range"));
                DrawMenu.Add("draw.W", new CheckBox("Draw W range", false));
                DrawMenu.Add("draw.E", new CheckBox("Draw E range"));
                DrawMenu.Add("draw.R", new CheckBox("Draw R range"));
                DrawMenu.Add("draw.enemyE", new CheckBox("Draw E damage on enemy healthbar"));
                DrawMenu.Add("draw.percentage", new CheckBox("Draw E damage percentage enemy"));
                DrawMenu.Add("draw.jungleE", new CheckBox("Draw E damage on jungle healthbar"));
                DrawMenu.Add("draw.killableMinions", new CheckBox("Draw E killable minions"));
                DrawMenu.Add("draw.stacks", new CheckBox("Draw E stacks enemy", false));
                DrawMenu.Add("draw.jumpSpots", new CheckBox("Draw jump spots"));
                if (blitzcrank) DrawMenu.Add("draw.balista", new CheckBox("Draw Balista range"));
            }
        }
        //Hellsing
        public static class Misc
        {
            private static Menu Menu { get; set; }

            private static readonly CheckBox _killsteal;
            private static readonly CheckBox _bigE;
            private static readonly CheckBox _saveSoulbound;
            private static readonly CheckBox _secureE;
            private static readonly CheckBox _harassPlus;
            private static readonly Slider _autoBelowHealthE;
            private static readonly Slider _reductionE;

            public static bool UseKillsteal
            {
                get { return _killsteal.CurrentValue; }
            }
            public static bool UseEBig
            {
                get { return _bigE.CurrentValue; }
            }
            public static bool SaveSouldBound
            {
                get { return _saveSoulbound.CurrentValue; }
            }
            public static bool SecureMinionKillsE
            {
                get { return _secureE.CurrentValue; }
            }
            public static bool UseHarassPlus
            {
                get { return _harassPlus.CurrentValue; }
            }
            public static int AutoEBelowHealth
            {
                get { return _autoBelowHealthE.CurrentValue; }
            }
            public static int DamageReductionE
            {
                get { return _reductionE.CurrentValue; }
            }

            static Misc()
            {
                Menu = Config.Menu.AddSubMenu("Auto Sentinel");

                Menu.AddGroupLabel("Auto Sentinel");
                
                // Initialize other misc features
                Sentinel.Initialize();
            }

            public static void Initialize()
            {
            }

            public static class Sentinel
            {
                private static readonly CheckBox _enabled;
                private static readonly CheckBox _noMode;
                private static readonly CheckBox _alert;
                private static readonly Slider _mana;

                private static readonly CheckBox _baron;
                private static readonly CheckBox _dragon;
                private static readonly CheckBox _mid;
                private static readonly CheckBox _blue;
                private static readonly CheckBox _red;

                public static bool Enabled
                {
                    get { return _enabled.CurrentValue; }
                }
                public static bool NoModeOnly
                {
                    get { return _noMode.CurrentValue; }
                }
                public static bool Alert
                {
                    get { return _alert.CurrentValue; }
                }
                public static int Mana
                {
                    get { return _mana.CurrentValue; }
                }

                public static bool SendBaron
                {
                    get { return _baron.CurrentValue; }
                }
                public static bool SendDragon
                {
                    get { return _dragon.CurrentValue; }
                }
                public static bool SendMid
                {
                    get { return _mid.CurrentValue; }
                }
                public static bool SendBlue
                {
                    get { return _blue.CurrentValue; }
                }
                public static bool SendRed
                {
                    get { return _red.CurrentValue; }
                }

                static Sentinel()
                {
                    Menu.AddGroupLabel("Sentinel (W) usage. Ported from Hellsings Kalista by TheYasuoMain!");

                  
                    {
                        _enabled = Menu.Add("enabled", new CheckBox("Enabled"));
                        _noMode = Menu.Add("noMode", new CheckBox("Only use when no mode active"));
                        _alert = Menu.Add("alert", new CheckBox("Alert when sentinel is taking damage"));
                        _mana = Menu.Add("mana", new Slider("Minimum mana available when casting W ({0}%)", 40));

                        Menu.AddLabel("Send to the following locations (no specific order):");
                        (_baron = Menu.Add("baron", new CheckBox("Baron"))).OnValueChange += OnValueChange;
                        (_dragon = Menu.Add("dragon", new CheckBox("Dragon"))).OnValueChange += OnValueChange;
                        (_mid = Menu.Add("mid", new CheckBox("Mid lane brush"))).OnValueChange += OnValueChange;
                        (_blue = Menu.Add("blue", new CheckBox("Blue buff"))).OnValueChange += OnValueChange;
                        (_red = Menu.Add("red", new CheckBox("Red buff"))).OnValueChange += OnValueChange;
                        SentinelManager.RecalculateOpenLocations();
                    }
                }

                private static void OnValueChange(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
                {
                    SentinelManager.RecalculateOpenLocations();
                }
                public static void Initialize()
                {
                }
                //Hellsing


            }
        }
    }
}
        
    


    


