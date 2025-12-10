using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VInspector;
using System.Linq; // 꼭 있어야 함
using UnityEngine.Localization.Settings;

public class TutorialTipControl : MonoBehaviour
{
    public Image tutorialImage;
    public TextMeshProUGUI tutorialTipText;
    
    public SerializedDictionary<string,Sprite> tipDic = new SerializedDictionary<string, Sprite>();

    void Awake()
    {
        int tipMaxCount = tipDic.Count;
        int randomIndex = Random.Range(0, tipMaxCount);

        // 랜덤 Tip 고름
        var randomElement = tipDic.ElementAt(randomIndex);
        
        string randomKey = randomElement.Key;
        Sprite randomSprite = randomElement.Value;

        tutorialImage.sprite = randomSprite;
        // Localized String을 가져와서 텍스트로 설정하기
        GetLocalizedText(randomKey);
    }

    void GetLocalizedText(string key)
    {
        // 현재 Locale을 기반으로 번역된 텍스트를 가져옴
        var localizedString = LocalizationSettings.StringDatabase.GetLocalizedString("Tutorial_Tip", key);

        // 튜토리얼 텍스트 설정
        tutorialTipText.text = localizedString;
    }
}
