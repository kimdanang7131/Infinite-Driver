using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using VInspector;
using UnityEngine.Localization;

public class UIManager_LobbyScene : MonoBehaviour
{
    public static UIManager_LobbyScene uiInstance;

    [Header("TopBar")]
    [SerializeField]  TextMeshProUGUI coinText;
    [SerializeField]  TextMeshProUGUI gemText;
    [SerializeField]  MusicSelectControl musicSelectControl;
    [SerializeField]  TextMeshProUGUI selectedMusicName;

    [Header("BestScore")]
    [SerializeField] TextMeshProUGUI bestscoreText;

    [Header("CarInfo")]
    [SerializeField] Image carChangeImage;
    [SerializeField] TextMeshProUGUI carChangeText;
    [SerializeField] Slider maxForwardVelocity;
    [SerializeField] Slider accelValue;
    [SerializeField] Slider steerValue;

    [Header("Lobby")]
    [SerializeField] GameObject lobby; 

    [Header("Shop")]
    [SerializeField] Button carChangeButton; // middle 현재 선택한 차 변경 시 버튼 클릭
    [SerializeField] Shop shop; // carChangeButton 클릭 시 Shop 창 열림
    
    [Header("Stage")]
    [SerializeField] Stage stage; 

    [Header("CashShop")]
    [SerializeField] GameObject cashShop;

    [Header("MusicControl")]
    [SerializeField] MusicControl musicControl;

    [Header("Notification")]
    [SerializeField] GameObject notification; // 알림창

    [Header("Canvas")]
    [SerializeField] Canvas canvas;

    List<GameObject> uiPanels = new List<GameObject>();

    #nullable enable
    public CarDataJson? LastPlayedCarDataJson { get; set;} = null; // 마지막으로 플레이한 차량 데이터

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//

    void Awake()
    {
        if(uiInstance != null)
        {
            Destroy(this);
        }    
        else
        {
            uiInstance = this;
        }

        uiPanels.Add(lobby);
        uiPanels.Add(shop.gameObject);
        uiPanels.Add(stage.gameObject);
        uiPanels.Add(cashShop.gameObject);
        uiPanels.Add(musicControl.gameObject);
    }

    void Start()
    {
        // 배경화면 최근 차량 업데이트
        //UpdateCarInfo();

        musicSelectControl.Initialize();  // ** SettingMuic도 awake이고, musicselectContorl의 start가 애초에 시작부터 false라서 작동하지 않음

        musicSelectControl.OnMainMusicNameChanged  += ChangeSelectedMusicName;
        SoundManager.soundInstance.OnNextMusicPlay += ChangeSelectedMusicName;

        canvas.sortingOrder = -1;
    }

    void OnDestroy()
    {
        musicSelectControl.OnMainMusicNameChanged -= ChangeSelectedMusicName;
        SoundManager.soundInstance.OnNextMusicPlay -= ChangeSelectedMusicName;
    }

    // 테스트용 
    [Button]
    public void InsertCoin1000()
    {
        PlayerData pData = JsonDataManager.jsonInstance.LoadPlayerData();
        pData.UpdateCoin(+1000);
        JsonDataManager.jsonInstance.Save(pData);

        UpdateCoinText(pData.coin);
    }

    [Button]
    public void InsertGem1000()
    {
        PlayerData pData = JsonDataManager.jsonInstance.LoadPlayerData();
        pData.UpdateGem(+1000);
        JsonDataManager.jsonInstance.Save(pData);

        UpdateGemText(pData.gem);
    }

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡLobby 관련ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//

    /** 마지막으로 플레이한 차량 UI 업데이트 -> LocaleManager Start에서 함 */
    public void UpdateCarInfo()
    {
        LastPlayedCarDataJson = JsonDataManager.jsonInstance.LoadLastCarDataJson(); // 마지막으로 플레이한 차량 데이터 로드

        // #1. PlayerData에 기본 차량 추가
        PlayerData pData = JsonDataManager.jsonInstance.LoadPlayerData();
        
        if(pData.AddCar(LastPlayedCarDataJson.carTag) == false)
            Utils.Log("UIManager_Lobby -> 차량 이미 있음");

        JsonDataManager.jsonInstance.Save(pData); // PlayerData 저장

        // #2. Topbar 재화 ,젬, 베스트 스코어 업데이트
        UpdateTopBarInfo();
        carChangeImage.sprite = Resources.Load<Sprite>(LastPlayedCarDataJson.carIconPath); // Resource 폴더에서 스프라이트 로드 -> Addressable

        // #3. 로컬라이제이션
        var localizedString = new LocalizedString("CarTable", LastPlayedCarDataJson.carTag);
        localizedString.StringChanged += (value) => carChangeText.text = value;

        // #4. 차량 정보 업데이트
        maxForwardVelocity.value = LastPlayedCarDataJson.maxForwardVelocity;
        accelValue.value = LastPlayedCarDataJson.accelValue;
        steerValue.value = LastPlayedCarDataJson.steerValue;
    }

    /** 마지막으로 플레이한 플레이어 데이터 업데이트 ( Coin, Gem , BestScore) */
    void UpdateTopBarInfo()
    {
        PlayerData pData = JsonDataManager.jsonInstance.LoadPlayerData();
        
        // TopBar Update
        UpdateCoinText(pData.coin);
        UpdateGemText(pData.gem);
        UpdateBestScoreText(pData.bestScore);
    }

