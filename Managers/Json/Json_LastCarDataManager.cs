using UnityEngine;

public class Json_LastCarDataManager : MonoBehaviour, IJsonData<CarDataJson>
{
    public string FileFormattedName => "LastPlayedCarData.json";
    public string DefaultCarTag => "P_Car_7C"; // 첫 시작시 띄울 기본 차량 데이터

    CarDataJson carDataJson;

    public void Init()
    {
        carDataJson = Load();

        if(carDataJson == null)
        {
            carDataJson = new CarDataJson();
            CarData carData = Resources.Load<CarData>("CarData/" + DefaultCarTag); // Resources 폴더의 CarData 폴더에서 로드
        
            // addressable로 바꿔야함
            if (carData == null)
            {
                Utils.LogError("CarData not found: " + DefaultCarTag + "Json_LastCarDataManager.cs/Init()");
                return;
            }
            
            carDataJson.SetCarDataJson(carData);
        }
        Save();
    }

    public void Save()
    {
        if(carDataJson == null)
        {
            Utils.LogError("Save failed: carDataJson is null");
            return;
        }

        JsonHelper.SaveToJson<CarDataJson>(carDataJson, FileFormattedName);
    }

    public CarDataJson Load()
    {
        return JsonHelper.LoadFromJson<CarDataJson>(FileFormattedName);
    }

    public void UpdateData(CarDataJson data)
    {
        carDataJson = data;
    }
}
