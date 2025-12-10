using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;


public class ShopCarData : MonoBehaviour
{
    [SerializeField] public CarData carData;

    [Header("CarInfo")]
    [SerializeField] TextMeshProUGUI carNameText;
    [SerializeField] Image carIconImage;
    [SerializeField] Slider maxForwardVelocity;
    [SerializeField] Slider accelValue;
    [SerializeField] Slider steerValue;

    [Header("BuyInfo")]
    [SerializeField] Button carBuyButton;
    [SerializeField] TextMeshProUGUI carBuyText;
    [SerializeField] TextMeshProUGUI carPriceText;
    [SerializeField] Image Owned;

    [Header("PassInfo")]
    [SerializeField] Button prev;
    [SerializeField] Button next;
    [SerializeField] Button returnToLobbyButton;

    LocalizedString statusString;

    public int ShopIndex { get; set; } = 0;

    void OnEnable()
    {
        // 차량이 사용중인지, 보유중인지, 미보유인지 PlayerData와 비교하여 Button의 형태 변환
        UpdateCarBuyButton();
    }

    void Start()
    {
        // #1. 차량 정보 업데이트
        UpdateCarInfo();

        // #2. 각종 버튼의 리스너 추가
        returnToLobbyButton.onClick.AddListener(() => UIManager_LobbyScene.uiInstance.ReturnShopToLobby());
        prev.onClick.AddListener(() => UIManager_LobbyScene.uiInstance.ShopShowPrev());
        next.onClick.AddListener(() => UIManager_LobbyScene.uiInstance.ShopShowNext());

        carBuyButton.onClick.AddListener(BuyCar);
    }

    public bool CheckSelectedCar(string carTag)
    {
        return (carData.carTag.CompareTo(carTag) == 0); // 같으면 1 다르면 0
    }

    /** 차량 UI 정보 업데이트 ( Localization ) */
    void UpdateCarInfo()
    {
        var localizedString = new LocalizedString("CarTable", carData.carTag);
        localizedString.StringChanged += (value) => carNameText.text = value;

        //carNameText.text         = carData.carName;
        carIconImage.sprite      = carData.carIcon;
        maxForwardVelocity.value = carData.maxForwardVelocity;
        accelValue.value         = carData.accelValue;
        steerValue.value         = carData.steerValue;
        //carPriceText.text        = carData.carPrice.ToString();
    }

    void UpdateBuyButtonText(string entry)
    {
        if (statusString != null)
            statusString.StringChanged -= OnTextChanged;

        statusString = new LocalizedString("MyTable", entry);
        statusString.StringChanged += OnTextChanged;
        statusString.RefreshString(); // 이걸 호출하면 바로 한번 텍스트 변경 실행됨
    }

    void OnTextChanged(string localizedValue)
    {
        carBuyText.text = localizedValue;
    }

    /** 차량이 사용중인지, 보유중인지, 미보유인지 PlayerData와 비교하여 Button의 형태 변환 */
    void UpdateCarBuyButton()
    {
        // 마지막으로 플레이한 차량 데이터 로드
        #nullable enable
        CarDataJson? json = JsonDataManager.jsonInstance.LoadLastCarDataJson();
        if(json == null)
        {
            CheckCanBuy(0);
            Utils.LogError();
            return;
        }
       
        // #1. 사용중인경우
        if(json.carTag.CompareTo(carData.carTag) == 0) 
        {
            carBuyButton.interactable = false;
            UpdateBuyButtonText("CarShop-Interact-Using"); // Localization
            carPriceText.text = "0";
        }
        else  // #2. 보유중 or 구매해야할때
        {
            PlayerData pData = JsonDataManager.jsonInstance.LoadPlayerData();

            if (pData.HasCar(carData.carTag)) // 보유
            {
                carBuyButton.interactable = true;
                UpdateBuyButtonText("CarShop-Interact-Select"); // Localization
                carPriceText.text = "0";
            }
            else // 미보유
            {
                carPriceText.text = carData.carPrice.ToString();
                CheckCanBuy(pData.coin); // 돈 체크 및 Button 상태 변환
            }
        }
    }

    /** 돈 체크 및 차감 */
    bool CheckCanBuy(in int money)
    {   
        // 구매 불가
        if(money < carData.carPrice)
        {
            carBuyButton.interactable = false;
            UpdateBuyButtonText("CarShop-Interact-CannotBuy");
            return false;
        }
        else // 구매 가능
        {
            carBuyButton.interactable = true;
            UpdateBuyButtonText("CarShop-Interact-Buy");
            return true;
        }
    }

    /** 차량 구매 */
    void BuyCar()
    {
        int carPrice = carData.carPrice;

        // 한번더 체크 후 돈 체크 및 차감
        if(CheckCanBuy(carPrice))
        {   
            PlayerData pData = JsonDataManager.jsonInstance.LoadPlayerData();
            // 미보유 상태, 구매할 돈도 있다면 한번더 체크해서 차량구매 후 PlayerData에 새로운 차량 등록
            if(pData.AddCar(carData.carTag))
            {
                // 차량 구매 및 돈 차감
                if(pData.UpdateCoin(-carData.carPrice))
                {
                    UIManager_LobbyScene.uiInstance.UpdateCoinText(pData.coin); // UI 업데이트
                }
            }
            else // 보유중인 경우 선택
            {
                JsonDataManager.jsonInstance.Save(carData);
            }
            
            JsonDataManager.jsonInstance.Save(pData); // PlayerData 저장  
        }

        UpdateCarBuyButton(); // 버튼 업데이트
    }
}
