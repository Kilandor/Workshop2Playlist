using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;
using UnityEngine;
using ZeepkistClient;
using ZeepSDK.Multiplayer;

namespace Workshop2Playlist;
public partial class Utilities
{
    //Check to see if we are the host
    public static bool IsOnlineHost()
    {
        return ZeepkistNetwork.IsConnectedToGame && ZeepkistNetwork.IsMasterClient;
    }
    
    //quickly format chat messages
    public static Dictionary<string, string> formatChatMessage(string prefix, string message,
        bool prefixBrackets = true, string prefixColor = "#18a81e", string messageColor = "#FFC531")
    {
        if (prefixBrackets)
            prefix = "[<color=" + prefixColor + ">" + prefix + "</color>]";
        else
            prefix = "<color=" + prefixColor + ">" + prefix + "</color>";
        return new Dictionary<string, string>
        {
            { "prefix", prefix },
            { "message", "<color=" + messageColor + ">" + message + "</color>" }
        };
    }
    
    //handle working with On screen Messsages
    public static void sendMessenger(string message, float duration, LogLevel type, string prefix = "",  bool usePrefix = false)
    {
        Dictionary<string, string> chatMessage = formatChatMessage(prefix, message);
        chatMessage["message"] = (usePrefix ? string.Join(",", chatMessage) : chatMessage["message"]);
        if(type == LogLevel.Info)
            PlayerManager.Instance.messenger.LogCustomColor(chatMessage["message"], duration,Color.white, new Color32(40, 167, 69, 255));
        else if(type == LogLevel.Error)
            PlayerManager.Instance.messenger.LogError(chatMessage["message"], duration);
    }

    public static async Task<bool> addWorkshopItem(string workshopUrl)
    {
        string apiUrlBase = "https://zeepkist.kilandor.com/workshop2playlist/index.php?workshopUrl=";
        using HttpClient client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(60);
        try
        {
            // Fetch the API response
            HttpResponseMessage response = await client.GetAsync(apiUrlBase+Uri.EscapeDataString(workshopUrl));
            //response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            // Parse JSON
            JObject data = JObject.Parse(responseBody);

            // Extract Steam status
            string status = data["status"]?.ToString();
            if (status == "success")
            {
                JArray levels = (JArray?)data["levels"];
                if (levels != null)
                {
                    string lastAuthor = "";
                    string lastTrack = "";
                    foreach (JObject level in levels)
                    {
                        string uid = level["UID"]?.ToString() ?? "";
                        string author = level["Author"]?.ToString() ?? "";
                        string name = level["Name"]?.ToString() ?? "";
                        ulong workshopId = level["WorkshopID"]?.ToObject<ulong>() ?? 0;

                        // Call your function
                        PlaylistItem playlistItem = new PlaylistItem(uid, workshopId, name, author);
                        MultiplayerApi.AddLevelToPlaylist(playlistItem, false);

                        Log($"Added level {name} by {author} (UID={uid}, WorkshopID={workshopId})", LogLevel.Info);
                        lastAuthor = author;
                        lastTrack = name;
                    }
                    if(levels.Count > 1)
                        sendMessenger("Added "+levels.Count+" tracks by "+lastAuthor+" to the playlist.", Plugin.Instance.messengerDuration.Value, LogLevel.Info);
                    else
                        sendMessenger("Added "+lastTrack+" by "+lastAuthor+" to the playlist.", Plugin.Instance.messengerDuration.Value, LogLevel.Info);
                    MultiplayerApi.UpdateServerPlaylist();
                }
                return true;
            }
            else if (status == "error")
            {
                Log("Server responded error: "+data["message"]+"\n URL: "+workshopUrl, LogLevel.Error);
                sendMessenger(data["message"].ToString(), Plugin.Instance.messengerDuration.Value, LogLevel.Error);
                return false;
            }
            else
            {
                Log("Unknown Error \n URL: "+workshopUrl, LogLevel.Error);
                sendMessenger("Error adding playlist item, please check the log.", Plugin.Instance.messengerDuration.Value, LogLevel.Error);
                return false;
            }
        }
        catch (HttpRequestException e)
        {
            Log("Request error: "+e.Message+"\n URL: "+workshopUrl, LogLevel.Error);
            sendMessenger("Error adding playlist item, please check the log.", Plugin.Instance.messengerDuration.Value, LogLevel.Error);
            return false;
        }
        catch (Exception e)
        {
            Log("Unexpected error: "+e.Message+"\n URL: "+workshopUrl, LogLevel.Error);
            sendMessenger("Error adding playlist item, please check the log.", Plugin.Instance.messengerDuration.Value, LogLevel.Error);
            return false;
        }
    }
}