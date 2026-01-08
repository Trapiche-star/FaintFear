using FaintFear;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Cabinet : Interactive
{
    private Transform leftHinge;
    private Transform rightHinge;

    bool isMoving = false; // 문이 움직이는 중인지 확인
    bool isOpen = false;   // 문이 현재 열려있는지 상태 확인 (true: 열림, false: 닫힘)

    private void Awake()
    {
        leftHinge = transform.GetChild(2);
        rightHinge = transform.GetChild(3);
    }



    public override void Interaction()
    {
        // 문이 움직이는 중이라면 입력 무시
        if (isMoving) return;

        if (!isOpen)
        {
            // 닫혀있으면 -> 연다 (목표 각도 -90도)
            Debug.Log("문 여는 중");
            StartCoroutine(MoveCabinetRoutine(-180f, 180f));

            //+ 문 여는 사운드 재생
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX("SFX_CabinetOpen");

        }
        else
        {
            // 열려있으면 -> 닫는다 (목표 각도 0도)
            Debug.Log("문 닫는 중");
            StartCoroutine(MoveCabinetRoutine(-0f, 0f));

            //+ 문 닫는 사운드 재생
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX("SFX_CabinetClose");
        }

        // 상태 반전 (열림 <-> 닫힘)
        isOpen = !isOpen;
    }

    IEnumerator MoveCabinetRoutine(float targetLeftAngle, float targetRightAngle)
    {
        isMoving = true; // 움직임 시작

        float duration = 1.0f;
        float elapsedTime = 0f;

        // 시작 회전값
        Quaternion startLeftRotation = leftHinge.localRotation;
        Quaternion startRightRotation = rightHinge.localRotation;
        // 목표 회전값 (인자로 받은 targetAngle 사용)
        Quaternion targetLeftRotation = Quaternion.Euler(0, targetLeftAngle, 0);
        Quaternion targetRightRotation = Quaternion.Euler(0, targetRightAngle, 0);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // 선형 보간으로 부드럽게 회전
            leftHinge.localRotation = Quaternion.Lerp(startLeftRotation, targetLeftRotation, t);
            rightHinge.localRotation = Quaternion.Lerp(startRightRotation, targetRightRotation, t);

            yield return null;
        }

        // 최종 각도 확실하게 고정
        leftHinge.localRotation = targetLeftRotation;
        rightHinge.localRotation = targetRightRotation;

        isMoving = false; // 움직임 종료
    }
}
