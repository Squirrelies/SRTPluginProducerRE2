using ProcessMemory;
using SRTPluginBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace SRTPluginProducerRE2
{
    public class SRTPluginProducerRE2 : IPluginProducer
    {
        // Public Properties
        public bool Available { get; private set; }
        public IPluginInfo Info => new PluginInfo();

        // Private Fields
        private ProcessMemoryHandler? processMemoryHandler;

        private MultilevelPointer? playerHPPtr;

        public int Startup()
        {
            Process? gameProc = Process.GetProcessesByName("re2")?.FirstOrDefault();
            IntPtr baseAddress = gameProc?.MainModule?.BaseAddress ?? IntPtr.Zero;
            uint pid = (uint)(gameProc?.Id ?? 0);
            if (pid != 0)
            {
                processMemoryHandler = new ProcessMemoryHandler(pid);
                unsafe
                {
                    playerHPPtr = new MultilevelPointer(processMemoryHandler, (nint*)(baseAddress + 0x09160F30), 0x50, 0x20);
                }
                Available = true;
                return 0;
            }
            return 1;
        }

        public int Shutdown()
        {
            Available = false;
            playerHPPtr = null;
            processMemoryHandler?.Dispose();
            processMemoryHandler = null;
            return 0;
        }

        public object? PullData()
        {
            if (Available && processMemoryHandler != null && playerHPPtr != null)
                return new { CurrentHP = playerHPPtr.DerefInt(0x58), MaxHP = playerHPPtr.DerefInt(0x54) };

            return null;
        }

        public object? CommandHandler(string command, KeyValuePair<string, string[]>[] arguments, out HttpStatusCode statusCode)
        {
            switch (command)
            {
                default:
                    {
                        statusCode = HttpStatusCode.NotImplemented;
                        return null;
                    }
            }
        }

        public bool Equals(IPlugin? other) => Equals(this, other);
        public bool Equals(IPluginProducer? other) => Equals(this, other);
    }
}
