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
    public int attackPower = 10; // 공격력 설정
    public float attackRange = 1.5f; // 공격 범위
    public int health = 100; // 초기 체력

    public SPUM_Prefabs anim; // 애니메이션 참조
    public bool canMove = false;
    private bool canAttack = true; // 공격 가능 여부
    public Rigidbody2D rb;
    public AudioSource quesFieldtArea;
    public AudioSource SwordSwing;
    public AudioSource SwordAttack;
    private QuestFieldFadeIn fadeInManager;
    private MoveCamera MoveCamera;
    private Vector3 originalScale;
    private Vector3 spawnPosition; // 시작 위치 저장

    private void Start()
    {
        MoveCamera = FindObjectOfType<MoveCamera>();
        originalScale = transform.localScale;
        rb = GetComponent<Rigidbody2D>();
        fadeInManager = FindObjectOfType<QuestFieldFadeIn>();
        spawnPosition = transform.position; // 처음 시작 위치를 저장

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

            // 5번 키 입력 확인 및 공격 가능 여부 확인
            if (Input.GetKeyDown(KeyCode.Alpha5) && canAttack) // Alpha5는 숫자 키 5를 의미합니다.
            {
                Attack();
            }

            // 공격 애니메이션이 끝났을 때 트리거 해제
            if (anim._anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") &&
                anim._anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                anim._anim.ResetTrigger("Attack");
            }
            // 사망 애니메이션이 끝났을 때 트리거 해제
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
            // 이동 방향에 맞게 방향 전환
            transform.localScale = new Vector3(Mathf.Sign(moveX) * Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            anim.PlayAnimation(1); // 걷기 애니메이션 실행
        }
        else
        {
            anim.PlayAnimation(0); // 정지 애니메이션 실행
        }
    }

    // 공격 메서드
    private void Attack()
    {
        canAttack = false; // 공격 후 공격 가능 여부를 false로 설정
        anim.PlayAnimation(4); // 공격 애니메이션 실행

        if (SwordSwing != null)
        {
            SwordSwing.Play();
        }

        // 공격 범위 내에 있는 적 감지
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, attackRange, Vector2.zero);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Monster")) // "Monster" 태그를 가진 적 감지
            {
                MonsterController monster = hit.collider.GetComponent<MonsterController>();
                if (monster != null)
                {
                    monster.TakeDamage(attackPower); // 적에게 10의 데미지 입힘
                    if (SwordAttack != null)
                    {
                        SwordAttack.Play();
                    }
                }
            }
        }

        StartCoroutine(AttackCooldown()); // 3초 쿨다운 시작
    }
    // 공격 쿨다운 코루틴
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(1f); // 3초 동안 대기
        canAttack = true; // 공격 가능 상태로 복구
    }

    // 체력 감소 메서드
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    // 플레이어 사망 처리 메서드
    private void Die()
    {
        anim.PlayAnimation(2); // 사망 애니메이션 실행
        canMove = false;
        rb.velocity = Vector2.zero; // 이동 멈춤
        StartCoroutine(Respawn()); // 부활 코루틴 시작
    }

    // 부활 코루틴
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3f); // 3초 대기
        transform.position = spawnPosition; // 시작 위치로 이동
        health = 100; // 체력 초기화
        anim.PlayAnimation(0); // Idle 애니메이션으로 복귀
        canMove = true; // 이동 가능 상태로 복구
    }
}
