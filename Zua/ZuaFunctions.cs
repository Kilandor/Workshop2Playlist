using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FMOD.Studio;
using UnityEngine;
using ZeepkistClient;
using ZeepSDK.Scripting;
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
            if (!Utilities.IsOnlineHost())
            {
                return;
            }
            
            Utilities.Log("Zua Adding workshop item "+workshopUrl, LogLevel.Info);

            Utilities.addWorkshopItem(workshopUrl);

        }
    }
}