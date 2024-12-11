using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FieldSecondTrigger : MonoBehaviour
{
    public TMP_Text dialogueText; // �ؽ�Ʈ�� ǥ���� TextMeshPro �ؽ�Ʈ
    public GameObject textBox; // �ؽ�Ʈ �ڽ� ������Ʈ
    public GameObject blackSphere; // ���� ��ü ������Ʈ
    public GameObject secondTrigger;
    public QuestFieldPlayer questFieldPlayer; // �÷��̾� ��Ʈ�ѷ� ����
    public AudioSource textEffect;
    public float backStepDistance = 1.0f; // �÷��̾ �ڷ� �̵��� �Ÿ�
    public float textDisplayDelay = 0.05f; // �� ���ھ� ��µǴ� ������
    private bool hasTriggeredOnce = false; // ù ��° Ʈ���� �浹 ���� Ȯ��
    private int dialogueIndex = 0;

    private readonly string[] additionalDialogues = new string[]
    {
        "������ �� �ɾ���� �޶����°� ����..",
        "�ƹ��͵� ���ٰ�! �׸���!!",
        "�� ��ġ�ڳ�..? �и��� ���Ѵ�. �ѹ� �� ���⸸ ��?!.",
        "�׸�! �׸�!! �׸�!!!",
        "���� ��ȭ�� �����ϴ�.",
        "���� ��ȭ�� �����ϴ�.",
        "���� ��ȭ�� �����ϴ�.",
        "���� ��ȭ�� �����ϴ�.",
        "���� ��ȭ�� �����ϴ�.",
        "(����.. ���� ��簡 ������?)",
        "StartCoroutine(DisplayTextWithAnimation(additionalDialogues[dialogueIndex]));",
        "�� ��� �̰� �� �ڵ��ε�, �Ǽ��߳�..��",
        "(�ƹ��� ���ð���..?)",
        "����! ���� ����. �ʵ� '�װ�'�� ���ؼ� �°� ����? �׷���?",
        "'�װ�' ������...",
        "���� �ִ� ������ ��. �� �ƴϾ�;;",
        "������ �� �ɾ���� �޶����°� ����..",
        ".....",
        "���� ��¥ ������?",
        "��, �� ���� ��ġ�� ��ų ���߳�..",
        "�� ��򰡿� ���� ������ �ִٴ� ���� �𸦰ž� ����������",
        "���� ��¥ ��ȭ �����ϱ� ���� ����! ���� ����Ʈ�� �޾�!!"
    };

    private void Start()
    {
        // �ؽ�Ʈ �ڽ��� �ؽ�Ʈ�� ��Ȱ��ȭ
        if (textBox != null)
        {
            textBox.SetActive(false);
        }
        if (dialogueText != null)
        {
            dialogueText.gameObject.SetActive(false);
        }
        if (blackSphere != null)
        {
            blackSphere.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // �÷��̾� �̵� ��Ȱ��ȭ
            if (questFieldPlayer != null)
            {
                if (questFieldPlayer.rb != null)
                {
                    questFieldPlayer.rb.velocity = Vector2.zero; // �ӵ��� 0���� �����Ͽ� �ﰢ������ ����
                    questFieldPlayer.rb.angularVelocity = 0f; // ȸ�� �ӵ��� 0���� ����
                }
                questFieldPlayer.canMove = false;
                questFieldPlayer.anim.PlayAnimation(0);
            }

            // �ؽ�Ʈ �ڽ��� �ؽ�Ʈ Ȱ��ȭ �� �ؽ�Ʈ ���� ����
            if (textBox != null)
            {
                textBox.SetActive(true);
            }
            if (dialogueText != null)
            {
                dialogueText.gameObject.SetActive(true);

                // ù ��°�� Ʈ���ſ� ����� ��
                if (!hasTriggeredOnce)
                {
                    StartCoroutine(DisplayTextWithAnimation("���� �ִ� ������ ��. �� �ƴϾ�;;"));
                    hasTriggeredOnce = true; // ù ��° Ʈ���� ó�� �Ϸ�
                }
                else
                {
                    // �߰� ��� ���
                    if (dialogueIndex < additionalDialogues.Length)
                    {
                        StartCoroutine(DisplayTextWithAnimation(additionalDialogues[dialogueIndex]));
                        dialogueIndex++;
                    }
                }
            }
        }
    }

    private IEnumerator DisplayTextWithAnimation(string message)
    {
        dialogueText.text = ""; // �ؽ�Ʈ �ʱ�ȭ

        // �� ���ھ� ���
        foreach (char letter in message)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textDisplayDelay); // �� ���� ��� �� ������
            // ȿ���� ���                                                   
            if (textEffect != null)
            {
                textEffect.Play();
            }
        }

        // ��� �ؽ�Ʈ�� ��µ� �� 1.5�� ���
        yield return new WaitForSeconds(1.5f);

        // ������ �ؽ�Ʈ�� ��µ� �� 2�� ����ϰ� �ؽ�Ʈ, �ؽ�Ʈ �ڽ�, ���� ��ü ��Ȱ��ȭ
        if (message == "���� ��¥ ��ȭ �����ϱ� ���� ����! ���� ����Ʈ�� �޾�!!")
        {
            yield return new WaitForSeconds(1.5f);
            if (textBox != null)
            {
                textBox.SetActive(false);
            }
            if (dialogueText != null)
            {
                dialogueText.gameObject.SetActive(false);
            }
            if (blackSphere != null)
            {
                blackSphere.SetActive(false);
            }
            questFieldPlayer.canMove = true; // �÷��̾� �̵� �����ϰ� ����
            secondTrigger.SetActive(false);

        }
        else
        {
            // ������ �ؽ�Ʈ�� �ƴ� ��� 2�� �ڿ� �÷��̾ 1ĭ �ڷ� �̵�
            StartCoroutine(DelayedMoveBack());
        }
    }

    private IEnumerator DelayedMoveBack()
    {
        yield return new WaitForSeconds(0f);

        // �÷��̾ 1ĭ �ڷ� �̵�
        Vector3 backPosition = questFieldPlayer.transform.position + new Vector3(backStepDistance, 0, 0);
        questFieldPlayer.transform.position = backPosition;

        // �ؽ�Ʈ �ڽ��� �ؽ�Ʈ ��Ȱ��ȭ
        if (textBox != null)
        {
            textBox.SetActive(false);
        }
        if (dialogueText != null)
        {
            dialogueText.gameObject.SetActive(false);
            questFieldPlayer.canMove = true;
        }
    }
}
