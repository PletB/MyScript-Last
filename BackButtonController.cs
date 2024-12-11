using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButtonController : MonoBehaviour
{
    // ������Ʈ ����
    public GameObject optionPanel;
    private CanvasGroup optionPanelCanvasGroup;

    // ��ư �� �ִϸ��̼� ���� ����
    public RectTransform backButton;  // Start ��ư�� RectTransform
    public float dropDuration = 1.0f;  // ��ư�� ���������� ���� �ð�
    public float bounceFactor = 0.3f;  // �ݵ� ũ�� (�󸶳� ���� ƨ����)
    public float bounceSpeed = 2.0f;  // �ݵ� �ӵ�

    // ���̵� �ƿ� ���� �ð�
    public float fadeDuration = 0.5f;

    // ȿ���� ����� �ҽ�
    public AudioSource buttonClickAudioSource; // ��ư Ŭ�� ȿ���� ����� ���� AudioSource

    void Start()
    {
        // CanvasGroup ������Ʈ�� �гο� �߰��Ͽ� �ʱ� ����
        optionPanelCanvasGroup = optionPanel.GetComponent<CanvasGroup>();
        if (optionPanelCanvasGroup == null)
        {
            optionPanelCanvasGroup = optionPanel.AddComponent<CanvasGroup>();
        }

        // Start ��ư �ʱ� ��ġ ���� (ȭ�� ����)
        backButton.anchoredPosition = new Vector2(-Screen.width - backButton.rect.width, backButton.anchoredPosition.y);
    }


    // Quit ��ư�� ������ �� ȣ��Ǵ� �޼���
    public void OnBackButtonPressed()
    {
        // ��ư Ŭ�� ȿ���� ���
        if (buttonClickAudioSource != null)
        {
            buttonClickAudioSource.PlayOneShot(buttonClickAudioSource.clip);  // ��ư Ŭ�� �� ȿ���� ���
        }

        StartCoroutine(FadeOutOptionPanel());
    }

    // Back ��ư�� �Ʒ����� ���� �ö󰡴� �ִϸ��̼� �ڷ�ƾ
    public IEnumerator RightBackButton()
    {
        float elapsedTime = 0f;
        Vector2 startPos = backButton.anchoredPosition;
        Vector2 endPos = new Vector2(-514f, backButton.anchoredPosition.y); // ���ϴ� ��ǥ ��ġ�� Y ���� ����

        // �Ʒ����� ���� �̵��ϴ� �ִϸ��̼�
        while (elapsedTime < dropDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / dropDuration;
            backButton.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        // ��Ȯ�� ��ġ ����
        backButton.anchoredPosition = endPos;
        StartCoroutine(BounceBackButton());
    }

    // Back ��ư�� �ݵ� �ִϸ��̼�
    private IEnumerator BounceBackButton()
    {
        float elapsedTime = 0f;
        bool bouncing = true;

        while (bouncing)
        {
            elapsedTime += Time.deltaTime;

            // �ݵ� ȿ�� ���
            float bounce = Mathf.Sin(elapsedTime * bounceSpeed) * Mathf.Exp(-elapsedTime * bounceSpeed) * bounceFactor;
            backButton.anchoredPosition = new Vector2(backButton.anchoredPosition.x, backButton.anchoredPosition.y + bounce);

            if (Mathf.Abs(bounce) < 0.01f)
            {
                bouncing = false;
            }

            yield return null;
        }
    }

    // �ɼ� �г��� ���̵� �ƿ� ȿ�� �ڷ�ƾ
    private IEnumerator FadeOutOptionPanel()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            optionPanelCanvasGroup.alpha = Mathf.Clamp01(1 - (elapsedTime / fadeDuration)); // 1���� 0������ ���� �� ����
            yield return null;
        }

        // ������ ��������� ���� �� ��Ȱ��ȭ
        optionPanelCanvasGroup.alpha = 0;
        optionPanel.SetActive(false);
    }
}
