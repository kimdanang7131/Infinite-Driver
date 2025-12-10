using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{  
    [SerializeField] ShopCarData[] displayCars;
    //[SerializeField] GameObject lobby;

    string selectedCarTag;
    ShopCarData selectedShopData;
    int displayIndex = 0;

    /** 차량 선택 */
    public void SelectCar(ShopCarData newSelectCarData)
    {
        ShopCarData lastSelectCarData = selectedShopData; // 임시 저장
        selectedShopData = newSelectCarData; // 갱신
    }

    /** 상점 열기 */
    public void OpenShop(string selectedCarTag)
    {
        this.selectedCarTag = selectedCarTag;
        gameObject.SetActive(true);

        for(int i = 0; i < displayCars.Length; i++)
        {
            displayCars[i].ShopIndex = i;

            if(displayCars[i].CheckSelectedCar(selectedCarTag))
            {
                selectedShopData = displayCars[i];
            }
        }
    }

    /** 이전 차량 보여주기 */
    public void ShowPrev()
    {
        displayCars[displayIndex].gameObject.SetActive(false);
        displayIndex--;
        if (displayIndex < 0)
        {
            displayIndex = displayCars.Length - 1;
        }
        displayCars[displayIndex].gameObject.SetActive(true);
    }

    /** 다음 차량 보여주기 */
    public void ShowNext()
    {
        displayCars[displayIndex].gameObject.SetActive(false);
        displayIndex++;
        if (displayIndex >= displayCars.Length)
        {
            displayIndex = 0;
        }
        displayCars[displayIndex].gameObject.SetActive(true);
    }

    /** 로비로 이동 클릭시 초기화*/
    public void ReturnToLobby()
    {
        // displayCars 비활성화
        foreach (ShopCarData carGroup in displayCars)
        {
            carGroup.gameObject.SetActive(false);
        }

        displayCars[0].gameObject.SetActive(true); // 첫번째만 활성화시키기
        displayIndex = 0;

        gameObject.SetActive(false);
    }
}
