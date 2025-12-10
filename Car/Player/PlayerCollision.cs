using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] AudioClip audioClip;

    [SerializeField] Transform scoreRayLT;
    [SerializeField] Transform scoreRayLB;
    [SerializeField] Transform scoreRayRT;
    [SerializeField] Transform scoreRayRB;

    float rayDistance = 2.5f;
    public LayerMask carLayer;

    //WaitForSeconds waitFor100ms = new WaitForSeconds(0.1f);

    void Start()
    {
        StartCoroutine(CheckCollisionCoroutine(scoreRayLT, Vector3.left));
        StartCoroutine(CheckCollisionCoroutine(scoreRayLB, Vector3.left));
        StartCoroutine(CheckCollisionCoroutine(scoreRayRT, Vector3.right));
        StartCoroutine(CheckCollisionCoroutine(scoreRayRB, Vector3.right));
    }

    IEnumerator CheckCollisionCoroutine(Transform origin, Vector3 direction)
    {
        while(true)
        {
            CheckCollision(origin, direction);
            yield return null;
        }
    }
    void CheckCollision(Transform origin, Vector3 direction)
    {
       RaycastHit hit;
       Ray ray = new Ray(origin.position, origin.TransformDirection(direction));
       
       if(Physics.Raycast(ray, out hit, rayDistance, carLayer))
       {
           AICarHandler aiCarHandler;
       
           if(hit.collider.gameObject.TryGetComponent<AICarHandler>(out aiCarHandler))
           {  
               if(aiCarHandler.IsScoreAdded == false)
               {
                   aiCarHandler.IsScoreAdded = true;
                   ShowScoreUI(hit.point , aiCarHandler.carScore);
                   SoundManager.soundInstance.PlayOneShot(SoundType.Effect, audioClip);
               }
           }
       }
    }

    void ShowScoreUI(Vector3 position, int score)
    {
        GameObject scoreTextObject = PoolManager.poolInstance.GetScoreTextFromPool();
        TextMeshProUGUI scoreText;
        
        if(scoreTextObject.TryGetComponent<TextMeshProUGUI>(out scoreText))
        {
            scoreText.text = $"+{score}";
            // 월드 좌표를 스크린 좌표로 변환
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(position);
            screenPosition.z = 1f;

            // Canvas의 Transform을 기준으로 UI 요소의 위치 설정
            scoreTextObject.transform.position = screenPosition;

            scoreText.DOFade(0f, 0.5f)
                .SetEase(Ease.OutQuad);

            scoreTextObject.transform.DOMoveY(scoreTextObject.transform.position.y + 50f, 0.5f)
                .SetEase(Ease.OutQuad)
                .OnComplete(()=> 
                {
                    scoreText.alpha = 1f;
                    PoolManager.poolInstance.ReturnTextToPool(scoreTextObject);
                });
                
            GameManager.gameInstance.AddScore(score);
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.CompareTag("CarAI"))
        {
            Time.timeScale = 0f;
            GameManager.gameInstance.GameOver();
        }
    }
}
