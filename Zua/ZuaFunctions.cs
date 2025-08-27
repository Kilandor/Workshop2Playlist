using System;
using ZeepSDK.Scripting.ZUA;

namespace Workshop2Playlist;

public class ZuaFunctions
{
    /// <summary>
    /// Closes the server
    /// </summary>
    public class AddWorkshopItem : ILuaFunction
    {
        /// <summary>
        /// Gets the namespace of the Lua function.
        /// </summary>
        public string Namespace => "Workshop2Playlist";

        /// <summary>
        /// Gets the name of the Lua function.
        /// </summary>
        public string Name => "AddWorkshopItem";

        /// <summary>
        /// Creates the delegate for the Lua function.
        /// </summary>
        public Delegate CreateFunction()
        {
            return new Action<string>(Implementation);
        }

        /// <summary>
        /// Implementation of the function to add a workshop item to playlist.
        /// </summary>
        /// /// <param name="workshopUrl">The full url to the steam workshop item.</param>
        private void Implementation(string workshopUrl)
        {
            if (!Utilities.IsOnlineHost() || !Plugin.Instance.modEnabled.Value)
            {
                return;
            }
            
            Utilities.Log("Zua Adding workshop item "+workshopUrl, Utilities.LogLevel.Info);

            Utilities.addWorkshopItem(workshopUrl);

        }
    }
}