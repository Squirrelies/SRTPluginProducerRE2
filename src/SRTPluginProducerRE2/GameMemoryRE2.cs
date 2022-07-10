using SRTPluginProducerRE2.Structs;
using SRTPluginProducerRE2.Structs.GameStructs;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace SRTPluginProducerRE2
{
    public struct GameMemoryRE2 : IGameMemoryRE2
    {
        private const string IGT_TIMESPAN_STRING_FORMAT = @"hh\:mm\:ss";

        public string GameName => "RE2R";

        public string? VersionInfo => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

        public GameTimer Timer { get => timer; }
        internal GameTimer timer;

        public CharacterEnumeration? PlayerCharacter { get => (CharacterEnumeration?)playerCharacter; set => playerCharacter = (int?)value; }
        internal int? playerCharacter;

        public GamePlayer Player { get => player; set => player = value; }
        internal GamePlayer player;

        public string? PlayerName => string.Format("{0}: ", PlayerCharacter?.ToString());

        public bool? IsPoisoned { get => isPoisoned == 0x01; }
        internal byte? isPoisoned;

        public GameRankManager RankManager { get => rankManager; }
        internal GameRankManager rankManager;

        public int? PlayerInventoryCount { get => playerInventoryCount; }
        internal int? playerInventoryCount;
        public InventoryEntry[]? PlayerInventory { get => playerInventory; }
        internal InventoryEntry[]? playerInventory;

        public EnemyHP[]? EnemyHealth { get => enemyHealth; }
        internal EnemyHP[]? enemyHealth;

        // Public Properties - Calculated
        public long IGTCalculated => unchecked(Timer.IGTRunningTimer - Timer.IGTCutsceneTimer - Timer.IGTPausedTimer);

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
