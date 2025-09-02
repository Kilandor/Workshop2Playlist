using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using ZeepSDK.Scripting;

namespace Workshop2Playlist;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("ZeepSDK")]
[BepInDependency("ZtreamerBot")]
public class Plugin : BaseUnityPlugin
{
    private Harmony harmony;
    public static Plugin Instance;
    public static ManualLogSource baseLogger;
    
    public ConfigEntry<bool> modEnabled;
    public ConfigEntry<bool> debugEnabled;
    
    public ConfigEntry<float> messengerDuration;
    public ConfigEntry<int> maxPackLimit;
    

    private void Awake()
    {
        harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll();
        
        Instance = this;
        baseLogger = Logger;
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
        maxPackLimit = Config.Bind("1. General", "Maximum pack import", 5,
            "The maximum number of tracks from a pack to import, will picked randomly.");
        
        debugEnabled = Config.Bind("9. Dev / Debug", "Debug Logs", false, "Provides extra output in logs for troubleshooting.");
        
    }
    
    public static void OnStreamerbotUDPEvent(string json)
    {
        if (!Utilities.IsOnlineHost() || !Plugin.Instance.modEnabled.Value)
            return;
    
        Utilities.Log($"Received StreamerBot message: {json}", Utilities.LogLevel.Debug);
        try
        {
            var parsed = JObject.Parse(json);
            foreach (var kvp in parsed)
            {
                Utilities.Log($"Key: {kvp.Key} => {kvp.Value}", Utilities.LogLevel.Debug);
            }

            string payloadType   = parsed["type"]?.ToString();
            string payloadAction = parsed["action"].ToString();
            string payloadValue  = parsed["value"]?.ToString();
            
            // Extract user-related fields for error handling
            string payloadUser = parsed["user"]?.ToString();
            string payloadRewardId = parsed["rewardId"]?.ToString();
            string payloadRedemptionId = parsed["redemptionId"]?.ToString();

        
            if (payloadType == "workshop2playlist")
            {
                Utilities.Log($"Type: {payloadType} | Action: {payloadAction}", Utilities.LogLevel.Debug);
                if (payloadAction == "addWorkshopItem")
                {
                    Utilities.addWorkshopItem(payloadValue, payloadUser, payloadRewardId, payloadRedemptionId);
                }
            }
        }
        catch (Exception ex)
        {
            Utilities.Log($"Failed to parse JSON: {ex.Message}", Utilities.LogLevel.Error);
        }
    }
    
}
