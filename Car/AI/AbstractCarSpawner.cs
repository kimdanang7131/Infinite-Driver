using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractCarSpawner : MonoBehaviour
{
    protected const int kSpawnDistance     = 1000; // Player 앞으로 1000정도까지 생성 ( collider의 z보다는 -100 정도 더 작게 설정 )
    protected const int kOutOfView         = 200;  // Player 뒤로 200정도 가면 Pool로 반환
    protected const int kSpawnInItMax      = 1000; // 초반 배치만 10개
    protected const int kSpawnInitInterval = 100;

    [Range(0,1f)]    public float renderRatio = 0.6f; // ** 수정할 것 , LevelType 변경되면 같이 변경될 것들 , 이벤트 구독

    protected int maxCarSpawnCount = 0; // 최대로 생성가능한 차량 수
    protected float lastPlayerPosZ = 0f;
    protected Transform playerCarTransform;
    protected LayerMask otherCarsLayerMask;
    protected LinkedList<GameObject> carAIRenderPool = new LinkedList<GameObject>(); // Render된 차량들 중간삭제가 자주 있어서 LinkedList로 관리

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//

    public abstract void TrySpawnCarFreq(); // 주기적으로 차 생성하는 코드, reverse는 주기마다 / forward는 플레이어 위치에서 일정거리 벗어났을 때
    public abstract void TrySpawnNewCars(); // 본인 차 Lane에 맞는지 체크하는 코드 이후 여기서 TryPlaceCarOnLane를 통해 스폰됨
    public abstract void ChangeValuesOnLevelChange(int level);   // GameManager의 레벨이 변경될때마다 -> reverse는 주기,ratio 변경 / forward는 생성시 체크하는 Collider 크기변경
    protected abstract bool CheckCanPlaceCar(in int carLaneIdx); // 혹시 다른 Lane에 설정했는지 , MaxCount이상 생성하려고 시도했는지

    /** 기초 설정 */
    public virtual void Init(Transform newPlayerCarTransfrom , LayerMask newOtherCarsLayerMask)
    {
        int spawnCount = kSpawnInItMax / kSpawnInitInterval; // spawnCount * 2개 생성
        maxCarSpawnCount = spawnCount;

        this.playerCarTransform = newPlayerCarTransfrom;
        this.otherCarsLayerMask = newOtherCarsLayerMask;
        GameManager.gameInstance.OnLevelChanged += ChangeValuesOnLevelChange; // reversecar스폰 주기 변경
    }

    /** Spawn할 위치에 다른 차량이 있는지 [설정한 Collider 크기만큼 겹치는지 확인]해서 겹치는게 없어야 스폰*/
    protected virtual bool CanSpawnCar(in BoxCollider boxCollider)
    {
        Collider[] colliders = Physics.OverlapBox(boxCollider.bounds.center, (boxCollider.bounds.size / 2) * 1.5f, Quaternion.identity, otherCarsLayerMask);
        return (colliders.Length == 0);
    }

    /** [Lane을 올바르게 설정했는지 -> CheckCanPlaceCar , MaxCount확인후 ] Ratio 적용해서 생성할지 정한 후 -> 위치 조정 + 배치  */
    protected virtual void TryPlaceCarOnLane(in int carLaneIdx, in float addZ, bool isReverse)
    {
        if(CheckCanPlaceCar(carLaneIdx) == false)
            return;

        // #1. renderRatio 비율에 맞춰 PoolManager를 통해 생성하고 x , z 위치 조정
        float randomValue = Random.value;
        bool checkCanSpawn = (randomValue <= renderRatio);

        if(checkCanSpawn)
        {
            GameObject carFromPool = PoolManager.poolInstance.GetRandomCarFromPool();

            // #2. 배치할 위치로 일단 이동
            float x = GlobalSettings.CarLanes[carLaneIdx]; // CarLane x 위치
            float randX = x + Random.Range(-2.5f, 2.5f); // 랜덤한 x위치
            float randZ = playerCarTransform.localPosition.z + addZ; // player 위치로부터 Z마다 일정 간격으로 생성
            carFromPool.transform.position = new Vector3(randX , 0.1f , randZ);
            
            // #3. 배치가 가능하다면 관리할 List에 추가
            AICarHandler aICarHandler;
            if(carFromPool.TryGetComponent<AICarHandler>(out aICarHandler))
            {
                switch(carLaneIdx)
                {
                    case (int)LaneType.LL:
                        aICarHandler.carScore = 5;
                        break;
                    case (int)LaneType.LR:
                        aICarHandler.carScore = 1;
                        break;
                    case (int)LaneType.RL:
                        aICarHandler.carScore = 1;
                        break;
                    case (int)LaneType.RR:
                        aICarHandler.carScore = 3;
                        break;
                    default:
                        Utils.LogError();
                        break;
                }
    
                aICarHandler.SetIsReverse(isReverse);
                carAIRenderPool.AddLast(carFromPool);
                carFromPool.SetActive(true); // 활성화
            }
        }
    }

    /** 주기적으로 거리가 멀어지면 Pool에서 제거하는 로직 */
    public virtual void CleanUpLogic()
    {
        LinkedListNode<GameObject> current = carAIRenderPool.First;
    
        while (current != null)
        {
            LinkedListNode<GameObject> next = current.Next; // 다음 노드 미리 저장
            
            float disance = current.Value.transform.localPosition.z - playerCarTransform.localPosition.z;

            // 너무 멀음 or 이미 일정거리 이상 지나쳤다면 Node에서 제거하고 Pool로 반환
            if (disance < -kOutOfView || disance > kSpawnInItMax + kOutOfView)
            {
                carAIRenderPool.Remove(current);
                PoolManager.poolInstance.ReturnCarToPool(current.Value);
            }
            current = next;
        }       
    }

    public void RemoveAllCars()
    {
        LinkedListNode<GameObject> current = carAIRenderPool.First;
    
        while (current != null)
        {
            LinkedListNode<GameObject> next = current.Next; // 다음 노드 미리 저장
            float disance = current.Value.transform.localPosition.z - playerCarTransform.localPosition.z;
            carAIRenderPool.Remove(current);
            PoolManager.poolInstance.ReturnCarToPool(current.Value);
            current = next;
        }   
    }
}
