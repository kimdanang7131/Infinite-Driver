using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class ToggleMusic : MonoBehaviour
{   
    [SerializeField] Toggle toggle;
    
    //[SerializeField] AudioClip music;
    [SerializeField] AudioData musicData; // ScriptableObject로 관리하는 음악 데이터
    [SerializeField] TextMeshProUGUI musicNameText;

    public string MusicName => musicData.musicKey; // music이 없다면 null, 아니면 name

    void Awake()
    {
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
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

    public void SetActivate()
    {
        toggle.isOn = true;
    }

    public void SetDeactivate()
    {
        toggle.isOn = false;
    }
    
    void OnToggleValueChanged(bool value)
    {
        UIManager_LobbyScene.uiInstance.Modify(); // 변화가 있을 시 저장버튼 활성화

        // true로 변하면 위로, false면 아래로 이동
        if (value)
        {
            UIManager_LobbyScene.uiInstance.ActivateContent(gameObject);
        }
        else
        {
            UIManager_LobbyScene.uiInstance.DeactivateContent(gameObject);
        }
    }
}
