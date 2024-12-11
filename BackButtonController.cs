using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButtonController : MonoBehaviour
{
    // 오브젝트 참조
    public GameObject optionPanel;
    private CanvasGroup optionPanelCanvasGroup;

    // 버튼 및 애니메이션 관련 변수
    public RectTransform backButton;  // Start 버튼의 RectTransform
    public float dropDuration = 1.0f;  // 버튼이 오른쪽으로 오는 시간
    public float bounceFactor = 0.3f;  // 반동 크기 (얼마나 많이 튕길지)
    public float bounceSpeed = 2.0f;  // 반동 속도

    // 페이드 아웃 지속 시간
    public float fadeDuration = 0.5f;

    // 효과음 오디오 소스
    public AudioSource buttonClickAudioSource; // 버튼 클릭 효과음 재생을 위한 AudioSource

    void Start()
    {
        // CanvasGroup 컴포넌트를 패널에 추가하여 초기 설정
        optionPanelCanvasGroup = optionPanel.GetComponent<CanvasGroup>();
        if (optionPanelCanvasGroup == null)
        {
            optionPanelCanvasGroup = optionPanel.AddComponent<CanvasGroup>();
        }

        // Start 버튼 초기 위치 설정 (화면 왼쪽)
        backButton.anchoredPosition = new Vector2(-Screen.width - backButton.rect.width, backButton.anchoredPosition.y);
    }


    // Quit 버튼이 눌렸을 때 호출되는 메서드
    public void OnBackButtonPressed()
    {
        // 버튼 클릭 효과음 재생
        if (buttonClickAudioSource != null)
        {
            buttonClickAudioSource.PlayOneShot(buttonClickAudioSource.clip);  // 버튼 클릭 시 효과음 재생
        }

        StartCoroutine(FadeOutOptionPanel());
    }

    // Back 버튼이 아래에서 위로 올라가는 애니메이션 코루틴
    public IEnumerator RightBackButton()
    {
        float elapsedTime = 0f;
        Vector2 startPos = backButton.anchoredPosition;
        Vector2 endPos = new Vector2(-514f, backButton.anchoredPosition.y); // 원하는 목표 위치의 Y 값을 지정

        // 아래에서 위로 이동하는 애니메이션
        while (elapsedTime < dropDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / dropDuration;
            backButton.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        // 정확한 위치 보정
        backButton.anchoredPosition = endPos;
        StartCoroutine(BounceBackButton());
    }

    // Back 버튼의 반동 애니메이션
    private IEnumerator BounceBackButton()
    {
        float elapsedTime = 0f;
        bool bouncing = true;

        while (bouncing)
        {
            elapsedTime += Time.deltaTime;

            // 반동 효과 계산
            float bounce = Mathf.Sin(elapsedTime * bounceSpeed) * Mathf.Exp(-elapsedTime * bounceSpeed) * bounceFactor;
            backButton.anchoredPosition = new Vector2(backButton.anchoredPosition.x, backButton.anchoredPosition.y + bounce);

            if (Mathf.Abs(bounce) < 0.01f)
            {
                bouncing = false;
            }

            yield return null;
        }
    }

    // 옵션 패널의 페이드 아웃 효과 코루틴
    private IEnumerator FadeOutOptionPanel()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            optionPanelCanvasGroup.alpha = Mathf.Clamp01(1 - (elapsedTime / fadeDuration)); // 1에서 0까지의 알파 값 설정
            yield return null;
        }

        // 완전히 사라지도록 설정 후 비활성화
        optionPanelCanvasGroup.alpha = 0;
        optionPanel.SetActive(false);
    }
}
