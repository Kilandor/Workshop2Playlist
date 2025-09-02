# Workshop2Playlist
Mod for Zeepkist to bring simplified track requests from the community to the host. Automatically adds them to the playlist without any need to any input from the host.

## Dependencies
ZeepSDK [Mod.io](https://mod.io/g/zeepkist/m/zeepsdk) - [Github](https://github.com/donderjoekel/ZeepSDK/)<br>
ZtreamerBot [Mod.io]() - [Github](https://github.com/Kilandor/ZtreamerBot)

## Features
- Auto add track to the playlist from workshop links<br />
![](https://zeepkist.kilandor.com/mods/workshop2playlist/images/added_tracks.png)
![](https://zeepkist.kilandor.com/mods/workshop2playlist/images/added_tracks_2.png)
- Integration wtih Ztreamer to allow for automatic redeems with channel points or commands. This is utiulized through Streamerbot
- Integration with Zua for ingame commands for submission

## Streamer.bot Setup
1. Import **Workshop2Playlist.streamerbot** this file can be found in examples. This will add the Actions and Commands.
2. Goto Platforms > Twitch > Channel Point Rewards
3. Create a new reward name doesn't matter, but leave the setting **Redemption Skips Queue** off, and **User Input Required** on.
4. Go to Actions > **Startup Disable Rewards** edit sub-action for **Reward Set Enabled State** chose your track request reward.
5. Now open **ToggleAddWorkshopItem** do the similar edit for **Reward Get Info** and **Reward Set Enabled** set these to your track reqeust reward.
6. Now goto Servers/Clients > UDP Server set **Port** to 12334 Recommended to set **Auto Start** to on, or you will have to manually start it.
7. The default cooldown is 15 seconds this can be configured in **Global Variables** it is **w2p_cooldown** this value is in miliseconds (1000 = 1 second). If it doesn't show up you can goto Action > **Startup Disable Rewards** and run a test trigger it will create it.
8. Now goto Commands and enable the **!toggleTrackRequest** command. Its set to allow moderators, you can use **!toggleTrackRequest**, **!toggletrackrequest** or **!ttr**. Track reqeust are auto-disabled on start up, or on starting a stream.
9. Simply use **!ttr** to enable it and try it out


## Zua Setup
So this is more complex as it requires coding, but it is possible.<br>
An example script can be found in **Examples\workshop2playlist.lua**<br>
Also without a mod such as **ChatUtils** the URL would have to be manually typed it currently.<br><br>
The ZUA function for this is **Workshop2Playlist.AddWorkshopItem(url)** this currently provides no response as the backend request has to operate asynchronous.