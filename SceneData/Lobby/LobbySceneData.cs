using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using VInspector;

public class LobbySceneData : MonoBehaviour , IDataSetting
{
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider engineVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider buttonVolumeSlider;
    [SerializeField] Slider effectVolumeSlider;
    
    [SerializeField] Button saveButton;

    public void UpdateSceneData()
    {
        if(SoundManager.soundInstance == null)
        {   
            Utils.Log("no sound manager");
            return;
        }
        
        SoundManager.soundInstance.ResetSlidersSceneChanged(masterVolumeSlider, engineVolumeSlider, musicVolumeSlider, buttonVolumeSlider, effectVolumeSlider);
        //SoundManager.soundInstance.GetEngineAudio().Pause();
        saveButton.onClick.AddListener(()=> SoundManager.soundInstance.SaveSoundSetting());
    }
}
