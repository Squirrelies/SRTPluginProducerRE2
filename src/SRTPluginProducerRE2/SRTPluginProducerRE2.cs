using Microsoft.AspNetCore.Mvc;
using ProcessMemory;
using SRTPluginBase;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SRTPluginProducerRE2
{
    public class SRTPluginProducerRE2 : IPluginProducer
    {
        // Properties
        public IPluginInfo Info => new PluginInfo();
        public IPluginHost? Host { get; set; }
        public object? Data { get; private set; }
        public DateTime? LastUpdated { get; private set; }

        // Fields
        private ProcessMemoryHandler? processMemoryHandler;
        private MultilevelPointer? playerHPPtr;

        public SRTPluginProducerRE2()
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
            }
        }

        public void Refresh()
        {
            if (processMemoryHandler != null && playerHPPtr != null)
            {
                Data = new { CurrentHP = playerHPPtr.DerefInt(0x58), MaxHP = playerHPPtr.DerefInt(0x54) };
                LastUpdated = DateTime.UtcNow;
            }
        }

        public IActionResult HttpHandler(ControllerBase controller)
        {
            return controller.NoContent();
        }

        public void Dispose()
        {
            playerHPPtr = null;
            processMemoryHandler?.Dispose();
            processMemoryHandler = null;
        }

        public async ValueTask DisposeAsync()
        {
            Dispose();
            await Task.CompletedTask;
        }

        public bool Equals(IPlugin? other) => Equals(this, other);
        public bool Equals(IPluginProducer? other) => Equals(this, other);
    }
}
