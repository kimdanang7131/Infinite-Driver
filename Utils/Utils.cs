using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Diagnostics;

public static class Utils 
{
    [Conditional("UNITY_EDITOR")]
    public static void Log(string msg)
    {
       UnityEngine.Debug.Log(msg);
    }

    [Conditional("UNITY_EDITOR")]
    public static void LogError(string message = "에서 오류 발생!!", 
        [CallerMemberName] string memberName = "", // 함수 이름
        [CallerFilePath] string filePath = "", // 스크립트?
        [CallerLineNumber] int lineNumber = 0) // Line 위치 ex) cs:11..
    {
        string fileName = System.IO.Path.GetFileName(filePath); // 여기서 스크립트 이름 가져옴
        UnityEngine.Debug.LogError($"[{fileName}:{lineNumber}] {memberName}() -> {message}");
    }

    public static void Shuffle<T>(ref List<T> list)
    {
        for(int i = 0; i < list.Count; i++)
        {
            int randIdx = Random.Range(i, list.Count);

            T temp = list[randIdx];
            list[randIdx] = list[i];
            list[i] = temp;
        }
    }

    // 0~255 범위로 입력하고 0~1 범위의 Color로 변환하는 함수 (int 버전)
    public static Color GetColorFrom255(int r, int g, int b, int a = 255)
    {
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    // 0~1 범위로 입력하고 0~1 범위의 Color로 변환하는 함수 (float 버전)
    public static Color GetColorFrom255(Color pColor)
    {
        return new Color(pColor.r / 255f, pColor.g / 255f, pColor.b / 255f, pColor.a / 255f);
    }
}
