using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using VInspector;

public class LocaleManager : MonoBehaviour
{
    [Header("LocaleCount"), Tooltip("언어 늘릴 때만 사용하세요")]
    [SerializeField,ReadOnly] int maxLocaleCount = 2; // 최대 언어 개수 -> 추후 확장 가능성 있음

    [SerializeField] LanguageSelect languageSelect; // 언어 버튼들
    [SerializeField] Button settingGroupButton; 

    bool isChanging = false;

    void Start()
    {
        int localeIdx = 0;

        // 이전에 플레이했을 경우
        if (PlayerPrefs.HasKey("LocaleIndex"))
        {
            localeIdx = PlayerPrefs.GetInt("LocaleIndex");

            if (localeIdx < 0 || localeIdx >= maxLocaleCount)
            {
                localeIdx = 0;
                PlayerPrefs.SetInt("LocaleIndex", localeIdx);
            }
        }
        else // 처음 플레이하는경우
        {
            // 자동으로 선택된 Locale이 몇 번째 인덱스인지 찾아서 저장
            localeIdx = LocalizationSettings.AvailableLocales.Locales.FindIndex(
                loc => loc.Identifier == LocalizationSettings.SelectedLocale.Identifier
            );
            
            if (localeIdx < 0 || localeIdx >= maxLocaleCount)
            {
                localeIdx = 0;
                PlayerPrefs.SetInt("LocaleIndex", localeIdx);
            }

            PlayerPrefs.SetInt("LocaleIndex", localeIdx);
        }

        // 처음시작하면 로컬라이제이션 세팅을 자동으로 변경
        ChangeLocale(localeIdx);
        
        // Locale이 변경 후 1프레임 후에 실행 -> Start이후에 실행되도록 보장
        StartCoroutine(OnDelayRoutine());
        settingGroupButton.onClick.AddListener(OnDelay);


        UIManager_LobbyScene.uiInstance.UpdateCarInfo();
    }

    void OnDestroy()
    {
        settingGroupButton.onClick.RemoveListener(OnDelay);
    }

    void OnDelay()
    {
        StartCoroutine(OnDelayRoutine());
    }
    IEnumerator OnDelayRoutine()
    {
        yield return null;

        SoundManager.soundInstance.LoadSoundSetting();
    }

    public void ChangeLocale(int index)
    {
        if (isChanging)
        {
            return;
        } 

        languageSelect.NewLanguageButtonClicked(index); // Button안에서 체크마크 표시

        PlayerPrefs.SetInt("LocaleIndex", index); // 로컬라이제이션 인덱스 저장
        StartCoroutine(ChangeLocaleRoutine(index));
    }

    IEnumerator ChangeLocaleRoutine(int index)
    {
        isChanging = true;

        yield return LocalizationSettings.InitializationOperation; // 로컬라이제이션 세팅 초기화될때까지 대기
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index]; // 로컬라이제이션 세팅 변경


        yield return null;
        // Lobby 씬 에 다시 들어왔을 때 Locale 변경 이후 1프레임 후에 실행
        UIManager_LobbyScene.uiInstance.ChangeSelectedMusicName(SoundManager.soundInstance.GetCurrentPlayingMusicName());

        isChanging = false;
    }
}
