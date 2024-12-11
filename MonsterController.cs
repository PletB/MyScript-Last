using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public float speed = 1f; // 이동 속도
    public float moveDistance = 1f; // 좌우 이동 거리
    public float chaseRange = 2.5f; // 플레이어를 추적할 범위
    public RectTransform player; // 플레이어의 Transform
    public AudioSource PlayerDamage;
    public int health = 30; // 몬스터의 체력
    public int attackPower = 10; // 몬스터의 공격력

    private Vector3 startingPosition; // 시작 위치
    private bool movingRight = true; // 오른쪽으로 이동 중인지 여부
    private bool isPlayerInContact = false; // 플레이어와의 충돌 여부 확인용 변수
    private Coroutine damageCoroutine; // 데미지 코루틴을 저장할 변수

    private void Start()
    {
        // 몬스터의 시작 위치 저장
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

    // 몬스터 자동 이동 로직
    private void MoveMonster()
    {
        // 이동 방향 설정
        float moveDirection = movingRight ? 1 : -1;

        // 현재 위치에서 이동 속도와 방향에 따라 새로운 위치 계산
        transform.Translate(Vector3.right * moveDirection * speed * Time.deltaTime);

        // 이동 범위에 도달했을 경우 방향 변경
        if (movingRight && transform.position.x >= startingPosition.x + moveDistance)
        {
            movingRight = false; // 왼쪽으로 이동
        }
        else if (!movingRight && transform.position.x <= startingPosition.x - moveDistance)
        {
            movingRight = true; // 오른쪽으로 이동
        }

        // 일정 확률로 방향 변경
        if (Random.Range(0f, 1f) < 0.01f) // 1% 확률로 방향 변경
        {
            movingRight = !movingRight; // 방향 반전
        }
    }

    // 플레이어 추적 로직
    private void ChasePlayer()
    {
        // 플레이어 방향으로 이동
        Vector3 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime);
    }

    // 플레이어가 추적 범위 내에 있는지 확인
    private bool IsPlayerInRange()
    {
        return Vector3.Distance(transform.position, player.position) <= chaseRange;
    }

    // 플레이어와의 충돌 시 데미지 처리
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player")) // 플레이어와 충돌했는지 확인
        {
            QuestFieldPlayer playerScript = collision.collider.GetComponent<QuestFieldPlayer>();
            if (playerScript != null && !isPlayerInContact) // 처음 충돌 시
            {
                isPlayerInContact = true; // 충돌 상태를 true로 설정
                damageCoroutine = StartCoroutine(DealDamageOverTime(playerScript)); // 데미지 코루틴 시작
            }
        }
    }

    // 플레이어와의 충돌 종료 시 코루틴 중지
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player")) // 플레이어와의 충돌이 끝났는지 확인
        {
            isPlayerInContact = false; // 충돌 상태를 false로 설정
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine); // 데미지 코루틴 중지
                damageCoroutine = null;
            }
        }
    }

    // 일정 시간마다 데미지를 주는 코루틴
    private IEnumerator DealDamageOverTime(QuestFieldPlayer playerScript)
    {
        while (isPlayerInContact) // 충돌 상태일 때만 반복
        {
            if (playerScript.health > 0) // 플레이어 체력이 0 이상인지 확인
            {
                playerScript.TakeDamage(attackPower); // 플레이어에게 데미지를 줌
                if (PlayerDamage != null)
                {
                    PlayerDamage.Play();
                }
            }
            yield return new WaitForSeconds(2f); // 2초 대기
        }
    }

    // 체력 감소 메서드 (예: 플레이어 공격으로 인한 데미지 처리)
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            StartCoroutine(Die()); // 사망 후 리스폰 코루틴 호출
        }
    }

    // Die 메서드를 IEnumerator로 변경하여 코루틴으로 실행되도록 설정
    private IEnumerator Die()
    {
        FieldFiveTrigger questTrigger = FindObjectOfType<FieldFiveTrigger>();
        if (questTrigger != null)
        {
            questTrigger.MonsterCaptured(); // 몬스터 처치 시 퀘스트 진행
        }

        // 몬스터를 비활성화하고, 2초 뒤 리스폰을 시작하도록 RespawnMonster 코루틴 호출
        GetComponent<SpriteRenderer>().enabled = false; // 시각적으로 비활성화
        GetComponent<Collider2D>().enabled = false; // 충돌 비활성화

        yield return new WaitForSeconds(0f); // 2초 대기 후 리스폰

        // 리스폰 코루틴 시작
        StartCoroutine(RespawnMonster());
    }

    // 리스폰 코루틴
    private IEnumerator RespawnMonster()
    {
        // 시각적 및 물리적 비활성화
        GetComponent<SpriteRenderer>().enabled = false; // 시각적으로 비활성화
        GetComponent<Collider2D>().enabled = false; // 충돌 비활성화

        yield return new WaitForSeconds(2f); // 2초 대기 후 리스폰

        // 랜덤한 위치로 이동하여 리스폰
        Vector3 randomPosition = startingPosition + new Vector3(Random.Range(-moveDistance, moveDistance), 0, 0);
        transform.position = randomPosition;
        health = 30; // 체력 초기화

        // 몬스터를 다시 활성화
        GetComponent<SpriteRenderer>().enabled = true; // 시각적으로 활성화
        GetComponent<Collider2D>().enabled = true; // 충돌 활성화
    }

    // 몬스터 비활성화 메서드
    public void DisableMonster()
    {
        GetComponent<SpriteRenderer>().enabled = false; // 시각적으로 비활성화
        GetComponent<Collider2D>().enabled = false; // 충돌 비활성화
        StopAllCoroutines(); // 모든 코루틴 정지 (리스폰 및 데미지 코루틴 포함)
    }
}
