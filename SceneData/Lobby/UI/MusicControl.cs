using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicControl : MonoBehaviour
{
    [SerializeField] GameObject activateContentParent;
    [SerializeField] GameObject deactivateContentParent;

    [SerializeField] Button saveButton;

    Dictionary<string , GameObject> musicListDic = new Dictionary<string, GameObject>();


    void Awake()
    {
        saveButton.onClick.AddListener(SaveMusic);
    }

    void Start()
    {
        LoadMusicList();
        InitializeMusicListDic();

        SetMusicListOrder();
    }

    void InitializeMusicListDic()
    {
        foreach (Transform child in activateContentParent.transform)
        {
            ToggleMusic toggleMusic;
            if (child.gameObject.TryGetComponent<ToggleMusic>(out toggleMusic))
            {
                musicListDic.Add(toggleMusic.MusicName, toggleMusic.gameObject);
            }          
        }

        foreach (Transform child in deactivateContentParent.transform)
        {
            ToggleMusic toggleMusic;
            if (child.gameObject.TryGetComponent<ToggleMusic>(out toggleMusic))
            {
                musicListDic.Add(toggleMusic.MusicName, toggleMusic.gameObject);
            }          
        }
    }

    /** OpenMusicControl 했을때  */
    public void SetMusicListOrder()
    {
        MusicData mData = JsonDataManager.jsonInstance.LoadMusicData();

        // ** Getcomponent 변경 1. musicDic을 순회하며 toggleMusic를 모두 Deactivate하여 DeactivateComponent가 되게끔 만든다
        foreach (KeyValuePair<string, GameObject> pair in musicListDic)
        {
            ToggleMusic toggleMusic = pair.Value.GetComponent<ToggleMusic>();
            if (toggleMusic != null)
            {
                toggleMusic.SetDeactivate();
            }
        }

        // ** Getcomponent 변경 2. loadedMusicList.Count와 이름이 같은 음악만 찾아서 activate하여 순서대로 위로 올려준다
        for (int i = 0; i < mData.musicNames.Count; i++)
        {
            if (musicListDic.ContainsKey(mData.musicNames[i]))
            {
                ToggleMusic toggleMusic = musicListDic[mData.musicNames[i]].GetComponent<ToggleMusic>();
                if (toggleMusic != null)
                {
                    toggleMusic.SetActivate();
                    musicListDic[mData.musicNames[i]].transform.SetSiblingIndex(i); // 순서대로 위로 이동
                }
            }
        }
    }

    /** Load한 내가 만든 재생목록에 음악이 없다면 Deactivate로 옮겨주기*/
    void LoadMusicList()
    {
        MusicData mData = JsonDataManager.jsonInstance.LoadMusicData();

        // #2. 모두 Dic에 저장
        Dictionary<string, ToggleMusic> musicObjects = new Dictionary<string, ToggleMusic>();
        foreach (Transform child in deactivateContentParent.transform)
        {
            ToggleMusic toggleMusic;
            if (child.gameObject.TryGetComponent<ToggleMusic>(out toggleMusic))
            {
                musicObjects.Add(toggleMusic.MusicName, toggleMusic);
            }
        }

        foreach(string activatedMusic in mData.musicNames)
        {
            musicObjects[activatedMusic].SetActivate();
        }
    }

    /** 자식 리스트로 넘겨주는 코드 */
    public MusicData GetMusicDataFromToggleMusic()
    {
        MusicData mData = new MusicData();

        // MusicControl 게임 오브젝트의 자식들을 순회
        foreach (Transform child in activateContentParent.transform)
        {
            ToggleMusic toggleMusic;
            
            // 자식 게임 오브젝트에 ToggleMusic 스크립트가 있는지 확인
            if(child.gameObject.TryGetComponent<ToggleMusic>(out toggleMusic))
            {
                // ToggleMusic 스크립트를 가진 게임 오브젝트를 리스트에 추가
                mData.musicNames.Add(toggleMusic.MusicName);
            }
        }

        return mData;
    }

    public void SaveMusic()
    {
        JsonDataManager.jsonInstance.Save(GetMusicDataFromToggleMusic()); // MusicData를 교체하는것이 아닌 MusicNames만 교체
        saveButton.interactable = false;

        UIManager_LobbyScene.uiInstance.SaveMusic();
    }

    public void ModifyMusicList()
    {
        saveButton.interactable = true;
    }

    public void ActivateContent(GameObject activateContent)
    {
        activateContent.transform.SetParent(activateContentParent.transform);
    }

    public void DeactivateContent(GameObject deactivateContent)
    {
        deactivateContent.transform.SetParent(deactivateContentParent.transform);
    }
}
