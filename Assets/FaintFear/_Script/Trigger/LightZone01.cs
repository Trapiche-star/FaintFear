using UnityEngine;
using UnityEngine.Rendering; // MaterialGlobalIlluminationFlags, Emission 키워드 제어용

namespace FaintFear
{
    /// <summary>
    /// 라이트 존 제어 클래스
    /// 트리거 기준으로 라이트와 메테리얼 에미션을 켜고 끄며,
    /// 시작 시점에 트리거 밖 에미션이 켜져 있을 수 있는 상황을 0으로 만든다.
    /// </summary>
    public class LightZone01 : MonoBehaviour
    {
        #region Variables

        // 건물 내부 모든 Point Light를 담고 있는 부모 오브젝트
        public GameObject allPointLights;

        // 에미션이 적용된 Mesh들을 담고 있는 부모 오브젝트
        public GameObject allEmissiveObjects;

        // 라이트가 켜졌을 때 적용될 Range 값
        public float activeRange = 8f;

        // 라이트가 꺼졌을 때 적용될 Range 값 (보통 0)
        public float inactiveRange = 0f;

        // 트리거 중심 기준으로 라이트 / 에미션이 켜질 수 있는 최대 거리
        public float triggerLightDistance = 15f;

        // 제어 대상 라이트 배열
        private Light[] lights;

        // 제어 대상 에미션 렌더러 배열
        private Renderer[] emissiveRenderers;

        // 한 번 꺼진 이후 다시 켜지지 않도록 막는 플래그
        private bool lightsPermanentlyOff = false;

        #endregion


        #region Unity Event Method

        private void Awake()
        {
            // Point Light 부모 아래의 모든 Light 컴포넌트 캐싱
            if (allPointLights != null)
                lights = allPointLights.GetComponentsInChildren<Light>(true);

            // 에미션 대상 부모 아래의 모든 Renderer 캐싱
            if (allEmissiveObjects != null)
                emissiveRenderers = allEmissiveObjects.GetComponentsInChildren<Renderer>(true);

            // 씬 시작 순간(첫 프레임)부터 "트리거 밖 에미션이 켜져 보이는" 상황을 막기 위해
            // 에미션을 먼저 무조건 OFF로 초기화한다.
            // 이렇게 하면 Start()에서 플레이어가 안에 있으면 필요한 것만 다시 켜고,
            // 밖에 있으면 그대로 OFF 상태가 유지된다.
            ForceDisableAllEmission();
        }

