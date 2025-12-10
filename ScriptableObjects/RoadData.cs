using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VInspector;

[CreateAssetMenu( fileName = "RoadData", menuName = "ScriptableObject/RoadData")]
public class RoadData : ScriptableObject
{
    [Header("# Map")]
    public MapType type;
    public GameObject map;
    public int insideRenderObjectSize = 6;
    public int outsideRenderObjectSize = 4;
    
    [Header("# InMapRandomTransform")]
    public List<GameObject> inProps;
    public float inOriginX = 60;
    public Vector3 inRangePos = new Vector3(20,0,20);
    public Vector3 inRangeRot = new Vector3(15,15,20);
    public float inMinScaleValue = 1.5f;
    public float inMaxScaleValue = 10f;
    public float inRenderRatio = 0.6f; // 랜덤 ratio

    [Header("# OutMapRandomTransform")]
    public List<GameObject> outProps;
    public float outOriginX = 150;
    public Vector3 outRangePos = new Vector3(55,0,20);
    public Vector3 outRangeRot = new Vector3(0, 50, 0);
    public float outMinScaleValue = 1.5f;
    public float outMaxScaleValue = 10f;
    public float outRenderRatio = 0.8f; // 랜덤 ratio


}
