using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZeepkistClient;

namespace Workshop2Playlist;

public class Patcher
{
     /*
    [HarmonyPatch(typeof(PressAnyKeyToStartGame), nameof(PressAnyKeyToStartGame.Awake))]
    public class SetupPressAnyKeyToStartGameAwake
    {
        public static void Postfix(PressAnyKeyToStartGame __instance)
        {
            if (Plugin.Instance.modEnabled.Value && (Application.isBatchMode || Plugin.Instance.batchOverride.Value))
            {
                //Disable input so you can't do anything
                //__instance.InputPlayer.DisableAllInput();
                PlayerManager.Instance.instellingen.Settings.audio_master = 0f;
                //Switch to Online Lobby Menu
                Utilities.Log("Switching to Main Menu - ModWarning");
                __instance.LoadNextLevel();
                //SceneManager.LoadScene(__instance.nextLevel);
                
            }
        }
    }
    */
    
}