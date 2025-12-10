using System.Collections;
using UnityEngine;

public class EndlessLevelHandler : MonoBehaviour
{
    const int kSectionRenderSize = 10;
    const float kSectionLength = 200f;
    GameObject[] sectionRender = new GameObject[kSectionRenderSize]; // 10개 렌더
    
    public Transform playerTransform; // 지속적으로 Player와의 거리를 검사하기 위해
    WaitForSeconds waitFor300ms = new WaitForSeconds(0.3f);

    void Start()
    {
        // #1. 처음에는 10개를 미리 셋팅
        for(int i=0; i < kSectionRenderSize; i++)
        {
            GameObject map = PoolManager.poolInstance.GetFromPool(GameManager.gameInstance.MapType);
            map.SetActive(true);


            sectionRender[i] = map;
            sectionRender[i].transform.position = new Vector3(0,0, kSectionLength  * i); // z 세팅

            EndlessSectionHandler handler;
            if(sectionRender[i].TryGetComponent<EndlessSectionHandler>(out handler))
            {
                handler.MoveGround();
            }
        }

        StartCoroutine(Reposition()); // 100ms ( 0.1초 ) 마다 10개의 맵 검사
    }

    IEnumerator Reposition()
    {   
        while(true)
        {
            RepositionLogic();
            yield return waitFor300ms;
        }
    }

    void RepositionLogic()
    {
        // #1. 0.1초마다 멀어진 section 위치 세팅시키는 로직 
        for(int i = 0; i < kSectionRenderSize; i++)
        {
            // Player 지나친 후 일정거리(kSecionLength만큼) 가 멀어지면 Reposition을 위한 설정
            if(sectionRender[i].transform.position.z - playerTransform.position.z < -kSectionLength)
            {
                Vector3 lastSectionPosition = sectionRender[i].transform.position; // 지나친 section 비활성화 

                // section을 가져오는데 성공하면 Pool에 반환 ( prop 반환 이후 map 반환하도록 설정함 )
                EndlessSectionHandler section;
                if(sectionRender[i].TryGetComponent<EndlessSectionHandler>(out section))
                {
                     section.ReturnToPool();
                }

                // 새로운 section 초기세팅
                sectionRender[i] = PoolManager.poolInstance.GetFromPool(GameManager.gameInstance.MapType); // Pool에서 랜덤으로 가져오기
                sectionRender[i].transform.position = new Vector3(lastSectionPosition.x, 0, lastSectionPosition.z + (kSectionLength * sectionRender.Length));

                if(sectionRender[i].TryGetComponent<EndlessSectionHandler>(out section))
                {
                     section.MoveGround();
                }
            }
        }
    }   
}
