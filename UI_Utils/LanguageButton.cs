using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class LanguageButton : MonoBehaviour
{
    public Image flagImage;
    public GameObject checkMark;

    public void SetSelected(bool isSelected)
    {
        checkMark.SetActive(isSelected);
        flagImage.color = isSelected ? Color.white : Color.gray;
    }
}
