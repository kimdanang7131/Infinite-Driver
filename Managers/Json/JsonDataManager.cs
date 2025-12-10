using UnityEngine;

public class JsonDataManager : MonoBehaviour
{
    public static JsonDataManager jsonInstance;
    
    [SerializeField] Json_LastCarDataManager lastCarDataManager;
    [SerializeField] Json_PlayerDataManager playerDataManager;
    [SerializeField] Json_MusicDataManager musicDataManager;

    private void Awake()
    {
        if (jsonInstance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            jsonInstance = this;
            DontDestroyOnLoad(gameObject);
        }

        lastCarDataManager.Init();
        playerDataManager.Init();
        musicDataManager.Init();
    }

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ Save Json ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//
    public void Save(in MusicData musicData)
    {
        musicDataManager.UpdateData(musicData); // MusicData를 교체하는것이 아닌 MusicNames만 교체
        musicDataManager.Save();
    }
    
    public void Save(in PlayerData playerData)
    {
        playerDataManager.UpdateData(playerData);
        playerDataManager.Save();
    }

    public void Save(in CarDataJson carDataJson)
    {
        lastCarDataManager.UpdateData(carDataJson);
        lastCarDataManager.Save();
    }
    
    public void Save(in CarData carData)
    {
        CarDataJson carDataJson = new CarDataJson();
        carDataJson.SetCarDataJson(carData);
        lastCarDataManager.UpdateData(carDataJson);
        lastCarDataManager.Save();
    }

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ Load Json ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//
    public PlayerData LoadPlayerData()
    {
        return playerDataManager.Load();
    }

    public MusicData LoadMusicData()
    {
        return musicDataManager.Load();
    }

    public CarDataJson LoadLastCarDataJson()
    {
        return lastCarDataManager.Load();
    }

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ Resources.Load ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//
    // addressable로 바꿔야함 -> PlayerCarHandler, Json_LastCarDataManager에서 사용
    public CarData LoadResourceCarData(string carTag)
    {
        CarData carData = Resources.Load<CarData>("CarData/" + carTag); // Resources 폴더의 CarData 폴더에서 로드
        
        // Resources 폴더에 CarData 폴더가 없거나, carTag에 해당하는 CarData가 없을 경우
        if (carData == null)
            Utils.LogError("CarData not found: " + carTag + "JsonDataManager.cs/GetCarData()");

        return carData;
    }
}
