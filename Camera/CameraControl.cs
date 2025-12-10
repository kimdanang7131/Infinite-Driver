using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    CinemachineVirtualCamera vc;
    CinemachineBasicMultiChannelPerlin noise;

    
    public static CameraControl camInstance;

    void Awake()
    {
        if(camInstance == null)
            camInstance = this;
        else
            Destroy(this.gameObject);
        
        // 카메라 컴포넌트 가져오기
        if(TryGetComponent<CinemachineVirtualCamera>(out vc) == false)
        {
            Utils.LogError();
            return;
        }
        
    }

    void Start()
    {
        noise = vc.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    /** 일단 뒤에서 다가옴을 위해서 */
    public void ShakeCameraHard()
    {
        noise.m_AmplitudeGain = 5f; // 진폭
        noise.m_FrequencyGain = 3f; // 주기
    }

    /** 옆에 부딪혔을때 가벼운 카메라 쉐이크 */
    public void ShakeCameraSmooth() 
    {
        float ampRand  = Random.Range(1.0f, 2f);
        float freqRand = Random.Range(1.0f, 2f);

        noise.m_AmplitudeGain = ampRand;  // 진폭
        noise.m_FrequencyGain = freqRand; // 주기
    }

    /** 카메라 쉐이크 리셋 */
    public void StopCameraShake()
    {
        noise.m_AmplitudeGain = 0f; // 진폭
        noise.m_FrequencyGain = 0f; // 주기
    }
}
