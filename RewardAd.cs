using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GoogleMobileAds.Api;

public class RewardAd : MonoBehaviour
{
    #if UNITY_ANDROID
        private string _adUnitId = "ca-app-pub-7481433129263845/8698895864";
    #elif UNITY_IPHONE
      private string _adUnitId = "ca-app-pub-3940256099942544/1712485313";
    #else
      private string _adUnitId = "unused";
    #endif

    private RewardedAd _rewardedAd;

    private int retryCount = 0;
    private const int maxRetry = 3;

    WaitForSecondsRealtime waitForReal200ms = new WaitForSecondsRealtime(0.2f);

    void Start()
    {
        LoadRewardedAd();
    }

    /// <summary>
    /// 보상형 광고를 로드합니다
    /// </summary>
    public void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
              _rewardedAd.Destroy();
              _rewardedAd = null;
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(_adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Utils.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Utils.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                _rewardedAd = ad;
                RegisterEventHandlers(_rewardedAd);
                RegisterReloadHandler(_rewardedAd);
            });
    }

    private void RetryLoadAd()
    {
        if (retryCount >= maxRetry)
        {
            Utils.Log("광고 로드 재시도 초과");
            return;
        }

        retryCount++;
        Invoke(nameof(LoadRewardedAd), 5f); // 5초 후 재시도
    }


    public void ShowRewardedAd(Action<int> onRewardEarned)
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            StartCoroutine(ShowRewardAd(onRewardEarned));
        }
    }
    IEnumerator ShowRewardAd(Action<int> onRewardEarned)
    {
        float timer   = 0f;
        float timeout = 8f; // 8초 기다리기

        // 광고가 준비되지 않으면 기다림
        while (!_rewardedAd.CanShowAd() && timer < timeout)
        {
            timer += 0.2f;
            yield return waitForReal200ms;
        }

        if (_rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                Utils.Log($"Rewarded! {reward.Type}, {reward.Amount}");
                onRewardEarned?.Invoke((int)reward.Amount); // 보상 지급 후 후속 로직 실행
            });
        }
        else
        {
            onRewardEarned?.Invoke(0); // 실패했으니 콜백 그냥 실행
        }
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Utils.Log("Rewarded ad 정상적으로 됨.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Utils.Log("Rewarded ad was 클릭됨.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Utils.Log("Rewarded ad was 전체스크린 오픈됨.");
        };


        //일단 이 2개씀
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Utils.Log("Rewarded ad was 전체스크린 오프됨.");
            retryCount = 0; // 성공했으니 재시도 횟수 초기화
            LoadRewardedAd();

        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Utils.LogError("Rewarded ad was 광고 네트워크때매 에러나서 실패 " + error);
            RetryLoadAd(); // 실패 시 재시도
        };
    }

    private void RegisterReloadHandler(RewardedAd ad)
    {
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Utils.Log("Rewarded Ad full screen content closed.");

            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Utils.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);

            // Reload the ad so that we can show another as soon as possible.
            RetryLoadAd();
        };
    }

}
