using System.Collections;
using UnityEngine;

/** 전체적으로 각 레벨에 따른 [생성 Ratio 및 생성 주기 ] 관리 */
public class ReverseCarSpawner : AbstractCarSpawner
{
    [Header("# 역방향 Collider")]
    [SerializeField] BoxCollider overlappedBoxColLR;
    [SerializeField] BoxCollider overlappedBoxColLL;

    // 생성 주기 -> GameManager의 Level타입에 따라 이벤트로 구독되어 변경될 값들
    float[] reverseSpawnFreq  = {0.85f, 0.65f, 0.5f,  0.4f};
    float[] renderRatioLevel  = {0.4f , 0.45f, 0.52f, 0.57f};
    float curReverseSpawnFreq = 0.85f;

    // 주기적으로 생성하기 위한 변수
    float lastSpawnReverseTime  = 0f;


    /** [Lane을 올바르게 설정했는지 , MaxCount확인후  Ratio 적용해서 생성할지 정한 후 -> 위치 조정 + 배치  */
    protected override bool CheckCanPlaceCar(in int carLaneIdx)
    {
        // 왼쪽 라인(Reverse)에 생성하려고할때 return;
        if(carAIRenderPool.Count >= maxCarSpawnCount || carLaneIdx >= 2)
        {
            return false;
        }
        else   
            return true;
    }

    /** Game의 레벨이 바뀔때마다 reverseCar 생성주기 변경 */
    public override void ChangeValuesOnLevelChange(int level)
    {
        if(level > (int)LevelType.Max)
            level = (int)LevelType.D;

        curReverseSpawnFreq = reverseSpawnFreq[level]; // 역방향 차량 스폰 주기 레벨에 따라 점점 짧아지도록
        renderRatio         = renderRatioLevel[level]; // 역방향 차량 생성 비율도 레벨에 따라 변경
    }

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//

    /** 주기적으로 코루틴을 통해 역방향 차량 생성 */
    public override void TrySpawnCarFreq()
    {
        StartCoroutine(TrySpawnReverseCarFreq());
    }

    IEnumerator TrySpawnReverseCarFreq()
    {
        while(true)
        {   
            // Time.time은 게임이 시작하고 난후 경과된 시간 , 지속적으로 주기적으로 업데이트해주면서 차 생성
            if(Time.time - curReverseSpawnFreq  > lastSpawnReverseTime)
            {
                lastSpawnReverseTime = Time.time;
                TrySpawnNewCars();
            }
            
            yield return null;
        }
    }


    /** 설정한 간격마다 주기적으로 새로운 차들 생성 */
    public override void TrySpawnNewCars()
    {
        // 함수 내부에서 차량생성가능한지 판단해서 생성 or 생성하지 않음
        if(CanSpawnCar(overlappedBoxColLL))
            TryPlaceCarOnLane((int)LaneType.LL, kSpawnDistance, true);

        if(CanSpawnCar(overlappedBoxColLR))
            TryPlaceCarOnLane((int)LaneType.LR, kSpawnDistance, true);
    }
}
