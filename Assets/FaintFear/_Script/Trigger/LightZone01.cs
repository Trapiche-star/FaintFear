using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 트리거 진입 시에만 조명을 켜는 구역 컨트롤러
    /// 이후에는 트리거 이탈로 조명이 꺼지지 않으며, 외부 이벤트에서만 소등을 제어한다
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class LightZone01 : MonoBehaviour
    {
        #region Variables

        [Header("Light Objects")]
        [SerializeField] private GameObject allPointLights;     // 포인트 라이트 묶음 오브젝트
        [SerializeField] private GameObject allEmissiveObjects; // 에미션 대상 렌더러 묶음

        [Header("Light Settings")]
        [SerializeField] private float activeRange = 8f;        // 점등 시 라이트 범위
        [SerializeField] private float inactiveRange = 0f;      // 소등 시 라이트 범위
        [SerializeField] private float triggerLightDistance = 15f; // 트리거 중심 기준 활성 거리

        private Light[] lights;                 // 제어 대상 라이트 배열
        private Renderer[] emissiveRenderers;   // 제어 대상 에미션 렌더러 배열
        private BoxCollider box;                // 트리거 콜라이더

        private bool lightsPermanentlyOff = false; // 영구 소등 상태 여부
        private bool hasBeenActivated = false;     // 트리거로 한 번이라도 켜졌는지 여부

        #endregion


        #region Unity Event Method

        // 초기 참조 설정 및 기본 상태 구성
        private void Awake()
        {
            box = GetComponent<BoxCollider>();        // 트리거 콜라이더 참조 확보
            box.isTrigger = true;                    // 물리 충돌이 아닌 트리거로 설정

            if (allPointLights != null)
                lights = allPointLights.GetComponentsInChildren<Light>(true); // 자식 라이트 수집

            if (allEmissiveObjects != null)
                emissiveRenderers = allEmissiveObjects.GetComponentsInChildren<Renderer>(true); // 자식 렌더러 수집

            ForceDisableAllEmission();                // 시작 시 모든 에미션 강제 OFF
        }

        // 저장 데이터 및 튜토리얼 상태에 따른 초기 점등 처리
        private void Start()
        {
            var data = SaveSystem.LoadPreview();      // 세이브 데이터 로드

            if (data != null && data.lightsPermaOff)
            {
                lightsPermanentlyOff = true;         // 만약 저장 상태가 영구 소등이면
                SetLightsActive(false);              // 즉시 소등
                enabled = false;                     // 이후 로직 비활성화
                return;                              // 더 이상 초기화 진행하지 않는다
            }

            bool tutorialCompleted =
                (data != null && data.tutorialCompleted) || GameManager.TutorialCompleted;

            if (!tutorialCompleted)
            {
                hasBeenActivated = true;             // 튜토리얼 미완료 시 시작부터 점등 상태
                SetLightsActive(true);               // 조명 활성화
            }
        }

        // 플레이어가 트리거에 들어왔을 때만 조명 ON
        private void OnTriggerEnter(Collider other)
        {
            if (lightsPermanentlyOff) return;         // 만약 영구 소등 상태라면 아무 것도 하지 않는다
            if (!other.CompareTag("Player")) return; // 만약 플레이어가 아니라면 무시한다
            if (hasBeenActivated) return;             // 만약 이미 한 번 켜졌다면 재처리하지 않는다

            hasBeenActivated = true;                 // 최초 진입 처리
            SetLightsActive(true);                   // 조명 ON
        }

        // OnTriggerExit 없음: 나가도 절대 소등되지 않도록 의도적으로 제거

        #endregion


        #region Custom Method

        // 라이트 및 에미션을 상태에 따라 활성/비활성
        public void SetLightsActive(bool state)
        {
            if (lightsPermanentlyOff && state) return; // 만약 영구 소등인데 켜려 한다면 무시한다

            Vector3 center = transform.position;       // 거리 계산 기준점

            if (lights != null)
            {
                foreach (Light l in lights)
                {
                    if (l == null) continue;           // 만약 라이트 참조가 없으면 건너뛴다

                    float dist = Vector3.Distance(l.transform.position, center);
                    bool enable = state && dist <= triggerLightDistance; // 만약 ON 상태이면서 거리 조건을 만족하면 활성

                    l.enabled = enable;                // 라이트 활성화 여부 적용
                    l.range = enable ? activeRange : inactiveRange; // 상태에 따른 범위 설정
                }
            }

            SetEmissionState(state, center);           // 에미션 상태 동기화
        }

        // 모든 에미션을 강제로 비활성화
        private void ForceDisableAllEmission()
        {
            if (emissiveRenderers == null) return;     // 만약 렌더러가 없다면 처리하지 않는다

            foreach (Renderer r in emissiveRenderers)
            {
                foreach (Material mat in r.materials)
                {
                    mat.DisableKeyword("_EMISSION");  // 에미션 키워드 비활성화
                    mat.SetColor("_EmissionColor", Color.black); // 발광 색상 제거
                }
            }
        }

        // 거리 조건에 따라 에미션을 활성/비활성
        private void SetEmissionState(bool state, Vector3 center)
        {
            if (emissiveRenderers == null) return;     // 만약 렌더러가 없다면 처리하지 않는다

            foreach (Renderer r in emissiveRenderers)
            {
                float dist = Vector3.Distance(r.transform.position, center);

                foreach (Material mat in r.materials)
                {
                    if (state && dist <= triggerLightDistance)
                    {
                        mat.EnableKeyword("_EMISSION");                       // 만약 ON 상태이고 거리 조건을 만족하면 에미션 활성
                        mat.SetColor("_EmissionColor", Color.white * 1.5f);  // 발광 색상 적용
                    }
                    else
                    {
                        mat.DisableKeyword("_EMISSION");                     // 그렇지 않으면 에미션 비활성
                        mat.SetColor("_EmissionColor", Color.black);         // 발광 색상 제거
                    }
                }
            }
        }

        // 외부 이벤트에서 영구 소등 처리
        public void SetPermanentlyOff()
        {
            lightsPermanentlyOff = true;               // 영구 소등 플래그 설정
            SetLightsActive(false);                    // 즉시 소등 적용
        }

        #endregion
    }
}
