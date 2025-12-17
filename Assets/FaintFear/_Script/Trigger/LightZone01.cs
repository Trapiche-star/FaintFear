using UnityEngine;
using UnityEngine.Rendering; // Emission 체크박스 제어용

namespace FaintFear
{
    // LightZone01
    // 트리거 안에 있을 때만 해당 구역의 라이트와 메테리얼 에미션이 켜질 수 있고,
    // 트리거 밖(또는 한 번 꺼진 이후)에는 항상 꺼진 상태를 유지한다.
    public class LightZone01 : MonoBehaviour
    {
        [Header("건물 내부 모든 Point Light 부모 오브젝트")]
        public GameObject allPointLights; // 모든 Light 컴포넌트를 가진 부모

        [Header("에미션 제어 대상 (램프 / 네온 / 샹들리에 부모 오브젝트)")]
        public GameObject allEmissiveObjects; // 발광 Mesh가 포함된 부모

        [Header("트리거 안일 때 Range")]
        public float activeRange = 8f;  // 라이트가 켜졌을 때의 Range

        [Header("트리거 밖일 때 Range (보통 0)")]
        public float inactiveRange = 0f; // 라이트가 꺼졌을 때의 Range

        [Header("라이트가 켜질 수 있는 최대 거리 (트리거 중심 기준)")]
        public float triggerLightDistance = 15f; // 이 거리 안에 있는 라이트/에미션만 켜짐

        private Light[] lights;                // 트리거 관리 대상 라이트들
        private Renderer[] emissiveRenderers;  // 트리거 관리 대상 발광 Mesh들

        // 한 번 OFF된 이후 다시 켜지지 않도록 제어하는 플래그
        private bool lightsPermanentlyOff = false;

        // ──────────────────────────────────────────────
        // Awake()
        // 자식 라이트 / 렌더러 캐싱만 수행
        // ──────────────────────────────────────────────
        void Awake()
        {
            if (allPointLights != null)
                lights = allPointLights.GetComponentsInChildren<Light>(true);

            if (allEmissiveObjects != null)
                emissiveRenderers = allEmissiveObjects.GetComponentsInChildren<Renderer>(true);
        }

        // ──────────────────────────────────────────────
        // Start()
        // 씬 시작 시, 플레이어가 트리거 안에 있으면
        // 트리거 중심 기준으로 일정 거리 안에 있는 라이트/에미션만 켜고,
        // 밖에 있는 것들은 꺼진 상태를 유지한다.
        // ──────────────────────────────────────────────
        void Start()
        {
            if (lights == null && emissiveRenderers == null) return;

            bool playerInside = false;

            // 트리거 영역 내 플레이어 존재 여부 검사
            Collider[] colliders = Physics.OverlapBox(
                transform.position,
                GetComponent<BoxCollider>().size / 2,
                transform.rotation
            );

            foreach (Collider col in colliders)
            {
                if (col.CompareTag("Player"))
                {
                    playerInside = true;
                    break;
                }
            }

            // 플레이어가 안에 있으면 켜기, 아니면 끄기
            if (playerInside)
            {
                SetLightsActive(true);
            }
            else
            {
                SetLightsActive(false);
            }
        }

        // ──────────────────────────────────────────────
        // OnTriggerExit()
        // 플레이어가 트리거 밖으로 나갔을 때
        // 해당 구역 라이트/에미션을 끄고,
        // 다시는 켜지지 않도록 플래그 설정
        // ──────────────────────────────────────────────
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                SetLightsActive(false);
                lightsPermanentlyOff = true; // 이후 state = true 요청은 무시
            }
        }

        // ──────────────────────────────────────────────
        // SetLightsActive()
        // 라이트와 에미션의 실제 상태를 제어하는 메서드
        // ──────────────────────────────────────────────
        public void SetLightsActive(bool state)
        {
            // 한 번 꺼진 이후라면, 다시 켜지지 않도록 막기
            if (lightsPermanentlyOff && state == true)
                return;

            Vector3 triggerCenter = transform.position; // 트리거 중심 좌표

            // 라이트 제어
            if (lights != null)
            {
                foreach (Light l in lights)
                {
                    float distance = Vector3.Distance(l.transform.position, triggerCenter);

                    // 켜기 조건: state == true + 트리거 중심에서 일정 거리 안
                    if (state && distance <= triggerLightDistance)
                    {
                        if (!l.gameObject.activeSelf) l.gameObject.SetActive(true);
                        l.enabled = true;
                        l.range = activeRange;
                    }
                    else
                    {
                        if (l.gameObject.activeSelf) l.gameObject.SetActive(true);
                        l.enabled = false;
                        l.range = inactiveRange;
                    }
                }
            }

            // 에미션 제어 (거리 기반)
            SetEmissionState(state, triggerCenter);
        }

        // ──────────────────────────────────────────────
        // SetEmissionState()
        // 머티리얼의 에미션 체크박스와 색상을
        // 트리거 중심 기준 거리로 나눠 ON/OFF 한다.
        // ──────────────────────────────────────────────
        private void SetEmissionState(bool state, Vector3 triggerCenter)
        {
            if (emissiveRenderers == null) return;

            foreach (Renderer rend in emissiveRenderers)
            {
                // 렌더러의 기준 위치와 트리거 중심 간 거리 계산
                float distance = Vector3.Distance(rend.transform.position, triggerCenter);

                foreach (Material mat in rend.sharedMaterials)
                {
                    if (mat == null) continue;

                    // 켜기 조건: state == true + 트리거 중심에서 일정 거리 안
                    if (state && distance <= triggerLightDistance)
                    {
                        // 불 켜짐: 에미션 ON
                        mat.EnableKeyword("_EMISSION");
                        mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                        mat.SetColor("_EmissionColor", Color.white * 1.5f);
                    }
                    else
                    {
                        // 트리거 밖이거나 state == false 인 경우: 에미션 OFF
                        mat.DisableKeyword("_EMISSION");
                        mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
                        mat.SetColor("_EmissionColor", Color.black);
                    }
                }
            }
        }
    }
}
