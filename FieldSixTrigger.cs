using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FieldSixTrigger : MonoBehaviour
{
    public SPUM_Prefabs anim;
    public Image fadeImage; // 페이드 아웃 효과를 위한 이미지
    public float fadeDuration = 1f; // 페이드 아웃 지속 시간
    private bool isTransitioning = false; // 전환 상태 확인용

    private QuestFieldPlayer playerController; // 플레이어 컨트롤러 참조

    private void Start()
    {
        // 시작 시 이미지의 알파 값을 0으로 설정 (투명하게)
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;
        }

        // QuestPlayerController 참조 설정
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<QuestFieldPlayer>();
        }
    }

    // 플레이어가 트리거에 닿으면 페이드 아웃 후 씬 전환
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            // 플레이어 이동 중지
            if (playerController != null)
            {
                playerController.canMove = false;
                playerController.GetComponent<Rigidbody2D>().velocity = Vector2.zero; // 즉시 속도를 0으로 설정하여 멈춤
                anim.PlayAnimation(0);
            }

            // 씬 전환 코루틴 시작
            StartCoroutine(TransitionToNextScene());
        }
    }

    // 페이드 아웃 효과를 주고 다음 씬으로 전환하는 코루틴
    private IEnumerator TransitionToNextScene()
    {
        isTransitioning = true;

        // 페이드 아웃
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            if (fadeImage != null)
            {
                Color color = fadeImage.color;
                color.a = alpha;
                fadeImage.color = color;
            }
            yield return null;
        }

        // 씬 전환
        SceneManager.LoadScene("BossScene"); // "BossScene"을 다음 씬 이름으로 변경하세요
    }
}
