using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Json_PlayerDataManager : MonoBehaviour , IJsonData<PlayerData>
{
    public string FileFormattedName => "PlayerData.json";
    
    PlayerData playerData;

    public void Init()
    {
        playerData = Load();
        if(playerData == null)
        {
            playerData = new PlayerData();
            playerData.isLoaded = true;
        }
        
        Save();
    }

    public void Save()
    {
        if(playerData == null)
        {
            Utils.LogError("Save failed: PlayerData is null");
            return;
        }

        JsonHelper.SaveToJson<PlayerData>(playerData, FileFormattedName);
    }

    public PlayerData Load()
    {
        return JsonHelper.LoadFromJson<PlayerData>(FileFormattedName);
    }

    public void UpdateData(PlayerData data)
    {
        if (data == null)
        {
            Utils.LogError("UpdateData failed: data is null");
            return;
        }

        playerData = data;
    }
}
