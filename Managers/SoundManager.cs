using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System;
using VInspector;

public class SoundManager : MonoBehaviour
{
    public static SoundManager soundInstance;
    public event Action<string> OnNextMusicPlay;

    [Header("AudioMixer")]
    public AudioMixer audioMixer;

    [Header("Sliders")]
    Slider masterVolumeSlider;
    Slider engineVolumeSlider;
    Slider musicVolumeSlider;
    Slider buttonVolumeSlider;
    Slider effectVolumeSlider;

    [Header("AudioSources")]
    [SerializeField] GameObject engineAudioObject;
    [SerializeField] GameObject musicAudioObject;
    [SerializeField] GameObject buttonAudioObject;
    [SerializeField] GameObject effectAudioObject;

    [Header("AudioDictionary")]
    [HideInInspector] public SerializedDictionary<SoundType , AudioSource> sceneAudioSources = new SerializedDictionary<SoundType, AudioSource>();

    [Header("MusicPlayer")]
    [SerializeField] MusicPlayer musicPlayer;

    WaitForSecondsRealtime waitfor500ms = new WaitForSecondsRealtime(0.5f);
    Coroutine checkMusicCoroutine;

    void Awake()
    {
        if(soundInstance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            soundInstance = this;
            DontDestroyOnLoad(gameObject);

            // GameObject에 DontDestroyOnLoad 적용 및 AudioSource 추출
            AddAudioSource(SoundType.Engine, engineAudioObject);
            AddAudioSource(SoundType.Music,  musicAudioObject);
            AddAudioSource(SoundType.Button, buttonAudioObject);
            AddAudioSource(SoundType.Effect, effectAudioObject);
        }
    }

    void Start()
    {   
        StartCoroutine(WaitAndStartMusic());
    }
    IEnumerator WaitAndStartMusic()
    {
        while (!GlobalMusicData.musicDataInstance.IsLoaded)
        {
            yield return null;
        }
    
        musicPlayer.StartInit();
    }

    // AudioSource를 추가하는 함수 (SoundType이 먼저)
    void AddAudioSource(SoundType soundType, GameObject audioObject)
    {
        if (audioObject != null)
        {
            if (audioObject.TryGetComponent(out AudioSource audioSource))
            {
                sceneAudioSources[soundType] = audioSource;
            }
            else
            {
                Utils.LogError();
            }
        }
    }

