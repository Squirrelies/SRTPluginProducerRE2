using System.Runtime.InteropServices;

namespace SRTPluginProducerRE2.Structs.GameStructs
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x8)]

    public struct GamePlayer
    {
        [FieldOffset(0x0)] private int maxHP;
        [FieldOffset(0x4)] private int currentHP;

        public int CurrentHP => currentHP;
        public int MaxHP => maxHP;
        public float Percentage => CurrentHP > 0 ? (float)CurrentHP / (float)MaxHP: 0f;
        public bool IsAlive => CurrentHP != 0 && MaxHP != 0 && CurrentHP > 0 && CurrentHP <= MaxHP;
        public PlayerState HealthState
        {
            get =>
                !IsAlive ? PlayerState.Dead :
                CurrentHP >= 801 ? PlayerState.Fine :
                CurrentHP >= 401 ? PlayerState.Caution :
                PlayerState.Danger;
        }

        public string CurrentHealthState => HealthState.ToString();
    }

    public enum PlayerState
    {
        Dead,
        Fine,
        Caution,
        Danger
    }
}