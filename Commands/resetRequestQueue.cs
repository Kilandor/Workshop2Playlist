using ZeepSDK.Chat;
using ZeepSDK.ChatCommands;

namespace Workshop2Playlist.Commands;

internal class ResetRequestQueue : ILocalChatCommand
{
    public string Prefix => "/";
    public string Command => "w2p reset";
    public string Description => "Resets the queue of requests, does not remove them from the playlist.";
    public void Handle(string arguments)
    {
        Plugin.Core.resetRequestQueue();
        ChatApi.AddLocalMessage("W2P request queue reset.");
    }
}