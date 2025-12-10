using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class RandomizeObject : MonoBehaviour
{   
    //const float kLength_Z = 200f;
    const float kPerZ = 25f;

    // 설정 데이터를 담는 내부 클래스
    [System.Serializable] // 인스펙터에서 편집 가능하도록
    public class ObjectSetting
    {
        public float originX;
        public Vector3 rangePos;
        public Vector3 rangeRot;
        public float minScaleValue;
        public float maxScaleValue;
        [Range(0f, 1f)] public float renderRatio; // 범위 제한

        // 무작위 값을 생성하여 반환하는 함수
        public Vector3 GetRandomPosition()
        {
            return new Vector3(Random.Range(originX + -rangePos.x ,originX + rangePos.x),
                               0,
                               Random.Range(-rangePos.z, rangePos.z));
        }

        public Vector3 GetRandomRotation()
        {
            return new Vector3(Random.Range(-rangeRot.x, rangeRot.x),
                               Random.Range(-rangeRot.y, rangeRot.y),
                               Random.Range(-rangeRot.z, rangeRot.z));
        }

        public float GetRandomScale()
        {
            return Random.Range(minScaleValue, maxScaleValue);
        }
    }



    [Header("# Inside")]
    public ObjectSetting insideSetting;
    public List<GameObject> insideRenderObjects = new List<GameObject>();

    [Header("# Outside")]
    public ObjectSetting outsideSetting;
    public List<GameObject> outsideRenderObjects = new List<GameObject>();



    [Button]
    void Batch()
    {
        BatchLogic(ref insideRenderObjects , insideSetting);
        BatchLogic(ref outsideRenderObjects , outsideSetting);
    }

    void BatchLogic(ref List<GameObject> renderObjects , ObjectSetting setting)
    {

        List<float> randomZList = new List<float>();
        randomZList = GetRandomIntervalZ(renderObjects.Count);
        
        for(int i = 0 ; i < randomZList.Count; i++)
        {
            int rand = Random.Range(i,randomZList.Count);
            
            float temp = randomZList[rand];
            randomZList[rand] = randomZList[i];
            randomZList[i] = temp;
        }
        
        for(int i=0; i<renderObjects.Count; i++)
        {
            GameObject obj = Instantiate(renderObjects[i] , transform);

            if (Random.value < setting.renderRatio)
            {
                // obj = poolManager.GetTreeFromPool(); // 이부분은 잠시 주석처리
                
                obj.SetActive(true);
                
                // 위치
                float random_Z  = randomZList[i];
                Vector3 randPos = setting.GetRandomPosition();
                obj.transform.localPosition  = new Vector3(obj.transform.localPosition.x + randPos.x , 0 , random_Z + randPos.z);

                // 회전, 스케일
                obj.transform.localRotation = Quaternion.Euler(setting.GetRandomRotation());
                obj.transform.localScale = Vector3.one * setting.GetRandomScale();
            }
            else
            {
                obj.SetActive(false);
            }
        }
    }


    List<float> GetRandomIntervalZ(int length)
    {   
        // 길이는 -100 ~ 100 ( dist )
        if( length == 0 )
        {
            Utils.LogError("GetRandomIntervalZ 재설정 필요");
            return new List<float>();
        }

        List<float> result = new List<float>();
        float interval = 200f / (length + 1); // 양 끝점 포함
        float start = -100f;

        for(int i = 1; i <= length; i++)
        {
            result.Add(start + interval * i);
        }

        return result;
    }




    /** originX가 적용되고 좌우도 적용된 localPosition 뽑아서 가져오기 */
    /*
    Vector3 GetDirectionalRandomPosition(GameObject obj, ObjectSetting setting)
    {
        // 위치
        Vector3 tempPosition = setting.GetRandomPosition();

        // 왼쪽이면 x좌표 반전
        if(isLeft) 
            tempPosition = -obj.transform.localPosition; 
                
        tempPosition = GetOriginAddedPosition(tempPosition, isLeft , setting.originX);

        return tempPosition;
    }

    public Vector3 GetOriginAddedPosition(Vector3 tempPosition , bool isLeft , float originX)
    {
        if(isLeft)
            return new Vector3(-originX + tempPosition.x , tempPosition.y , tempPosition.z);
        else
            return new Vector3( originX + tempPosition.x , tempPosition.y , tempPosition.z);
    }
    */
}
