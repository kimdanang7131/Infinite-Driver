using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VInspector;

public class LobbyTutorial : MonoBehaviour
{  
    [SerializeField] GameObject[] lobbyTutorials;

    int tutorialIndex = 0;

    void Start()
    {
        int check = PlayerPrefs.GetInt("LobbyTutorial");
        if(check == 0)
        {
            if(lobbyTutorials.Length > 0)
                lobbyTutorials[0].SetActive(true);
        }
    }

    public void NextTutorial()
    {
        lobbyTutorials[tutorialIndex].SetActive(false);
        tutorialIndex++;

        if(tutorialIndex >= lobbyTutorials.Length)
        {
            Utils.Log("튜토리얼 끝");
            EndSetting();
        }  
        else
        {
            lobbyTutorials[tutorialIndex].SetActive(true);
        }
    }


    void EndSetting()
    {
        PlayerPrefs.SetInt("LobbyTutorial",1);
        tutorialIndex = 0;
    }

    [Button]
    public void ResetTutorial()
    {
        Utils.Log("LobbyTutorial Reset");
        PlayerPrefs.SetInt("LobbyTutorial", 0);
    }
}
