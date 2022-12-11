namespace SRTPluginProducerRE2.JSONClasses.SRTPluginManager
{
    public class Rootobject
    {
        public Pluginconfig[] PluginConfig { get; set; }
        public Managerconfig ManagerConfig { get; set; }
        public Srtconfig SRTConfig { get; set; }
        public Extensionsconfig[] ExtensionsConfig { get; set; }
        public Interfaceconfig[] InterfaceConfig { get; set; }
    }

    public class Managerconfig
    {
        public int platform { get; set; }
        public string internalName { get; set; }
        public string pluginName { get; set; }
        public string currentVersion { get; set; }
        public string downloadURL { get; set; }
        public string[] contributors { get; set; }
    }

    public class Srtconfig
    {
        public int platform { get; set; }
        public string internalName { get; set; }
        public string pluginName { get; set; }
        public string currentVersion { get; set; }
        public string downloadURL { get; set; }
        public string[] contributors { get; set; }
    }

    public class Pluginconfig
    {
        public int platform { get; set; }
        public string internalName { get; set; }
        public string pluginName { get; set; }
        public string currentVersion { get; set; }
        public string downloadURL { get; set; }
        public string[] contributors { get; set; }
    }

    public class Extensionsconfig
    {
        public int platform { get; set; }
        public string internalName { get; set; }
        public string pluginName { get; set; }
        public string currentVersion { get; set; }
        public string downloadURL { get; set; }
        public string[] contributors { get; set; }
    }

    public class Interfaceconfig
    {
        public int platform { get; set; }
        public string internalName { get; set; }
        public string pluginName { get; set; }
        public string currentVersion { get; set; }
        public string downloadURL { get; set; }
        public string[] contributors { get; set; }
    }
}