    public void SaveSoundSetting()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
        PlayerPrefs.SetFloat("EngineVolume", engineVolumeSlider.value);
        PlayerPrefs.SetFloat("MusicVolume" , musicVolumeSlider.value);
        PlayerPrefs.SetFloat("ButtonVolume", buttonVolumeSlider.value);
        PlayerPrefs.SetFloat("EffectVolume", effectVolumeSlider.value);
    }

    /** OnvalueChanged없이 Slider 초기화 */
    void ForceRefreshSlider(Slider slider, float targetValue)
    {
        slider.SetValueWithoutNotify(targetValue == 1f ? 0.99f : 1f); // OnValueChanged 콜백을 호출하지 않고 값을 설정
        slider.SetValueWithoutNotify(targetValue); // 슬라이더 이벤트 방지 (선택사항)
        slider.onValueChanged.Invoke(targetValue); // 원래 값 다시 설정 => 강제 갱신
    }

    /** 슬라이더 값 -> Localize 변경되면 같이 변경해주기  */
    public void LoadSoundSetting()
    {
        float master = PlayerPrefs.GetFloat("MasterVolume", 1);
        float engine = PlayerPrefs.GetFloat("EngineVolume", 1);
        float music  = PlayerPrefs.GetFloat("MusicVolume" , 1);
        float button = PlayerPrefs.GetFloat("ButtonVolume", 1);
        float effect = PlayerPrefs.GetFloat("EffectVolume", 1);

        ForceRefreshSlider(masterVolumeSlider, master);
        ForceRefreshSlider(engineVolumeSlider, engine);
        ForceRefreshSlider(musicVolumeSlider , music);
        ForceRefreshSlider(buttonVolumeSlider, button);
        ForceRefreshSlider(effectVolumeSlider, effect);

        SetMasterVolume(master);
        SetEngineVolume(engine);
        SetMusicVolume(music);
        SetButtonVolume(button);
        SetEffectVolume(effect);
    }

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡGetㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//

    /** 엔진 오디오 소스 반환 */
    public AudioSource GetEngineAudio()
    {
        return sceneAudioSources[SoundType.Engine];
    }
    /** 노래 오디오 소스 반환 */
    public AudioSource GetMusicAudioSource()
    {
        return sceneAudioSources[SoundType.Music];
    }
    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡGameSceneㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//
    void SetSliders(Slider masterSlider, Slider engineSlider, Slider musicSlider, Slider buttonSlider, Slider effectSlider)
    {
        masterVolumeSlider = masterSlider;
        engineVolumeSlider = engineSlider;
        musicVolumeSlider  = musicSlider;
        buttonVolumeSlider = buttonSlider;
        effectVolumeSlider = effectSlider;
    }

    /** Slider 교체, 씬에 맞는 audioSource 교체, 값 로드, 이벤트 새로등록 */
    public void ResetSlidersSceneChanged(Slider masterSlider, Slider engineSlider, Slider musicSlider, Slider buttonSlider, Slider effectSlider)
    {
        SetSliders(masterSlider,engineSlider,musicSlider,buttonSlider,effectSlider);
        LoadSoundSetting();

        RemoveSliderListeners();
        AddSliderListeners();
    }

    /** Scene 옮기고나서 Listener 등록 */
    public void AddSliderListeners()
    {
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        engineVolumeSlider.onValueChanged.AddListener(SetEngineVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        buttonVolumeSlider.onValueChanged.AddListener(SetButtonVolume);
        effectVolumeSlider.onValueChanged.AddListener(SetEffectVolume);
    }

    /** Scene 옮기기 전에 Listener 제거 */
    public void RemoveSliderListeners()
    {
        if (masterVolumeSlider != null) masterVolumeSlider.onValueChanged.RemoveListener(SetMasterVolume);
        if (engineVolumeSlider != null) engineVolumeSlider.onValueChanged.RemoveListener(SetEngineVolume);
        if (musicVolumeSlider  != null) musicVolumeSlider.onValueChanged.RemoveListener(SetMusicVolume);
        if (buttonVolumeSlider != null) buttonVolumeSlider.onValueChanged.RemoveListener(SetButtonVolume);
        if (effectVolumeSlider != null) effectVolumeSlider.onValueChanged.RemoveListener(SetEffectVolume);
    }

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//

    /** 저장하기 버튼누를때마다 MusicManager json 업데이트 */
    public void MusicUpdate()
    {
        musicPlayer.UpdateMusic();
    }

    /** SoundType에 맞춰 그 오디오소스의 클립으로 변경 */
    public void SetClip(SoundType soundType , AudioClip clip)
    {
        sceneAudioSources[soundType].clip = clip;
    }
    /** SoundType에 맞춰 그 오디오소스 Play */
    public void PlaySound(SoundType soundType , AudioClip clip)
    {
        musicPlayer.PlayMusic(clip.name); // MusicMAnager에서 같은 클립이 있는지 확인후 있다면 재생, MusicIndex 재설정

        if (checkMusicCoroutine != null)
        {
            StopCoroutine(checkMusicCoroutine);
            checkMusicCoroutine = null;
        }

        OnNextMusicPlay(clip.name); // UIManager_LobbyScene에서 이걸 통해 GetCurrentPlayingMusicName()으로 현재 재생중인 음악 이름 등록하기때문에 이름이 좀 그렇네
        checkMusicCoroutine = StartCoroutine(CheckMusicFinished());
    }
    IEnumerator CheckMusicFinished()
    {
        AudioSource source = sceneAudioSources[SoundType.Music];
        while (source.isPlaying)
        {
            yield return waitfor500ms; // 0.1초 간격으로 확인
        }

        if (musicPlayer != null)
        {
            string nextMusicName = musicPlayer.PlayNextMusic();
            OnNextMusicPlay(nextMusicName);

            checkMusicCoroutine = StartCoroutine(CheckMusicFinished());
        }
        else
        {
            checkMusicCoroutine = null;
        }
    }

    /** SoundType에 맞춰 그 오디오소스 Stop */
    public void StopSound(SoundType soundType)
    {
        if (checkMusicCoroutine != null)
        {
            StopCoroutine(checkMusicCoroutine);
            checkMusicCoroutine = null;
        }

        sceneAudioSources[soundType].Stop();
    }

    /** 현재 플레이중인 음악 이름 가져오기 */
    public string GetCurrentPlayingMusicName()
    {
        //** Lobby부터면 != 필요없음
        if(sceneAudioSources[SoundType.Music] != null && sceneAudioSources[SoundType.Music].isPlaying)
        {
            return sceneAudioSources[SoundType.Music].clip.name;
        }
        else
            return "";
    }

    public void PlayMusicPlayEvent(string musicName)
    {
        OnNextMusicPlay(musicName);
    }

    /** SoundType에 맞춰 그 오디오소스 PlayShot */
    public void PlayOneShot(SoundType soundType , AudioClip clip)
    {
        sceneAudioSources[soundType].PlayOneShot(clip);
    }

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//

    /** 전체적인 볼륨 조절 */
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    /** 자동차 엔진음 볼륨 조절 */
    public void SetEngineVolume(float volume)
    {
        audioMixer.SetFloat("EngineVolume", Mathf.Log10(volume) * 20);
    }

    /** 배경음악 볼륨 조절 */
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    /** 버튼음 볼륨 조절 */
    public void SetButtonVolume(float volume)
    {
        audioMixer.SetFloat("ButtonVolume", Mathf.Log10(volume) * 20);
    }

    /** 효과음 볼륨 조절 */
    public void SetEffectVolume(float volume)
    {
        audioMixer.SetFloat("EffectVolume", Mathf.Log10(volume) * 20);
    }
}
