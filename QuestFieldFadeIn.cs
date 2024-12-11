using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestFieldFadeIn : MonoBehaviour
{
    public Image fadeImage; // 페이드 효과를 줄 이미지 (검정색 이미지로 설정)
    public float fadeDuration = 1.5f; // 페이드 인 효과가 지속되는 시간
    public QuestFieldPlayer questFieldPlayer; // QuestPlayerController 스크립트 참조

    private void Start()
    {
        // 페이드 인 시작
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            StartCoroutine(FadeIn());
        }
    }

    // 페이드 인 효과를 구현하는 코루틴
    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        color.a = 1f;
        fadeImage.color = color;

        // 페이드 인 애니메이션 (불투명에서 투명으로)
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        // 페이드 인이 끝난 후 이미지 완전히 투명하게 설정하고 비활성화
        color.a = 0f;
        fadeImage.color = color;
        fadeImage.gameObject.SetActive(false);

        // 페이드 인 완료 후 플레이어 이동 시작
        if (questFieldPlayer != null)
        {
            questFieldPlayer.StartMovement();
        }
    }
}
