using UnityEngine;
using System.Collections.Generic;

public class Json_MusicDataManager : MonoBehaviour , IJsonData<MusicData>
{
    public string FileFormattedName => "LastMusicList.json";
    MusicData musicData;
    
    [SerializeField] AudioData[] audioDatas;

    public void Init()
    {
        musicData = Load();

        if(musicData == null)
        {
            musicData = new MusicData();
            musicData.musicNames = GetDefaultMusicList();
            musicData.isLoaded = true;
        }

        Save();
    }
    
    public void Save()
    {
        if(musicData == null)
        {
            Utils.LogError("Save failed: MusicData is null");
            return;
        }

        JsonHelper.SaveToJson<MusicData>(musicData, FileFormattedName);
    }
    
    public MusicData Load() 
    {
        return JsonHelper.LoadFromJson<MusicData>(FileFormattedName);
    }

    /** MusicData를 교체하는것이 아닌 MusicNames만 교체 */
    public void UpdateData(MusicData data)
    {
        if(musicData == null)
        {
            Utils.LogError("Save failed: MusicData is null");
            return;
        }

        musicData.musicNames = data.musicNames;
    }

    List<string> GetDefaultMusicList()
    {
        // 초기 음악 목록 생성
        List<string> defaultMusicList = new List<string>();

        foreach(AudioData data in audioDatas)
        {
            defaultMusicList.Add(data.musicKey);
        }

        return defaultMusicList;
    }
}