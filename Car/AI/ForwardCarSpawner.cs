using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardCarSpawner : AbstractCarSpawner
{
    // 생성할때 겹치는 차량 있는지 파악하기 위한 Colliders
    [Header("# 정방향 Collider")]
    [SerializeField] BoxCollider overlappedBoxColRR;
    [SerializeField] BoxCollider overlappedBoxColRL;
    
    float[] colliderSizeLevel = { 220f, 170f , 120f , 120f  }; // LevelType 변경되면 같이 변경될 것들 , 이벤트 구독
    float[] renderRatioLevel  = { 0.51f, 0.44f, 0.418f, 0.435f };
    float spawnInterVal       = 220f;  //**  Player이전위치 - 현재위치 간격마다 생성 , 200 - 150 - 100 3단계 레벨 설정
    private Coroutine spawnCoroutine;
    
    int kMaxSpawnCount = 30;

    /** 초반에 미리 차량 세팅 */
    public void InitSpawn()
    {
        int spawnCount = kSpawnInItMax / kSpawnInitInterval; // spawnCount * 2개 생성
        
        for(int i = 1; i <= spawnCount; i++)
        {
            TryPlaceCarOnLane((int)LaneType.RR, kSpawnInitInterval * i , false);
            TryPlaceCarOnLane((int)LaneType.RL, kSpawnInitInterval * i , false);
        }
    }

    /** [Lane을 올바르게 설정했는지 , MaxCount확인후 ] Ratio 적용해서 생성할지 정한 후 -> 위치 조정 + 배치  */
    protected override bool CheckCanPlaceCar(in int carLaneIdx)
    {
        // 왼쪽 라인(Reverse)에 생성하려고할때 return;
        if(carAIRenderPool.Count >= kMaxSpawnCount || carLaneIdx <= 1)
        {
            //Debug.Log("ForwardSpawner -> 스폰카운트 최대치 , TryPlaceForwardCarOnLane 라인 체크");
            return false;
        }
        else   
            return true;
    }

    /** Game의 레벨이 바뀔때마다 Spawn을 위한 Collider Z의 사이즈 조절 */
    public override void ChangeValuesOnLevelChange(int level)
    {
        // level [0 1 2] 3  -> 최대 2까지
        if(level > (int)LevelType.Max)
            level = (int)LevelType.D;

        float colliderSize = colliderSizeLevel[level];
        renderRatio = renderRatioLevel[level];
        
        // 정방향 스폰가능한지 체크하는 콜라이더 사이즈 조절 ( 작을수록 더 빨리 스폰됨 )
        overlappedBoxColRR.size = new Vector3(8, 5, colliderSize);
        overlappedBoxColRL.size = new Vector3(8, 5, colliderSize);

        spawnInterVal = colliderSize;
    }

    private void OnDestroy()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
    }

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//

    /** 주기적으로 코루틴을 통해 정방향 차량 생성 */
    public override void TrySpawnCarFreq()
    {
        spawnCoroutine = StartCoroutine(TrySpawnForwardCarFreq());
    }
    IEnumerator TrySpawnForwardCarFreq()
    {
        while(true)
        {
           // Spawn간격마다 Spawn하든안하든 false인 상태라서 
           if(Mathf.Abs(playerCarTransform.position.z - lastPlayerPosZ) >= spawnInterVal)
           {
               TrySpawnNewCars();
               lastPlayerPosZ = playerCarTransform.position.z;
           }
           yield return null;
        }
    }

    /** 설정한 간격마다 주기적으로 새로운 차들 생성 */
    public override void TrySpawnNewCars()
    {   
        // 함수 내부에서 차량생성가능한지 판단해서 생성 or 생성하지 않음
        if(CanSpawnCar(overlappedBoxColRR))
            TryPlaceCarOnLane((int)LaneType.RR, kSpawnDistance, false);

        if(CanSpawnCar(overlappedBoxColRL))
            TryPlaceCarOnLane((int)LaneType.RL, kSpawnDistance, false);
    }
}
