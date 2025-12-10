using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TestScript : MonoBehaviour
{
    [SerializeField] AudioSource musicAudio;
    [SerializeField] AssetReferenceT<AudioClip>[] audioclip;

    AudioClip firstClip;
    void Start() 
    {
        
    }
    public void Button_SpawnMusic()
    {
        StartCoroutine(LoadAndPlayMusic());
    }
    IEnumerator LoadAndPlayMusic()
    {
        var handle = audioclip[0].LoadAssetAsync<AudioClip>();
        yield return handle;

        // 나머지 클립 로드
        for (int i = 1; i < audioclip.Length; i++)
        {
            audioclip[i].LoadAssetAsync<AudioClip>().Completed += h => {
                Utils.Log(h.Result.name);
            };
        }
    }

    public void Button_Release()
    {
        musicAudio.Stop();
        musicAudio.clip = null;

        for(int i=0; i < audioclip.Length; i++)
        {
            audioclip[i].ReleaseAsset();
        }
    }
}
