using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundController : MonoBehaviour
{
    public RawImage backgroundImage; // ������ ��� �̹��� (RawImage ������Ʈ)
    public float scrollSpeedX = 0.1f; // X�� �̵� �ӵ�
    public float scrollSpeedY = 0.0f; // Y�� �̵� �ӵ�
    public float moveDuration = 2f; // ����� �����̴� �ð�
    public float stopDuration = 1f; // ����� ���ߴ� �ð�

    private float moveTimer = 0f;
    private float stopTimer = 0f;
    private bool isMoving = true; // ����� �����̰� �ִ��� ����

    void Update()
    {
        // ����� ������ ��
        if (isMoving)
        {
            moveTimer += Time.deltaTime;
            if (moveTimer < moveDuration)
            {
                // ����� UV ��ǥ�� �̵�
                backgroundImage.uvRect = new Rect(
                    backgroundImage.uvRect.position + new Vector2(scrollSpeedX, scrollSpeedY) * Time.deltaTime,
                    backgroundImage.uvRect.size
                );
            }
            else
            {
                // ���� �ð��� ������ ���߱� ���·� ��ȯ
                isMoving = false;
                moveTimer = 0f; // Ÿ�̸� �ʱ�ȭ
            }
        }
        else
        {
            // ����� ���� ���� ��
            stopTimer += Time.deltaTime;
            if (stopTimer >= stopDuration)
            {
                // ���ߴ� �ð��� ������ �ٽ� �����̱� ����
                isMoving = true;
                stopTimer = 0f; // Ÿ�̸� �ʱ�ȭ
            }
        }
    }
}