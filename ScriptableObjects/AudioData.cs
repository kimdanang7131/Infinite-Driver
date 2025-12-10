using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "ScriptableObject/AudioData")]
class AudioData : ScriptableObject
{
    [SerializeField] public string labelName;
    [SerializeField] public SoundType type;
    [SerializeField] public string musicKey;
    [SerializeField] public string addressableKey;
    [SerializeField] public int numberOfCount;
}