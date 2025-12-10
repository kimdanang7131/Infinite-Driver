using UnityEngine;
using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour
{
    public static AdManager adInstance;
    [SerializeField] FrontAd frontAd;

    void Awake()
    {
        if (adInstance == null)
        {
            adInstance = this;
            DontDestroyOnLoad(gameObject);
            MobileAds.Initialize(initStatus => { Utils.Log("Android SDK Initialized"); });
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public bool Button_PlayButtonClicked(string sceneName)
    {
        bool canShowAd = CanShowFrontAd(); 

        if(canShowAd)
        {
            frontAd.ShowInterstitialAd(() =>
            {
                PlayerPrefs.SetInt("frontAd", 0);
                LoadingManager.LoadScene(sceneName);
            });
        }
        else
        {
            int frontAdCount = PlayerPrefs.GetInt("frontAd",0);
            frontAdCount++;
            PlayerPrefs.SetInt("frontAd", frontAdCount);
            Utils.Log($"전면 광고 남은 횟수 ({frontAdCount} / {GlobalSettings.maxFrontAdCount})");
        }

        return canShowAd;
    }

    /** GlobalSetting에서 설정한 7번이상 Play 및 Retry를 했다면 광고 진행 */
    public bool CanShowFrontAd()
    {
        int frontAdCount = PlayerPrefs.GetInt("frontAd",0);
        
        return frontAdCount >= GlobalSettings.maxFrontAdCount;
    }
}