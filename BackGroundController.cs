using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundController : MonoBehaviour
{
    public RawImage backgroundImage; // 움직일 배경 이미지 (RawImage 컴포넌트)
    public float scrollSpeedX = 0.1f; // X축 이동 속도
    public float scrollSpeedY = 0.0f; // Y축 이동 속도
    public float moveDuration = 2f; // 배경이 움직이는 시간
    public float stopDuration = 1f; // 배경이 멈추는 시간

    private float moveTimer = 0f;
    private float stopTimer = 0f;
    private bool isMoving = true; // 배경이 움직이고 있는지 여부

    void Update()
    {
        // 배경이 움직일 때
        if (isMoving)
        {
            moveTimer += Time.deltaTime;
            if (moveTimer < moveDuration)
            {
                // 배경의 UV 좌표를 이동
                backgroundImage.uvRect = new Rect(
                    backgroundImage.uvRect.position + new Vector2(scrollSpeedX, scrollSpeedY) * Time.deltaTime,
                    backgroundImage.uvRect.size
                );
            }
            else
            {
                // 일정 시간이 지나면 멈추기 상태로 전환
                isMoving = false;
                moveTimer = 0f; // 타이머 초기화
            }
        }
        else
        {
            // 배경이 멈춰 있을 때
            stopTimer += Time.deltaTime;
            if (stopTimer >= stopDuration)
            {
                // 멈추는 시간이 끝나면 다시 움직이기 시작
                isMoving = true;
                stopTimer = 0f; // 타이머 초기화
            }
        }
    }
}