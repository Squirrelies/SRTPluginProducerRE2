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

        public int Startup()
        {
            return 0;
        }

        public int Shutdown()
        {
            return 0;
        }

        public object? PullData()
        {
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
