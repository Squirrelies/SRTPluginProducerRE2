using SRTPluginProviderRE2.Structures;
using System;
using System.Globalization;

namespace SRTPluginProviderRE2
{
    public struct GameMemoryRE2 : IGameMemoryRE2
    {
        private const string IGT_TIMESPAN_STRING_FORMAT = @"hh\:mm\:ss\.fff";

        public int PlayerCurrentHealth { get => _playerCurrentHealth; }
        internal int _playerCurrentHealth;

        public int PlayerMaxHealth { get => _playerMaxHealth; }
        internal int _playerMaxHealth;

        public int PlayerDeathCount { get => _playerDeathCount; }
        internal int _playerDeathCount;

        public int PlayerInventoryCount { get => _playerInventoryCount; }
        internal int _playerInventoryCount;

        public InventoryEntry[] PlayerInventory { get => _playerInventory; }
        internal InventoryEntry[] _playerInventory;

        public EnemyHP[] EnemyHealth { get => _enemyHealth; }
        internal EnemyHP[] _enemyHealth;

        public long IGTRunningTimer { get => _igtRunningTimer; }
        internal long _igtRunningTimer;

        public long IGTCutsceneTimer { get => _igtCutsceneTimer; }
        internal long _igtCutsceneTimer;

        public long IGTMenuTimer { get => _igtMenuTimer; }
        internal long _igtMenuTimer;

        public long IGTPausedTimer { get => _igtPausedTimer; }
        internal long _igtPausedTimer;

        public int Difficulty { get => _difficulty; }
        internal int _difficulty;

        public int Rank { get => _rank; }
        internal int _rank;

        public float RankScore { get => _rankScore; }
        internal float _rankScore;

        public int Saves { get => _saves; }
        internal int _saves;

        public int MapID { get => _mapID; }
        internal int _mapID;

        public float FrameDelta { get => _frameDelta; }
        internal float _frameDelta;

        public bool IsRunning { get => _isRunning != 0x00; }
        internal byte _isRunning;

        public bool IsCutscene { get => _isCutscene != 0x00; }
        internal byte _isCutscene;

        public bool IsMenu { get => _isMenu != 0x00; }
        internal byte _isMenu;

        public bool IsPaused { get => _isPaused != 0x00; }
        internal byte _isPaused;

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

        public string DifficultyName
        {
            get
            {
                switch (Difficulty)
                {
                    case 0:
                        return "Assisted";
                    case 1:
                        return "Standard";
                    case 2:
                        return "Hardcore";
                    case 3:
                        return "Nightmare";
                    case 4:
                        return "Inferno";
                    default:
                        return "Unknown";
                }
            }
        }

        public string ScoreName
        {
            get
            {
                TimeSpan SRank;
                TimeSpan BRank;
                if (Difficulty == 0)
                {
                    SRank = new TimeSpan(0, 2, 30, 0);
                    BRank = new TimeSpan(0, 4, 0, 0);
                }
                else if (Difficulty == 1 || Difficulty == 3 || Difficulty == 4)
                {
                    SRank = new TimeSpan(0, 2, 0, 0);
                    BRank = new TimeSpan(0, 4, 0, 0);
                }
                else if (Difficulty == 2)
                {
                    SRank = new TimeSpan(0, 1, 45, 0);
                    BRank = new TimeSpan(0, 4, 0, 0);
                }

                if (IGTTimeSpan <= SRank && Saves <= 5)
                    return "S";
                else if (IGTTimeSpan <= SRank && Saves > 5)
                    return "A";
                else if (IGTTimeSpan > SRank && IGTTimeSpan <= BRank)
                    return "B";
                else if (IGTTimeSpan > BRank)
                    return "C";
                else
                    return string.Empty;
            }
        }
    }
}
