using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    [SerializeField] AudioClip soundClip;
    
    Button soundButton;

    void Awake()
    {
        if(TryGetComponent<Button>(out soundButton) == false)
        {
            Utils.LogError("No Button -> SoundDatas");
        }
    }

    void Start()
    {
        soundButton.onClick.AddListener(()=> SoundManager.soundInstance.PlayOneShot(SoundType.Button,soundClip));
    }
}
