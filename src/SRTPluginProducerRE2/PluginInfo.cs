using SRTPluginBase;
using System;

namespace SRTPluginProducerRE2
{
    internal class PluginInfo : IPluginInfo
    {
        public string Name => "Game Memory Provider (Resident Evil 2 (2019))";

        public string Description => "A game memory provider plugin for Resident Evil 2 (2019).";

        public string Author => "Squirrelies";

        public Uri MoreInfoURL => new Uri("https://github.com/Squirrelies/SRTPluginProducerRE2");

        public int VersionMajor => assemblyVersion?.Major ?? 0;

        public int VersionMinor => assemblyVersion?.Minor ?? 0;

        public int VersionBuild => assemblyVersion?.Build ?? 0;

        public int VersionRevision => assemblyVersion?.Revision ?? 0;

        private readonly Version? assemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

        public bool Equals(IPluginInfo? other) => Equals(this, other);
    }
}
