using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Steamworks.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZeepkistClient;
using ZeepkistNetworking;
using ZeepSDK.Multiplayer;
using ZeepSDK.Scripting;
using ZeepSDK.Storage;

namespace Workshop2Playlist;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("ZeepSDK")]
[BepInDependency("Ztreamerbot")]
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
        
        //Register Zua types
        //ScriptingApi.RegisterType<List<PlaylistSaveJSON>>();
       
       //Register Zua Events
       //ScriptingApi.RegisterEvent<OnLobbyTimerEvent>();
       
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
            //PlayerManager.Instance.currentMaster.setupScript.PlayerInputs.
            // Example: log the message
            Utilities.Log($"Received StreamerBot message: {json}", LogLevel.Debug);

            // Optional: parse it if needed
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
                    Utilities.Log("Players " + PlayerManager.Instance.currentMaster.carSetups[0].cc.playerNum, LogLevel.Debug);
                    if (payloadAction == "addWorkshopItem")// && Plugin.Instance.disableSteeringEnabled.Value)
                    {
                        Utilities.addWorkshopItem(payloadValue);
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.Log($"[MyPlugin] Failed to parse JSON: {ex.Message}", LogLevel.Error);
            }
        }
    
}
