using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MultiTextDisplayController : MonoBehaviour
{
    // �ؽ�Ʈ ������Ʈ �迭 �� ����� �޽�����
    public TMP_Text[] textObjects;  // Text1���� Text8������ TMP_Text �迭
    public StartButtonController startButtonController; // StartButtonController ��ũ��Ʈ ����
    public float displayDelay = 0.05f;  // �� ���ھ� ��� ������
    public float betweenTextDelay = 1.5f;  // �� �ؽ�Ʈ ������ ��� �ð�

    // ����� �޽��� �迭
    public string[] messages = {
        "��,, ���� �Ƴ�..", // Text1 �޽���
        "�׷�! �ٽ� �������� �Ұ��Ұ�!", // Text2 �޽���
        "���ӿ� �� �� ȯ����!", // Text3 �޽���
        "��! ���� ���� ��ư�� ������!", // Text4 �޽���
        "���, ģ����!", // Text5 �޽���
        "������ �����ϱ� ���� �̸����� �������!", // Text6 �޽���
        "����! ��ſ� ������ �ϱ� �ٷ�!", // Text7 �޽���
        "����, �ʰ� �� ������ Ŭ���� �� �� ������ �𸣰ڳ�,,��" // Text8 �޽���
    };

    // �߰��� BGM, ȿ���� ����� �ҽ�
    public AudioSource bgmAudioSource;  // BGM ����� ���� AudioSource
    public AudioSource textEffectAudioSource; // Text ȿ����

    // Start���� ��� �ؽ�Ʈ ��Ȱ��ȭ �� ���������� ���
    void Start()
    {
        // ��� �ؽ�Ʈ ������Ʈ�� ��Ȱ��ȭ
        foreach (var textObject in textObjects)
        {
            textObject.gameObject.SetActive(false);
        }

        // BGM ��� ����
        if (bgmAudioSource != null)
        {
            bgmAudioSource.Play();
        }

        // �ؽ�Ʈ ��� �ڷ�ƾ ����
        StartCoroutine(DisplayAllTexts());
    }

    // ��� �ؽ�Ʈ�� ���������� ����ϴ� �ڷ�ƾ
    private IEnumerator DisplayAllTexts()
    {
        // �ؽ�Ʈ 4���� ���������� ���
        for (int i = 0; i < 4; i++)
        {
            yield return StartCoroutine(DisplayText(textObjects[i], messages[i]));
            yield return new WaitForSeconds(betweenTextDelay);

            textObjects[i].gameObject.SetActive(false);
        }

        yield return StartCoroutine(startButtonController.StartButtonWait());
    }

    // �ؽ�Ʈ�� �� ���ھ� ����ϴ� �ڷ�ƾ
    public IEnumerator DisplayText(TMP_Text textObject, string message)
    {
        textObject.gameObject.SetActive(true);
        textObject.text = "";

        foreach (char letter in message)
        {
            textObject.text += letter;

            // �ؽ�Ʈ ȿ���� ���
            if (textEffectAudioSource != null)
            {
                textEffectAudioSource.PlayOneShot(textEffectAudioSource.clip);  // �� ���ڸ��� ȿ���� ���
            }

            yield return new WaitForSeconds(displayDelay); // �� ���� ��� �� 0.1�� ���
        }
    }

    // Start ��ư�� ���� �� Text5����6���� ��� ���
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