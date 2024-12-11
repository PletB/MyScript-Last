using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FieldSixTrigger : MonoBehaviour
{
    public SPUM_Prefabs anim;
    public Image fadeImage; // ���̵� �ƿ� ȿ���� ���� �̹���
    public float fadeDuration = 1f; // ���̵� �ƿ� ���� �ð�
    private bool isTransitioning = false; // ��ȯ ���� Ȯ�ο�

    private QuestFieldPlayer playerController; // �÷��̾� ��Ʈ�ѷ� ����

    private void Start()
    {
        // ���� �� �̹����� ���� ���� 0���� ���� (�����ϰ�)
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;
        }

        // QuestPlayerController ���� ����
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<QuestFieldPlayer>();
        }
    }

    // �÷��̾ Ʈ���ſ� ������ ���̵� �ƿ� �� �� ��ȯ
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            // �÷��̾� �̵� ����
            if (playerController != null)
            {
                playerController.canMove = false;
                playerController.GetComponent<Rigidbody2D>().velocity = Vector2.zero; // ��� �ӵ��� 0���� �����Ͽ� ����
                anim.PlayAnimation(0);
            }

            // �� ��ȯ �ڷ�ƾ ����
            StartCoroutine(TransitionToNextScene());
        }
    }

    // ���̵� �ƿ� ȿ���� �ְ� ���� ������ ��ȯ�ϴ� �ڷ�ƾ
    private IEnumerator TransitionToNextScene()
    {
        isTransitioning = true;

        // ���̵� �ƿ�
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            if (fadeImage != null)
            {
                Color color = fadeImage.color;
                color.a = alpha;
                fadeImage.color = color;
            }
            yield return null;
        }

        // �� ��ȯ
        SceneManager.LoadScene("BossScene"); // "BossScene"�� ���� �� �̸����� �����ϼ���
    }
}
