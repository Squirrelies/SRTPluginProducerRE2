using Microsoft.Extensions.Logging;
using SRTPluginBase;

namespace SRTPluginProducerRE2
{
    public partial class SRTPluginProducerRE2 : IPluginProducer
    {
        public const int PluginEventId = 9000;

        // Plugin events
        private const string PLUGIN_HTTPHANDLERASYNC_RECEIVED_EVENT_NAME = "Plugin HttpHandlerAsync Received";
        [LoggerMessage(PluginEventId + 0, LogLevel.Debug, "Plugin HTTP request received \"{route}\"", EventName = PLUGIN_HTTPHANDLERASYNC_RECEIVED_EVENT_NAME)]
        private partial void LogPluginHttpHandlerAsyncReceived(string? route);
    }
}
