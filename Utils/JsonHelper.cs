using UnityEngine;
using System.IO;

public static class JsonHelper
{
    public static void SaveToJson<T>(T data, string fileFormattedName)
    {
        string json = JsonUtility.ToJson(data);
        string path = Path.Combine(Application.persistentDataPath, fileFormattedName);
        File.WriteAllText(path, json);
    }

    public static T LoadFromJson<T>(string fileFormattedName) where T: new()
    {
        string path = Path.Combine(Application.persistentDataPath, fileFormattedName);

        if(File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }
        else
        {
            Utils.Log("Json File not found: " + path);
            return default;
        }
    }
}