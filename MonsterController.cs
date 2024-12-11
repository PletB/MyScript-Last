using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public float speed = 1f; // �̵� �ӵ�
    public float moveDistance = 1f; // �¿� �̵� �Ÿ�
    public float chaseRange = 2.5f; // �÷��̾ ������ ����
    public RectTransform player; // �÷��̾��� Transform
    public AudioSource PlayerDamage;
    public int health = 30; // ������ ü��
    public int attackPower = 10; // ������ ���ݷ�

    private Vector3 startingPosition; // ���� ��ġ
    private bool movingRight = true; // ���������� �̵� ������ ����
    private bool isPlayerInContact = false; // �÷��̾���� �浹 ���� Ȯ�ο� ����
    private Coroutine damageCoroutine; // ������ �ڷ�ƾ�� ������ ����

    private void Start()
    {
        // ������ ���� ��ġ ����
        startingPosition = transform.position;
    }

    private void Update()
    {
        if (IsPlayerInRange())
        {
            ChasePlayer();
        }
        else
        {
            MoveMonster();
        }
    }

    // ���� �ڵ� �̵� ����
    private void MoveMonster()
    {
        // �̵� ���� ����
        float moveDirection = movingRight ? 1 : -1;

        // ���� ��ġ���� �̵� �ӵ��� ���⿡ ���� ���ο� ��ġ ���
        transform.Translate(Vector3.right * moveDirection * speed * Time.deltaTime);

        // �̵� ������ �������� ��� ���� ����
        if (movingRight && transform.position.x >= startingPosition.x + moveDistance)
        {
            movingRight = false; // �������� �̵�
        }
        else if (!movingRight && transform.position.x <= startingPosition.x - moveDistance)
        {
            movingRight = true; // ���������� �̵�
        }

        // ���� Ȯ���� ���� ����
        if (Random.Range(0f, 1f) < 0.01f) // 1% Ȯ���� ���� ����
        {
            movingRight = !movingRight; // ���� ����
        }
    }

    // �÷��̾� ���� ����
    private void ChasePlayer()
    {
        // �÷��̾� �������� �̵�
        Vector3 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime);
    }

    // �÷��̾ ���� ���� ���� �ִ��� Ȯ��
    private bool IsPlayerInRange()
    {
        return Vector3.Distance(transform.position, player.position) <= chaseRange;
    }

    // �÷��̾���� �浹 �� ������ ó��
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player")) // �÷��̾�� �浹�ߴ��� Ȯ��
        {
            QuestFieldPlayer playerScript = collision.collider.GetComponent<QuestFieldPlayer>();
            if (playerScript != null && !isPlayerInContact) // ó�� �浹 ��
            {
                isPlayerInContact = true; // �浹 ���¸� true�� ����
                damageCoroutine = StartCoroutine(DealDamageOverTime(playerScript)); // ������ �ڷ�ƾ ����
            }
        }
    }

    // �÷��̾���� �浹 ���� �� �ڷ�ƾ ����
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player")) // �÷��̾���� �浹�� �������� Ȯ��
        {
            isPlayerInContact = false; // �浹 ���¸� false�� ����
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine); // ������ �ڷ�ƾ ����
                damageCoroutine = null;
            }
        }
    }

    // ���� �ð����� �������� �ִ� �ڷ�ƾ
    private IEnumerator DealDamageOverTime(QuestFieldPlayer playerScript)
    {
        while (isPlayerInContact) // �浹 ������ ���� �ݺ�
        {
            if (playerScript.health > 0) // �÷��̾� ü���� 0 �̻����� Ȯ��
            {
                playerScript.TakeDamage(attackPower); // �÷��̾�� �������� ��
                if (PlayerDamage != null)
                {
                    PlayerDamage.Play();
                }
            }
            yield return new WaitForSeconds(2f); // 2�� ���
        }
    }

    // ü�� ���� �޼��� (��: �÷��̾� �������� ���� ������ ó��)
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            StartCoroutine(Die()); // ��� �� ������ �ڷ�ƾ ȣ��
        }
    }

    // Die �޼��带 IEnumerator�� �����Ͽ� �ڷ�ƾ���� ����ǵ��� ����
    private IEnumerator Die()
    {
        FieldFiveTrigger questTrigger = FindObjectOfType<FieldFiveTrigger>();
        if (questTrigger != null)
        {
            questTrigger.MonsterCaptured(); // ���� óġ �� ����Ʈ ����
        }

        // ���͸� ��Ȱ��ȭ�ϰ�, 2�� �� �������� �����ϵ��� RespawnMonster �ڷ�ƾ ȣ��
        GetComponent<SpriteRenderer>().enabled = false; // �ð������� ��Ȱ��ȭ
        GetComponent<Collider2D>().enabled = false; // �浹 ��Ȱ��ȭ

        yield return new WaitForSeconds(0f); // 2�� ��� �� ������

        // ������ �ڷ�ƾ ����
        StartCoroutine(RespawnMonster());
    }

    // ������ �ڷ�ƾ
    private IEnumerator RespawnMonster()
    {
        // �ð��� �� ������ ��Ȱ��ȭ
        GetComponent<SpriteRenderer>().enabled = false; // �ð������� ��Ȱ��ȭ
        GetComponent<Collider2D>().enabled = false; // �浹 ��Ȱ��ȭ

        yield return new WaitForSeconds(2f); // 2�� ��� �� ������

        // ������ ��ġ�� �̵��Ͽ� ������
        Vector3 randomPosition = startingPosition + new Vector3(Random.Range(-moveDistance, moveDistance), 0, 0);
        transform.position = randomPosition;
        health = 30; // ü�� �ʱ�ȭ

        // ���͸� �ٽ� Ȱ��ȭ
        GetComponent<SpriteRenderer>().enabled = true; // �ð������� Ȱ��ȭ
        GetComponent<Collider2D>().enabled = true; // �浹 Ȱ��ȭ
    }

    // ���� ��Ȱ��ȭ �޼���
    public void DisableMonster()
    {
        GetComponent<SpriteRenderer>().enabled = false; // �ð������� ��Ȱ��ȭ
        GetComponent<Collider2D>().enabled = false; // �浹 ��Ȱ��ȭ
        StopAllCoroutines(); // ��� �ڷ�ƾ ���� (������ �� ������ �ڷ�ƾ ����)
    }
}
