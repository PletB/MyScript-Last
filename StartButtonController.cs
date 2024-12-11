using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButtonController : MonoBehaviour
{
    // ��� �ؽ�Ʈ, InputField ������Ʈ
    public TMP_Text warningText;
    public TMP_Text errorText;
    public GameObject mainPlayerName;  // MainPlayerName ������Ʈ (InputField ����)
    public GameObject realOptionButton;  // RealOptionButton ������Ʈ (InputField ����)

    // ��ũ��Ʈ ����
    private MultiTextDisplayController multiTextDisplayController;
    private OptionButtonController optionButtonController;
    private QuitButtonController quitButtonController;
    private RealOptionButtonController realOptionButtonController;

    //�߰��� ȿ���� ����� �ҽ�
    public AudioSource textEffectAudioSource; // Text ȿ����
    public AudioSource buttonClickAudioSource; // ��ư Ŭ�� ȿ���� ����� ���� AudioSource
    public AudioSource warningTextAudioSource; // ��� ȿ���� ����� ���� AudioSource

    // ��ư �� �ִϸ��̼� ���� ����
    public RectTransform startButton;  // Start ��ư�� RectTransform
    public float dropDuration = 1.0f;  // ��ư�� �������� �ð�
    public float bounceFactor = 0.3f;  // �ݵ� ũ�� (�󸶳� ���� ƨ����)
    public float bounceSpeed = 2.0f;  // �ݵ� �ӵ�
    private bool isStartButtonPressed = false;  // Start ��ư�� ���ȴ��� ����
    private bool isNameEntered = false;  // �÷��̾ �̸��� �Է��ߴ��� ����
    private int warningCount = -1;  // Text7,8 ��� �޽��� Ƚ��
    private bool isWarningActive = false;  // Text7,8 ��� �޽����� ��� ������ ����
    private bool isStartButtonReturn = false;  // ��ư �ִϸ��̼��� �۵������� ����

    void Start()
    {
        // Start ��ư �ʱ� ��ġ ���� (ȭ�� ����)
        startButton.anchoredPosition = new Vector2(startButton.anchoredPosition.x,  Screen.height+ startButton.rect.height);

        // MultiTextDisplayController�� ã�Ƽ� ����
        multiTextDisplayController = FindObjectOfType<MultiTextDisplayController>();

        // optionButtonController ã�Ƽ� ����
        optionButtonController = FindObjectOfType<OptionButtonController>();

        // quitButtonController ã�Ƽ� ����
        quitButtonController = FindObjectOfType<QuitButtonController>();

        // realOptionButtonController ã�Ƽ� ����
        realOptionButtonController = FindObjectOfType<RealOptionButtonController>();

        // ó���� ��� �ؽ�Ʈ ��Ȱ��ȭ
        warningText.gameObject.SetActive(false);

        // ó���� ��� �ؽ�Ʈ ��Ȱ��ȭ
        errorText.gameObject.SetActive(false);

        // ó���� InputField ��Ȱ��ȭ
        mainPlayerName.gameObject.SetActive(false);

        // InputField�� onEndEdit �̺�Ʈ�� �����Ͽ� �̸��� �ԷµǾ����� Ȯ��
        TMP_InputField inputField = mainPlayerName.GetComponent<TMP_InputField>();
        inputField.onEndEdit.AddListener(delegate { CheckNameInput(inputField.text); });
    }

    // Start ��ư�� ������ �� ȣ��Ǵ� �޼���
    public void OnStartButtonPressed()
    {
        // ��� �޽����� ��� ���� ���� ��ư Ŭ���� ����
        if (isWarningActive || isStartButtonReturn)
        {
            return; // �Լ� ����, Ŭ���� ������
        }

        // ��ư Ŭ�� ȿ���� ���
        if (buttonClickAudioSource != null)
        {
            buttonClickAudioSource.PlayOneShot(buttonClickAudioSource.clip);  // ��ư Ŭ�� �� ȿ���� ���
        }

        // Start ��ư�� ó�� Ŭ������ �� MoveButtonUp
        if (!isStartButtonPressed)
        {
            isStartButtonPressed = true;
            StartCoroutine(MoveButtonUp());
        }
        else
        {
            // �� ��° Ŭ�� ��, OnInputField �ڷ�ƾ ����
            StartCoroutine(OnInputField());
        }
    }

    // Start ��ư�� ��� �Լ�
    public IEnumerator StartButtonWait()
    {
        // Start ��ư �ִϸ��̼� ����
        yield return StartCoroutine(DropStartButton());

        float waitTime = 0f;  // ��� �ð� ���� ����
        float maxWaitTime = 3f;  // 3��
        string firstWarningMessage = "��ư �� �����ٷ�..?"; // ù ��° ��� �ؽ�Ʈ
        string secondWarningMessage = "�� ���� ��ư �� �������.. ��û��!"; // �� ��° ��� �ؽ�Ʈ
        string thirdWarningMessage = "�� ��ư ������ ���� �𸣴�..? ����.. �׳� ������;;"; // �� ��° ��� �ؽ�Ʈ
        bool displayedFirstWarning = false;  // ù ��° ��� �ؽ�Ʈ�� ��µǾ����� ����
        bool displayedSecondWarning = false;  // �� ��° ��� �ؽ�Ʈ�� ��µǾ����� ����

        // Start ��ư�� ���� ������ ���
        while (!isStartButtonPressed)
        {
            yield return null; // �� ������ ���

            // Ŭ�� ��� �ð��� maxWaitTime�� �ʰ��ϸ� ù ��° �ؽ�Ʈ ���
            waitTime += Time.deltaTime;
            if (waitTime >= maxWaitTime && !displayedFirstWarning)
            {
                displayedFirstWarning = true;  // ù ��° ��� �ؽ�Ʈ�� ��µ�

                yield return StartCoroutine(DisplayWarningAndReactivateButton(firstWarningMessage));

                // ��� �ð� �ʱ�ȭ �� �� ��° �ؽ�Ʈ ��� ���� �̵�
                waitTime = 0f;
            }

            // Ŭ�� ��� �ð��� maxWaitTime�� �ʰ��ϸ� �� ��° �ؽ�Ʈ ���
            if (waitTime >= maxWaitTime && displayedFirstWarning && !displayedSecondWarning)
            {
                displayedSecondWarning = true;  // �� ��° ��� �ؽ�Ʈ�� ��µ�

                yield return StartCoroutine(DisplayWarningAndReactivateButton(secondWarningMessage));

                // ��� �ð� �ʱ�ȭ �� �� ��° �ؽ�Ʈ ��� ���� �̵�
                waitTime = 0f;
            }

            // Ŭ�� ��� �ð��� maxWaitTime�� �ʰ��ϸ� �� ��° �ؽ�Ʈ ���
            if (waitTime >= maxWaitTime && displayedSecondWarning)
            {
                // StartButton ��Ȱ��ȭ
                startButton.gameObject.SetActive(false);

                yield return StartCoroutine(DisplayTextWithAnimation(thirdWarningMessage));

                // �ؽ�Ʈ ����� ���� �� 1�� ���
                yield return new WaitForSeconds(1f);

                // ���� ����
                Application.Quit();
            }
        }

        // Start ��ư�� ���� �� MultiTextDisplayController���� �ؽ�Ʈ�� ��� ���
        yield return StartCoroutine(multiTextDisplayController.ContinueDisplayingTextsFromIndex(4));
    }

    // Text6 �� ���� �Լ�
    public IEnumerator StartButtonReturn()
    {
        isStartButtonReturn = true; // ��ư �ִϸ��̼� �۵� ��

        // Start ��ư �ִϸ��̼� ����
        yield return StartCoroutine(DropStartButton());

        // Option ��ư �ִϸ��̼� ����
        yield return StartCoroutine(optionButtonController.DropOptionButton());

        // Quit ��ư �ִϸ��̼� ����
        yield return StartCoroutine(quitButtonController.DropQuitButton());

        // RealOption ��ư �ִϸ��̼� ����
        yield return StartCoroutine(realOptionButtonController.UpRealOptionButton());

        // InputField Ȱ��ȭ
        mainPlayerName.gameObject.SetActive(true);

        // Start ��ư �ִϸ��̼� ����
        yield return StartCoroutine(OnInputField());
    }

    // Start ��ư�� �Ʒ��� �������� �ִϸ��̼� �ڷ�ƾ
    private IEnumerator DropStartButton()
    {
        float elapsedTime = 0f;
        Vector2 startPos = startButton.anchoredPosition;
        Vector2 endPos = new Vector2(startButton.anchoredPosition.x, 50f);

        // ��� �ִϸ��̼�
        while (elapsedTime < dropDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / dropDuration;
            startButton.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        // ��Ȯ�� ��ġ ����
        startButton.anchoredPosition = endPos;
        StartCoroutine(BounceStartButton());
    }

    // Start ��ư�� ȭ�� ���� �ö󰡴� �ִϸ��̼� �ڷ�ƾ
    private IEnumerator MoveButtonUp()
    {
        float elapsedTime = 0f;
        Vector2 startPos = startButton.anchoredPosition;
        Vector2 endPos = new Vector2(startButton.anchoredPosition.x, Screen.height + startButton.rect.height);

        // �ö󰡴� �ִϸ��̼� ����
        while (elapsedTime < dropDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / dropDuration;
            startButton.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        // ��Ȯ�� ��ġ ����
        startButton.anchoredPosition = endPos;
    }

    // Start ��ư�� �ݵ� �ִϸ��̼�
    private IEnumerator BounceStartButton()
    {
        float elapsedTime = 0f;
        bool bouncing = true;

        while (bouncing)
        {
            elapsedTime += Time.deltaTime;

            // �ݵ� ȿ�� ���
            float bounce = Mathf.Sin(elapsedTime * bounceSpeed) * Mathf.Exp(-elapsedTime * bounceSpeed) * bounceFactor;
            startButton.anchoredPosition = new Vector2(startButton.anchoredPosition.x, startButton.anchoredPosition.y + bounce);

            if (Mathf.Abs(bounce) < 0.01f)
            {
                bouncing = false;
            }

            yield return null;
        }
    }

    // ��� �޽����� ����ϰ� StartButton�� �ٽ� Ȱ��ȭ�ϴ� �ڷ�ƾ
    public IEnumerator DisplayWarningAndReactivateButton(string warningMessage)
    {
        // StartButton ��Ȱ��ȭ
        startButton.gameObject.SetActive(false); 

        // ��� �ؽ�Ʈ �ִϸ��̼� ���
        yield return StartCoroutine(DisplayTextWithAnimation(warningMessage));

        // �ؽ�Ʈ ����� ���� �� 1�� ���
        yield return new WaitForSeconds(1f);

        // ��� �ؽ�Ʈ ��Ȱ��ȭ
        warningText.gameObject.SetActive(false);

        // StartButton �ٽ� Ȱ��ȭ
        startButton.gameObject.SetActive(true);
    }

    // �� ���ھ� �ؽ�Ʈ�� ����ϴ� �ڷ�ƾ
    private IEnumerator DisplayTextWithAnimation(string message)
    {
        warningText.gameObject.SetActive(true);  // ��� �ؽ�Ʈ Ȱ��ȭ
        warningText.text = "";  // �ؽ�Ʈ �ʱ�ȭ

        // �� ���ھ� ���
        for (int i = 0; i < message.Length; i++)
        {
            warningText.text += message[i];  // �� ���ھ� �߰�

            // �ؽ�Ʈ ȿ���� ���
            if (textEffectAudioSource != null)
            {
                textEffectAudioSource.PlayOneShot(textEffectAudioSource.clip);  // �� ���ڸ��� ȿ���� ���
            }

            yield return new WaitForSeconds(0.1f);  // �� ���� ��� �� 0.1�� ���
        }
    }

    // �÷��̾ InputField�� �̸��� �Է��Ͽ����� ����
    private void CheckNameInput(string inputText)
    {
        if (!string.IsNullOrEmpty(inputText))
        {
            isNameEntered = true;  // �̸��� �ԷµǾ����� ���
            PlayerPrefs.SetString("PlayerName", inputText); // PlayerPrefs�� �̸� ����
        }
        else
        {
            isNameEntered = false;  // �̸��� �Էµ��� �ʾ����� ���
        }
    }

    public IEnumerator OnInputField()
    {
        // �̸��� �ԷµǾ����� Ȯ��
        if (isNameEntered)
        {
            // �̸��� �Էµ� ���, ��ư���� ���� �ø��� Text7�� Text8�� ���
            yield return StartCoroutine(MoveButtonUp());  // Start ��ư ����
            yield return StartCoroutine(optionButtonController.MoveButtonUp());  // Option ��ư ����
            yield return StartCoroutine(quitButtonController.MoveButtonUp());  // Quit ��ư ����

            mainPlayerName.SetActive(false); // InputField ��Ȱ��ȭ
            realOptionButton.SetActive(false); //BackButton ��Ȱ��ȭ

            // Text7�� Text8�� ���������� ���
            yield return StartCoroutine(multiTextDisplayController.DisplayText(multiTextDisplayController.textObjects[6], multiTextDisplayController.messages[6]));
            yield return new WaitForSeconds(multiTextDisplayController.betweenTextDelay);
            multiTextDisplayController.textObjects[6].gameObject.SetActive(false);

            yield return StartCoroutine(multiTextDisplayController.DisplayText(multiTextDisplayController.textObjects[7], multiTextDisplayController.messages[7]));
            yield return new WaitForSeconds(multiTextDisplayController.betweenTextDelay);
            multiTextDisplayController.textObjects[7].gameObject.SetActive(false);

            // ���� ������ �̵�
            SceneManager.LoadScene("CityScene"); // "CityScene"�� ���� ���� �̸����� ����
        }

        if(!isNameEntered)
        {
            // �̸��� �Էµ��� �ʾ��� ��� ��� �޽��� ���
            StartCoroutine(DisplayWarningText7And8());
        }
    }

    // ��� �ؽ�Ʈ�� ����ϴ� �ڷ�ƾ
    private IEnumerator DisplayWarningText7And8()
    {
        isWarningActive = true;  // ��� �޽����� ��� ������ ���
        isStartButtonReturn = false; // ��ư �ִϸ��̼��� ����

        // ��� �޽����� ���� �ؽ�Ʈ ����
        if (warningCount == 0)
        {
            errorText.text = "�Ʊ� ���� ������? �̸� �ۼ��϶��!";

            errorText.gameObject.SetActive(true);  // ��� �ؽ�Ʈ Ȱ��ȭ

            // ��� ȿ���� ���
            if (warningTextAudioSource != null)
            {
                warningTextAudioSource.PlayOneShot(warningTextAudioSource.clip);
            }

            // ��� �޽��� ��� �� ��� �ð�
            yield return new WaitForSeconds(2.0f);

            // ��� �ؽ�Ʈ ��Ȱ��ȭ
            errorText.gameObject.SetActive(false);
        }
        else if (warningCount == 1)
        {
            errorText.text = "Ȥ�� �ѱ��� �� �˾Ƶ��..??";

            errorText.gameObject.SetActive(true);  // ��� �ؽ�Ʈ Ȱ��ȭ

            // ��� ȿ���� ���
            if (warningTextAudioSource != null)
            {
                warningTextAudioSource.PlayOneShot(warningTextAudioSource.clip);
            }

            // ��� �޽��� ��� �� ��� �ð�
            yield return new WaitForSeconds(2.0f);

            // ��� �ؽ�Ʈ ��Ȱ��ȭ
            errorText.gameObject.SetActive(false);
        }
        else if (warningCount == 2)
        {
            errorText.text = "����.. ��û�� ��..�� ������ �̸� ���°ͺ��� ���� ��;;";

            // ��� �ؽ�Ʈ Ȱ��ȭ
            errorText.gameObject.SetActive(true);

            // ��� ȿ���� ���
            if (warningTextAudioSource != null)
            {
                warningTextAudioSource.PlayOneShot(warningTextAudioSource.clip);
            }

            // ��� �޽����� ����� ���� �� �ֵ��� ���
            yield return new WaitForSeconds(2.0f);

            // ��� �ؽ�Ʈ ��Ȱ��ȭ
            errorText.gameObject.SetActive(false);

            // ������ ��� �޽��� ���� ���� ���� ����
            Application.Quit();  // ���� ���� ����
            yield break;
        }

        // ��� Ƚ�� ����
        warningCount++;

        isWarningActive = false;  // ��� �޽��� ����� ������ ���
    }
}