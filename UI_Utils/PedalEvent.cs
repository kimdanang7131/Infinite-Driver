using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PedalEvent : MonoBehaviour , IPointerDownHandler , IPointerUpHandler
{
    [SerializeField]
    PlayerCarHandler carHandler;

    public void OnPointerDown(PointerEventData eventData)
    {
        if(carHandler != null)
            carHandler.Brake();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(carHandler != null)
            carHandler.BrakeRelease();
    }
}
