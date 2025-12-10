using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Collections;
using UnityEngine.Localization;
using DG.Tweening;

public class UIManager_GameScene : MonoBehaviour
{
    public static UIManager_GameScene uiInstance; // 싱글톤으로 전역접근 설정
    int coinAddNum = 10;

    [Header("GameOver")]
    [SerializeField] GameObject gameOverGroup;
    [SerializeField] Button continueButton;
    [SerializeField] Button retryButton;
    [SerializeField] Button lobbyButton;
    [SerializeField] TextMeshProUGUI finalScoreText;
    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] TextMeshProUGUI gemText;
    [SerializeField] RectTransform bestScoreRectTransform;
    [SerializeField] AudioClip bestScoreSound;
    int bestScore = 0;
    Vector2 initBestScorePosition; // 초기 위치 저장 변수

    [Header("ActionText")]
    [SerializeField] GameObject actionTextGroup;
    [SerializeField] GameObject actionTextObject;
    [SerializeField] GameObject timerObject;
    [SerializeField] TextMeshProUGUI timerText;

    [Header("CoinDOTween")]
    [SerializeField] RectTransform coinImageRect;
    [SerializeField] Image coinImage;
    [SerializeField] Color coinChangeColor;
    [SerializeField] AudioClip coinAddSound;
    const float CoinChangeScale = 1.2f;

    WaitForSecondsRealtime waitForReal500ms = new WaitForSecondsRealtime(0.5f);
    WaitForSecondsRealtime waitForReal150ms = new WaitForSecondsRealtime(0.15f);
    WaitForSecondsRealtime waitForReal250ms = new WaitForSecondsRealtime(0.25f);

    [Header("InGame GUI")]
    [SerializeField, Tooltip("Setting")] GameObject pauseGroup; 
    [SerializeField] RectTransform rotarySystem;
    [SerializeField] TextMeshProUGUI scoreText;


    [Header("TopBar")]
    [SerializeField]  MusicSelectControl musicSelectControl;
    [SerializeField]  TextMeshProUGUI selectedMusicName;
    
    [Header("RewardAd")]

    [SerializeField] RewardAd rewardAd;

    [SerializeField] AICarSpawner aICarSpawner;
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject notificationPopUp;

    bool isRewardAdUsed = false;
    

    void Awake()
    {
        if (uiInstance != null)
        {
            Destroy(this);
        }
        else
        {
            uiInstance = this;
        }

        initBestScorePosition = bestScoreRectTransform.anchoredPosition;
    }

    void Start()
    {
        GameManager.gameInstance.OnGameStart    += GameStart;
        GameManager.gameInstance.OnGameOver     += GameOver;
        GameManager.gameInstance.OnScoreChanged += UpdateScoreText;

        // Json에 저장된 PlayerData이용해서 Coin, Gem , bestScore 가져오기
        LoadPlayerData();
        
        musicSelectControl.Initialize();  // ** SettingMuic도 awake이고, musicselectContorl의 start가 애초에 시작부터 false라서 작동하지 않음
        musicSelectControl.OnMainMusicNameChanged  += ChangeSelectedMusicName;
        SoundManager.soundInstance.OnNextMusicPlay += ChangeSelectedMusicName;
        ChangeSelectedMusicName(SoundManager.soundInstance.GetCurrentPlayingMusicName());
        
        isRewardAdUsed = false;
    }

    /** 재시작이라서 필요는 없음 */
    void OnDestroy()
    {
        musicSelectControl.OnMainMusicNameChanged  -= ChangeSelectedMusicName;
        SoundManager.soundInstance.OnNextMusicPlay -= ChangeSelectedMusicName;
        GameManager.gameInstance.OnGameOver     -= GameOver;
        GameManager.gameInstance.OnScoreChanged -= UpdateScoreText;
    }

    /** Json에 저장된 PlayerData이용해서 Coin, Gem , bestScore 가져오기 */
    void LoadPlayerData()
    {
        PlayerData pData = JsonDataManager.jsonInstance.LoadPlayerData();

        coinText.text = $"{pData.coin}";
        gemText.text  = $"{pData.gem}";
        bestScore     = pData.bestScore;
    }

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡScoreTextㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//
    public void UpdateScoreText(int newScore)
    {
        scoreText.text = $"{newScore}"; 
    }

    public void UpdateRotary(float ratio)
    {
        float normalizedRatio = (ratio * 2) - 1;

        // 변환된 비율을 -75와 75 사이의 각도로 매핑
        float rotaryRotRoll = Mathf.Lerp(-73, 73, (normalizedRatio + 1) / 2);
        rotarySystem.rotation = Quaternion.Euler(new Vector3(0, 0, rotaryRotRoll));
    }

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡActionTextGroupㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//

    /** 게임 시작했을때 321 GO 틀어주는 함수 */
    public void GameStart(float startTime)
    {   
        // #1. 초기 세팅
        actionTextGroup.SetActive(true);
        actionTextObject.SetActive(false);
        timerObject.SetActive(true);

        // #2. 3 2 1 텍스트 시작
        timerText.text = $"{startTime}";
        StartCoroutine(UpdateStartTimer(startTime));
    }

    /** 3 2 1까지 코루틴 적용 후 Invoke 진행 */
    IEnumerator UpdateStartTimer(float startTime)
    {
        float sTime = startTime;
        int cTime = 0;

        while(sTime > 0)
        {
            sTime -= Time.deltaTime;
            cTime = Mathf.CeilToInt(sTime);
            
            timerText.text = $"{cTime}";
            yield return null;
        }

        timerObject.SetActive(false);
        actionTextObject.SetActive(true);

        Invoke("FinishTimerSettings", 0.6f);
    }

    /** 마무리 세팅 */
    void FinishTimerSettings()
    {
        timerText.text = "3";
        actionTextGroup.SetActive(false);
    }

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡGameOverGroupㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//

    void GameOver()
    {
        gameOverGroup.SetActive(true);

        continueButton.interactable = false;
        retryButton.interactable    = false;
        lobbyButton.interactable    = false;
        // 스코어 DOTween 적용 및 bestScore 갱신
        UpdateFinalScore();
    }

    /** 최종 스코어 가져와서 DOTWeen 이용해서 등록 */
    void UpdateFinalScore()
    {
        int finalScore = 0;
        if(int.TryParse(scoreText.text, out finalScore) == false)
        {
            //** 게임종료시켜야함
            Application.Quit();
            return;
        }

        // DOTween으로 숫자들이 1초동안 랜덤하게 변하며 최종스코어로 갱신됨
        int fromScore = 0;
        finalScoreText.DOCounter(fromScore, finalScore, 0.5f).SetUpdate(true).OnComplete(() =>
        {
            StartCoroutine(CheckBestScoreAndEnableButtons(finalScore));
        });
    }

    /** finalScore 체크해서 DOTween 기능과 함께 BestScore 갱신 */
    IEnumerator CheckBestScoreAndEnableButtons(int finalScore)
    {
        // #1. 갱신해야할때
        if (finalScore >= bestScore)
        {
            // #2. AnchorPos까지 DOMove
            Vector2 targetPosition = new Vector2(103f, 70f);
            bestScoreRectTransform.DOAnchorPos(targetPosition, 0.5f).SetUpdate(true).SetEase(Ease.OutBounce);

            // #3. 사운드 있을때만 재생
            if (bestScoreSound != null)
                SoundManager.soundInstance.PlayOneShot(SoundType.Button, bestScoreSound);
            
            // #4. bestScore 갱신
            PlayerData pData = JsonDataManager.jsonInstance.LoadPlayerData();
            pData.UpdateBestScore(finalScore);
            JsonDataManager.jsonInstance.Save(pData); // PlayerData 저장
            yield return new WaitForSecondsRealtime(0.5f); // 0.5초 대기
        }

        if(isRewardAdUsed == false)
        {
            continueButton.interactable = true;
        }
        
        retryButton.interactable = true;
        lobbyButton.interactable = true;
    }

    /** 이어하기(광고) , 재시도(3번에 한번 광고) , Exit - 메인메뉴로 이동ㅊ*/
    public void AdContinue()
    {
        ResetBestScoreAnchorPos();

        // 광고형 보상이 게시되고, closed를 누르면 콜백되는 함수
        rewardAd.ShowRewardedAd((int amount) =>
        {
            if (aICarSpawner != null)
            {
                aICarSpawner.RemoveAllCars();
            }
            StartCoroutine(HandleReward(amount));
        });

        //notificationPopUp.SetActive(true);
        //Time.timeScale = 0f;

        //if(aICarSpawner != null)
        //{
        //    aICarSpawner.RemoveAllCars();
        //    isFrontAdUsed = true;
        //    continueButton.interactable = false;
        //    gameOverGroup.SetActive(false);
        //    GameManager.gameInstance.AdContinue();
        //}
    }
    IEnumerator HandleReward(int amount)
    {
        yield return null; // 실제 시간 기준 지연

        Time.timeScale = 0f;

        continueButton.interactable = false;
        isRewardAdUsed = true;

        PlayerData pData = JsonDataManager.jsonInstance.LoadPlayerData();
        pData.UpdateGem(+amount);
        JsonDataManager.jsonInstance.Save(pData);
        
        gemText.text = $"{pData.gem}";
        PlayerPrefs.SetInt("frontAd", 0);
        notificationPopUp.SetActive(true);
    }


    public void Retry()
    {
        StartCoroutine(AnimateCoinImageAndReduceScore("GameScene"));
    }
    public void Lobby()
    {
        StartCoroutine(AnimateCoinImageAndReduceScore("LobbyScene"));
    }

    /** AddCount양에 맞춰 Score 감소, Coin 증가 */
    IEnumerator AnimateCoinImageAndReduceScore(string sceneName)
    {
        ChangeScoreByAmount(); // 스코어 양에따라 addCount증가
        int score = 0;

        if (int.TryParse(finalScoreText.text, out score))
        {
            // #1. coinAddNum 만큼 score 감소시키며 그 값만큼 [ 현재 나의 coin 재화 증가 ]
            while(score >= coinAddNum)
            {
                score -= coinAddNum; // 정한 interval만큼 score 감소
                CoinAddAnimationAndSound(0.075f , coinAddNum); // 정한 interval만큼 coin 재화 증가

                finalScoreText.text = score.ToString(); // 남은 score 재화 표시
                yield return waitForReal150ms; // 애니메이션 대기
            }

            // #2. 남은 score 나의 coin에 추가
            if(score > 0)
            {
                CoinAddAnimationAndSound(0.075f , score); // 정한 interval만큼 coin 재화 증가
                score = 0;

                finalScoreText.text = score.ToString(); // 전부 털어낸것 확인
                yield return waitForReal150ms;
            }
            
            // #3. 현재 내 재화(coin) PlayerData 저장 및 갱신
            int coinNum = 0; 
            if (int.TryParse(coinText.text, out coinNum))
            {
                PlayerData pData = JsonDataManager.jsonInstance.LoadPlayerData();
                pData.SetCoin(coinNum);
                JsonDataManager.jsonInstance.Save(pData); // PlayerData 저장
            }

            yield return waitForReal250ms; // 애니메이션 종료 대기

            ResetBestScoreAnchorPos();
            gameOverGroup.SetActive(false);

            if(sceneName.CompareTo("GameScene") == 0)
            {
                if(AdManager.adInstance.Button_PlayButtonClicked("GameScene") == false)
                {
                    LoadingManager.LoadScene(sceneName);
                }
            }
            else
            {
                LoadingManager.LoadScene(sceneName);
            }
        }
    }

    void CoinAddAnimationAndSound(float duration , int coinAddAmount)
    {
        // 스케일 커졌다가 작아지는 애니메이션
        coinImageRect.DOScale(CoinChangeScale, duration).SetUpdate(true).OnComplete(() =>
        {
            // 원래 스케일로 돌아가는 애니메이션
            coinImageRect.DOScale(1f, duration).SetUpdate(true);
                
            // 코인 수 갱신
            int coinNum = 0; // 초기화
            if (int.TryParse(coinText.text, out coinNum))
            {
                coinNum += coinAddAmount;
                coinText.text = coinNum.ToString();
            }
        });

        PlayCoinAddSound();
    }

    void PlayCoinAddSound()
    {
        SoundManager.soundInstance.PlayOneShot(SoundType.Effect, coinAddSound);
    }

    /** 스코어 양에따라 addCount증가 */
    void ChangeScoreByAmount()
    {
        int score = 0;
        if (int.TryParse(finalScoreText.text, out score))
        {   
            // 0 ~ 100
            if(score < 10)
            {
                coinAddNum = 1;
            }
            else if(score < 100)
            {
                coinAddNum = 10;
            }
            else if( score < 1000 ) // 100 ~ 999
            {
                coinAddNum = 50;
            }
            else if( score < 10000 ) // 1000 ~ 9999
            {
                coinAddNum = 500;
            }
        }
    }

    /*
    // 클릭 이벤트를 처리하여 애니메이션 중단
    public void OnClickSkipAnimation()
    {
        isAnimating = false; // 애니메이션 종료 상태로 설정
        coinImageRect.Kill(); // DOTween 애니메이션 즉시 중단
        coinImage.Kill(); // 색상 애니메이션도 즉시 중단

        // 코인 증가 즉시 처리
        int coinNum = 0;
        if (int.TryParse(coinText.text, out coinNum))
        {
            coinNum++; // 코인 1 증가
            coinText.text = coinNum.ToString();
        }
        else
        {
            Debug.LogError("coinText의 값이 숫자가 아닙니다.");
        }

        // 즉시 게임 종료 및 씬 변경
        PlayerData? playerdata = JsonDataManager.jsonInstance.LoadPlayerData();
        if (playerdata != null)
        {
            playerdata.coin = coinNum;
            JsonDataManager.jsonInstance.SavePlayerData(playerdata);
        }

        ResetBestScoreAnchorPos();
        gameOverGroup.SetActive(false);
        SceneManager.LoadScene("NextSceneName"); // 원하는 씬으로 로드
    }
    */

    void ResetBestScoreAnchorPos()
    {
        bestScoreRectTransform.anchoredPosition = initBestScorePosition;
    }

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//

    public void InGamePause()
    {
        pauseGroup.SetActive(true);
        GameManager.gameInstance.GamePause();
    }

    public void InGameContinue()
    {
        pauseGroup.SetActive(false);
        GameManager.gameInstance.GameContinue();
    }

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//

    public void ChangeSelectedMusicName(string newMusicName)
    {
        var localizedString = new LocalizedString("MusicTable", newMusicName);
        localizedString.StringChanged += (value) => selectedMusicName.text = value;
    }

    /** select된 Music의 이름으로 바꾸고 scroll 끄기 */
    public void CloseScrollArea(string newMusicName)
    {
        ChangeSelectedMusicName(newMusicName);
        musicSelectControl.OpenCloseMusicList();
    }

    public void SetMusicListOrder()
    {
        musicSelectControl.SetMusicListOrder();
    }
}
