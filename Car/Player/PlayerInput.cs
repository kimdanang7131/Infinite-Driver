using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] PlayerCarHandler carHandler; // carHandler에 input을 전달하기 위해

    public bool isDragging { get; private set;} = false; // 터치하여 Handle 회전시킬때

    float prevMouseX  = 0f; // 마우스 클릭 ( 스크린 좌표계 )-> 마우스 드래그로 움직임 사이의 델타값 
    float deltaMouseX = 0f; // 마우스 드래그와 드래그 사이에 발생하는 프레임과 프레임 사이의 델타값 
    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ// 

    void Update() 
    {
        if (GameManager.gameInstance.isGameOver)
            return;

        // #1. CarHandler에 전하는 값들 ( 2개 함수 세트 )
        HandleInput();
        UpdateCarHandlerInput();
    }

    /** 현재는 마우스 클릭한곳까지 좌우 이동 or 드래그한만큼 좌우 이동 시키는 함수 로직 */
    void HandleInput()
    {
        // 유니티 Editor PC : 마우스 입력
        if(Application.platform == RuntimePlatform.WindowsEditor)
        {
            // #1. 마우스 클릭 시작
            if (Input.GetMouseButtonDown(0))
            {
                // 마우스 포인터가 UI 요소 위에 없는 경우에만 Raycasting 수행
                if(!EventSystem.current.IsPointerOverGameObject())
                {
                    prevMouseX = Input.mousePosition.x; // 스크린 좌표계의 마우스 X값 저장
                    isDragging = true;
                }
            }

            // #2. 마우스 드래그 중
            if (Input.GetMouseButton(0) && isDragging)
            {
                Vector3 curMousePos = Input.mousePosition;
                deltaMouseX = curMousePos.x - prevMouseX; // 클릭을 시작했을때와 드래그 중 사이의 델타값 구하기
                prevMouseX  = curMousePos.x; // 이전값 갱신
            }

            // #3. 마우스 클릭 종료
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                deltaMouseX  = 0;
            }
        }
        // Mobile : 터치 입력
        else if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {   
            if(Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                switch(touch.phase)
                {
                    case TouchPhase.Began:
                        if(!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                        {
                            prevMouseX = touch.position.x;
                            isDragging = true;
                        }
                        break;
                    case TouchPhase.Moved:
                        if(isDragging)
                        {
                            Vector3 curMousePos = touch.position;
                            deltaMouseX = curMousePos.x - prevMouseX;
                            prevMouseX  = curMousePos.x;
                        }
                        break;
                    case TouchPhase.Ended:
                        isDragging = false;
                        deltaMouseX = 0f;
                        break;
                }
            }
        }   
    }

    /** CarHandler에게 전달할 값들 한꺼번에 전달 */
    void UpdateCarHandlerInput()
    {
        if(isDragging && Mathf.Abs(deltaMouseX) > 0.5f)
        {
            carHandler.SetIntervalBetweenMouseX(deltaMouseX * 2f); // 좌우 속도
        }

        carHandler.SetSteerDeltaX(deltaMouseX * 0.4f); // 차 회전 시스템
        carHandler.SetIsDragging(isDragging);   // 드래그 여부
    }
}



//
/* 화면에 마우스 클릭 좌표를 Ray를 통해 얻기 위한 함수 Plane의 위쪽만 선택 가능함 
bool CanScreenToRayByDrag(out Ray ray , out float dist)
{
    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    Plane plane = new Plane(Vector3.up, new Vector3(0,transform.position.y,0));
    return plane.Raycast(ray , out dist);
}
*/