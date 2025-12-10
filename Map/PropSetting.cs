using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropSetting : MonoBehaviour
{
    public MapType mapType;
    public bool isLeft = false;

    List<GameObject> insideRenderObjects  = new List<GameObject>();
    List<GameObject> outsideRenderObjects = new List<GameObject>();

    bool isCreated = false;

    void OnEnable() 
    {
        if(isCreated == false)
        {
            isCreated = true;
            return;
        }

        RoadData roadData =  PoolManager.poolInstance.GetRoadData(mapType);
        
        // inside 설정 및 처리
        SetInObjects(ref insideRenderObjects, roadData);

        // outside 설정 및 처리
        SetOutObjects(ref outsideRenderObjects, roadData);
    }

    /** Pool로 반환하면서 위치 초기화 */
    public void ReturnToPool()
    {
        foreach(GameObject obj in insideRenderObjects)
        {
            PoolManager.poolInstance.ReturnInPropToPool(obj,mapType);
        }
        foreach(GameObject obj in outsideRenderObjects)
        {
            PoolManager.poolInstance.ReturnOutPropToPool(obj,mapType);
        }

        insideRenderObjects.Clear();
        outsideRenderObjects.Clear();
    }

    #region Setting InOutObjects
    void SetInObjects(ref List<GameObject> objects, RoadData roadData)
    {
        // 6개 랜덤으로 가져오기
        objects = PoolManager.poolInstance.GetInPropsFromPool(mapType , roadData.insideRenderObjectSize);

        List<float> randomZList = new List<float>();
        randomZList = GetIntervalZ_Positions(roadData.insideRenderObjectSize); // 크기만큼 랜덤으로 뽑기위해

        for(int i = 0 ; i < randomZList.Count; i++)
        {
            GameObject obj = objects[i];
            obj.transform.SetParent(transform); // 부모변경

            if (Random.value < roadData.inRenderRatio)
            {
                // 위치
                float random_Z  = randomZList[i];

                // x, z 랜덤으로 가져옴
                Vector3 randPos = GetRandomPosition(roadData.inOriginX, roadData.inRangePos);   
                obj.transform.localPosition  = new Vector3( randPos.x , 0 , random_Z + randPos.z);

                // 랜덤 회전, 랜덤 스케일
                Vector3 rotation = Vector3.zero;
                if(isLeft)
                {
                    rotation = new Vector3(0,90,0);
                }
                else
                {
                    rotation = new Vector3(0,-90,0);
                }

                obj.transform.localRotation = Quaternion.Euler(rotation);
                Vector3 randRot = GetRandomRotation(roadData.inRangeRot);
                rotation += randRot;
                obj.transform.localRotation = Quaternion.Euler(rotation);
                obj.transform.localScale = Vector3.one * GetRandomScale(roadData.inMinScaleValue, roadData.inMaxScaleValue);
            }
            else
            {
                obj.SetActive(false);
            }
        }
    }

    void SetOutObjects(ref List<GameObject> objects, RoadData roadData)
    {
        objects = PoolManager.poolInstance.GetOutPropsFromPool(mapType, roadData.outsideRenderObjectSize);

        List<float> randomZList = new List<float>();
        randomZList = GetIntervalZ_Positions(roadData.outsideRenderObjectSize);

        for(int i = 0 ; i < randomZList.Count; i++)
        {
            GameObject obj = objects[i];
            obj.transform.SetParent(transform);
            //objects.Add(obj);

            if (Random.value < roadData.outRenderRatio)
            {
                // 위치
                float randomZ  = randomZList[i];
                Vector3 randPos = GetRandomPosition(roadData.outOriginX, roadData.outRangePos);

                obj.transform.localPosition  = new Vector3(randPos.x , 0 , randomZ + randPos.z);
                // 회전, 스케일
                Vector3 rotation = Vector3.zero;
                if(isLeft)
                {
                    rotation = new Vector3(0,270,0);
                }
                else
                {
                    rotation = new Vector3(0,-90,0);
                }
                obj.transform.localRotation = Quaternion.Euler(rotation);
                obj.transform.localRotation = Quaternion.Euler(GetRandomRotation(roadData.outRangeRot));
                obj.transform.localScale = Vector3.one * GetRandomScale(roadData.outMinScaleValue, roadData.outMaxScaleValue);
            }
            else // 일단 안보이게 꺼두기
            {
                obj.SetActive(false);
            }
        }
    }
    #endregion

    #region RandomTransform
    Vector3 GetRandomPosition(float originX, Vector3 rangePos)
    {
        if(isLeft == true)
        {
            originX *= -1;
        }

        return new Vector3(Random.Range(originX + -rangePos.x ,originX + rangePos.x),
                           0,
                           Random.Range(-rangePos.z, rangePos.z));
    }
    Vector3 GetRandomRotation(Vector3 rangeRot)
    {
        return new Vector3(Random.Range(-rangeRot.x, rangeRot.x),
                           Random.Range(-rangeRot.y, rangeRot.y),
                           Random.Range(-rangeRot.z, rangeRot.z));
    }
    float GetRandomScale(float minScaleValue, float maxScaleValue)
    {
        return Random.Range(minScaleValue, maxScaleValue);
    }
    #endregion

    /** 도로 위에 놓일 곳들 원하는 크기만큼 미리 점으로 찍어서 전달 */
    List<float> GetIntervalZ_Positions(int length)
    {   
        // 길이는 -100 ~ 100 ( dist )
        if( length == 0 )
        {
            Utils.Log("GetRandomIntervalZ 재설정 필요");
            return new List<float>();
        }

        List<float> result = new List<float>();
        float interval = 200f / (length + 1); // 양 끝점 포함 -> 8개까지 random할때 나누면 0~7까지나오는거에서 + 1까지 해줘서 0 ~ 8 양끝점 넣는원리,
        float start = -100f;

        for(int i = 1; i <= length; i++)
        {
            result.Add(start + interval * i);
        }

        return result;
    }
}
