using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelHandler : MonoBehaviour
{
    [SerializeField] CarHandler carHandler;
    float wheelRadius = 0.15f;

    void Start()
    {
        CarHandler[] carHandlers = GetComponentsInParent<CarHandler>(); // true면 비활성화 오브젝트 포함

        if(carHandlers.Length > 0)
            carHandler = carHandlers[0];

        if(carHandler == null)
        {
            Utils.LogError();
        }
    }

    // ** 나중에 AI Wheel 설정할때 부모 Handler -> Player, AI 나눠야겠는데
    void FixedUpdate() 
    {
        if(!GameManager.gameInstance.isGameOver && carHandler != null)
        {
            float rotationSpeed  = carHandler.CurrentForwardVelocity / wheelRadius * Time.deltaTime;
            transform.Rotate(Vector3.right, rotationSpeed);
        }
    }
}
