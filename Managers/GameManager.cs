using System;
using System.Collections;
using UnityEngine;
using VInspector;

public class GameManager : MonoBehaviour
{
    public event Action<int> OnLevelChanged;
    public event Action<int> OnScoreChanged;
    public event Action<float> OnGameStart;
    public event Action OnGameOver; // UI에게 알리기 위해
    public event Action OnWaitForStart;

    public static GameManager gameInstance; // 싱글톤으로 전역접근 설정
    
    [Header("# Frame Setting")]
    public float TargetFrameRate = 60.0f; // 목표 프레임 속도
    
    [Header("# GameScene Settings")]
    [SerializeField, Tooltip("게임시작전 세팅")] float waitForStartTime = 3f;
    [SerializeField] public CameraControl vc;
    public bool isGameOver { get; set; } = true;

    [Header("# SkyBoxSetting")]
    [SerializeField] SkyBoxControl skyBoxControl;

    [Header("GameOver")]
    [SerializeField] AudioClip gameoverSound;

    public LevelType LevelType { get; set; } = LevelType.A;
    public SceneType SceneType { get; set; } = SceneType.Loading;
    public MapType MapType     { get; set; } = MapType.Desert;

    // Player 참조
    public PlayerCarHandler playerCar;
    public Transform playerCarTransform;
    
    public AudioClip nextLevelAudioClip;

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//
    int score    = 0; // 점수
    int curLevel = 0; // 레벨 ( 게임 진행 시간에 따라 Json으로 관리 )
    int levelUpScore1 = 100;
    int levelUpScore2 = 300;
    int levelUpScore3 = 700;


    float timeBetScore = 0f; // 점수 증가 시간
    float speed = 0f; // 플레이어의 현재속도에 비례해 점수 증가

    WaitForSeconds waitFor500ms = new WaitForSeconds(0.5f);

    void Awake() 
    {
        if(gameInstance != null)
        {
            Destroy(this);
        }
        else
        {
            gameInstance = this;
        }

        QualitySettings.vSyncCount = 0; // 수직동기화 끄기
        Application.targetFrameRate = (int)TargetFrameRate; // 목표 프레임 속도 설정

        //** GameScene에서 사용 예시 -> Lobby 할때 변경
        MapType type = LoadMapType(PlayerPrefs.GetString("Map")) ?? MapType.Desert; // ?? 기준 -> null이 아니면 A , null이면 B
        MapType = type;

        if(skyBoxControl != null)
        {
            skyBoxControl.SetMapType(MapType);
        }
    }

    void Start()
    {
        Reset();
    }

    void Update()
    {
        if(isGameOver)
            return;

        // 속도에 따른 점수 증가
        speed = playerCar.CurrentForwardVelocity * 1f;

        // Score 증가 로직, 코인같은 아이템은 없고 마지막에 DOTWEEN으로 증가?
        timeBetScore += speed * Time.deltaTime;
        if(timeBetScore >= 100)
        {
            timeBetScore = 0f;

            score++;
            UpdateScore(score);
        }
    }

    /** Retry or Lobby에서 Play시 값 초기화 */
    public void Reset()
    {
        SoundManager.soundInstance.GetEngineAudio().Stop();
       
        // #1. 기본 수치 재설정
        speed = 0f;
        score = 0;
        UpdateScore(0);
        
        // #2. 게임 상태 재설정
        isGameOver = true;
        Time.timeScale = 1f; // Pause 방지
        
        StartGame();
        StartCoroutine(CheckLevelUp());
    }
    IEnumerator CheckLevelUp()
    {
        int levelUpCount = 0; // 레벨업 횟수 카운터

        while (true)
        {
            if (score >= levelUpScore1 && levelUpCount == 0)
            {
                ChangeLevel(curLevel + 1);
                levelUpCount++;
            }
            else if (score >= levelUpScore2 && levelUpCount == 1)
            {
                ChangeLevel(curLevel + 1);
                levelUpCount++;
            }
            else if (score >= levelUpScore3 && levelUpCount == 2)
            {
                ChangeLevel(curLevel + 1);
                levelUpCount++;
            }


            if (levelUpCount >= 3)
            {
                Utils.Log("StopCoroutine");
                StopCoroutine(CheckLevelUp()); // 레벨업 2번 후 코루틴 종료
                break;
            }

            yield return waitFor500ms; // 0.1초마다 확인
        }
    }

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡGameSceneDataㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//

    public void SetPlayerCar(PlayerCarHandler playerCar)
    {
        this.playerCar     = playerCar;
        playerCarTransform = playerCar.transform;
    }

    public void SetCameraControl(CameraControl cameraControl)
    {
        vc = cameraControl;
    }

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ이벤트ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//

    /** Lobby Scene에서 Play버튼 눌렀을 때마다 시작 대기시간 */
    public void StartGame()
    {
        OnGameStart(waitForStartTime);
        Invoke("WaitForStart", waitForStartTime);
    }
    
    /** ActionaManager와 연동 */
    void WaitForStart()
    {
        isGameOver = false;

        // 게임이 시작되고나서 PlayerCar의 EngineSound도 시작
        if(skyBoxControl != null)
            skyBoxControl.StartSkyBoxBlending();
            
        SoundManager.soundInstance.GetEngineAudio().Play();
        OnWaitForStart();
    }

    void ChangeLevel(int newLevel)
    {
        if(newLevel >= (int)LevelType.Max)
        {
            newLevel = (int)LevelType.D;
            return;
        }

        SoundManager.soundInstance.PlayOneShot(SoundType.Effect, nextLevelAudioClip);
        curLevel = newLevel;
        OnLevelChanged(curLevel); // 이벤트
    }

    public void UpdateScore(int newScore)
    {
        OnScoreChanged?.Invoke(newScore);
    }

    public void AddScore(int addScore)
    {
        score += addScore;
        OnScoreChanged?.Invoke(score);
    }

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//

    [Button]
    void LevelUp()
    {
        ChangeLevel(curLevel + 1);
        Utils.Log(curLevel.ToString());
    }

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//


    /** 게임 일시정지 + 소리까지 */
    public void GamePause()
    {
        Time.timeScale = 0f;
        SoundManager.soundInstance.GetEngineAudio().Pause();
    }
    /** 게임 Continue + 소리까지 */
    public void GameContinue()
    {
        Time.timeScale = 1f;
        SoundManager.soundInstance.GetEngineAudio().UnPause();
    }
    /** 게임 일시정지 + 소리까지, UI 전달 */
    public void GameOver()
    {
        SoundManager.soundInstance.PlayOneShot(SoundType.Effect, gameoverSound);
        
        GamePause();
        isGameOver = true;

        OnGameOver();
    }
    /** 게임 Continue + 소리까지 */
    public void AdContinue()
    {
        GameContinue();
        isGameOver = false;
    }

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//
    MapType? LoadMapType(string mapKey)
    {
        if (string.IsNullOrEmpty(mapKey))
        {
            return null;
        }
    
        switch (mapKey)
        {
            case "Desert":
                return MapType.Desert;
            case "City":
                return MapType.City;
            default:
                return null;
        }
    }
}

