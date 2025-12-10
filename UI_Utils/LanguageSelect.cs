using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageSelect : MonoBehaviour
{
    public LanguageButton[] languageButtons; // 언어 버튼들

    private int curSelectedIndex = -1;

    public void NewLanguageButtonClicked(int index)
    {
        if(curSelectedIndex == index)
            return;
            
        // 1. 모든 버튼 초기화
        for (int i = 0; i < languageButtons.Length; i++)
        {
            bool isSelected = (i == index);
            languageButtons[i].SetSelected(isSelected);
        }

        SoundManager.soundInstance.SaveSoundSetting(); // 언어 변경시 사운드 세팅 저장
        curSelectedIndex = index;
    }
}
