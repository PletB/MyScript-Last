using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FieldSecondTrigger : MonoBehaviour
{
    public TMP_Text dialogueText; // 텍스트를 표시할 TextMeshPro 텍스트
    public GameObject textBox; // 텍스트 박스 오브젝트
    public GameObject blackSphere; // 검은 구체 오브젝트
    public GameObject secondTrigger;
    public QuestFieldPlayer questFieldPlayer; // 플레이어 컨트롤러 참조
    public AudioSource textEffect;
    public float backStepDistance = 1.0f; // 플레이어가 뒤로 이동할 거리
    public float textDisplayDelay = 0.05f; // 한 글자씩 출력되는 딜레이
    private bool hasTriggeredOnce = false; // 첫 번째 트리거 충돌 여부 확인
    private int dialogueIndex = 0;

    private readonly string[] additionalDialogues = new string[]
    {
        "나한테 말 걸어봤자 달라지는건 없어..",
        "아무것도 없다고! 그만해!!",
        "와 미치겠네..? 분명히 말한다. 한번 더 오기만 해?!.",
        "그만! 그만!! 그만!!!",
        "현재 대화가 없습니다.",
        "현재 대화가 없습니다.",
        "현재 대화가 없습니다.",
        "현재 대화가 없습니다.",
        "현재 대화가 없습니다.",
        "(좋아.. 다음 대사가 뭐였지?)",
        "StartCoroutine(DisplayTextWithAnimation(additionalDialogues[dialogueIndex]));",
        "어 잠깐만 이건 내 코드인데, 실수했네..ㅎ",
        "(아무도 못봤겠지..?)",
        "좋아! 내가 졌어. 너도 '그것'을 원해서 온거 맞지? 그렇지?",
        "'그건' 말이지...",
        "저기 있는 놈한테 가. 나 아니야;;",
        "나한테 말 걸어봤자 달라지는건 없어..",
        ".....",
        "이제 진짜 갔겠지?",
        "휴, 내 보물 위치를 들킬 뻔했네..",
        "이 어딘가에 나의 보물이 있다는 것을 모를거야 ㅎㅎㅎㅎㅎ",
        "이제 진짜 대화 없으니까 제발 꺼져! 가서 퀘스트나 받아!!"
    };

    private void Start()
    {
        // 텍스트 박스와 텍스트를 비활성화
        if (textBox != null)
        {
            textBox.SetActive(false);
        }
        if (dialogueText != null)
        {
            dialogueText.gameObject.SetActive(false);
        }
        if (blackSphere != null)
        {
            blackSphere.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어 이동 비활성화
            if (questFieldPlayer != null)
            {
                if (questFieldPlayer.rb != null)
                {
                    questFieldPlayer.rb.velocity = Vector2.zero; // 속도를 0으로 설정하여 즉각적으로 멈춤
                    questFieldPlayer.rb.angularVelocity = 0f; // 회전 속도도 0으로 설정
                }
                questFieldPlayer.canMove = false;
                questFieldPlayer.anim.PlayAnimation(0);
            }

            // 텍스트 박스와 텍스트 활성화 및 텍스트 내용 설정
            if (textBox != null)
            {
                textBox.SetActive(true);
            }
            if (dialogueText != null)
            {
                dialogueText.gameObject.SetActive(true);

                // 첫 번째로 트리거에 닿았을 때
                if (!hasTriggeredOnce)
                {
                    StartCoroutine(DisplayTextWithAnimation("저기 있는 놈한테 가. 나 아니야;;"));
                    hasTriggeredOnce = true; // 첫 번째 트리거 처리 완료
                }
                else
                {
                    // 추가 대사 출력
                    if (dialogueIndex < additionalDialogues.Length)
                    {
                        StartCoroutine(DisplayTextWithAnimation(additionalDialogues[dialogueIndex]));
                        dialogueIndex++;
                    }
                }
            }
        }
    }

    private IEnumerator DisplayTextWithAnimation(string message)
    {
        dialogueText.text = ""; // 텍스트 초기화

        // 한 글자씩 출력
        foreach (char letter in message)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textDisplayDelay); // 한 글자 출력 후 딜레이
            // 효과음 재생                                                   
            if (textEffect != null)
            {
                textEffect.Play();
            }
        }

        // 모든 텍스트가 출력된 후 1.5초 대기
        yield return new WaitForSeconds(1.5f);

        // 마지막 텍스트가 출력된 후 2초 대기하고 텍스트, 텍스트 박스, 검은 구체 비활성화
        if (message == "이제 진짜 대화 없으니까 제발 꺼져! 가서 퀘스트나 받아!!")
        {
            yield return new WaitForSeconds(1.5f);
            if (textBox != null)
            {
                textBox.SetActive(false);
            }
            if (dialogueText != null)
            {
                dialogueText.gameObject.SetActive(false);
            }
            if (blackSphere != null)
            {
                blackSphere.SetActive(false);
            }
            questFieldPlayer.canMove = true; // 플레이어 이동 가능하게 설정
            secondTrigger.SetActive(false);

        }
        else
        {
            // 마지막 텍스트가 아닌 경우 2초 뒤에 플레이어를 1칸 뒤로 이동
            StartCoroutine(DelayedMoveBack());
        }
    }

    private IEnumerator DelayedMoveBack()
    {
        yield return new WaitForSeconds(0f);

        // 플레이어를 1칸 뒤로 이동
        Vector3 backPosition = questFieldPlayer.transform.position + new Vector3(backStepDistance, 0, 0);
        questFieldPlayer.transform.position = backPosition;

        // 텍스트 박스와 텍스트 비활성화
        if (textBox != null)
        {
            textBox.SetActive(false);
        }
        if (dialogueText != null)
        {
            dialogueText.gameObject.SetActive(false);
            questFieldPlayer.canMove = true;
        }
    }
}
