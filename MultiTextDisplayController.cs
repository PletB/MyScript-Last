using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MultiTextDisplayController : MonoBehaviour
{
    // 텍스트 오브젝트 배열 및 출력할 메시지들
    public TMP_Text[] textObjects;  // Text1부터 Text8까지의 TMP_Text 배열
    public StartButtonController startButtonController; // StartButtonController 스크립트 참조
    public float displayDelay = 0.05f;  // 한 글자씩 출력 딜레이
    public float betweenTextDelay = 1.5f;  // 각 텍스트 사이의 대기 시간

    // 출력할 메시지 배열
    public string[] messages = {
        "휴,, 드디어 됐네..", // Text1 메시지
        "그럼! 다시 정식으로 소개할게!", // Text2 메시지
        "게임에 온 걸 환영해!", // Text3 메시지
        "자! 게임 시작 버튼을 눌러봐!", // Text4 메시지
        "잠깐, 친구야!", // Text5 메시지
        "게임을 시작하기 전에 이름부터 적어야지!", // Text6 메시지
        "좋아! 즐거운 게임을 하길 바래!", // Text7 메시지
        "물론, 너가 이 게임을 클리어 할 수 있을지 모르겠네,,ㅋ" // Text8 메시지
    };

    // 추가된 BGM, 효과음 오디오 소스
    public AudioSource bgmAudioSource;  // BGM 재생을 위한 AudioSource
    public AudioSource textEffectAudioSource; // Text 효과음

    // Start에서 모든 텍스트 비활성화 후 순차적으로 출력
    void Start()
    {
        // 모든 텍스트 오브젝트를 비활성화
        foreach (var textObject in textObjects)
        {
            textObject.gameObject.SetActive(false);
        }

        // BGM 재생 시작
        if (bgmAudioSource != null)
        {
            bgmAudioSource.Play();
        }

        // 텍스트 출력 코루틴 시작
        StartCoroutine(DisplayAllTexts());
    }

    // 모든 텍스트를 순차적으로 출력하는 코루틴
    private IEnumerator DisplayAllTexts()
    {
        // 텍스트 4까지 순차적으로 출력
        for (int i = 0; i < 4; i++)
        {
            yield return StartCoroutine(DisplayText(textObjects[i], messages[i]));
            yield return new WaitForSeconds(betweenTextDelay);

            textObjects[i].gameObject.SetActive(false);
        }

        yield return StartCoroutine(startButtonController.StartButtonWait());
    }

    // 텍스트를 한 글자씩 출력하는 코루틴
    public IEnumerator DisplayText(TMP_Text textObject, string message)
    {
        textObject.gameObject.SetActive(true);
        textObject.text = "";

        foreach (char letter in message)
        {
            textObject.text += letter;

            // 텍스트 효과음 재생
            if (textEffectAudioSource != null)
            {
                textEffectAudioSource.PlayOneShot(textEffectAudioSource.clip);  // 한 글자마다 효과음 재생
            }

            yield return new WaitForSeconds(displayDelay); // 한 글자 출력 후 0.1초 대기
        }
    }

    // Start 버튼이 눌린 후 Text5부터6까지 계속 출력
    public IEnumerator ContinueDisplayingTextsFromIndex(int startIndex)
    {
        for (int i = startIndex; i < 6; i++)
        {
            yield return StartCoroutine(DisplayText(textObjects[i], messages[i]));
            yield return new WaitForSeconds(betweenTextDelay);

            textObjects[i].gameObject.SetActive(false);
        }

        yield return StartCoroutine(startButtonController.StartButtonReturn());
    }
}