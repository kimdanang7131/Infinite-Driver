using UnityEngine;

public interface IJsonData<T>
{
    string FileFormattedName { get; }

    void Init();
    void Save();
    T Load();
    void UpdateData(T data);
}