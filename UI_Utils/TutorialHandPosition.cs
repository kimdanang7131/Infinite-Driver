using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class TutorialHandPosition : MonoBehaviour
{
    [Tooltip("Left HighlightObject, Right HandObject")]
    public SerializedDictionary<GameObject,GameObject> handposDic = new SerializedDictionary<GameObject, GameObject>();

    void OnEnable()
    {
        foreach(KeyValuePair<GameObject, GameObject> pair in handposDic)
        {
            GameObject highlightObject = pair.Key;
            GameObject handObject = pair.Value;

            if(highlightObject != null && handObject != null)
            {
                RectTransform highlightRect = highlightObject.GetComponent<RectTransform>();
                RectTransform handRect = handObject.GetComponent<RectTransform>();

                if (highlightRect != null && handRect != null)
                {
                    // 1. 하이라이트의 월드 좌표를 가져옴
                    Vector3 worldPos = GetWorldCenter(highlightRect);
                    // handObject의 부모 기준으로 로컬 위치 계산
                    Vector3 localPos = handRect.parent.InverseTransformPoint(worldPos);

                    // 살짝 오른쪽 아래로 보정
                    localPos += new Vector3(80f, -80f, 0f);

                    handRect.localPosition = localPos;
                }
            }
        }
    }
    Vector3 GetCenterAnchorWorldPosition(RectTransform rect)
    {
        if (rect == null)
            return rect.position;

        // anchoredPosition 기준 위치
        Vector3 localPos = rect.anchoredPosition;

        // 중앙 앵커 기준으로 계산하기 위해 부모의 피봇/앵커 무시하고 변환
        return rect.TransformPoint(localPos);
    }
    Vector3 GetWorldCenter(RectTransform rect)
    {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);
        return (corners[0] + corners[2]) * 0.5f; // 좌하단 + 우상단 → 중앙
    }
}
