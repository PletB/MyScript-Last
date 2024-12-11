using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionButtonController : MonoBehaviour
{
    // 버튼 및 애니메이션 관련 변수
    public RectTransform optionButton;  // Option 버튼의 RectTransform
    public float dropDuration = 1.0f;  // 버튼이 내려오는 시간
    public float bounceFactor = 0.3f;  // 반동 크기 (얼마나 많이 튕길지)
    public float bounceSpeed = 2.0f;  // 반동 속도

    // 효과음 오디오 소스
    public AudioSource buttonClickAudioSource; // 버튼 클릭 효과음 재생을 위한 AudioSource

    void Start()
    {
        // Option 버튼 초기 위치 설정 (화면 위로)
        optionButton.anchoredPosition = new Vector2(optionButton.anchoredPosition.x, Screen.height + optionButton.rect.height);
    }

    // Option 버튼이 눌렸을 때 호출되는 메서드
    public void OnOptionButtonPressed()
    {
        // 버튼 클릭 효과음 재생
        if (buttonClickAudioSource != null)
        {
            buttonClickAudioSource.PlayOneShot(buttonClickAudioSource.clip);  // 버튼 클릭 시 효과음 재생
        }

        Application.Quit();  // 게임 강제 종료
    }

    // Option 버튼이 아래로 내려오는 애니메이션 코루틴
    public IEnumerator DropOptionButton()
    {
        float elapsedTime = 0f;
        Vector2 startPos = optionButton.anchoredPosition;
        Vector2 endPos = new Vector2(optionButton.anchoredPosition.x, -198f);

        // 드롭 애니메이션
        while (elapsedTime < dropDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / dropDuration;
            optionButton.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        // 정확한 위치 보정
        optionButton.anchoredPosition = endPos;
        StartCoroutine(BounceOptionButton());
    }

    // Option 버튼이 화면 위로 올라가는 애니메이션 코루틴
    public IEnumerator MoveButtonUp()
    {
        float elapsedTime = 0f;
        Vector2 startPos = optionButton.anchoredPosition;
        Vector2 endPos = new Vector2(optionButton.anchoredPosition.x, Screen.height + optionButton.rect.height);

        // 올라가는 애니메이션 실행
        while (elapsedTime < dropDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / dropDuration;
            optionButton.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        // 정확한 위치 보정
        optionButton.anchoredPosition = endPos;
    }

    // Option 버튼의 반동 애니메이션
    private IEnumerator BounceOptionButton()
    {
        float elapsedTime = 0f;
        bool bouncing = true;

        while (bouncing)
        {
            elapsedTime += Time.deltaTime;

            // 반동 효과 계산
            float bounce = Mathf.Sin(elapsedTime * bounceSpeed) * Mathf.Exp(-elapsedTime * bounceSpeed) * bounceFactor;
            optionButton.anchoredPosition = new Vector2(optionButton.anchoredPosition.x, optionButton.anchoredPosition.y + bounce);

            if (Mathf.Abs(bounce) < 0.01f)
            {
                bouncing = false;
            }

            yield return null;
        }
    }
}
