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

        public string VersionInfo => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

        public GameTimer Timer { get => _timer; }
        internal GameTimer _timer;

        public CharacterEnumeration PlayerCharacter { get => (CharacterEnumeration)_playerCharacter; set => _playerCharacter = (int)value; }
        internal int _playerCharacter;

        public GamePlayer Player { get => _player; set => _player = value; }
        internal GamePlayer _player;

        public string PlayerName => string.Format("{0}: ", PlayerCharacter.ToString());

        public bool IsPoisoned { get => _isPoisoned == 0x01; }
        internal byte _isPoisoned;

        public GameRankManager RankManager { get => _rankManager; }
        internal GameRankManager _rankManager;

        public int PlayerInventoryCount { get => _playerInventoryCount; }
        internal int _playerInventoryCount;
        public InventoryEntry[] PlayerInventory { get => _playerInventory; }
        internal InventoryEntry[] _playerInventory;

        public EnemyHP[] EnemyHealth { get => _enemyHealth; }
        internal EnemyHP[] _enemyHealth;

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
