using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionButtonController : MonoBehaviour
{
    // ��ư �� �ִϸ��̼� ���� ����
    public RectTransform optionButton;  // Option ��ư�� RectTransform
    public float dropDuration = 1.0f;  // ��ư�� �������� �ð�
    public float bounceFactor = 0.3f;  // �ݵ� ũ�� (�󸶳� ���� ƨ����)
    public float bounceSpeed = 2.0f;  // �ݵ� �ӵ�

    // ȿ���� ����� �ҽ�
    public AudioSource buttonClickAudioSource; // ��ư Ŭ�� ȿ���� ����� ���� AudioSource

    void Start()
    {
        // Option ��ư �ʱ� ��ġ ���� (ȭ�� ����)
        optionButton.anchoredPosition = new Vector2(optionButton.anchoredPosition.x, Screen.height + optionButton.rect.height);
    }

    // Option ��ư�� ������ �� ȣ��Ǵ� �޼���
    public void OnOptionButtonPressed()
    {
        // ��ư Ŭ�� ȿ���� ���
        if (buttonClickAudioSource != null)
        {
            buttonClickAudioSource.PlayOneShot(buttonClickAudioSource.clip);  // ��ư Ŭ�� �� ȿ���� ���
        }

        Application.Quit();  // ���� ���� ����
    }

    // Option ��ư�� �Ʒ��� �������� �ִϸ��̼� �ڷ�ƾ
    public IEnumerator DropOptionButton()
    {
        float elapsedTime = 0f;
        Vector2 startPos = optionButton.anchoredPosition;
        Vector2 endPos = new Vector2(optionButton.anchoredPosition.x, -198f);

        // ��� �ִϸ��̼�
        while (elapsedTime < dropDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / dropDuration;
            optionButton.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        // ��Ȯ�� ��ġ ����
        optionButton.anchoredPosition = endPos;
        StartCoroutine(BounceOptionButton());
    }

    // Option ��ư�� ȭ�� ���� �ö󰡴� �ִϸ��̼� �ڷ�ƾ
    public IEnumerator MoveButtonUp()
    {
        float elapsedTime = 0f;
        Vector2 startPos = optionButton.anchoredPosition;
        Vector2 endPos = new Vector2(optionButton.anchoredPosition.x, Screen.height + optionButton.rect.height);

        // �ö󰡴� �ִϸ��̼� ����
        while (elapsedTime < dropDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / dropDuration;
            optionButton.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        // ��Ȯ�� ��ġ ����
        optionButton.anchoredPosition = endPos;
    }

    // Option ��ư�� �ݵ� �ִϸ��̼�
    private IEnumerator BounceOptionButton()
    {
        float elapsedTime = 0f;
        bool bouncing = true;

        while (bouncing)
        {
            elapsedTime += Time.deltaTime;

            // �ݵ� ȿ�� ���
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
