using UnityEngine;
using UnityEngine.UI;

public class CashShopData : MonoBehaviour
{
    public int gainCoinCost;
    public int gemCost;

    public AudioClip buyClip;
    public AudioClip cannotBuyClip;

    void Start()
    {
        Utils.Log("Button");
        GetComponent<Button>().onClick.AddListener(Buy);
    }

    public void Buy()
    {
        PlayerData pData = JsonDataManager.jsonInstance.LoadPlayerData();
        
        // gem 체크
        if(pData.gem < gemCost)
        {   
            Utils.Log("부족");
            SoundManager.soundInstance.PlayOneShot(SoundType.Button, cannotBuyClip);
            UIManager_LobbyScene.uiInstance.OpenNotification();
        }
        else
        {
            Utils.Log("구매완료");
            pData.UpdateGem(-gemCost);
            pData.UpdateCoin(+gainCoinCost);

            SoundManager.soundInstance.PlayOneShot(SoundType.Button, buyClip);
            JsonDataManager.jsonInstance.Save(pData);

            UIManager_LobbyScene.uiInstance.UpdateCoinText(pData.coin);
            UIManager_LobbyScene.uiInstance.UpdateGemText(pData.gem);
        }
    }
}