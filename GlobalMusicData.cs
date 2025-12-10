using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GlobalMusicData : MonoBehaviour
{
    public static GlobalMusicData musicDataInstance;

    [SerializeField] AssetReferenceT<AudioClip>[] audioclips; // Reference를 통해 LoadAssetAsync를 하기 위해 
    public Dictionary<string,AudioClip> playListDic = new Dictionary<string, AudioClip>(); // 음악리스트로 string과 clip을 매핑
    public float LoadProgressRatio {get; private set;} // 업데이트 progress 상황
    public bool IsLoaded {get; private set;}
    WaitUntil waitUntilLoaded;

    void Awake() 
    {
        if(musicDataInstance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            musicDataInstance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public IEnumerator LoadMusicAssets()
    {
        yield return null;

        int loadCount = 0;
        foreach(var clip in audioclips)
        {
            clip.LoadAssetAsync<AudioClip>().Completed += handle =>
            {
                AudioClip loadedClip = handle.Result;
                playListDic.Add(loadedClip.name, loadedClip);
                loadCount++;
    
                LoadProgressRatio = (float)loadCount / audioclips.Length;

                if(loadCount == audioclips.Length)
                {
                    IsLoaded = true;
                }
            };
        }
    }



}
