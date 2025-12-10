using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VInspector;

public class PoolManager : MonoBehaviour
{
    public static PoolManager poolInstance; // 싱글톤으로 전역접근 설정

    [Header("# RoadData")]
    public SerializedDictionary<MapType, RoadData> roadDataDic = new SerializedDictionary<MapType, RoadData>();
    [SerializeField] public int roadPoolSize = 20;
    [SerializeField] public int roadPropPoolSize = 300;
    Dictionary<int, Queue<GameObject>> roadMapPool     = new Dictionary<int, Queue<GameObject>>();
    Dictionary<int, Queue<GameObject>> roadInPropPool  = new Dictionary<int, Queue<GameObject>>();
    Dictionary<int, Queue<GameObject>> roadOutPropPool = new Dictionary<int, Queue<GameObject>>();

    [Header("# CarData")]
    public List<CarData> carDataList = new List<CarData>();
    [SerializeField] public int carPoolSize = 160; // 총 carPoool의 개수 ( 차를 레벨당 random으로 뽑아서 )
    Queue<GameObject> carPool = new Queue<GameObject>();
    
    [Header("# UI")]
    [SerializeField] public GameObject scoreTextObject;
    [SerializeField] public int scoreTextSize = 40;
    Queue<GameObject> scoreTextPool = new Queue<GameObject>();

    public GameObject canvasObject;

    void Awake() 
    {
        if(poolInstance != null)
        {
            Destroy(this);
        }
        else
        {
            poolInstance = this;
        }

        MakeRoadPool();
        MakeCarPool();
        MakeScoreTextPool();
    }

    /** 도로 Pool 생성 로직 */
    void MakeRoadPool()
    {
        // KayValuePair -> const pair<~,~>& a 라고 생각하면됨
        foreach(KeyValuePair<MapType, RoadData> roadDataPair in roadDataDic)
        {
            // 각 맵에 맞춰 ScriptableObject를 넣어뒀다면 거기에 맞춰 Pool생성
            MapType mapType   = roadDataPair.Key;
            RoadData roadData = roadDataPair.Value;

            // #1. 맵 풀링
            roadMapPool.Add((int)mapType, new Queue<GameObject>());
            for(int i = 0; i < roadPoolSize; i++)
            {
                GameObject mapInstance = Instantiate(roadData.map);
                mapInstance.SetActive(false);
                roadMapPool[(int)mapType].Enqueue(mapInstance);
            }

            // #2. InProp 풀링
            int rangeCount = roadDataDic[mapType].inProps.Count; // 범위 내에서 랜덤으로 뽑기 위해서
            int randNum = 0;
            roadInPropPool.Add((int)mapType, new Queue<GameObject>());
            
            for(int j = 0; j < roadPropPoolSize; j++)
            {
                // 랜덤으로 뽑기
                randNum = Random.Range(0,rangeCount);
                GameObject propInstance = Instantiate(roadData.inProps[randNum]);

                propInstance.SetActive(false);
                roadInPropPool[(int)mapType].Enqueue(propInstance);
            }

            // #3. OutProp 풀링
            roadOutPropPool.Add((int)mapType, new Queue<GameObject>());
            rangeCount = roadDataDic[mapType].outProps.Count; // 범위 내에서 랜덤으로 뽑기 위해서
            for(int j = 0; j < roadPropPoolSize; j++)
            {
                // 랜덤으로 뽑기
                randNum = Random.Range(0,rangeCount);
                GameObject propInstance = Instantiate(roadData.outProps[randNum]);

                propInstance.SetActive(false);
                roadOutPropPool[(int)mapType].Enqueue(propInstance);
            }
        }
    }

    void MakeCarPool()
    {
        int totalCount = carDataList.Count;
        int randIdx = 0;

        for(int i = 0; i < carPoolSize; i++)
        {   
            randIdx = Random.Range(0, totalCount);

            GameObject carPrefab = Instantiate<GameObject>(carDataList[randIdx].carPrefab);
            carPrefab.SetActive(false);
            carPool.Enqueue(carPrefab);
        }
    }

