using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestFieldFadeIn : MonoBehaviour
{
    public Image fadeImage; // ���̵� ȿ���� �� �̹��� (������ �̹����� ����)
    public float fadeDuration = 1.5f; // ���̵� �� ȿ���� ���ӵǴ� �ð�
    public QuestFieldPlayer questFieldPlayer; // QuestPlayerController ��ũ��Ʈ ����

    private void Start()
    {
        // ���̵� �� ����
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            StartCoroutine(FadeIn());
        }
    }

    // ���̵� �� ȿ���� �����ϴ� �ڷ�ƾ
    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        color.a = 1f;
        fadeImage.color = color;

        // ���̵� �� �ִϸ��̼� (�������� ��������)
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        // ���̵� ���� ���� �� �̹��� ������ �����ϰ� �����ϰ� ��Ȱ��ȭ
        color.a = 0f;
        fadeImage.color = color;
        fadeImage.gameObject.SetActive(false);

        // ���̵� �� �Ϸ� �� �÷��̾� �̵� ����
        if (questFieldPlayer != null)
        {
            questFieldPlayer.StartMovement();
        }
    }
}
