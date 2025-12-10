using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] Transform parent;
    CarData carData;

    void SetCarData(CarData carData)
    {
        this.carData = carData;
    }

    public void SpawnPlayerCar(CarData carData)
    {
        SetCarData(carData);
        Instantiate(carData.carPrefab, Vector3.zero, Quaternion.identity, parent);
    }
}
