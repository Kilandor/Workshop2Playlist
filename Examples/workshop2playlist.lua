local loadMessage = "<color=green>Loaded Script</color>"
local unloadMessage = "<color=red>Unloaded Script</color>"
local hostName = "<color=#87CEEB>Workshop2Playlist</color>"

--This is called when we first load the script.
function OnLoad()
    Zua.ListenTo("ChatApi_OnChatMessageReceived")
    ChatApi.AddLocalMessage(loadMessage);
end

function OnUnload()
    ChatApi.AddLocalMessage(unloadMessage);
end


function ChatApi_OnChatMessageReceived(playerId, username, message)
    -- Command parsing section
    -- Remove <noparse> tags
    local clean_message = message:gsub("</?noparse>", "")

    -- Extract command and argument string
    local cmd, args = clean_message:match("^(%S+)%s*(.*)$")
    cmd = string.lower(cmd)
    if not cmd then
        return
    end

    local parsed_args = {}
    -- Match quoted arguments OR normal words while keeping order
    for quoted, non_quoted in ('""'..args):gmatch'(%b"")([^"]*)' do
        table.insert(parsed_args, quoted ~= '""' and quoted:sub(2,-2) or nil)
        for word in non_quoted:gmatch'%S+' do
            table.insert(parsed_args, word)
        end
    end
    if cmd == "!trackrequest" then
        if #parsed_args < 1 then
            RoomService.SendPrivateChatMessage(playerId, hostName, "Invalid Argument, Should be a steam Workshop URL")
        else
            RoomService.SendPrivateChatMessage(playerId, hostName, "URL: "..parsed_args[1])
            Workshop2Playlist.AddWorkshopItem(parsed_args[1])
        end
    end
end