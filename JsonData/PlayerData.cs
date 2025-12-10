using System;
using System.Collections.Generic;


// 순수하게 플레이어의 데이터를 저장하는 클래스 , 마지막 차량등의 정보는 LobbyUI
[Serializable]
public class PlayerData 
{
    public List<string> ownedCar = new List<string>(); // 소유하고 있는 차량 태그
    public int coin; 
    public int gem;  
    public int bestScore; 
    public bool isLoaded = false;

        //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//

    public bool SetCoin(int amount)
    {
        if(CheckUpdateCoin(amount))
        {
            coin = amount;
            return true;
        }
        else 
            return false;
    }

    public bool UpdateCoin(int amount)
    {
        if(CheckUpdateCoin(amount))
        {
            coin += amount;
            return true;
        }
        else 
            return false;
    }
    public bool UpdateGem(int amount)
    {
        if(CheckUpdateGem(amount))
        {
            gem += amount;
            return true;
        }
        else 
            return false;
    }

    public void UpdateBestScore(int score)
    {
        if(score > bestScore)
        {
            bestScore = score;
        }
    }

    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//

    /** UpdateCoin 전에 무조건 체크 */
    public bool CheckUpdateCoin(int price)
    {
        return (coin + price >= 0);
    }

    /** UpdateGem 전에 무조건 체크 */
    public bool CheckUpdateGem(int price)
    {
        return (gem + price >= 0);
    }

    /** PlayerData 생성자 */
    public bool HasCar(string carTag)
    {
        return ownedCar.Contains(carTag);
    }

    /** PlayerData에 차량추가 */
    public bool AddCar(string carTag)
    {
        if (!HasCar(carTag))
        {
            ownedCar.Add(carTag);
            return true;
        }
        else
            return false;
    }
}
