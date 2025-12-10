using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicSelectControl : MonoBehaviour
{
    public event Action<string> OnMainMusicNameChanged;

    [SerializeField] List<GameObject> musicList = new List<GameObject>();
    Dictionary<string, GameObject> musicListDic = new Dictionary<string, GameObject>();

    List<string> playList = new List<string>();

    [SerializeField] Button openCloseButton;
    [SerializeField] GameObject scrollAreaObject;
    
    int musicIndex = 0;

    // 뮤직 Select 판 업데이트, 뮤직 순서 변경, 이벤트 등록
    public void Initialize()
    {
         // 일단 싹다끄고 순서대로 키기
        foreach(GameObject musicObject in musicList)
        {
            SettingMusic settingMusic;
            if(musicObject.TryGetComponent<SettingMusic>(out settingMusic))
            {
                musicListDic.Add(settingMusic.MusicName, settingMusic.gameObject);
            }
        }

        // 일단 싹다끄고 순서대로 키기
        foreach(GameObject musicObject in musicList)
        {
            musicObject.SetActive(false);
        }

        MusicData mData = JsonDataManager.jsonInstance.LoadMusicData();

        if(mData.musicNames.Count > 0)
        {
            foreach(string musicName in mData.musicNames)
            {
                musicListDic[musicName].SetActive(true);
            }
        }

        SetMusicListOrder();
        
        openCloseButton.onClick.AddListener(OpenCloseMusicList);
    }

    void OnDestroy()
    {
        openCloseButton.onClick.RemoveListener(OpenCloseMusicList);
    }
    
    public void OpenCloseMusicList()
    {
        // scrollAreaObject가 null인지 확인
        if (scrollAreaObject != null)
        {
            bool active = scrollAreaObject.activeSelf;
            scrollAreaObject.SetActive(!active);
        }
        else
        {
            Utils.Log("scrollAreaObject is null.");
        }
    }

    /** MusicControl에서 저장버튼을 클릭시 거기에 맞춰서 순서조정 및 List 만들기 */
    public void ModifyActivatedMusicList()
    {
        // 일단 싹다끄고 순서대로 키기
        foreach(GameObject musicObject in musicList)
        {
            musicObject.SetActive(false);
        }

        MusicData mData = JsonDataManager.jsonInstance.LoadMusicData();

        // 저장버튼 클릭시 변경한 목록의 첫 번째 음악 실행 및 텍스트 변경
        if(mData.musicNames.Count == 0)
        {  
            SoundManager.soundInstance.StopSound(SoundType.Music);
            OnMainMusicNameChanged("");
            return;
        }
        else
        {
            string firstMusicName = mData.musicNames[0];

            if (musicListDic.ContainsKey(firstMusicName))
            {
                GameObject musicObject = musicListDic[firstMusicName];
                musicObject.SetActive(true);

                SettingMusic settingMusic;
                if (musicObject.TryGetComponent<SettingMusic>(out settingMusic) && settingMusic != null)
                {
                    settingMusic.PlayMusic();

                    OnMainMusicNameChanged(firstMusicName);
                }
                else
                {
                    Utils.Log("SettingMusic component not found on " + firstMusicName);
                }
            }
            else
            {
                Utils.Log("Music not found in musicListDic: " + firstMusicName);
            }
        }

        SetMusicListOrder();
    }

    public void SetMusicListOrder()
    {
        MusicData mData = JsonDataManager.jsonInstance.LoadMusicData();

        for (int i = 0; i < mData.musicNames.Count; i++)
        {
            musicListDic[mData.musicNames[i]].SetActive(true);
            musicListDic[mData.musicNames[i]].transform.SetSiblingIndex(i);
        }
    }
}
