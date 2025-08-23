using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using ZeepkistClient;
using ZeepkistNetworking;
using ZeepSDK.Multiplayer;
using ZeepSDK.Playlist;

namespace Workshop2Playlist;

public class Playlist
{
    //Slightly modifed version of PlaylistMenu Function
    public static async void LoadThisPlaylist(string url)
    {
        await Task.Delay(3000);
        DirectoryInfo playlistDir =
            new DirectoryInfo(Directory.CreateDirectory(PlayerManager.GetTargetFolder() + "\\Zeepkist\\Playlists")
                .FullName);
        List<OnlineZeeplevel> thePlaylist = new List<OnlineZeeplevel>();
        PlaylistSaveJSON playlistSaveJson = Playlist.loadPlaylistJSON(Path.Combine(playlistDir.FullName, url));
        Utilities.Log("Loading Playlist " + Path.Combine(playlistDir.FullName, url));
        for (int index = 0; index < playlistSaveJson.UID.Count; ++index)
        {
            LevelScriptableObject level;
            if (LevelManager.Instance.TryGetLevel(playlistSaveJson.UID[index], out level))
            {
                if (level.IsAdventureLevel || level.WorkshopID != 0UL)
                    thePlaylist.Add(new OnlineZeeplevel()
                    {
                        Author = level.Author,
                        Name = level.Name,
                        Collaborators = level.Collaborators,
                        OverrideAuthorName = level.OverrideAuthorName,
                        UID = level.UID,
                        WorkshopID = level.WorkshopID
                    });
            }
        }
        if (playlistSaveJson.levels.Count > 0)
            thePlaylist = playlistSaveJson.levels;
        
        ZeepkistNetwork.CurrentLobby.CurrentPlaylistIndex = 0;
        ZeepkistNetwork.CurrentLobby.NextPlaylistIndex = (playlistSaveJson.shufflePlaylist) ? UnityEngine.Random.Range(0, thePlaylist.Count): 0;
        ZeepkistNetwork.CurrentLobby.Playlist = thePlaylist;
        ZeepkistNetwork.CurrentLobby.PlaylistRandom = playlistSaveJson.shufflePlaylist;
        ZeepkistNetwork.CurrentLobby.PlaylistTime = playlistSaveJson.roundLength;
        
        MultiplayerApi.UpdateServerPlaylist();
    }
    
    //Slightly modifed version of PlaylistMenu Function
    public static PlaylistSaveJSON loadPlaylistJSON(string url)
    {
        try
        {
            PlaylistSaveJSON playlistSaveJson = JsonUtility.FromJson<PlaylistSaveJSON>(File.ReadAllText(url));
            if (playlistSaveJson.roundLength < 60.0)
                playlistSaveJson.roundLength = 60.0;
            return playlistSaveJson;
        }
        catch
        {
            Utilities.Log("Failed to load playlist: " + url,  LogLevel.Error);
            return null;
        }
    }

    public static List<string> getAllPlaylistFiles()
    {
        List<string> fileNames = new List<string>();
        string playlistsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Zeepkist",
            "Playlists");
        
        if (!Directory.Exists(playlistsPath))
            return fileNames;

        String[] files = Directory.GetFiles(playlistsPath, "*.zeeplist");

        // Extract just the filenames
        foreach (string file in files)
        {
            fileNames.Add(Path.GetFileName(file));
        }

        return fileNames;
    }
    public static List<PlaylistSaveJSON> getALLPlaylists(int pageIndex = 1)
    {
        const int pageSize = 10;

        // Get all playlists
        IReadOnlyList<PlaylistSaveJSON> playlists = PlaylistApi.GetPlaylists();
        
        //Return the full list
        if(pageIndex == 0)
            return playlists as List<PlaylistSaveJSON>;
        
        int playlistsCount = playlists.Count;

        // Calculate the items to skip
        int skip = (pageIndex - 1) * pageSize;
        
        // Get the paginated subset
        var pagedLevels = playlists.Skip(skip)
            .Take(pageSize)
            .ToList();

        // Return new PlaylistSaveJSON with just the paginated results
        return pagedLevels;
    }
    
    public static PlaylistSaveJSON getPlaylistByIndex(int playlistIndex = 1)
    {
        // Get all playlists
        IReadOnlyList<PlaylistSaveJSON> playlists = PlaylistApi.GetPlaylists();
        
        if(playlistIndex >= 0  && playlistIndex <= playlists.Count)
            return playlists[playlistIndex];
        
        return new PlaylistSaveJSON();
    }
    
    public static void addToPlaylist()
    {
        /*
        try
        {
            
            if (ZeepkistNetwork.CurrentLobby == null)
                return -1;
            OnlineZeeplevel onlineZeepLevel = playlistItem.ToOnlineZeepLevel();
            ZeepkistNetwork.CurrentLobby.Playlist.
            ZeepkistNetwork.CurrentLobby.Playlist.Add(onlineZeepLevel);
            int index = ZeepkistNetwork.CurrentLobby.Playlist.Count - 1;

            if (setAsPlayNext)
            {
                ZeepkistNetwork.CurrentLobby.NextPlaylistIndex = index;
            }

            return index;
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(AddLevelToPlaylist)}: " + e);
            return -1;
        }
        */
    }
}