using UnityEngine;
using GoogleMobileAds.Api;

public class BottomBanner : MonoBehaviour
{
    BannerView _bannerView;

    #if UNITY_ANDROID
     private string _adUnitId = "ca-app-pub-7481433129263845/1228731859";
    #elif UNITY_IPHONE
      private string _adUnitId = "ca-app-pub-3940256099942544/2934735716";
    #else
      private string _adUnitId = "unused";
    #endif

    public void Start()
    {
        //MobileAds.Initialize((InitializationStatus initStatus) =>
        //{
        //    // Mobile Ads SDK가 초기화되면 호출되는 콜백
        //    Utils.Log("Google Mobile Ads SDK initialized.");
        //    
        //});
        LoadAd();
    }

    /** 배너 뷰 만들고 로드시키는 함수 */
    public void LoadAd()
    {
        // #1. 배너뷰 없으면 생성 ( 첫 초기화 때 )
        if(_bannerView == null)
        {
            CreateBannerView();
        }

        // #2. 맞춤형 광고 커스텀 가능 -> AdRequest.Builder() 사용 -> .SetBirthday 등등
        var adRequest = new AdRequest();

        // #3. 커스텀한 [ BannerView & Request ] 광고 로드 시작
        _bannerView.LoadAd(adRequest);
    }

    /** 기본적으로 커스텀하지 않으면 320*50의 banner view 생성 */
    public void CreateBannerView()
    {
        // #1. 배너뷰가 이미 존재하면 Destroy
        if (_bannerView != null)
        {
            DestroyAd();
        }

        // #2. 적응형 배너 생성 코드
        AdSize adaptiveSize = AdSize.GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);


        // 기본적으로 커스텀하지 않으면 320*50 , 상단에 생성됨
        // _adUnitId -> BannerView가 광고를 로드하는 광고 단위 ID
        // Adsize   -> 사용할 광고 크기 -> new Adsize(250,250) 이런식으로도 지정가능
        // AdPosition -> 광고를 배치할 위치 + [ 0, 50 이런식으로 위치 지정도 가능 ]
        _bannerView = new BannerView(_adUnitId, adaptiveSize, AdPosition.Bottom);
    }

    /** 배너뷰 있으면 제거하는 코드 */
    public void DestroyAd()
    {
        if (_bannerView != null)
        {
            Utils.Log("Destroying banner view.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }

    // Hide와 Show가 있으면 사용자가 끄거나 숨길 수 있음
    // 광고 로드 실패 시 처리
    //_bannerView.OnAdFailedToLoad += HandleAdFailedToLoad; -> new BannerView 이후에 등록
    //
    //private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    //{
    //    // 광고 로드 실패에 대한 구체적인 정보
    //    Utils.Log($"Ad failed to load: {args.Message}");
    //
    //    // 사용자에게 실패 메시지 알리기 (예: 재시도 버튼, 알림)
    //    ShowRetryMessage(); // 예시: 실패 후 재시도 버튼을 표시하는 함수
    //}
}
