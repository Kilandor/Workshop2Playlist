# Workshop2Playlist
Mod for Zeepkist to bring simplified track requests from the community to the host. Automatically adds them to the playlist without any need to any input from the host.

## Dependencies
ZeepSDK&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[Mod.io](https://mod.io/g/zeepkist/m/zeepsdk) - [Github](https://github.com/donderjoekel/ZeepSDK/)<br>
Room Service&nbsp;&nbsp;&nbsp;[Mod.io](https://mod.io/g/zeepkist/m/room-service) - [Github](https://github.com/Kilandor/RoomService)<br>
ZtreamerBot&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[Mod.io](https://mod.io/g/zeepkist/m/ztreamerbot) - [Github](https://github.com/Kilandor/ZtreamerBot)

## Features
- Auto add track to the playlist from workshop links<br />
  ![](https://zeepkist.kilandor.com/mods/workshop2playlist/images/added_tracks.png)
  ![](https://zeepkist.kilandor.com/mods/workshop2playlist/images/added_tracks_2.png)
- Integration with Ztreamer to allow for automatic redeems with channel points or commands. This is utilized through Streamerbot
- Integration with Zua for in game commands for submission
- Configure the maximum number of tracks to import from packs, then they are randomly selected
- Prevent duplicate tracks
- Queue the next requested track with a button in-game, Stream Deck, or streamer.bot action.

## Updating to a new version
If upgrading from 1.1.0 or earlier, you will need to follow steps **4-7**
Starting with version 1.2.0 you will no longer need to redo anything when updating Streamer.bot
The commands will disable when updated, so you will need to enable them again if you want to use them.


## Streamer.bot Setup
1. Import **[Workshop2Playlist.sb](Examples/Workshop2Playlist.sb)** this file can be found in examples. This will add the Actions and Commands.
2. Goto Platforms > Twitch > **Channel Point Rewards**
3. Create a new reward name doesn't matter, but leave the setting **Redemption Skips Queue** off, and **User Input Required** on.
4. Right-click on the new reward and choose **Copy Reward ID**
5. Goto Actions and click on **Setup W2P** then right-click on the **test trigger** and choose **Test Trigger**
6. Input the reward ID you copied in step 4 and press ok
7. Next choose between **pause** or **disable**, this effects how the reward is handled, pause keeps it enabled but prevents it being used, disable removes the reward till it's enabled again. 
8. Now goto Servers/Clients > UDP Server set **Port** to **12334** Recommended to set **Auto Start** to on, or you will have to manually start it. 
9. The default cooldown is 15 seconds this can be configured in **Global Variables** it is **w2p_cooldown** this value is in milliseconds (1000 = 1 second). If it doesn't show up you can goto Action > **Startup Disable Rewards** and run a test trigger it will create it. 
10. Now go to Commands and enable the **!toggleTrackRequest** command. It's set to allow moderators, you can use **!toggleTrackRequest**, **!toggletrackrequest** or **!ttr**. Track requests are auto-disabled on start-up or on starting a stream. 
11. Simply use **!ttr** to enable it and try it out

## Stream Deck Setup
1. Use the [Elgato Marketplace](https://marketplace.elgato.com/product/streamerbot-5c942a07-4bf6-4207-a2f2-f8599c398f2a) to install the Streamer.bot plugin.
2. In Streamer.bot goto Integrations > Elgato Stream Deck > Turn on **Auto Start** and press **Start Server**
3. In Stream Deck, add a Streamer.bot > **Status Indicator** button, and a **Action** button.
4. Edit the **Status Indicator** button and find **Streamer.bot: No Instance Found** click the ⚙️ button. Then simply click **New** and then just click **Save**. (you only need to do this for one button)
5. Now scroll to the bottom and find **Import** and then open **[Stream Deck.txt](Examples/StreamDeck.txt)** it has two sections find the **Track Queue** block and copy the data and paste it in, and choose import.
6. Repeat the same process in step 5, for the **Action** button, using the **Skip Next Track Request** data
6. Now simply request some tracks and try it out.

## Zua Setup
So this is more complex as it requires coding, but it is possible.<br>
This is an example script, it can also be found in the examples folder **[workshop2playlist.lua](Examples/workshop2playlist.lua)**<br>
Also without a mod such as **ChatUtils** the URL would have to be manually typed it currently.<br><br>
The ZUA function for this is **Workshop2Playlist.AddWorkshopItem(url)** this currently provides no response as the backend request has to operate asynchronous.

## Commands
- **/w2p reset** - This command clears the reset queue, and resets the information with next queue button, also for Streamer.bot and Stream Deck. This should only need to be used if you have loaded a new playlist but had requests remaining.
- **/w2p skip**  - Skips the next request in the queue and removes it.
- **/w2p next**  - Sets the next request in the queue to be played.
 