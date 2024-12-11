using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FieldFirstTrigger : MonoBehaviour
{
    public TMP_Text questText; // 퀘스트 안내 텍스트 (TextMeshPro 텍스트)
    public AudioSource SignBoardOpen; // 표지판 텍스트 출력 효과음
    public AudioSource SignBoardClose; // 표지판 텍스트 비활성화ㄴ 효과음

    private void Start()
    {
        // 퀘스트 텍스트 비활성화 (시작 시)
        if (questText != null)
        {
            questText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 트리거에 들어올 때 텍스트 활성화
        if (other.CompareTag("Player") && questText != null)
        {
            questText.gameObject.SetActive(true);
            questText.text = "퀘스트 받는 곳->";
            // 효과음 재생
            if (SignBoardOpen != null)
            {
                SignBoardOpen.Play();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 플레이어가 트리거를 벗어날 때 텍스트 비활성화
        if (other.CompareTag("Player") && questText != null)
        {
            questText.gameObject.SetActive(false);
            // 효과음 재생
            if (SignBoardClose != null)
            {
                SignBoardClose.Play();
            }
        }
    }
}