    /** select된 Music의 이름으로 바꾸고 scroll 끄기 */
    public void CloseScrollArea(string newMusicName)
    {
        ChangeSelectedMusicName(newMusicName);
        musicSelectControl.OpenCloseMusicList();
    }

    public void ChangeSelectedMusicName(string newMusicName)
    {
        var localizedString = new LocalizedString("MusicTable", newMusicName);
        localizedString.StringChanged += (value) => selectedMusicName.text = value;
    }

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡTopBar 관련 업데이트ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//

    public void UpdateCoinText(int coin)
    {
        coinText.text = coin.ToString();
    }
    public void UpdateGemText(int gem)
    {
        gemText.text = gem.ToString();
    }
    public void UpdateBestScoreText(int bestScore)
    {
        bestscoreText.text = bestScore.ToString();
    }
    
    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡCashShop 관련ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//
    
    /** 젬 부족시 PopUp 띄움 */
    public void OpenNotification(string message)
    {
        notification.SetActive(true);
        notification.GetComponentInChildren<TextMeshProUGUI>().text = message;
    }

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡShop 관련ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//

    void CloseUIPanels()
    {
        foreach(GameObject ui in uiPanels)
        {
            ui.SetActive(false);
        }
    }

    /** carChangeButton(carImage) 클릭시 -> Shop 이동 */
    public void OpenShop()
    {
        CloseUIPanels();

        if(LastPlayedCarDataJson != null)
            shop.OpenShop(LastPlayedCarDataJson.carTag); // Shop UI 활성화
    }

    /** Shop에서 Prev 버튼 클릭시 이전 차량 보여주기 */
    public void ShopShowPrev()
    {
        shop.ShowPrev();
    }
    public void ShopShowNext()
    {
        shop.ShowNext();
    }

    /** Shop에서 Return 버튼 클릭시 Lobby로 이동 */
    public void ReturnShopToLobby()
    {
        shop.ReturnToLobby();
        lobby.gameObject.SetActive(true);
        UpdateCarInfo();
    }

    /****
    /** Shop에서 차량 선택시 현재 선택차량 해제 후 -> 새로운 차량 선택 */
    public void ShopSelectCar(ShopCarData carData)
    {
        LastPlayedCarDataJson = JsonDataManager.jsonInstance.LoadLastCarDataJson(); 
        shop.SelectCar(carData);
    }

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡStage 관련ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//

    /** Play 버튼 클릭시 -> Stage 이동 */
    public void OpenStage()
    {
        CloseUIPanels();

        // 혹시 모르니 한번더 최근 차 저장
        CarDataJson LastPlayedCarDataJson = JsonDataManager.jsonInstance.LoadLastCarDataJson();
        JsonDataManager.jsonInstance.Save(LastPlayedCarDataJson); // 마지막으로 플레이한 차량 데이터 저장

        stage.gameObject.SetActive(true);
    }

    /** Stage에서 Prev 버튼 클릭시 이전 차량 보여주기 */
    public void StageShowPrev()
    {
        stage.ShowPrev();
    }
    public void StageShowNext()
    {
        stage.ShowNext();
    }
    /** Stage에서 Return 버튼 클릭시 Lobby로 이동 */
    public void ReturnStageToLobby()
    {
        stage.ReturnToLobby();
        lobby.gameObject.SetActive(true);
        UpdateCarInfo();
    }

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡCashShop 관련ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//

    public void OpenCashShop()
    {
        CloseUIPanels();
        cashShop.gameObject.SetActive(true);
    }

    //** 캐시 구매 

    /** CashShop에서 Return 버튼 클릭시 Lobby로 이동 */
    public void ReturnCashShopToLobby()
    {   
        // 혹시모르니 다 꺼주기
        CloseUIPanels();
        lobby.gameObject.SetActive(true);
    }

    public void CheckCanBuy(int cost)
    {
        PlayerData data = JsonDataManager.jsonInstance.LoadPlayerData();
        if(data.gem < cost)
        {   
            OpenNotification();
        }
        else
        {
            data.gem -= cost;
            data.coin += cost;
        }
    }

    public void OpenNotification()
    {
        notification.SetActive(true);
    }
    public void CloseNotification()
    {
        notification.SetActive(false);
    }


    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡmusicControl 관련ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//

    public void OpenMusicControl()
    {
        CloseUIPanels();
        musicControl.gameObject.SetActive(true);
        musicControl.SetMusicListOrder();
    }

    public void ActivateContent(GameObject gameObject)
    {
        musicControl.ActivateContent(gameObject);
    }

    public void DeactivateContent(GameObject gameObject)
    {
        musicControl.DeactivateContent(gameObject);
    }


    public void Modify()
    {
        musicControl.ModifyMusicList();
    }

    /** CashShop에서 Return 버튼 클릭시 Lobby로 이동 */
    public void ReturnMusicControlToLobby()
    {
        CloseUIPanels();
        lobby.gameObject.SetActive(true);
    }

    /** Music 저장하기 버튼 누름 */
    public void SaveMusic()
    {
        musicSelectControl.ModifyActivatedMusicList();
        SoundManager.soundInstance.MusicUpdate();
    }

    public void SetMusicListOrder()
    {
        musicSelectControl.SetMusicListOrder();
    }


}
