using System.Runtime.InteropServices;

namespace SRTPluginProducerRE2.Structs.GameStructs
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x20)]

    public struct GameRankManager
    {
        [FieldOffset(0x0)] private int rank;
        [FieldOffset(0x4)] private float rankScore;

        public int Rank => rank;
        public float RankScore => rankScore;
    }
}