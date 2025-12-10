using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour 
{
    public static string nextScene;
    const string LoadingSceneName = "LoadingScene";

    public Slider loadingBar;
    public TextMeshProUGUI loadingValText;
    WaitForSecondsRealtime waitforReal1500ms = new WaitForSecondsRealtime(1.5f);

    void Start()
    {
        StartCoroutine(StartLoadingScene());
    }
    IEnumerator StartLoadingScene()
    {
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false; // 자동으로 전환하지 않고 수동으로 전환 -> progress가 100되어도 전환되지 않기 위해
        
        float timer = 0f;
        float smoothProgress = 0f;  // 부드러운 진행률을 위한 변수

        while(true)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 0.4f;
            
            // 90퍼가 사실상 100퍼 , 안정적으로 하기위해
            if(op.progress < 0.9f)
            {
                smoothProgress = Mathf.Lerp(smoothProgress, op.progress ,timer);
                loadingBar.value = smoothProgress;
                loadingValText.text = $"{(int)(loadingBar.value * 100)} %";

                // 90퍼 채우고 변수 초기화 할거해주고 넘기기
                if(loadingBar.value >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                smoothProgress = Mathf.Lerp(smoothProgress, 1f, timer);
                loadingBar.value = smoothProgress;

                loadingValText.text = $"100 %";

                // 1초 후에 자동전환으로 변경
                if(loadingBar.value >= 1f)
                {
                    yield return waitforReal1500ms;
                    op.allowSceneActivation = true;
                    break;
                }
            }
        }
    }

    /** 전역에서 Scene 이동 시 사용 */
    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene(LoadingSceneName);
    }   
}