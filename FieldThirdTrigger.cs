using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FieldThirdTrigger : MonoBehaviour
{
    public Transform fourthTriggerLocation; // �� ��° Ʈ������ ��ġ ����
    public QuestFieldPlayer questFieldPlayer; // �÷��̾� ��Ʈ�ѷ� ����
    public TMP_Text teleportText; // �ڷ���Ʈ �ȳ� �ؽ�Ʈ (TextMeshPro �ؽ�Ʈ)
    public AudioSource teleportEffect; // �ڷ���Ʈ ȿ����
    public AudioSource SignBoardEffect; // �ڷ���Ʈ ȿ����
    public AudioSource SignBoardClose; // �ڷ���Ʈ ȿ����

    private bool canTeleport = false; // �ڷ���Ʈ ���� ���� Ȯ��

    private void Start()
    {
        // �ؽ�Ʈ ��Ȱ��ȭ
        if (teleportText != null)
        {
            teleportText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // �÷��̾ Ʈ���ſ� ����� ��
        if (other.CompareTag("Player") && questFieldPlayer != null && fourthTriggerLocation != null)
        {
            // �ڷ���Ʈ �ȳ� �ؽ�Ʈ Ȱ��ȭ
            if (teleportText != null)
            {
                teleportText.gameObject.SetActive(true);
                teleportText.text = "����Ű: �� Ű�� ��������.";
                // ȿ���� ���
                if (SignBoardEffect != null)
                {
                    SignBoardEffect.Play();
                }
            }

            canTeleport = true; // �ڷ���Ʈ ���� ���·� ����
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // �÷��̾ Ʈ���Ÿ� ����� ��
        if (other.CompareTag("Player"))
        {
            // �ؽ�Ʈ ��Ȱ��ȭ
            if (teleportText != null)
            {
                teleportText.gameObject.SetActive(false);
                // ȿ���� ���
                if (SignBoardClose != null)
                {
                    SignBoardClose.Play();
                }
            }

            canTeleport = false; // �ڷ���Ʈ �Ұ��� ���·� ����
        }
    }

    private void Update()
    {
        // �ڷ���Ʈ�� ������ �� �� ����Ű �Է� �� �� ��° Ʈ���� ��ġ�� �̵�
        if (canTeleport && Input.GetKeyDown(KeyCode.UpArrow))
        {
            questFieldPlayer.transform.position = fourthTriggerLocation.position;
            // ȿ���� ���
            if (teleportEffect != null)
            {
                teleportEffect.Play();
            }

            // �ؽ�Ʈ ��Ȱ��ȭ
            if (teleportText != null)
            {
                teleportText.gameObject.SetActive(false);
            }

            canTeleport = false; // �ڷ���Ʈ ���� �ʱ�ȭ
        }
    }
}
