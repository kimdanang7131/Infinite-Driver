using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CarHandler : MonoBehaviour
{
    [SerializeField] public CarData carData;
    
    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//
    [SerializeField , HideInInspector] protected Rigidbody rb;

  
    public float CurrentForwardVelocity { get => rb.velocity.z; }

    protected bool isBraking  = false; // [UI] 누르고 뗐을 때 브레이크 여부
    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ// 

    protected virtual void Awake() 
    {
        if(this.TryGetComponent<Rigidbody>(out rb) == false)
            Utils.LogError();

        rb.velocity = Vector3.zero;
    }

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//
    protected abstract void Steer(); /** 차의 좌우 이동 로직 */

    /** 차의 앞뒤이동 로직 */
    protected virtual void CarMove() 
    {
        // 브레이크 중일 때
        if(isBraking)
        {
            if(rb.velocity.z <= carData.minForwardVelocity) // 최소속도는 이하로는 브레이크 불가
            {
                if(rb.velocity.z >= carData.maxForwardVelocity)
                    return;

                rb.AddForce(transform.forward * carData.accelValue); // 가속
                return;
            }
                

            rb.AddForce(transform.forward * carData.brakeValue * -1f); // 브레이크
        }
        else // 정방향 주행
        {
            if(rb.velocity.z >= carData.maxForwardVelocity) // 최대속도를 넘어가지 않도록
                return;

            rb.AddForce(transform.forward * carData.accelValue); // 가속
        }
    }

    /** Player일경우 -> [UI] UI IPointerDownHandler 이용하여 누르기 */
    public void Brake()
    {
        isBraking = true;
    }
    /** Player일경우 -> [UI] UI IPointerUpHandler 이용하여 떼기 */
    public void BrakeRelease()
    {
        isBraking = false;
    }
}