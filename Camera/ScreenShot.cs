using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ScreenShot : MonoBehaviour
{
    public Camera screenshotCamera; // 스크린샷을 찍을 카메라
    private string desktopPath;

    void Start()
    {
        // 바탕화면 경로를 설정
        desktopPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "coin_screenshot.png");
        TakeScreenshot();
    }

    void TakeScreenshot()
    {
        // 512x512 해상도로 스크린샷을 찍기 위한 RenderTexture 설정
        RenderTexture rt = new RenderTexture(512, 512, 24);
        screenshotCamera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(512, 512, TextureFormat.RGB24, false);

        // 카메라로 렌더링
        screenshotCamera.Render();
        RenderTexture.active = rt;

        // 렌더링된 화면을 Texture2D에 복사
        screenShot.ReadPixels(new Rect(0, 0, 512, 512), 0, 0);
        screenshotCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        // PNG로 인코딩
        byte[] bytes = screenShot.EncodeToPNG();

        // 바탕화면에 파일 저장
        File.WriteAllBytes(desktopPath, bytes);
        //Debug.Log("Screenshot saved to " + desktopPath);
    }
}
