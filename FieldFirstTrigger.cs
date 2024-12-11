using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FieldFirstTrigger : MonoBehaviour
{
    public TMP_Text questText; // ����Ʈ �ȳ� �ؽ�Ʈ (TextMeshPro �ؽ�Ʈ)
    public AudioSource SignBoardOpen; // ǥ���� �ؽ�Ʈ ��� ȿ����
    public AudioSource SignBoardClose; // ǥ���� �ؽ�Ʈ ��Ȱ��ȭ�� ȿ����

    private void Start()
    {
        // ����Ʈ �ؽ�Ʈ ��Ȱ��ȭ (���� ��)
        if (questText != null)
        {
            questText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // �÷��̾ Ʈ���ſ� ���� �� �ؽ�Ʈ Ȱ��ȭ
        if (other.CompareTag("Player") && questText != null)
        {
            questText.gameObject.SetActive(true);
            questText.text = "����Ʈ �޴� ��->";
            // ȿ���� ���
            if (SignBoardOpen != null)
            {
                SignBoardOpen.Play();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // �÷��̾ Ʈ���Ÿ� ��� �� �ؽ�Ʈ ��Ȱ��ȭ
        if (other.CompareTag("Player") && questText != null)
        {
            questText.gameObject.SetActive(false);
            // ȿ���� ���
            if (SignBoardClose != null)
            {
                SignBoardClose.Play();
            }
        }
    }
}
