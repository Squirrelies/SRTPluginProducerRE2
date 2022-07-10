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
        // Private Constants
        private const long POINTER_UPDATE_POLLING_INTERVAL = 2000L; // Interval in milliseconds (ms) to wait before updating pointer paths.

        // Public Properties
        public bool Available { get; private set; }
        public IPluginInfo Info => new PluginInfo();

        // Private Fields
        private GameMemoryRE2Scanner? gameMemoryScanner;
        private Stopwatch? stopwatch;

        public int Startup()
        {
            Process? gameProcess = Process.GetProcessesByName("re2")?.FirstOrDefault();
            if (gameProcess is not null)
            {
                gameMemoryScanner = new GameMemoryRE2Scanner(gameProcess);
                stopwatch = new Stopwatch();
                stopwatch.Start();
                Available = true;
                return 0;
            }
            else
                return 1;
        }

        public int Shutdown()
        {
            Available = false;
            gameMemoryScanner?.Dispose();
            gameMemoryScanner = null;
            stopwatch?.Stop();
            stopwatch = null;
            return 0;
        }

        public object? PullData()
        {
            try
            {
                if (!Available) // Not available? Bail out!
                    return null;

                if (stopwatch?.ElapsedMilliseconds >= POINTER_UPDATE_POLLING_INTERVAL)
                {
                    gameMemoryScanner?.UpdatePointers();
                    stopwatch.Restart();
                }
                return gameMemoryScanner?.Refresh();
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (Win32Exception ex)
            {
                //if ((ProcessMemory.Win32Error)ex.NativeErrorCode != ProcessMemory.Win32Error.ERROR_PARTIAL_COPY)
                //    hostDelegates.ExceptionMessage(ex);// Only show the error if its not ERROR_PARTIAL_COPY. ERROR_PARTIAL_COPY is typically an issue with reading as the program exits or reading right as the pointers are changing (i.e. switching back to main menu).
                return null;
            }
            catch (Exception ex)
            {
                //hostDelegates.ExceptionMessage(ex);
                return null;
            }
#pragma warning restore CS0168 // Variable is declared but never used
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
