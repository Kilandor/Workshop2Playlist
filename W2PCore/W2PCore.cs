using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;
using ZeepkistClient;
using ZeepSDK.Level;
using ZeepSDK.Multiplayer;

namespace Workshop2Playlist;
public class RequestQueue
{
    public string name { get; set; }
    public string uid { get; set; }
    public int index { get; set; }

    public RequestQueue(string trackName, string trackUID, int playlistIndex)
    {
        name = trackName;
        uid = trackUID;
        index = playlistIndex;
    }
}
public class W2PCore
{
    List<RequestQueue> requestQueue = new List<RequestQueue>();
    private static readonly SemaphoreSlim updatePlaylistQueue = new SemaphoreSlim(1, 1);
    public async Task<bool> addWorkshopItem(string workshopUrl, string user = "", string rewardId = "", string redemptionId = "")
    {
        string apiUrlBase = "https://zeepkist.kilandor.com/workshop2playlist/index.php?workshopUrl=";
        Uri uri = new Uri(workshopUrl);
        var query = HttpUtility.ParseQueryString(uri.Query);
        
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
            
            // Build payload for Streamer.bot
            var argsObj = new JObject
            {
                ["user"] = user,
                ["rewardId"] = rewardId,
                ["redemptionId"] = redemptionId,
                ["workshopId"] = query["id"]
            };

            var redemptionPayload = new JObject
            {
                ["request"] = "DoAction",
                ["action"] = new JObject { ["name"] = "W2PHandleRedmptionStatus" },
                ["args"] = argsObj
            };
            
            if (status == "success")
            {
                JArray levels = (JArray?)data["levels"];
                if (levels != null)
                {
                    string lastAuthor = "";
                    string lastTrack = "";
                    
                    //get a list of all the UID's in the playlist
                    var playlistUids = ZeepkistNetwork.CurrentLobby.Playlist
                        .Select(level => level.UID)
                        .ToHashSet(); // For faster lookups

                    // use the list of UID's to remove any duplicates from the incomming list
                    for (int i = levels.Count - 1; i >= 0; i--)
                    {
                        var levelObj = levels[i] as JObject;
                        if (levelObj != null && levelObj["UID"] != null)
                        {
                            string uid = (string)levelObj["UID"];

                            if (playlistUids.Contains(uid))
                            {
                                levels.RemoveAt(i);
                            }
                        }
                    }

                    if (levels.Count == 0)
                    {
                        Utilities.sendMessenger("Track(s) already exist, no new ones added.", Plugin.Instance.messengerDuration.Value, Utilities.LogLevel.Error);
                        if (user != "" && rewardId != "" && redemptionId != "")
                        {
                            argsObj["status"] = false;
                            argsObj["reason"] = "duplicate";
                            ZtreamerBot.UDPBroadcast.Send(redemptionPayload);
                            return false;
                        }
                    }
                    
                    if (levels.Count > Plugin.Instance.maxPackLimit.Value)
                    {
                        while (levels.Count > Plugin.Instance.maxPackLimit.Value)
                        {
                            int index = UnityEngine.Random.Range(0, levels.Count);
                            levels.RemoveAt(index);
                        }
                    }
                    foreach (JObject level in levels)
                    {
                        string uid = level["UID"]?.ToString() ?? "";
                        string author = level["Author"]?.ToString() ?? "";
                        string name = level["Name"]?.ToString() ?? "";
                        ulong workshopId = level["WorkshopID"]?.ToObject<ulong>() ?? 0;

                        // Call your function
                        PlaylistItem playlistItem = new PlaylistItem(uid, workshopId, name, author);
                        MultiplayerApi.AddLevelToPlaylist(playlistItem, false);
                        requestQueue.Add(new(name, uid, ZeepkistNetwork.CurrentLobby.Playlist.Count - 1));

                        Utilities.Log($"Added level {name} by {author} (UID={uid}, WorkshopID={workshopId})", Utilities.LogLevel.Info);
                        lastAuthor = author;
                        lastTrack = name;
                    }
                    
                    if(levels.Count > 1)
                        Utilities.sendMessenger("Added "+levels.Count+" tracks by "+lastAuthor+" to the playlist.", Plugin.Instance.messengerDuration.Value, Utilities.LogLevel.Info);
                    else
                        Utilities.sendMessenger("Added "+lastTrack+" by "+lastAuthor+" to the playlist.", Plugin.Instance.messengerDuration.Value, Utilities.LogLevel.Info);
                   
                    
                    asyncUpdateServerPlaylist();
                    if (user != "" && rewardId != "" && redemptionId != "")
                    {
                        argsObj["status"] = true;
                        argsObj["queueCount"] = requestQueue.Count;
                        ZtreamerBot.UDPBroadcast.Send(redemptionPayload);
                    }
                    return true;
                }
            }
            else if (status == "error")
            {
                Utilities.Log("Server responded error: "+data["message"]+"\n URL: "+workshopUrl, Utilities.LogLevel.Error);
                Utilities.sendMessenger(data["message"].ToString(), Plugin.Instance.messengerDuration.Value, Utilities.LogLevel.Error);
            }
            else
            {
                Utilities.Log("Unknown Error \n URL: "+workshopUrl, Utilities.LogLevel.Error);
                Utilities.sendMessenger("Error adding playlist item, please check the log.", Plugin.Instance.messengerDuration.Value, Utilities.LogLevel.Error);
            }
            
            if (user != "" && rewardId != "" && redemptionId != "")
            {
                argsObj["status"] = false;
                ZtreamerBot.UDPBroadcast.Send(redemptionPayload);
            }

            return false;
        }
        catch (HttpRequestException e)
        {
            Utilities.Log("Request error: "+e.Message+"\n URL: "+workshopUrl, Utilities.LogLevel.Error);
            Utilities.sendMessenger("Error adding playlist item, please check the log.", Plugin.Instance.messengerDuration.Value, Utilities.LogLevel.Error);
            return false;
        }
        catch (Exception e)
        {
            Utilities.Log("Unexpected error: "+e.Message+"\n URL: "+workshopUrl, Utilities.LogLevel.Error);
            Utilities.sendMessenger("Error adding playlist item, please check the log.", Plugin.Instance.messengerDuration.Value, Utilities.LogLevel.Error);
            return false;
        }
    }

    public void queueNextRequest()
    {
        if (requestQueue.Count == 0)
        {
            Utilities.sendMessenger("Requested tracks list is empty.", Plugin.Instance.messengerDuration.Value, Utilities.LogLevel.Error);
            return;
        }

        RequestQueue request = requestQueue.First();

        if (ZeepkistNetwork.CurrentLobby.NextPlaylistIndex == request.index)
        {
            Utilities.sendMessenger("Next requested track is already queued.", Plugin.Instance.messengerDuration.Value, Utilities.LogLevel.Error);
            return;
        }

        MultiplayerApi.SetNextLevelIndex(request.index);
        Utilities.sendMessenger($"Requested track {request.name} queued next.", Plugin.Instance.messengerDuration.Value, Utilities.LogLevel.Error);
    }
    
    public static async Task asyncUpdateServerPlaylist()
    {
        // Ensure only one call runs at a time
        await updatePlaylistQueue.WaitAsync();
        try
        {
            MultiplayerApi.UpdateServerPlaylist();
            // The delay zeepkist has for playlist updates is around 3 seconds
            await Task.Delay(5000); 
        }
        finally
        {
            updatePlaylistQueue.Release();
        }
    }

    public void levelLoaded()
    {
        if(requestQueue.Count == 0)
            return;
        
        //find the index of the level in the queue ( if it exists)
        var request = requestQueue.FindIndex(r => r.uid == LevelApi.CurrentLevel.UID);
        
        if (request != null)
        {
            Utilities.Log(requestQueue[request].name+" played removed from queue.", Utilities.LogLevel.Info);
            requestQueue.RemoveAt(request);
            updateRequestQueueCount();
        }
    }

    public void updateRequestQueueCount()
    {
        var argsObj = new JObject
        {
            ["queueCount"] = requestQueue.Count
        };

        var queueCountPayload = new JObject
        {
            ["request"] = "DoAction",
            ["action"] = new JObject { ["name"] = "UpdateTrackQueueCount" },
            ["args"] = argsObj
        };
        ZtreamerBot.UDPBroadcast.Send(queueCountPayload);
    }
    
    public void resetRequestQueue()
    {
        requestQueue.Clear();
        updateRequestQueueCount();
    }
    
}