using SRTPluginProviderRE2.Structures;
using System;
using System.Globalization;

namespace SRTPluginProviderRE2
{
    public struct GameMemoryRE2 : IGameMemoryRE2
    {
        private const string IGT_TIMESPAN_STRING_FORMAT = @"hh\:mm\:ss\.fff";

        public long IGTRunningTimer { get => _igtRunningTimer; }
        internal long _igtRunningTimer;

        public long IGTCutsceneTimer { get => _igtCutsceneTimer; }
        internal long _igtCutsceneTimer;

        public long IGTMenuTimer { get => _igtMenuTimer; }
        internal long _igtMenuTimer;

        public long IGTPausedTimer { get => _igtPausedTimer; }
        internal long _igtPausedTimer;

        public int PlayerCurrentHealth { get => _playerCurrentHealth; }
        internal int _playerCurrentHealth;

        public int PlayerMaxHealth { get => _playerMaxHealth; }
        internal int _playerMaxHealth;

        public bool PlayerPoisoned { get => _playerPoisoned == 0x01; }
        internal byte _playerPoisoned;

        public int Rank { get => _rank; }
        internal int _rank;

        public float RankScore { get => _rankScore; }
        internal float _rankScore;

        public InventoryEntry[] PlayerInventory { get => _playerInventory; }
        internal InventoryEntry[] _playerInventory;

        public EnemyHP[] EnemyHealth { get => _enemyHealth; }
        internal EnemyHP[] _enemyHealth;

        // Public Properties - Calculated
        public long IGTCalculated => unchecked(IGTRunningTimer - IGTCutsceneTimer - IGTPausedTimer);

        public long IGTCalculatedTicks => unchecked(IGTCalculated * 10L);

        public TimeSpan IGTTimeSpan
        {
            get
            {
                TimeSpan timespanIGT;

                if (IGTCalculatedTicks <= TimeSpan.MaxValue.Ticks)
                    timespanIGT = new TimeSpan(IGTCalculatedTicks);
                else
                    timespanIGT = new TimeSpan();

                return timespanIGT;
            }
        }

        public string IGTFormattedString => IGTTimeSpan.ToString(IGT_TIMESPAN_STRING_FORMAT, CultureInfo.InvariantCulture);
    }
}
