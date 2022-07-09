using SRTPluginProducerRE2.Structs;
using SRTPluginProducerRE2.Structs.GameStructs;
using System;

namespace SRTPluginProducerRE2
{
    public interface IGameMemoryRE2
    {
        string GameName { get; }

        string VersionInfo { get; }

        GameTimer Timer { get; }

        CharacterEnumeration PlayerCharacter { get; }

        GamePlayer Player { get; }

        string PlayerName { get; }

        bool IsPoisoned { get; }

        GameRankManager RankManager { get; }

        int PlayerInventoryCount { get; }

        InventoryEntry[] PlayerInventory { get; }

        EnemyHP[] EnemyHealth { get; }

        long IGTCalculated { get; }

        long IGTCalculatedTicks { get; }

        TimeSpan IGTTimeSpan { get; }

        string IGTFormattedString { get; }
    }
}
