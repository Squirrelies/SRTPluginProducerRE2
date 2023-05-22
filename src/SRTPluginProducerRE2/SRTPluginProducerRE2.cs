using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProcessMemory;
using SRTPluginBase;
using SRTPluginProducerRE2.JSONClasses.SRTPluginManager;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SRTPluginProducerRE2
{
    public partial class SRTPluginProducerRE2 : IPluginProducer
    {
        private readonly ILogger<SRTPluginProducerRE2> logger;
        private readonly IPluginHost pluginHost;

        // Properties
        public IPluginInfo Info => new PluginInfo();
        public object? Data { get; private set; }
        public DateTime? LastUpdated { get; private set; }

        // Fields
        private ProcessMemoryHandler? processMemoryHandler;
        private MultilevelPointer? playerHPPtr;

        public SRTPluginProducerRE2(ILogger<SRTPluginProducerRE2> logger, IPluginHost pluginHost)
        {
            this.logger = logger;
            this.pluginHost = pluginHost;

            Process? gameProc = Process.GetProcessesByName("re2")?.FirstOrDefault();
            IntPtr baseAddress = gameProc?.MainModule?.BaseAddress ?? IntPtr.Zero;
            uint pid = (uint)(gameProc?.Id ?? 0);
            if (pid != 0)
            {
                processMemoryHandler = new ProcessMemoryHandler(pid);
                unsafe
                {
                    playerHPPtr = new MultilevelPointer(processMemoryHandler, (nint*)(baseAddress + 0x091610D0), 0x50, 0x20);
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

        public async Task<IActionResult> HttpHandlerAsync(ControllerBase controller)
        {
            LogPluginHttpHandlerAsyncReceived(controller.Request.Path.Value);
            switch (controller.RouteData.Values["Command"] as string)
            {
                // Example of implementing custom http responses. This implementation may not be best practice, it is just here to illustrate the possible strength and possibilities.
                // GET: /api/v1/Plugin/SRTPluginProducerRE2/Info2
                // GET: /api/v1/Plugin/SRTPluginProducerRE2/Info2?Override=SRTPluginProviderSIGNALIS
                case "Info2":
                    {
                        string? pluginName = null;
                        Rootobject? jsonCfg = null;
                        try
                        {
                            jsonCfg = await System.Text.Json.JsonSerializer.DeserializeAsync<Rootobject>(await HttpClientFactory.Create(new HttpClientHandler()).GetStreamAsync(@"https://raw.githubusercontent.com/SpeedrunTooling/SRTPlugins/main/SRTPluginManager.cfg"));
                        }
                        catch { }

                        if (jsonCfg is not null)
                        {
                            pluginName = controller.Request.Query.ContainsKey("Override") ? controller.Request.Query["Override"].FirstOrDefault() : "SRTPluginProviderRE2";
                            Pluginconfig? pluginConfig = jsonCfg!.PluginConfig.Where(a => string.Equals(a.pluginName, pluginName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                            if (pluginConfig is not null)
                            {
                                controller.Response.ContentType = "application/json";
                                await System.Text.Json.JsonSerializer.SerializeAsync(controller.Response.Body, pluginConfig);
                                return controller.StatusCode((int)HttpStatusCode.OK);
                            }
                        }
                        
                        return controller.NotFound(pluginName);
                    }

                // Example of handling unknown http requests.
                // GET: /api/v1/Plugin/SRTPluginProducerRE2/rksjbvgjbaethkae
                default:
                    {
                        return controller.NotFound($"Unknown command: {((IDictionary<string, object>)controller.RouteData.Values)["Command"]}{Environment.NewLine}Parameters: {controller.Request.Query.Select(a => $"\"{a.Key}\"=\"{a.Value}\"").Aggregate((o, n) => $"{o}, {n}")}");
                    }
            }
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
