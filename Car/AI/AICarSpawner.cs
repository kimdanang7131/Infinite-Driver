using System.Collections;
using UnityEngine;

// PoolManager와 연동
public class AICarSpawner : MonoBehaviour
{   
    [Header("#Spawners")]
    [SerializeField] AbstractCarSpawner forwardCarSpawner;
    [SerializeField] AbstractCarSpawner reverseCarSpawner;

    [Tooltip("주기마다 생성할 차량위치에 차량이 있는지 확인하기 위해서 필요한 값들")]
    [SerializeField] LayerMask otherCarsLayerMask;
    protected Transform playerCarTransform;
  
    WaitForSeconds waitFor1000ms = new WaitForSeconds(1f); // 주기마다 거리에 맞춰 CleanUp함수들 호출
    
    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//

    void Start()
    {
        playerCarTransform = GameManager.gameInstance.playerCarTransform;

        // 초기 세팅
        forwardCarSpawner.Init(playerCarTransform, otherCarsLayerMask);
        reverseCarSpawner.Init(playerCarTransform, otherCarsLayerMask);
    
        // #1. 정방향 RenderPool 배치
        //ForwardCarSpawner forward;
        //if(forwardCarSpawner.TryGetComponent<ForwardCarSpawner>(out forward) == false)
        //{
        //    Utils.LogError();
        //}
        //forward.InitSpawn();

        // #2. 일정시간 이후 역방향, 정방향 RenderPool 주기적으로 배치
        GameManager.gameInstance.OnWaitForStart += WaitForStart;
    }

    /** x초 후에 코루틴으로 주기마다 정,역방향 차량 생성 시작 */
    void WaitForStart()
    {
        forwardCarSpawner.TrySpawnCarFreq(); 
        reverseCarSpawner.TrySpawnCarFreq(); 
        StartCoroutine(CleanCarsOutOfView());
    }

    void OnDestroy()
    {
        GameManager.gameInstance.OnWaitForStart -= WaitForStart;   
    }

    /** 카메라 범위를 벗어났을때 ( 너무 멀거나 이미 지나침 ) Pool로 반환 */
    IEnumerator CleanCarsOutOfView()
    {
        while(true)
        {
            CleanUpCarBeyondView(); 
            yield return waitFor1000ms;
        }
    }
    void CleanUpCarBeyondView()
    {
        forwardCarSpawner.CleanUpLogic(); // #1. 정방향 cars 검사
        reverseCarSpawner.CleanUpLogic(); // #2. 역방향 Cars 검사
    }

    public void RemoveAllCars()
    {
        forwardCarSpawner.RemoveAllCars();
        reverseCarSpawner.RemoveAllCars();
    }
}



