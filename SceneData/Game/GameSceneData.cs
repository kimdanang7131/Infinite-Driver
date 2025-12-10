using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class GameSceneData : MonoBehaviour , IDataSetting
{
    [SerializeField] PlayerCarHandler playerCar;
    [SerializeField] CameraControl cameraControl;
    
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider engineVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider buttonVolumeSlider;
    [SerializeField] Slider effectVolumeSlider;

    [SerializeField] Button saveButton;

    public void UpdateSceneData()
    {
        if(GameManager.gameInstance == null)
        {
            return;
        }

        GameManager.gameInstance.SetPlayerCar(playerCar);
        GameManager.gameInstance.SetCameraControl(cameraControl);

        if(SoundManager.soundInstance == null)
        {   
            Utils.LogError("no sound manager");
            return;
        }

        SoundManager.soundInstance.ResetSlidersSceneChanged(masterVolumeSlider, engineVolumeSlider, musicVolumeSlider, buttonVolumeSlider, effectVolumeSlider);
        saveButton.onClick.AddListener(()=> SoundManager.soundInstance.SaveSoundSetting());
    }
}
