using UnityEngine;
using UnityEngine.SceneManagement;

public enum MapType   { Desert, City,  Infinite , Max }
public enum LevelType { A , B  , C , D , Max}
public enum LaneType  { LL, LR , RL , RR }
public enum SoundType { Master, Engine, Music, Button, Effect }
public enum SceneType { Loading, Lobby, Game, Max }
public enum TierType  { Common, Rare, Epic, Myth, Max }

public class SystemManager : MonoBehaviour
{
    public static SystemManager systemInstance;
    [SerializeField] SoundManager soundManager;

    public SceneType sceneType { get; set; } = SceneType.Lobby;

    void Awake()
    {
        if(systemInstance != null)
        {
            Destroy(this);
        }
        else
        {
            systemInstance = this;
            DontDestroyOnLoad(this);

            SceneManager.sceneLoaded += OnSceneLoaded; // 이벤트 등록
        }
    }

    /** Scene이 새로 로드될때마다 각 Scecne에 맞는 IDataSetting을 통해 각 씬에 Data 전달 */
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Scene에 맞는 IDataSetting을 찾아서 UpdateSceneData 호출
        IDataSetting dataSetting = null;
        switch (scene.name)
        {
            case "LoadingScene":
                sceneType = SceneType.Loading; 
                break;
            case "LobbyScene":
                dataSetting  = FindObjectOfType<LobbySceneData>() as IDataSetting;
                sceneType = SceneType.Lobby;
                break;
            case "GameScene":
                dataSetting  = FindObjectOfType<GameSceneData>() as IDataSetting;
                sceneType = SceneType.Game;
                break;
        }

        // IDataSetting이 존재하면 UpdateSceneData 호출
        if (dataSetting != null)
        {
            dataSetting.UpdateSceneData(); // 각 씬에 맞는 Data 전달
        }
    }
}
