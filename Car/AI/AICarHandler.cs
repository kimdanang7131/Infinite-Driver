using System.Collections;
using UnityEngine;

public class AICarHandler : CarHandler
{
    // Overlapped Check
    [SerializeField] LayerMask otherCarsLayerMask; // Physics.BoxCastNonAlloc을 위한 변수
    [SerializeField , HideInInspector] BoxCollider boxCollider;

    const int kRandOffset = 50;

    // maxForward는 carData, Max는 Currnet와 같은
    public float CurrentMaxForwardVelocity  = 0f;
    int[] reverseLevelSpeed = {0, 20, 40, 70};
    int additionalLevelSpeed = 0;

    RaycastHit[] raycastHits = new RaycastHit[1];  // Collision Detection

    public bool IsScoreAdded { get; set;} = false;
    public int carScore = 5;
    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//

    //int drivingInLane = 0;  // Lanes
    public bool isReverse = false;

    static readonly WaitForSeconds waitFor100ms = new WaitForSeconds(0.1f);

    protected override void Awake() 
    {
        base.Awake();

        if(gameObject.TryGetComponent<BoxCollider>(out boxCollider) == false)
            Utils.LogError();
    }

    void OnEnable() 
    {
        isReverse = false;
        IsScoreAdded = false;

        int randOffset = Random.Range(-kRandOffset, 0);
        SetSpeed(carData.maxForwardVelocity + randOffset + additionalLevelSpeed); // 현재 레벨타입에 맞게 추가속도 있음

        StartCoroutine(UpdateLessOftenCRT());
    }

    void FixedUpdate() 
    {
        if(GameManager.gameInstance.isGameOver)
            return;
        
        CarMove(); // 앞뒤 이동
        Steer();
    }

    /** 최대속도만 변경 */
    void SetCurrentMaxVelocity(in float newCurrentMax)
    {
        CurrentMaxForwardVelocity = newCurrentMax;
    }

    /** 최대속도 변경, 현재 속도를 최대속도로 급속도로 변경 */
    void SetSpeed(in float newMaxForwardVelocity)
    {
        SetCurrentMaxVelocity(newMaxForwardVelocity);
       // rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, CurrentMaxForwardVelocity);
       // rb.velocity = transform.forward * rb.velocity.magnitude;

        rb.velocity = transform.forward * CurrentMaxForwardVelocity;
    }

    /** 차의 속도를 GameManager의 LevelType에 맞게 변경 -> 이벤트 구독으로 인해서 */
    void SetReverseCarLevelSpeed(int level)
    {
        if(level > (int)LevelType.D)
            level = (int)LevelType.D;
    
        int speed = reverseLevelSpeed[level];
        additionalLevelSpeed = speed;
    }

    protected override void CarMove()
    {
        if(rb.velocity.z > CurrentMaxForwardVelocity)
            return;

        rb.AddForce(transform.forward * carData.accelValue);

        rb.velocity = transform.forward * rb.velocity.magnitude;
    } 
    
    /** 차의 좌우이동 로직 -> AICarHandler에서는 Deprecated */ 
    protected override void Steer()
    {
    }

    /** 역주행 차 설정 */
    public void SetIsReverse(bool isReverse)
    {
        this.isReverse = isReverse;

        float rValue = 0f; // rotationValue

        if(this.isReverse)
        {
            rValue = 180f;
            SetSpeed(carData.maxForwardVelocity * 0.085f);
        }

        this.transform.localRotation = Quaternion.Euler(0,rValue,0);
    }

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//
    
    /** 0.2초마다 차 앞에 BoxCollider를 하나 놓아서 등록된 LayerMask인 차가 겹칠때만 Brake()를 실행하도록 */
    IEnumerator UpdateLessOftenCRT()
    {
        while(true)
        {
            if(!isReverse)
                SetSpeedIfOtherCarsAhead();

            yield return waitFor100ms;
        }
    }

    /** 앞에 차 있는지 박스를 MaxDistance위치에 쏴서 LayerMask를 통해 확인 */
    void SetSpeedIfOtherCarsAhead()
    {
        float newZ = transform.position.z + 30f; // 30f는 앞에 차가 있는지 확인할 거리

        int numberOfHits = Physics.BoxCastNonAlloc(new Vector3(transform.position.x, transform.position.y, newZ),
                             boxCollider.bounds.center, transform.forward, raycastHits, Quaternion.identity, 30 , otherCarsLayerMask); 
        
        //Debug.DrawRay(new Vector3(transform.position.x, transform.position.y, newZ), transform.forward * 30f, Color.red);
        if(numberOfHits > 0)
        {   
            GameObject hitObject = raycastHits[0].collider.gameObject;
            AICarHandler hitHandler;

            if(hitObject.TryGetComponent<AICarHandler>(out hitHandler) == false)
            {
                Utils.LogError();
                return;
            }
            else
            {
                float newVelocity = hitHandler.CurrentMaxForwardVelocity;
                SetSpeed(newVelocity);
            }
        }
    }
}
