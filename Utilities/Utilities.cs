using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ZeepkistClient;
using ZeepSDK.Messaging;

namespace Workshop2Playlist;
public partial class Utilities
{
    public static ITaggedMessenger messenger = MessengerApi.CreateTaggedMessenger("W2P");
    private static readonly SemaphoreSlim messengerQueue = new SemaphoreSlim(1, 1);
    
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
    public static async Task sendMessenger(string message, float duration, LogLevel type)
    {
        // Ensure only one call runs at a time
        await messengerQueue.WaitAsync();
        try
        {
            if (type == LogLevel.Info)
                messenger.LogSuccess(message, duration);
            else if (type == LogLevel.Warning)
                messenger.LogWarning(message, duration);
            else if (type == LogLevel.Error || type == LogLevel.Debug)
                messenger.LogError(message, duration);
            //wait duration of the message + 100ms to ensure it is gone
            await Task.Delay(((int)duration * (1000)+100)); 
        }
        finally
        {
            messengerQueue.Release();
        }
    }
}