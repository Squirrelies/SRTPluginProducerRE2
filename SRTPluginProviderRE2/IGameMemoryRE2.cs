using SRTPluginProviderRE2.Structures;
using System;

namespace SRTPluginProviderRE2
{
    public interface IGameMemoryRE2
    {
        int PlayerCurrentHealth { get; }
        int PlayerMaxHealth { get; }
        int PlayerDeathCount { get; }
        int PlayerInventoryCount { get; }
        InventoryEntry[] PlayerInventory { get; }
        EnemyHP[] EnemyHealth { get; }
        long IGTRunningTimer { get; }
        long IGTCutsceneTimer { get; }
        long IGTMenuTimer { get; }
        long IGTPausedTimer { get; }
        int Difficulty { get; }
        int Rank { get; }
        float RankScore { get; }
        int Saves { get; }
        int MapID { get; }
        float FrameDelta { get; }
        bool IsRunning { get; }
        bool IsCutscene { get; }
        bool IsMenu { get; }
        bool IsPaused { get; }
        long IGTCalculated { get; }
        long IGTCalculatedTicks { get; }
        TimeSpan IGTTimeSpan { get; }
        string IGTFormattedString { get; }
        string DifficultyName { get; }
        string ScoreName { get; }
    }
}
