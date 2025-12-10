using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;

public class DownLoadManager : MonoBehaviour
{
    [Header("Toggle")]
    public GameObject downMessage;
    public GameObject downloadInfoGroup;
    public GameObject loadMusicTextObject;

    [Header("Slider")]
    public Slider downSlider;

    [Header("Text")]
    public TextMeshProUGUI downPercentText;
    public TextMeshProUGUI curFileSizeText;
    public TextMeshProUGUI maxFileSizeText;
    public TextMeshProUGUI maxFileSizePopupText;
    public TextMeshProUGUI loadMusicText;
    

    [Header("Label")]
    [SerializeField] public AssetLabelReference musicLabel;

    long patchSize;
    Dictionary<string,long> patchMap = new Dictionary<string, long>();

    const string lobbySceneName = "LobbyScene";
    //[SerializeField] public RectTransform loadingImage;
    List<string> labels = new List<string>();

    void Start()
    {
        StartCoroutine(InitAddressable());
        StartCoroutine(CheckUpdateFiles());
    }

    IEnumerator InitAddressable()
    {
        var init = Addressables.InitializeAsync();
        yield return init;
    }

    string GetFileSize(long size)
    {
        string ret = string.Empty;

        if(size > 1024 * 1024 * 1024)
        {
            ret = $"{size / (1024 * 1024 * 1024)} GB";
        }
        else if(size > 1024 * 1024)
        {
            ret = $"{size / (1024 * 1024)} MB";
        }
        else if(size > 1024)
        {
            ret = $"{size / (1024)} KB";
        }
        else
        {
            ret = $"{size} B";
        }

        return ret;
    }

    IEnumerator StartLoadMusicAsync()
    {
        
        loadMusicTextObject.SetActive(true);
        loadMusicText.text = "Loading Music...";

        downPercentText.text = " 0 %"; // (선택) 표시 바꾸기
        downSlider.SetValueWithoutNotify(0f);

        var musicLoader = GlobalMusicData.musicDataInstance;
        var loadMusicCoroutine = musicLoader.LoadMusicAssets();
        float timer  = 0f;

        while (loadMusicCoroutine.MoveNext())
        {
            timer += Time.unscaledDeltaTime * 0.4f;
            downSlider.value = Mathf.Lerp(downSlider.value, musicLoader.LoadProgressRatio, timer);

            // 근접하면 스냅
            if (Mathf.Abs(downSlider.value - musicLoader.LoadProgressRatio) < 0.01f && musicLoader.IsLoaded)
            {
                downSlider.value = musicLoader.LoadProgressRatio;
            }

            downPercentText.text = $" {(int)(musicLoader.LoadProgressRatio * 100)} % ";
            yield return null;
        }

        downSlider.value = 1f;
        downPercentText.text = "100 %";

        loadMusicText.text = "Starting Game...";
        yield return new WaitForSeconds(1.5f);
        LoadingManager.LoadScene(lobbySceneName);
    }

    #region Check DownLoad
    /** Update파일 용량체크 및 이미 다운로드 되어있는지 파악후 팝업 띄우기 or 넘기기 */
    IEnumerator CheckUpdateFiles()
    {
        labels.Add(musicLabel.labelString);

        patchSize = default;

        // 파일의 각 레벨들의 총합 전체 사이즈 가져오기
        foreach(string label in labels)
        {
            var handle = Addressables.GetDownloadSizeAsync(label);
            yield return handle;

            patchSize += handle.Result;
        }

        // 다운로드 할 파일이 있음
        if(patchSize > decimal.Zero)
        {
            // 다운로드 팝업창, Slider, 다운로드 크기 표시, Percent표시 텍스트 등등 사전준비 완료시키기
            downMessage.SetActive(true);
            downloadInfoGroup.SetActive(true);
            
            string patchSizeText = GetFileSize(patchSize);
            maxFileSizePopupText.text = patchSizeText;
            maxFileSizeText.text = patchSizeText;
        }
        else // 다운로드가 완료된 상태
        {
            StartCoroutine(StartLoadMusicAsync());
        }
    }
    #endregion

    #region Download

    /** 다운로드할게 있다면 팝업창이 띄워지고, 다운로드 버튼을 눌렀을 경우 코루틴 시작 */
    public void Button_DownLoad()
    {
        StartCoroutine(PatchFiles());
    }
    IEnumerator PatchFiles()
    {
        // 파일의 각 레벨들의 총합 전체 사이즈 가져오기
        foreach(string label in labels)
        {
            var handle = Addressables.GetDownloadSizeAsync(label);
            yield return handle;

            if(handle.Result > 0)
            {
                StartCoroutine(DownloadLabel(label));
            }
        }

        yield return CheckDownLoad();
    }

    IEnumerator DownloadLabel(string label)
    {
        if (!patchMap.ContainsKey(label))
            patchMap.Add(label, 0);

        // 다운로드 핸들 가져오기
        var handle = Addressables.DownloadDependenciesAsync(label);

        while(!handle.IsDone)
        {
            // 다운로드 중일 때의 상태 추적
            patchMap[label] = handle.GetDownloadStatus().DownloadedBytes;
            yield return new WaitForEndOfFrame();
        }

        // 다운로드가 완료된 후, 최종적으로 TotalBytes를 갱신
        patchMap[label] = handle.GetDownloadStatus().TotalBytes;
        Addressables.Release(handle);
    }

    IEnumerator CheckDownLoad()
    {
        downPercentText.text = " 0 % ";

        while (true)
        {
            long curPatchedSize = patchMap.Sum(tmp => tmp.Value); // 전체 다운로드된 크기 합
            float progress = (float)curPatchedSize / patchSize; // 다운로드된 크기 퍼센트 진행상황

            // UI로 볼수있게 slider와 percent 가시화
            downSlider.value = progress;
            downPercentText.text = $" {(int)(progress * 100)} % ";
            curFileSizeText.text = GetFileSize(curPatchedSize); 

            // 전체 다운로드 완료
            if (curPatchedSize >= patchSize)
            {
                downloadInfoGroup.SetActive(false);
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        yield return StartCoroutine(StartLoadMusicAsync());
    }
    #endregion
}
