using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class ToggleSound : MonoBehaviour
{
    [SerializeField] AudioClip soundClip;
    
    Toggle toggleButton;

    void Awake()
    {
        if(TryGetComponent<Toggle>(out toggleButton) == false)
        {
            Utils.LogError("No Toggle -> SoundDatas");
        }
    }

    void Start()
    {
        //toggleButton.onValueChanged.AddListener(ToggleValueChanged);
    }

    void ToggleValueChanged(bool value)
    {
        SoundManager.soundInstance.PlayOneShot(SoundType.Button,soundClip);
    }
}
