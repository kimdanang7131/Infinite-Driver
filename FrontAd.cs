using UnityEngine;
using GoogleMobileAds.Api;
using System;
using System.Collections;

public class FrontAd : MonoBehaviour
{
    private InterstitialAd _interstitialAd;

    #if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-7481433129263845/2679622319";
      #elif UNITY_IPHONE
    private string _adUnitId = "ca-app-pub-3940256099942544/4411468910";
      #else
    private string _adUnitId = "unused";
      #endif


    Action OnAdClosedCallback;
    WaitForSecondsRealtime waitForReal200ms = new WaitForSecondsRealtime(0.2f);
    
    public void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        //MobileAds.Initialize((InitializationStatus initStatus) =>
        //{
        //    // This callback is called once the MobileAds SDK is initialized.
        //    
        //});
        LoadInterstitialAd();
        
    }

    /** 전면광고 로드 */
    public void LoadInterstitialAd()
    {
        // #1. 기존 광고 존재하면 제거
        if (_interstitialAd != null)
        {
              _interstitialAd.Destroy();
              _interstitialAd = null;
        }

        // #2. Custom 광고요청 설정
        var adRequest = new AdRequest();

        // #3. 광고 ID, 광고 요청, 콜백 설정
        InterstitialAd.Load(_adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Utils.Log("전면 광고 로드 실패");
                    return;
                }

                _interstitialAd = ad;
                RegisterEventHandlers(_interstitialAd); // 광고 끄기 등 이벤트 등록
            });
    }

    /** 광고 Close 및 이벤트 설정 함수 -> 기본적으로 전면광고는 한번 로드하고나서 새로운 광고를 다시 로드해야됨 */
    private void RegisterEventHandlers(InterstitialAd interstitialAd)
    { 
        // 광고 닫힘 처리
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            OnAdClosedCallback?.Invoke();
            LoadInterstitialAd(); // 광고가 닫히면 새로운 광고 로드
        };

         // 광고 실패 처리
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            OnAdClosedCallback?.Invoke();
            LoadInterstitialAd(); // 실패 후 광고 재로드
        };
    }

    /** 실질적으로 전면 광고 Show해주는 함수 -> 로드는 별개 */
    public void ShowInterstitialAd(Action OnAdClosed)
    {
        OnAdClosedCallback = OnAdClosed;

        if (_interstitialAd != null)
        {
            StartCoroutine(ShowInterstitial());
        }
        else
        {
            OnAdClosedCallback?.Invoke(); // 광고 없으면 그냥 콜백 실행
        }
    }
    IEnumerator ShowInterstitial()
    {
        float timer   = 0f;
        float timeout = 8f; // 8초 기다리기

        // 광고가 준비되지 않으면 기다림
        while (!_interstitialAd.CanShowAd() && timer < timeout)
        {
            timer += 0.2f;
            yield return waitForReal200ms;
        }

        if (_interstitialAd.CanShowAd())
        {
            _interstitialAd.Show();
        }
        else
        {
            OnAdClosedCallback?.Invoke(); // 실패했으니 콜백 그냥 실행
        }
    }
} 
