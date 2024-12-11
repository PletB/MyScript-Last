using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuestFieldPlayer : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float runSpeedMultiplier = 1.5f;
    public float jumpForce = 5f;
    public int attackPower = 10; // ���ݷ� ����
    public float attackRange = 1.5f; // ���� ����
    public int health = 100; // �ʱ� ü��

    public SPUM_Prefabs anim; // �ִϸ��̼� ����
    public bool canMove = false;
    private bool canAttack = true; // ���� ���� ����
    public Rigidbody2D rb;
    public AudioSource quesFieldtArea;
    public AudioSource SwordSwing;
    public AudioSource SwordAttack;
    private QuestFieldFadeIn fadeInManager;
    private MoveCamera MoveCamera;
    private Vector3 originalScale;
    private Vector3 spawnPosition; // ���� ��ġ ����

    private void Start()
    {
        MoveCamera = FindObjectOfType<MoveCamera>();
        originalScale = transform.localScale;
        rb = GetComponent<Rigidbody2D>();
        fadeInManager = FindObjectOfType<QuestFieldFadeIn>();
        spawnPosition = transform.position; // ó�� ���� ��ġ�� ����

        if (quesFieldtArea != null)
        {
            quesFieldtArea.Play();
        }
    }

    private void Update()
    {
        if (canMove)
        {
            HandleMovement();
            MoveCamera.FollowPlayer();

            // 5�� Ű �Է� Ȯ�� �� ���� ���� ���� Ȯ��
            if (Input.GetKeyDown(KeyCode.Alpha5) && canAttack) // Alpha5�� ���� Ű 5�� �ǹ��մϴ�.
            {
                Attack();
            }

            // ���� �ִϸ��̼��� ������ �� Ʈ���� ����
            if (anim._anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") &&
                anim._anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                anim._anim.ResetTrigger("Attack");
            }
            // ��� �ִϸ��̼��� ������ �� Ʈ���� ����
            if (anim._anim.GetCurrentAnimatorStateInfo(0).IsTag("Die") &&
                anim._anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                anim._anim.ResetTrigger("Die");
            }
        }
    }

    public void StartMovement()
    {
        canMove = true;
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float speed = Input.GetKey(KeyCode.Space) ? moveSpeed * runSpeedMultiplier : moveSpeed;
        rb.velocity = new Vector2(moveX * speed, rb.velocity.y);

        if (Input.GetKeyDown(KeyCode.CapsLock) && Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }

        HandlePlayerAnimation(moveX);
    }

    private void HandlePlayerAnimation(float moveX)
    {
        if (moveX != 0)
        {
            // �̵� ���⿡ �°� ���� ��ȯ
            transform.localScale = new Vector3(Mathf.Sign(moveX) * Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            anim.PlayAnimation(1); // �ȱ� �ִϸ��̼� ����
        }
        else
        {
            anim.PlayAnimation(0); // ���� �ִϸ��̼� ����
        }
    }

    // ���� �޼���
    private void Attack()
    {
        canAttack = false; // ���� �� ���� ���� ���θ� false�� ����
        anim.PlayAnimation(4); // ���� �ִϸ��̼� ����

        if (SwordSwing != null)
        {
            SwordSwing.Play();
        }

        // ���� ���� ���� �ִ� �� ����
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, attackRange, Vector2.zero);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Monster")) // "Monster" �±׸� ���� �� ����
            {
                MonsterController monster = hit.collider.GetComponent<MonsterController>();
                if (monster != null)
                {
                    monster.TakeDamage(attackPower); // ������ 10�� ������ ����
                    if (SwordAttack != null)
                    {
                        SwordAttack.Play();
                    }
                }
            }
        }

        StartCoroutine(AttackCooldown()); // 3�� ��ٿ� ����
    }
    // ���� ��ٿ� �ڷ�ƾ
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(1f); // 3�� ���� ���
        canAttack = true; // ���� ���� ���·� ����
    }

    // ü�� ���� �޼���
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    // �÷��̾� ��� ó�� �޼���
    private void Die()
    {
        anim.PlayAnimation(2); // ��� �ִϸ��̼� ����
        canMove = false;
        rb.velocity = Vector2.zero; // �̵� ����
        StartCoroutine(Respawn()); // ��Ȱ �ڷ�ƾ ����
    }

    // ��Ȱ �ڷ�ƾ
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3f); // 3�� ���
        transform.position = spawnPosition; // ���� ��ġ�� �̵�
        health = 100; // ü�� �ʱ�ȭ
        anim.PlayAnimation(0); // Idle �ִϸ��̼����� ����
        canMove = true; // �̵� ���� ���·� ����
    }
}
