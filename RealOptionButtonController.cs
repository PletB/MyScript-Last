using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealOptionButtonController : MonoBehaviour
{
    // 오브젝트 참조
    public GameObject optionPanel;
    public Slider volumeSlider; // 볼륨 조절 슬라이더
    private BackButtonController backButtonController;
    private CanvasGroup optionPanelCanvasGroup;

    // 버튼 및 애니메이션 관련 변수
    public RectTransform realOptionButton;  // RealOption 버튼의 RectTransform
    public float dropDuration = 1.0f;  // 버튼이 올라오는 시간
    public float bounceFactor = 0.3f;  // 반동 크기 (얼마나 많이 튕길지)
    public float bounceSpeed = 2.0f;  // 반동 속도
    public float fadeDuration = 0.5f;  // 페이드인/아웃 지속 시간

    // 효과음 오디오 소스
    public AudioSource bgmAudioSource;  // BGM 재생을 위한 AudioSource
    public AudioSource buttonClickAudioSource; // 버튼 클릭 효과음 재생을 위한 AudioSource
    public AudioSource textEffectAudioSource; // Text 효과음
    public AudioSource warningTextAudioSource; // 경고 효과음 재생을 위한 AudioSource

    void Start()
    {
        // RealOption 버튼 초기 위치 설정 (화면 밖 아래로)
        realOptionButton.anchoredPosition = new Vector2(realOptionButton.anchoredPosition.x, -Screen.height - realOptionButton.rect.height);

        // realOptionButtonController 찾아서 참조
        backButtonController = FindObjectOfType<BackButtonController>();

        // CanvasGroup 컴포넌트를 패널에 추가하여 초기 설정
        optionPanelCanvasGroup = optionPanel.GetComponent<CanvasGroup>();
        if (optionPanelCanvasGroup == null)
        {
            optionPanelCanvasGroup = optionPanel.AddComponent<CanvasGroup>();
        }

        optionPanelCanvasGroup.alpha = 0;  // 패널 투명도 초기화

        optionPanel.SetActive(false); // 패널 비활성화

        // 슬라이더 초기화 및 이벤트 연결
        volumeSlider.onValueChanged.AddListener(AdjustVolume);
        volumeSlider.value = bgmAudioSource.volume; // 슬라이더 초기 값 설정
        volumeSlider.value = buttonClickAudioSource.volume; // 슬라이더 초기 값 설정
        volumeSlider.value = textEffectAudioSource.volume; // 슬라이더 초기 값 설정
        volumeSlider.value = warningTextAudioSource.volume; // 슬라이더 초기 값 설정
    }

    // 볼륨 슬라이더 값에 따라 볼륨 조절
    private void AdjustVolume(float value)
    {
        bgmAudioSource.volume = value; // 슬라이더 값에 따라 오디오 소스의 볼륨 설정
        buttonClickAudioSource.volume = value; // 슬라이더 값에 따라 오디오 소스의 볼륨 설정
        textEffectAudioSource.volume = value; // 슬라이더 값에 따라 오디오 소스의 볼륨 설정
        warningTextAudioSource.volume = value; // 슬라이더 값에 따라 오디오 소스의 볼륨 설정
    }

    // RealOption 버튼이 눌렸을 때 호출되는 메서드
    public void OnRealOptionButtonPressed()
    {
        // 버튼 클릭 효과음 재생
        if (buttonClickAudioSource != null)
        {
            buttonClickAudioSource.PlayOneShot(buttonClickAudioSource.clip);  // 버튼 클릭 시 효과음 재생
        }

        StartCoroutine(FadeInOptionPanel());
        StartCoroutine(backButtonController.RightBackButton());
    }

    // RealOption 버튼이 아래에서 위로 올라가는 애니메이션 코루틴
    public IEnumerator UpRealOptionButton()
    {
        float elapsedTime = 0f;
        Vector2 startPos = realOptionButton.anchoredPosition;
        Vector2 endPos = new Vector2(realOptionButton.anchoredPosition.x, -372f); // 원하는 목표 위치의 Y 값을 지정

        // 아래에서 위로 이동하는 애니메이션
        while (elapsedTime < dropDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / dropDuration;
            realOptionButton.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        // 정확한 위치 보정
        realOptionButton.anchoredPosition = endPos;
        StartCoroutine(BounceRealOptionButton());
    }

    // RealOption 버튼의 반동 애니메이션
    private IEnumerator BounceRealOptionButton()
    {
        float elapsedTime = 0f;
        bool bouncing = true;

        while (bouncing)
        {
            elapsedTime += Time.deltaTime;

            // 반동 효과 계산
            float bounce = Mathf.Sin(elapsedTime * bounceSpeed) * Mathf.Exp(-elapsedTime * bounceSpeed) * bounceFactor;
            realOptionButton.anchoredPosition = new Vector2(realOptionButton.anchoredPosition.x, realOptionButton.anchoredPosition.y + bounce);

            if (Mathf.Abs(bounce) < 0.01f)
            {
                bouncing = false;
            }

            yield return null;
        }
    }

    // 옵션 패널의 페이드인 효과 코루틴
    public IEnumerator FadeInOptionPanel()
    {
        // 패널 활성화
        optionPanel.SetActive(true);

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            optionPanelCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration); // 0에서 1까지의 알파 값 설정
            yield return null;
        }

        // 완전히 보이도록 설정
        optionPanelCanvasGroup.alpha = 1;
    }
}