        private void Start()
        {
            // 제어 대상이 없으면 처리 중단
            if (lights == null && emissiveRenderers == null)
                return;

            bool playerInside = false;

            // 트리거 영역 내에 플레이어가 있는지 검사
            BoxCollider box = GetComponent<BoxCollider>();
            if (box == null)
            {
                // 트리거 콜라이더가 없으면 안전하게 전체 OFF 유지
                SetLightsActive(false);
                return;
            }

            // OverlapBox로 시작 시점에 플레이어가 트리거 안에 있는지 확인
            Collider[] colliders = Physics.OverlapBox(
                transform.position,
                box.size / 2f,
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

            // 플레이어가 트리거 안에 있으면 켜기, 아니면 끄기
            // (Awake에서 에미션은 이미 OFF로 초기화되었기 때문에,
            // 여기서 켜는 경우에만 명확하게 ON 상태로 바뀐다.)
            if (playerInside)
                SetLightsActive(true);
            else
                SetLightsActive(false);
        }

        private void OnTriggerExit(Collider other)
        {
            // 플레이어가 트리거를 벗어났을 때
            if (other.CompareTag("Player"))
            {
                // 라이트 및 에미션 끄기
                SetLightsActive(false);

                // 이후 다시 켜지지 않도록 영구 OFF 처리
                lightsPermanentlyOff = true;
            }
        }

        #endregion


        #region Custom Method

        // 라이트와 에미션의 전체 ON / OFF 상태를 제어
        public void SetLightsActive(bool state)
        {
            // 한 번 영구 OFF 되었다면 다시 켜지지 않도록 차단
            if (lightsPermanentlyOff && state)
                return;

            // 트리거 중심 좌표
            Vector3 triggerCenter = transform.position;

            // 라이트 제어 처리
            if (lights != null)
            {
                foreach (Light l in lights)
                {
                    // 라이트가 null이면 건너뜀
                    if (l == null)
                        continue;

                    // 트리거 중심과 라이트 사이 거리 계산
                    float distance = Vector3.Distance(l.transform.position, triggerCenter);

                    // 켜기 조건: state == true && 거리 조건 충족
                    if (state && distance <= triggerLightDistance)
                    {
                        // 라이트 오브젝트가 꺼져 있으면 켬 (비활성 상태면 enabled만으로는 불가)
                        if (!l.gameObject.activeSelf)
                            l.gameObject.SetActive(true);

                        // 라이트 컴포넌트 활성화 및 Range 설정
                        l.enabled = true;
                        l.range = activeRange;
                    }
                    else
                    {
                        // 라이트 오브젝트는 켜두되(필요 시), 라이트 컴포넌트만 끔
                        // (오브젝트 자체를 끄면 다른 컴포넌트/자식에 영향이 있을 수 있어 유지)
                        if (!l.gameObject.activeSelf)
                            l.gameObject.SetActive(true);

                        l.enabled = false;
                        l.range = inactiveRange;
                    }
                }
            }

            // 에미션 상태 제어
            // (Awake에서 이미 전체 OFF를 해두었기 때문에,
            // 여기서는 '켜야 하는 대상만 켜거나' '전체를 끄는' 역할만 수행하면 된다.)
            SetEmissionState(state, triggerCenter);
        }

        // 씬 시작 순간부터 트리거 밖 에미션이 켜질 가능성을 0으로 만들기 위한 초기화 함수
        private void ForceDisableAllEmission()
        {
            // 에미션 대상이 없으면 종료
            if (emissiveRenderers == null)
                return;

            // 모든 렌더러 / 모든 머티리얼을 대상으로 에미션 OFF 강제 적용
            foreach (Renderer rend in emissiveRenderers)
            {
                if (rend == null)
                    continue;

                foreach (Material mat in rend.sharedMaterials)
                {
                    if (mat == null)
                        continue;

                    // 에미션 키워드 OFF
                    mat.DisableKeyword("_EMISSION");

                    // GI 플래그 OFF (에미션이 간접광에 영향을 주지 않도록)
                    mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;

                    // 에미션 컬러를 완전 검정으로
                    mat.SetColor("_EmissionColor", Color.black);
                }
            }
        }

        // 메테리얼 에미션 상태를 트리거 중심 기준 거리로 ON / OFF
        private void SetEmissionState(bool state, Vector3 triggerCenter)
        {
            // 에미션 대상이 없으면 종료
            if (emissiveRenderers == null)
                return;

            foreach (Renderer rend in emissiveRenderers)
            {
                // 렌더러가 null이면 건너뜀
                if (rend == null)
                    continue;

                // 트리거 중심과 렌더러 사이 거리 계산
                float distance = Vector3.Distance(rend.transform.position, triggerCenter);

                foreach (Material mat in rend.sharedMaterials)
                {
                    if (mat == null)
                        continue;

                    // 켜기 조건: state == true && 거리 조건 충족
                    if (state && distance <= triggerLightDistance)
                    {
                        // 에미션 ON
                        mat.EnableKeyword("_EMISSION");
                        mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                        mat.SetColor("_EmissionColor", Color.white * 1.5f);
                    }
                    else
                    {
                        // 에미션 OFF
                        mat.DisableKeyword("_EMISSION");
                        mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
                        mat.SetColor("_EmissionColor", Color.black);
                    }
                }
            }
        }

        #endregion
    }
}
