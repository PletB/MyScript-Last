using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour 
{
    public RectTransform playerRect; // 플레이어의 Transform
    public float cameraFollowSpeed = 2.0f; // 카메라가 따라가는 속도
    public float followThreshold = 0.5f; // 화면 비율 절반 지점
    public LayerMask wallLayer; // 벽과 충돌을 감지할 레이어

    public Transform targetToZoom; // 확대할 대상 (Monster1 오브젝트 등)
    public float zoomDuration = 1.0f; // 카메라 줌 인/아웃 지속 시간
    public float zoomSize = 3.0f; // 확대할 때의 카메라 크기

    private Camera mainCamera;
    private Vector3 originalCameraPosition;
    private float originalCameraSize;
    private bool isCameraBlocked = false; // 카메라가 벽에 의해 막혔는지 여부
    private bool isRightBlocked = false; // 오른쪽 벽에 닿았는지 여부
    private bool isFollowingPlayer = true; // 플레이어를 따라가는 상태인지 확인하는 변수

    void Start()
    {
        mainCamera = Camera.main;
        originalCameraPosition = mainCamera.transform.position;
        originalCameraSize = mainCamera.orthographicSize;
    }

    // QuestCameraController 스크립트의 코드 추가
    public void StartFollowingPlayer()
    {
        isFollowingPlayer = true;
        FollowPlayer(); // 첫 호출 시 한번 업데이트를 실행하도록 보장
    }

    // 플레이어를 따라가는 로직
    public void FollowPlayer()
    {
        if (!isFollowingPlayer || playerRect == null) return;

        // 벽과의 충돌 감지
        if (IsCameraBlockedByWall())
        {
            isCameraBlocked = true;
        }

        // 플레이어의 화면 위치 계산
        Vector3 playerScreenPos = RectTransformUtility.WorldToScreenPoint(mainCamera, playerRect.position);
        Vector3 viewportPos = mainCamera.ScreenToViewportPoint(playerScreenPos);

        // 오른쪽 벽에 닿은 상태라면 오른쪽으로 이동하지 않도록 제어
        if (isRightBlocked && viewportPos.x > 0.5f)
        {
            return; // 오른쪽 벽에 닿은 상태에서 오른쪽 이동을 막음
        }

        // 왼쪽 벽에 막혔을 때, 플레이어가 화면의 절반 이상에 도달하면 카메라가 다시 따라가도록 설정
        if (isCameraBlocked && viewportPos.x > followThreshold)
        {
            isCameraBlocked = false;
        }

        // 카메라가 벽에 막혀있지 않으면 플레이어를 따라가도록 설정
        if (!isCameraBlocked && (viewportPos.x >= followThreshold || viewportPos.x <= (1 - followThreshold)))
        {
            Vector3 targetPosition = new Vector3(playerRect.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z);
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, Time.deltaTime * cameraFollowSpeed);
        }
    }

    // 벽과의 충돌 감지 로직
    private bool IsCameraBlockedByWall()
    {
        float cameraHalfHeight = mainCamera.orthographicSize;
        float cameraHalfWidth = cameraHalfHeight * mainCamera.aspect;

        Vector2 leftRayOrigin = new Vector2(mainCamera.transform.position.x - cameraHalfWidth, mainCamera.transform.position.y);
        Vector2 rightRayOrigin = new Vector2(mainCamera.transform.position.x + cameraHalfWidth, mainCamera.transform.position.y);

        // 왼쪽과 오른쪽으로 레이캐스트 쏘기
        RaycastHit2D hitLeft = Physics2D.Raycast(leftRayOrigin, Vector2.left, 0.1f, wallLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(rightRayOrigin, Vector2.right, 0.1f, wallLayer);

        // 왼쪽과 오른쪽 각각의 충돌 감지
        isRightBlocked = hitRight.collider != null;
        return hitLeft.collider != null;
    }

    // 특정 대상(Monster1)으로 카메라를 확대하고 1초 동안 유지 후 원래 위치로 돌아오는 메서드
    public void ZoomInOnTarget()
    {
        if (mainCamera != null && targetToZoom != null)
        {
            isFollowingPlayer = false; // 줌 인 중에는 플레이어를 따라가지 않음
            StartCoroutine(ZoomInAndOutCoroutine());
        }
    }

    private IEnumerator ZoomInAndOutCoroutine()
    {
        Vector3 targetPosition = new Vector3(targetToZoom.position.x, targetToZoom.position.y, mainCamera.transform.position.z);
        float elapsedTime = 0f;

        // 카메라 확대
        while (elapsedTime < zoomDuration)
        {
            mainCamera.transform.position = Vector3.Lerp(originalCameraPosition, targetPosition, elapsedTime / zoomDuration);
            mainCamera.orthographicSize = Mathf.Lerp(originalCameraSize, zoomSize, elapsedTime / zoomDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = targetPosition;
        mainCamera.orthographicSize = zoomSize;

        // 1초 대기
        yield return new WaitForSeconds(1f);

        // 카메라 복귀
        elapsedTime = 0f;
        while (elapsedTime < zoomDuration)
        {
            mainCamera.transform.position = Vector3.Lerp(targetPosition, originalCameraPosition, elapsedTime / zoomDuration);
            mainCamera.orthographicSize = Mathf.Lerp(zoomSize, originalCameraSize, elapsedTime / zoomDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = originalCameraPosition;
        mainCamera.orthographicSize = originalCameraSize;

        isFollowingPlayer = true; // 줌 인/아웃이 끝나면 다시 플레이어를 따라감
    }
}
