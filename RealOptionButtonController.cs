using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealOptionButtonController : MonoBehaviour
{
    // ������Ʈ ����
    public GameObject optionPanel;
    public Slider volumeSlider; // ���� ���� �����̴�
    private BackButtonController backButtonController;
    private CanvasGroup optionPanelCanvasGroup;

    // ��ư �� �ִϸ��̼� ���� ����
    public RectTransform realOptionButton;  // RealOption ��ư�� RectTransform
    public float dropDuration = 1.0f;  // ��ư�� �ö���� �ð�
    public float bounceFactor = 0.3f;  // �ݵ� ũ�� (�󸶳� ���� ƨ����)
    public float bounceSpeed = 2.0f;  // �ݵ� �ӵ�
    public float fadeDuration = 0.5f;  // ���̵���/�ƿ� ���� �ð�

    // ȿ���� ����� �ҽ�
    public AudioSource bgmAudioSource;  // BGM ����� ���� AudioSource
    public AudioSource buttonClickAudioSource; // ��ư Ŭ�� ȿ���� ����� ���� AudioSource
    public AudioSource textEffectAudioSource; // Text ȿ����
    public AudioSource warningTextAudioSource; // ��� ȿ���� ����� ���� AudioSource

    void Start()
    {
        // RealOption ��ư �ʱ� ��ġ ���� (ȭ�� �� �Ʒ���)
        realOptionButton.anchoredPosition = new Vector2(realOptionButton.anchoredPosition.x, -Screen.height - realOptionButton.rect.height);

        // realOptionButtonController ã�Ƽ� ����
        backButtonController = FindObjectOfType<BackButtonController>();

        // CanvasGroup ������Ʈ�� �гο� �߰��Ͽ� �ʱ� ����
        optionPanelCanvasGroup = optionPanel.GetComponent<CanvasGroup>();
        if (optionPanelCanvasGroup == null)
        {
            optionPanelCanvasGroup = optionPanel.AddComponent<CanvasGroup>();
        }

        optionPanelCanvasGroup.alpha = 0;  // �г� ���� �ʱ�ȭ

        optionPanel.SetActive(false); // �г� ��Ȱ��ȭ

        // �����̴� �ʱ�ȭ �� �̺�Ʈ ����
        volumeSlider.onValueChanged.AddListener(AdjustVolume);
        volumeSlider.value = bgmAudioSource.volume; // �����̴� �ʱ� �� ����
        volumeSlider.value = buttonClickAudioSource.volume; // �����̴� �ʱ� �� ����
        volumeSlider.value = textEffectAudioSource.volume; // �����̴� �ʱ� �� ����
        volumeSlider.value = warningTextAudioSource.volume; // �����̴� �ʱ� �� ����
    }

    // ���� �����̴� ���� ���� ���� ����
    private void AdjustVolume(float value)
    {
        bgmAudioSource.volume = value; // �����̴� ���� ���� ����� �ҽ��� ���� ����
        buttonClickAudioSource.volume = value; // �����̴� ���� ���� ����� �ҽ��� ���� ����
        textEffectAudioSource.volume = value; // �����̴� ���� ���� ����� �ҽ��� ���� ����
        warningTextAudioSource.volume = value; // �����̴� ���� ���� ����� �ҽ��� ���� ����
    }

    // RealOption ��ư�� ������ �� ȣ��Ǵ� �޼���
    public void OnRealOptionButtonPressed()
    {
        // ��ư Ŭ�� ȿ���� ���
        if (buttonClickAudioSource != null)
        {
            buttonClickAudioSource.PlayOneShot(buttonClickAudioSource.clip);  // ��ư Ŭ�� �� ȿ���� ���
        }

        StartCoroutine(FadeInOptionPanel());
        StartCoroutine(backButtonController.RightBackButton());
    }

    // RealOption ��ư�� �Ʒ����� ���� �ö󰡴� �ִϸ��̼� �ڷ�ƾ
    public IEnumerator UpRealOptionButton()
    {
        float elapsedTime = 0f;
        Vector2 startPos = realOptionButton.anchoredPosition;
        Vector2 endPos = new Vector2(realOptionButton.anchoredPosition.x, -372f); // ���ϴ� ��ǥ ��ġ�� Y ���� ����

        // �Ʒ����� ���� �̵��ϴ� �ִϸ��̼�
        while (elapsedTime < dropDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / dropDuration;
            realOptionButton.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        // ��Ȯ�� ��ġ ����
        realOptionButton.anchoredPosition = endPos;
        StartCoroutine(BounceRealOptionButton());
    }

    // RealOption ��ư�� �ݵ� �ִϸ��̼�
    private IEnumerator BounceRealOptionButton()
    {
        float elapsedTime = 0f;
        bool bouncing = true;

        while (bouncing)
        {
            elapsedTime += Time.deltaTime;

            // �ݵ� ȿ�� ���
            float bounce = Mathf.Sin(elapsedTime * bounceSpeed) * Mathf.Exp(-elapsedTime * bounceSpeed) * bounceFactor;
            realOptionButton.anchoredPosition = new Vector2(realOptionButton.anchoredPosition.x, realOptionButton.anchoredPosition.y + bounce);

            if (Mathf.Abs(bounce) < 0.01f)
            {
                bouncing = false;
            }

            yield return null;
        }
    }

    // �ɼ� �г��� ���̵��� ȿ�� �ڷ�ƾ
    public IEnumerator FadeInOptionPanel()
    {
        // �г� Ȱ��ȭ
        optionPanel.SetActive(true);

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            optionPanelCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration); // 0���� 1������ ���� �� ����
            yield return null;
        }

        // ������ ���̵��� ����
        optionPanelCanvasGroup.alpha = 1;
    }
}