    void MakeScoreTextPool()
    {
        for(int i = 0; i < scoreTextSize; i++)
        {
            GameObject scoreText = Instantiate(scoreTextObject, canvasObject.transform);
            scoreText.SetActive(false);

            scoreTextPool.Enqueue(scoreText);
        }
    }

    public RoadData GetRoadData(MapType type)
    {
        return roadDataDic[type];
    }
    
    public GameObject GetScoreTextFromPool()
    {
        if(scoreTextPool.Count == 0)
        {
            Utils.LogError();
            return null;
        }

        GameObject scoreText = scoreTextPool.Dequeue();
        scoreText.SetActive(true);
        return scoreText;
    }

    public GameObject GetFromPool(MapType type)
    {
        if(roadMapPool[(int)type].Count == 0)
        {
            Utils.LogError();
            return null;
        }
        GameObject returnObj = roadMapPool[(int)type].Dequeue();
        returnObj.SetActive(true);
        returnObj.transform.position = Vector3.zero; // 위치 초기화

        return returnObj;
    }
    public GameObject GetRandomCarFromPool()
    {
        if(carPool.Count <= 0)
        {
            Utils.LogError();
        }
        
        GameObject retObj = carPool.Dequeue();
       
        return retObj;
    }


    /** type에 맞는 InProps중에서 random으로 뽑아서 반환 */
    public List<GameObject> GetInPropsFromPool(MapType type, int count) 
    {
        List<GameObject> poolObjs = new List<GameObject>();

        for(int i = 0; i < count; i++)
        {
            if(roadInPropPool[(int)type].Count == 0)
            {
                Utils.LogError();
                return null;
            }

            GameObject obj = roadInPropPool[(int)type].Dequeue();
            obj.transform.position = Vector3.zero;
            obj.SetActive(true);

            poolObjs.Add(obj);
        }

        return poolObjs;
    }
    public List<GameObject> GetOutPropsFromPool(MapType type, int count) 
    {
        List<GameObject> poolObjs = new List<GameObject>();

        for(int i = 0; i < count; i++)
        {
            if(roadOutPropPool[(int)type].Count == 0)
            {
                Utils.LogError();
                return null;
            }

            GameObject obj = roadOutPropPool[(int)type].Dequeue();
            obj.transform.position = Vector3.zero;
            obj.SetActive(true);
            
            poolObjs.Add(obj);
        }

        return poolObjs;
    }

    public void ReturnMapToPool(GameObject gameObject, MapType type)
    {
        gameObject.transform.position = Vector3.zero;
        gameObject.SetActive(false);

        roadMapPool[(int)type].Enqueue(gameObject);
    }

    public void ReturnInPropToPool(GameObject gameObject, MapType type)
    {
        if (gameObject.transform.parent != null) // 부모가 있는 경우에만 DetachChildren() 호출
        {
            gameObject.transform.parent.DetachChildren();
        }
        gameObject.transform.position = Vector3.zero;
        gameObject.SetActive(false);

        roadInPropPool[(int)type].Enqueue(gameObject);
    }

    public void ReturnOutPropToPool(GameObject gameObject, MapType type)
    {
        if (gameObject.transform.parent != null) // 부모가 있는 경우에만 DetachChildren() 호출
        {
            gameObject.transform.parent.DetachChildren();
        }
        
        gameObject.transform.position = Vector3.zero;
        gameObject.SetActive(false);

        roadOutPropPool[(int)type].Enqueue(gameObject);
    }
    
    public void ReturnCarToPool(GameObject gameObject)
    {
        if(gameObject == null)
        {
            Utils.LogError();
            return;
        }
        
        // 부모가 carPoolGroup이 아닌 경우에만 SetParent() 호출
        //if (gameObject.transform.parent != null)
        //    gameObject.transform.parent.DetachChildren();

        //gameObject.transform.position = Vector3.zero;
        gameObject.SetActive(false);
        
        carPool.Enqueue(gameObject);
    }

    public void ReturnTextToPool(GameObject gameObject)
    {
        if(gameObject == null)
        {
            Utils.LogError();
            return;
        }

        
        gameObject.SetActive(false);
        scoreTextPool.Enqueue(gameObject);
    }
}

