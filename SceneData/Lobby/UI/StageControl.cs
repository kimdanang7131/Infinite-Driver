using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public StageData[] stageDatas;
    StageData selectedStageData;
    int stageIndex = 0;

    /** 버튼으로 이전, 다음 스테이지 볼 수 있도록 */
    public void ShowPrev()
    {
        stageDatas[stageIndex].gameObject.SetActive(false);
        
        stageIndex--;
        if(stageIndex < 0)
        {
            stageIndex = stageDatas.Length - 1;
        }

        stageDatas[stageIndex].gameObject.SetActive(true);
    }
    public void ShowNext()
    {
        stageDatas[stageIndex].gameObject.SetActive(false);
        
        stageIndex++;
        if(stageIndex >= stageDatas.Length)
        {
            stageIndex = 0;
        }

        stageDatas[stageIndex].gameObject.SetActive(true);
    }

    /** Stage 전부 끄고, 1번만 켜두고 현재 Group 닫기*/
    public void ReturnToLobby()
    {
        foreach (StageData stageGroup in stageDatas)
        {
            stageGroup.gameObject.SetActive(false);
        }
        stageDatas[0].gameObject.SetActive(true);
        stageIndex = 0;

        gameObject.SetActive(false);
    }
}
