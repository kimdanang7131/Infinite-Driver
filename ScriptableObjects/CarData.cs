using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CarData", menuName = "ScriptableObject/CarData")]
public class CarData : ScriptableObject
{
    [SerializeField] public string carTag;
    [SerializeField] public string carName;
    [SerializeField] public int carPrice;
    [SerializeField] public TierType tierType;
    [SerializeField] public Sprite carIcon;
    [SerializeField,TextArea] public string carDesc;
    public AnimationCurve carPitchCurve;

    [Header("# InGame")]
    [SerializeField] public GameObject carPrefab;
    [SerializeField] public float maxForwardVelocity = 250f; // 차량 최대 속도 
    [SerializeField] public float minForwardVelocity = 10f;  // 차량 최소 속도
    [SerializeField] public float accelValue = 35f; 
    [SerializeField] public float brakeValue = 70f; 

    [SerializeField] public float steerValue = 3f; 
    [SerializeField] public float steerBrakeValue = 2f; 
}

[Serializable]
public class CarDataJson
{
    public string carTag;
    public string carName;
    public int carPrice;
    public string tierType;
    public string carIconPath;
    public float maxForwardVelocity;
    public float minForwardVelocity;
    public float accelValue;
    public float steerValue;

    public void SetCarDataJson(CarData carData)
    {
        carTag = carData.carTag;
        carName = carData.carName;
        carPrice = carData.carPrice;
        tierType = carData.tierType.ToString();
        carIconPath = $"CarIcon/" + carData.carName;
        maxForwardVelocity = carData.maxForwardVelocity;
        minForwardVelocity = carData.minForwardVelocity;
        accelValue = carData.accelValue;
        steerValue = carData.steerValue;
    }

    public void SetCarDataJson(CarDataJson carDataJson)
    {
        carTag = carDataJson.carTag;
        carName = carDataJson.carName;
        carPrice = carDataJson.carPrice;
        tierType = carDataJson.tierType.ToString();
        carIconPath = $"CarIcon/" + carDataJson.carName;
        maxForwardVelocity = carDataJson.maxForwardVelocity;
        minForwardVelocity = carDataJson.minForwardVelocity;
        accelValue = carDataJson.accelValue;
        steerValue = carDataJson.steerValue;
    }
}
