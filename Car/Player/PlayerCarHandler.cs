using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCarHandler : CarHandler
{
    // 속도 관련 ( 좌우 이동 )
    const float kMaxSteerDeltaRotYaw  = 7f; // 일단 필요없는듯 Itv를 이용한 Lerp라 Deprecated ^^
    const float kMaxSteerDeltaRotRoll = 10f; // 일단 필요없는듯 Itv를 이용한 Lerp라 Deprecated ^^
    const float kMaxSteerDeltaX   = 50f;
    const float kMaxSteerDistance = 25.5f;

    [SerializeField] public Transform modelTransform; // ** 나중에 CarData로 교체예정
    [SerializeField] public PlayerSpawner playerSpawner;
    SoundType soundType = SoundType.Engine;

    float steerIntervalX = 0f; // [PlayerInput] (Ray로 찍은 마우스 위치 X) 와 (내 X좌표) 사이의 간격
    float steerDeltaX    = 0f; // [PlayerInput] 마우스 이동 사이의 델타값
    float rotYaw  = 0f;        // 차가 SteerDeltaX의 값에 따라 Yaw 회전
    float rotRoll = 0f;        // 차가 SteerDeltaX의 값에 따라 Roll 회전
    Quaternion targetRotation = Quaternion.identity;

    bool isDragging = false; // [PlayerInput] Drag를 했는지 안했는지 판별 여부
    //public float InitForwardVelocity = 0f;

    [Header("SFX")] 
    AudioSource carEngineAS;
    AnimationCurve curve;

    

    protected override void Awake() 
    {
        base.Awake();
    }

    void Start()
    {   
        //** 일단 테스트를 위해 ?로 표시함
        CarDataJson cData = JsonDataManager.jsonInstance.LoadLastCarDataJson(); // Json을 받음
        carData = JsonDataManager.jsonInstance.LoadResourceCarData(cData.carTag); // -> Resources로 뽑아오는데 Addressable로 바꾸기

        playerSpawner.SpawnPlayerCar(carData); // carData를 통해 차량 생성

        // SoundManager에서 AudioSource 가져오기
        carEngineAS = SoundManager.soundInstance.GetEngineAudio();
        curve = carData.carPitchCurve;
    }

    void FixedUpdate() 
    {
        if(GameManager.gameInstance.isGameOver)
            return;
        
        CarMove(); // 앞뒤 이동
        Steer();   // 좌우 이동
    }

    /** [PlayerInput] 마우스 드래그 상태에 따라 차량 회전 */
    void Update()
    {
        if(GameManager.gameInstance.isGameOver)
            return;

        targetRotation = Quaternion.Euler(new Vector3(modelTransform.rotation.x, rotYaw, rotRoll));
        
        // 드래그 중일 때와 아닐 때 차량 회전
        if(isDragging)
            modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, targetRotation,  10f * Time.deltaTime); 
        else
            modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, Quaternion.identity, 15f * Time.deltaTime);   

        // 속도에 따른 차량 사운드 조절
        UpdateCarAudio();  
    }

    /** 현재 속도에 맞춰서 차의 Pitch 사운드 조절 , UI 회전계 속도 업데이트 */
    void UpdateCarAudio()
    {
        if(GameManager.gameInstance.isGameOver)
            return;

        float speedPercentage = rb.velocity.z / carData.maxForwardVelocity; // 현재 속도의 백분율
        
        UIManager_GameScene.uiInstance.UpdateRotary(speedPercentage); // UI의 회전계 업데이트
        carEngineAS.pitch = curve.Evaluate(speedPercentage); // 속도에 따른 피치값 변경
    }
    void FadeOutCarAudio()
    {
        if(GameManager.gameInstance.isGameOver)
            return;

        carEngineAS.volume = Mathf.Lerp(carEngineAS.volume , 0, Time.deltaTime * 10);
    }
    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//
    
    /** [PlayerInput] 마우스 드래그 상태에 따라 차량 좌우 이동 */
    public void SetIntervalBetweenMouseX(float newSteerIntervalX)
    {
        newSteerIntervalX = Mathf.Clamp(newSteerIntervalX, -kMaxSteerDeltaX, kMaxSteerDeltaX);
        steerIntervalX = newSteerIntervalX;
    }

    /** [PlayerInput] 마우스 드래그 상태에 따라 차량 좌우 이동 */
    public void SetSteerDeltaX(float newSteerDeltaX)
    {
        steerDeltaX = newSteerDeltaX;   

        rotYaw  = Mathf.Clamp(steerDeltaX, -kMaxSteerDeltaRotYaw , kMaxSteerDeltaRotYaw);
        rotRoll = Mathf.Clamp(steerDeltaX, -kMaxSteerDeltaRotRoll, kMaxSteerDeltaRotRoll);
    }

    /** [PlayerInput] 마우스가 드래그 상태인지 받아서 저장 */
    public void SetIsDragging(bool newDragging)
    {
        isDragging = newDragging;
    }

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//

    /** 차의 좌우이동 로직 */
    protected override void Steer()
    {
        if(!isDragging)
            steerIntervalX = Mathf.Lerp(steerIntervalX, 0, carData.steerBrakeValue * Time.deltaTime); // 마우스 드래그가 끝나면 서서히 0으로 돌아가게

        // 내 위치와 마우스 위치 사이의 거리를 Lerp하며 이동
        float lerpX = Mathf.Lerp(transform.position.x, transform.position.x + steerIntervalX, carData.steerValue * Time.deltaTime); 
        lerpX = Mathf.Clamp(lerpX, -kMaxSteerDistance, kMaxSteerDistance); // 차량이 화면 밖으로 나가지 않도록

        // 차량이 화면 밖으로 나가면 카메라 흔들기 ( 적정한 거리에서 화면 흔들기 시작 )
        if(Mathf.Abs(lerpX) > (kMaxSteerDistance - 1f))
        {
            CameraControl.camInstance.ShakeCameraSmooth();
            Brake();
        }
        else
        {
            CameraControl.camInstance.StopCameraShake();
            BrakeRelease();
        }

        transform.position = new Vector3(lerpX , 0, transform.position.z);
    }
}
