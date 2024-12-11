using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FieldThirdTrigger : MonoBehaviour
{
    public Transform fourthTriggerLocation; // 네 번째 트리거의 위치 참조
    public QuestFieldPlayer questFieldPlayer; // 플레이어 컨트롤러 참조
    public TMP_Text teleportText; // 텔레포트 안내 텍스트 (TextMeshPro 텍스트)
    public AudioSource teleportEffect; // 텔레포트 효과음
    public AudioSource SignBoardEffect; // 텔레포트 효과음
    public AudioSource SignBoardClose; // 텔레포트 효과음

    private bool canTeleport = false; // 텔레포트 가능 여부 확인

    private void Start()
    {
        // 텍스트 비활성화
        if (teleportText != null)
        {
            teleportText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 트리거에 닿았을 때
        if (other.CompareTag("Player") && questFieldPlayer != null && fourthTriggerLocation != null)
        {
            // 텔레포트 안내 텍스트 활성화
            if (teleportText != null)
            {
                teleportText.gameObject.SetActive(true);
                teleportText.text = "방향키: 윗 키를 누르세요.";
                // 효과음 재생
                if (SignBoardEffect != null)
                {
                    SignBoardEffect.Play();
                }
            }

            canTeleport = true; // 텔레포트 가능 상태로 설정
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 플레이어가 트리거를 벗어났을 때
        if (other.CompareTag("Player"))
        {
            // 텍스트 비활성화
            if (teleportText != null)
            {
                teleportText.gameObject.SetActive(false);
                // 효과음 재생
                if (SignBoardClose != null)
                {
                    SignBoardClose.Play();
                }
            }

            canTeleport = false; // 텔레포트 불가능 상태로 설정
        }
    }

    private void Update()
    {
        // 텔레포트가 가능할 때 윗 방향키 입력 시 네 번째 트리거 위치로 이동
        if (canTeleport && Input.GetKeyDown(KeyCode.UpArrow))
        {
            questFieldPlayer.transform.position = fourthTriggerLocation.position;
            // 효과음 재생
            if (teleportEffect != null)
            {
                teleportEffect.Play();
            }

            // 텍스트 비활성화
            if (teleportText != null)
            {
                teleportText.gameObject.SetActive(false);
            }

            canTeleport = false; // 텔레포트 상태 초기화
        }
    }
}
