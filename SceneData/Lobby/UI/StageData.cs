using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageData : MonoBehaviour
{
    public MapType MapType;

    [Header("PassInfo")]
    [SerializeField] Button prev;
    [SerializeField] Button next;
    [SerializeField] Button returnToLobbyButton;
    
    [SerializeField] Button playButton;
    
    void Start()
    {
        prev.onClick.AddListener(() => UIManager_LobbyScene.uiInstance.StageShowPrev());
        next.onClick.AddListener(() => UIManager_LobbyScene.uiInstance.StageShowNext());
        returnToLobbyButton.onClick.AddListener(() => UIManager_LobbyScene.uiInstance.ReturnStageToLobby());

        playButton.onClick.AddListener(PlayGame); // 선택한 MapType으로 게임 시작
    }

    public void PlayGame()
    {
        // 맵 타입 저장 후 시작
        //JsonDataManager.jsonInstance.SaveMapTypeDataJson(MapType);
        PlayerPrefs.SetString("Map", MapType.ToString());
        
        if(AdManager.adInstance.Button_PlayButtonClicked("GameScene") == false)
        {
            LoadingManager.LoadScene("GameScene");
        }
    }
}
