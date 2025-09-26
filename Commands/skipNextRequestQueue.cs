using ZeepSDK.Chat;
using ZeepSDK.ChatCommands;

namespace Workshop2Playlist.Commands;

internal class SkipNextRequestQueue : ILocalChatCommand
{
    public string Prefix => "/";
    public string Command => "w2p skip";
    public string Description => "Skips the next request in the queue and removes it.";
    public void Handle(string arguments)
    {
        Plugin.Core.skipNextRequestQueue();
        ChatApi.AddLocalMessage("W2P skipped next request in queue.");
    }
}