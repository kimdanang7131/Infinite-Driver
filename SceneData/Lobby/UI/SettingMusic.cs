using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SettingMusic : MonoBehaviour
{
    //[SerializeField] AudioClip music;
    [SerializeField] AudioData musicData; // ScriptableObject로 관리하는 음악 데이터
    [SerializeField] TextMeshProUGUI musicNameText;

    [SerializeField] Button selectMusicButton;

    public string MusicName => musicData.musicKey; // music이 없다면 null, 아니면 name

    void Awake()
    {
        selectMusicButton.onClick.AddListener(SelectMusic);
    }

    void Start()
    {
        //if(music != null)
        //{
        //    var localizedString = new LocalizedString("MusicTable", music.name);
        //    localizedString.StringChanged += (value) => musicNameText.text = value;
        //}

        if(!string.IsNullOrEmpty(MusicName))
        {
            var localizedString = new LocalizedString("MusicTable", MusicName);
            localizedString.StringChanged += (value) => musicNameText.text = value;
        }
    }

    void OnDestroy()
    {
        selectMusicButton.onClick.RemoveAllListeners();
    }

    public void PlayMusic()
    {
        //SoundManager.soundInstance.PlaySound(SoundType.Music, music);

        Addressables.LoadAssetAsync<AudioClip>(MusicName).Completed += handle =>
        {
            if(handle.Status == AsyncOperationStatus.Succeeded)
            {
                AudioClip clip = handle.Result;
                SoundManager.soundInstance.PlaySound(SoundType.Music, clip);
            }
            else
            {
                Utils.LogError($"Failed to load music: {MusicName}");
            }
        };
    }
    public void SelectMusic()
    {
        PlayMusic();
    }
}
