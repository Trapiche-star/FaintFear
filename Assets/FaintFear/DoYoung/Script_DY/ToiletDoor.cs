using UnityEngine;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// 하나의 도어 프레임에 여러 개의 화장실 칸 문을 제어하는 상호작용 도어
    /// </summary>
    public class ToiletDoor : Interactive, IActionProvider
    {
        #region Variables

        [Header("Door Hinges")]
        [SerializeField] private Transform[] doorHinges; // 각 화장실 칸 문의 힌지 트랜스폼 배열

        [Header("Open Angles")]
        [SerializeField] private float openAngle = -90f; // 문이 열릴 때 회전 각도

        [Header("UI")]
        [SerializeField] private SequenceTextManager sequenceText; // 텍스트 출력과 시퀀스를 담당

        private bool[] doorStates;     // 각 문이 열려있는지 여부
        private bool isMoving = false; // 문 애니메이션 진행 여부

        #endregion


        #region Unity Event Method

        // 문 상태 배열을 초기화한다
        private void Awake()
        {
            if (doorHinges == null || doorHinges.Length == 0)
                return; // 만약 [문 힌지가 하나도 지정되지 않았다면] [초기화를 진행하지 않는다]

            doorStates = new bool[doorHinges.Length];
            // 문 개수에 맞춰 상태 배열을 생성한다
        }

        #endregion


        #region Custom Method

        // 플레이어 상호작용을 처리한다
        public override void Interaction()
        {
            if (isMoving) return;
            // 만약 [문이 현재 움직이는 중이면] [중복 상호작용을 차단한다]

            int doorIndex = GetTargetDoorIndex();
            if (doorIndex < 0) return;
            // 만약 [어느 문도 감지되지 않았다면] [처리를 중단한다]

            StartCoroutine(MoveDoorRoutine(doorIndex));
            // 감지된 문 하나만 열거나 닫는 애니메이션을 실행한다
        }

        // 레이캐스트로 맞은 문이 몇 번째 문인지 계산한다
        private int GetTargetDoorIndex()
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;

            if (!Physics.Raycast(ray, out hit, 3f))
                return -1; // 만약 [레이캐스트가 아무 것도 맞추지 못했다면] [유효하지 않은 인덱스를 반환한다]

            for (int i = 0; i < doorHinges.Length; i++)
            {
                if (hit.transform == doorHinges[i])
                    return i; // 만약 [맞은 오브젝트가 해당 문 힌지라면] [그 인덱스를 반환한다]
            }

            return -1;
            // 어느 문에도 해당하지 않으면 유효하지 않은 인덱스를 반환한다
        }

        // 특정 문 하나를 열거나 닫는 코루틴
        private IEnumerator MoveDoorRoutine(int index)
        {
            isMoving = true;
            // 문 이동 중 상태로 설정한다

            float duration = 1.0f;
            float elapsed = 0f;

            Transform hinge = doorHinges[index];
            // 상호작용한 문의 힌지를 참조한다

            Quaternion startRot = hinge.localRotation;
            // 문의 시작 회전값을 저장한다

            Quaternion targetRot =
                Quaternion.Euler(0f, doorStates[index] ? 0f : openAngle, 0f);
            // 현재 상태에 따라 목표 회전값을 계산한다

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                hinge.localRotation =
                    Quaternion.Lerp(startRot, targetRot, elapsed / duration);
                // 지정된 시간 동안 문을 부드럽게 회전시킨다

                yield return null;
                // 다음 프레임까지 대기한다
            }

            hinge.localRotation = targetRot;
            // 최종 회전값을 보정한다

            doorStates[index] = !doorStates[index];
            // 해당 문의 열림 상태를 반전시킨다

            isMoving = false;
            // 문 이동 완료 상태로 설정한다
        }

        // HUD 메시지를 출력한다
        private void ShowHUDMessage(string message)
        {
            if (sequenceText == null) return;
            // 만약 [텍스트 매니저가 존재하지 않으면] [출력을 중단한다]

            sequenceText.ShowMessage(message);
        }

        #endregion


        #region Property

        // Action UI에 표시할 문구를 제공한다
        public string GetActionText()
        {
            return "문 열기";
            // 화장실 칸 문은 항상 동일한 액션 텍스트를 제공한다
        }

        #endregion
    }
}
