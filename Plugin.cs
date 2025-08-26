using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using ZeepSDK.Scripting;

namespace Workshop2Playlist;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("ZeepSDK")]
[BepInDependency("ZtreamerBot")]
public class Plugin : BaseUnityPlugin
{
    private Harmony harmony;
    public static Plugin Instance;
    public static ManualLogSource Logger;
    
    public ConfigEntry<bool> modEnabled;
    public ConfigEntry<bool> debugEnabled;
    
    public ConfigEntry<float> messengerDuration;
    

    private void Awake()
    {
        harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll();
        
        Instance = this;
        Logger = base.Logger;
        ConfigSetup();
        
        // Plugin startup logic
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        
        //Register Events
        ZtreamerBot.Plugin.Instance.StreamerBotUDPAction += OnStreamerbotUDPEvent;
        
       //Register Zua Functions
       ScriptingApi.RegisterFunction<ZuaFunctions.AddWorkshopItem>();

    }

    private void OnDestroy()
    {
        harmony?.UnpatchSelf();
        harmony = null;
    }
    
    private void ConfigSetup()
    {
        modEnabled = Config.Bind("1. General", "Plugin Enabled", true, "Is the plugin currently enabled?");
        messengerDuration = Config.Bind("1. General", "Messenger duration", 5f, "How long the message shows up for.");
        
        //hostName = Config.Bind("2. Host", "Hostname", "Lobby", "Hostname to use for messages for the server.");
        
        debugEnabled = Config.Bind("9. Dev / Debug", "Debug Logs", false, "Provides extra output in logs for troubleshooting.");
        
    }
    
    public static void OnStreamerbotUDPEvent(string json)
    {
        if (!Utilities.IsOnlineHost() || !Plugin.Instance.modEnabled.Value)
            return;
        
        Utilities.Log($"Received StreamerBot message: {json}", LogLevel.Debug);
        try
        {
            var parsed = ZtreamerBot.Plugin.Instance.ParseUDPPayload(json);
            parsed.TryGetValue("type", out var payloadTypeObj);
            parsed.TryGetValue("action", out var payloadActionObj);
            parsed.TryGetValue("value", out var payloadValueObj);

            string payloadType = payloadTypeObj?.ToString();
            string payloadAction = payloadActionObj?.ToString();
            string payloadValue = payloadValueObj?.ToString();

            if (payloadType == "workshop2playlist")
            {
                Utilities.Log($"Type: {payloadType} | Action: {payloadAction}", LogLevel.Debug);
                if (payloadAction == "addWorkshopItem")// && Plugin.Instance.disableSteeringEnabled.Value)
                {
                    Utilities.addWorkshopItem(payloadValue);
                }
            }
        }
        catch (Exception ex)
        {
            Utilities.Log($"Failed to parse JSON: {ex.Message}", LogLevel.Error);
        }
    }
    
}
