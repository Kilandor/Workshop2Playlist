using ZeepSDK.Chat;
using ZeepSDK.ChatCommands;

namespace Workshop2Playlist.Commands;

internal class NextRequestQueue : ILocalChatCommand
{
    public string Prefix => "/";
    public string Command => "w2p next";
    public string Description => "Sets the next request in the queue to be played.";
    public void Handle(string arguments)
    {
        Plugin.Core.nextRequestQueue();
    }
}