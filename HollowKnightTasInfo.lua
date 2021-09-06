local gameVersion
local markAddresses = {
    v1028 = {0x400000 + 0x1B1CF60, 0x400, 0x18, 0x20, 0xF78},
    v1221 = {0x400000 + 0x1B1CF60, 0x400, 0x18, 0x20, 0xF38},
    v1432_mod = {0x400000 + 0x2121D18, 0x0, 0x40, 0x4A0, 0x48, 0xA0, 0x68},
    v1432 = {0x400000 + 0x2115D28, 0x28, 0xB8, 0x10, 0x60, 0x238, 0xF48}
}

local lastSaveData = false

local fullpath = debug.getinfo(1,"S").source:sub(2)
local dirname, filename = fullpath:match('^(.*/)([^/]-)$')

function onPaint()
    local infoAddress = getInfoAddress()

    if infoAddress == 0 then
        return
    end

    local infoText = readString(infoAddress)
    local gameInfo = {}

    local generatorState
    local lastScene
    local newScene
    local transitionCount
    local saveData = false

    print("in onPaint")
    

    for line in infoText:gmatch("[^\r\n]+") do -- splits up infoText by newline characters (^ matches anything but \r and \n here)
        if line:find("^Enemy=") ~= nil then -- ^ matches to the start of the string here
            local hpData = splitString(line:sub(7), "|")
            for i = 1, #hpData, 3 do
                gui.text(hpData[i], hpData[i + 1], hpData[i + 2])
            end
        elseif line:find("^LineHitbox=") ~= nil then
            local hitboxData = splitString(line:sub(12), "|")
            for i = 1, #hitboxData, 5 do
                gui.line(hitboxData[i], hitboxData[i + 1], hitboxData[i + 2], hitboxData[i + 3], hitboxData[i + 4])
            end
        elseif line:find("^CircleHitbox=") ~= nil then
            local hitboxData = splitString(line:sub(14), "|")
            for i = 1, #hitboxData, 4 do
                gui.ellipse(hitboxData[i], hitboxData[i + 1], hitboxData[i + 2], hitboxData[i + 2], hitboxData[i + 3])
            end
        elseif line:find("^GeneratorState=") ~= nil then
            generatorState = splitString(line:sub(16), ",")
        elseif line:find("^LastScene=") ~= nil then
            lastScene = line:sub(11)
        elseif line:find("^NewScene=") ~= nil then
            newScene = line:sub(10)
        elseif line:find("^TransitionCount=") ~= nil then
            transitionCount = tonumber(line:sub(17))
        elseif line:find("^SaveData=") ~= nil then
            print("found savedata in memory")
            print(line)
            if line:sub(10) == "1" then
                saveData = true
            else
                saveData = false
            end
        else
            print("gameinfo:")
            print(line)
            table.insert(gameInfo, line)
        end
    end

    print("checking if new saveData")
    if not lastSaveData and saveData then
        local oldFile
        print("new savedata found")
        local rngFile = io.open(dirname .. "tas_rng.csv", "r")
        if rngFile ~= nil then
            oldFile = rngFile:read("a")
            rngFile:close()
        else
            oldFile = ""
        end

        
        rngFile = io.open(dirname .. "tas_rng.csv", "w+")
        local newFile = splitLines(oldFile)

        
        newFile[transitionCount] = "" .. lastScene .. "," .. newScene .. "," .. generatorState[1] .. "," ..
                                       generatorState[2] .. "," .. generatorState[3] .. "," .. generatorState[4]
        for i = 1, #newFile do
            rngFile:write(newFile[i], "\n")
        end
    end

    drawGameInfo(gameInfo)
    lastSaveData = saveData
end

function drawGameInfo(textArray)
    local screenWidth, screenHeight = gui.resolution()
    for i, v in ipairs(textArray) do
        gui.text(screenWidth, 23 * (i - 1), v)
    end
end

function readString(address)
    local text = {}
    local len = memory.readu16(address + 0x10)
    for i = 1, len do
        text[i] = string.char(memory.readu16(address + 0x12 + i * 2))
    end
    return table.concat(text)
end

function splitString(text, sep)
    if sep == nil then
        sep = "%s"
    end
    local t = {}
    for str in string.gmatch(text, "([^" .. sep .. "]+)") do
        table.insert(t, str)
    end
    return t
end

function splitLines(text)
    local t = {}
    for str in string.gmatch(text, "[^\r\n]+") do
        table.insert(t, str)
    end
    return t
end

function getInfoAddress()
    if gameVersion == nil then
        for k, v in pairs(markAddresses) do
            if getPointerAddress(v) == 1234567890123456789 then
                gameVersion = k;
                v[#v] = v[#v] + 0x8
            end
        end
    end

    if gameVersion ~= nil then
        return getPointerAddress(markAddresses[gameVersion])
    end

    return 0;
end

function getPointerAddress(offsets)
    local address = 0
    for i, v in ipairs(offsets) do
        address = memory.readu64(address + v)
        if address == 0 then
            return 0
        end
    end
    return address
end