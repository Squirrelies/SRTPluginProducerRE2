using SRTPluginProviderRE2.Structures;
using System;

namespace SRTPluginProviderRE2
{
    public interface IGameMemoryRE2
    {
        long IGTRunningTimer { get; }
        long IGTCutsceneTimer { get; }
        long IGTMenuTimer { get; }
        long IGTPausedTimer { get; }
        int PlayerCurrentHealth { get; }
        int PlayerMaxHealth { get; }
        bool PlayerPoisoned { get; }
        int Rank { get; }
        float RankScore { get; }
        InventoryEntry[] PlayerInventory { get; }
        EnemyHP[] EnemyHealth { get; }

        long IGTCalculated { get; }
        long IGTCalculatedTicks { get; }
        TimeSpan IGTTimeSpan { get; }
        string IGTFormattedString { get; }
    }
}
