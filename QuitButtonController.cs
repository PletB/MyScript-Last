using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitButtonController : MonoBehaviour
{
    // ��ư �� �ִϸ��̼� ���� ����
    public RectTransform quitButton;  // Quit ��ư�� RectTransform
    public float dropDuration = 1.0f;  // ��ư�� �������� �ð�
    public float bounceFactor = 0.3f;  // �ݵ� ũ�� (�󸶳� ���� ƨ����)
    public float bounceSpeed = 2.0f;  // �ݵ� �ӵ�

    // ȿ���� ����� �ҽ�
    public AudioSource buttonClickAudioSource; // ��ư Ŭ�� ȿ���� ����� ���� AudioSource

    void Start()
    {
        // Quit ��ư �ʱ� ��ġ ���� (ȭ�� ����)
        quitButton.anchoredPosition = new Vector2(quitButton.anchoredPosition.x, Screen.height + quitButton.rect.height);
    }

    // Quit ��ư�� ������ �� ȣ��Ǵ� �޼���
    public void OnQuitButtonPressed()
    {
        // ��ư Ŭ�� ȿ���� ���
        if (buttonClickAudioSource != null)
        {
            buttonClickAudioSource.PlayOneShot(buttonClickAudioSource.clip);  // ��ư Ŭ�� �� ȿ���� ���
        }

        Application.Quit();  // ���� ���� ����
    }

    // Quit ��ư�� �Ʒ��� �������� �ִϸ��̼� �ڷ�ƾ
    public IEnumerator DropQuitButton()
    {
        float elapsedTime = 0f;
        Vector2 startPos = quitButton.anchoredPosition;
        Vector2 endPos = new Vector2(quitButton.anchoredPosition.x, 176f);

        // ��� �ִϸ��̼�
        while (elapsedTime < dropDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / dropDuration;
            quitButton.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        // ��Ȯ�� ��ġ ����
        quitButton.anchoredPosition = endPos;
        StartCoroutine(BounceQuitButton());
    }

    // Quit ��ư�� ȭ�� ���� �ö󰡴� �ִϸ��̼� �ڷ�ƾ
    public IEnumerator MoveButtonUp()
    {
        float elapsedTime = 0f;
        Vector2 startPos = quitButton.anchoredPosition;
        Vector2 endPos = new Vector2(quitButton.anchoredPosition.x, Screen.height + quitButton.rect.height);

        // �ö󰡴� �ִϸ��̼� ����
        while (elapsedTime < dropDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / dropDuration;
            quitButton.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        // ��Ȯ�� ��ġ ����
        quitButton.anchoredPosition = endPos;
    }

    // Quit ��ư�� �ݵ� �ִϸ��̼�
    private IEnumerator BounceQuitButton()
    {
        float elapsedTime = 0f;
        bool bouncing = true;

        while (bouncing)
        {
            elapsedTime += Time.deltaTime;

            // �ݵ� ȿ�� ���
            float bounce = Mathf.Sin(elapsedTime * bounceSpeed) * Mathf.Exp(-elapsedTime * bounceSpeed) * bounceFactor;
            quitButton.anchoredPosition = new Vector2(quitButton.anchoredPosition.x, quitButton.anchoredPosition.y + bounce);

            if (Mathf.Abs(bounce) < 0.01f)
            {
                bouncing = false;
            }

            yield return null;
        }
    }
}
