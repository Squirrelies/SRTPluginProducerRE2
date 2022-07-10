using System.Runtime.InteropServices;

namespace SRTPluginProducerRE2.Structs.GameStructs
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x20)]

    public struct GameTimer
    {
        [FieldOffset(0x0)] private readonly long igtRunningTimer;
        [FieldOffset(0x8)] private readonly long igtCutsceneTimer;
        [FieldOffset(0x10)] private readonly long igtMenuTimer;
        [FieldOffset(0x18)] private readonly long igtPausedTimer;

        public long IGTRunningTimer => igtRunningTimer;
        public long IGTCutsceneTimer => igtCutsceneTimer;
        public long IGTMenuTimer => igtMenuTimer;
        public long IGTPausedTimer => igtPausedTimer;
    }
}