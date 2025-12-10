using System.Collections;
using UnityEngine;

public class SkyBoxControl : MonoBehaviour
{
    const string BlendName = "_CubemapTransition";
    const float BlendMax  = 1f;
    const float BlendMin  = 0f;
    const float BlendSpeed = 0.25f;    
    // 맵 마다 시작 Blend값
    const float BlendBase_Desert = 0.3f;
    const float BlendBase_City   = 0.8f;

    [SerializeField] Material dynamicSkyMaterial; // 변경할 SkyMaterial

    Coroutine blendCoroutine;
    float curBlendValue = 0f; // 코루틴 내에서 활동할 Blend 변수
    bool blendDirection = true; // true: BlendMax로 이동, false: BlendMin으로 이동

    void Start()
    {
        if (dynamicSkyMaterial == null)
        {
            Utils.LogError("SkyBoxControl: dynamicSkyMaterial이 할당되지 않았습니다.");
            return;
        }
    }

    /** Map 상태에 따른 Material Blend 시작값 설정 및 코루틴 시작*/
    public void SetMapType(MapType type)
    {
        switch (type)
        {
            case MapType.Desert:
                curBlendValue = BlendBase_Desert;
                break;
            case MapType.City:
                curBlendValue = BlendBase_City;
                break;
            default:
                Utils.Log("type Error");
                curBlendValue = 0f;
                break;
        } 
        dynamicSkyMaterial.SetFloat(BlendName, curBlendValue);

    }

    /** 코루틴 시작 */
    public void StartSkyBoxBlending()
    {
        StartCoroutine(LoopBlend());  
    }

    /** SkyMaterial의 Blend값이 0~1을 왔다갔다하는 코루틴 */
    IEnumerator LoopBlend()
    {
        while (true)
        {
            float targetBlendValue = blendDirection ? BlendMax : BlendMin; // true: BlendMax로 이동, false: BlendMin으로 이동

            if (blendCoroutine != null)
            {
                StopCoroutine(blendCoroutine);
            }

            blendCoroutine = StartCoroutine(LerpDynamicMaterialBlend(targetBlendValue));
            yield return blendCoroutine; // 코루틴이 완료될 때까지 대기

            blendDirection = !blendDirection; // 방향 전환
        }
    }

    /** 0 or 1이 완전하게 될때까지 정해진 속도에 따라 Material Blend 조정 */
    IEnumerator LerpDynamicMaterialBlend(float target)
    {
        int flip = curBlendValue < target ? 1 : -1; // 뒤집는 변수
        float calculateValue = 0.1f * flip; // 조금더 부드러운 변화를 위해서 0.1f로 막아둠 ( 패트병 뚜껑같은 느낌 )

        while (Mathf.Abs(curBlendValue - target) > 0.01f)
        {
            curBlendValue += calculateValue * BlendSpeed * Time.deltaTime;
            curBlendValue = Mathf.Clamp(curBlendValue, BlendMin, BlendMax);
            dynamicSkyMaterial.SetFloat(BlendName, curBlendValue);
            yield return null;
        }

        // 깔끔하게 확정 박기
        curBlendValue = target;
        dynamicSkyMaterial.SetFloat(BlendName, curBlendValue);
    }
}
