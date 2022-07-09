using System.Runtime.InteropServices;

namespace SRTPluginProducerRE2.Structs.GameStructs
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x20)]

    public struct GameTimer
    {
        [FieldOffset(0x0)] private long igtRunningTimer;
        [FieldOffset(0x8)] private long igtCutsceneTimer;
        [FieldOffset(0x10)] private long igtMenuTimer;
        [FieldOffset(0x18)] private long igtPausedTimer;

        public long IGTRunningTimer => igtRunningTimer;
        public long IGTCutsceneTimer => igtCutsceneTimer;
        public long IGTMenuTimer => igtMenuTimer;
        public long IGTPausedTimer => igtPausedTimer;
    }
}