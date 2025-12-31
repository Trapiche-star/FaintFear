using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 라이트 존 제어 클래스
    /// 플레이어 위치를 기준으로 라이트와 에미션을 켜고 끄며,
    /// 시작 시 트리거 밖 에미션이 보이지 않도록 초기 상태를 정리한다
    /// </summary>
    public class LightZone01 : MonoBehaviour
    {
        #region Variables

        // 건물 내부 모든 Point Light를 담고 있는 부모 오브젝트
        [SerializeField] private GameObject allPointLights;

        // 에미션이 적용된 메쉬들을 담고 있는 부모 오브젝트
        [SerializeField] private GameObject allEmissiveObjects;

        // 라이트가 켜졌을 때 사용할 Range 값
        [SerializeField] private float activeRange = 8f;

        // 라이트가 꺼졌을 때 사용할 Range 값
        [SerializeField] private float inactiveRange = 0f;

        // 트리거 중심 기준으로 라이트/에미션이 켜질 수 있는 최대 거리
        [SerializeField] private float triggerLightDistance = 15f;

        // 제어 대상 라이트 배열
        private Light[] lights;

        // 제어 대상 에미션 렌더러 배열
        private Renderer[] emissiveRenderers;

        // 한 번 꺼진 이후 다시 켜지지 않도록 막는 플래그
        private bool lightsPermanentlyOff = false;

        #endregion


        #region Unity Event Method

        // 씬이 시작되기 전에 라이트/에미션 제어 대상을 캐싱하고 초기 상태를 정리한다
        private void Awake()
        {
            // 만약 Point Light 부모가 존재한다면
            if (allPointLights != null)
                // 그래서 자식에 포함된 모든 Light 컴포넌트를 가져온다
                lights = allPointLights.GetComponentsInChildren<Light>(true);

            // 만약 에미션 대상 부모가 존재한다면
            if (allEmissiveObjects != null)
                // 그래서 자식에 포함된 모든 Renderer를 가져온다
                emissiveRenderers = allEmissiveObjects.GetComponentsInChildren<Renderer>(true);

            // 그래서 씬 시작 순간 트리거 밖 에미션이 보이는 상황을 방지하기 위해
            // 모든 에미션을 강제로 OFF 상태로 만든다
            ForceDisableAllEmission();
        }

        // 첫 프레임에 플레이어가 트리거 안에 있는지 검사해 초기 상태를 결정한다
        private void Start()
        {
            if (IsTutorialCompleted())
            {
                lightsPermanentlyOff = true;
                SetLightsActive(false);
                return;
            }

            // 만약 라이트와 에미션 대상이 모두 없다면
            if (lights == null && emissiveRenderers == null)
                return; // 더 이상 처리할 것이 없으므로 끝낸다

            bool playerInside = false;

            // 만약 이 오브젝트에 BoxCollider가 없다면
            BoxCollider box = GetComponent<BoxCollider>();
            if (box == null)
            {
                // 안전하게 모든 라이트를 끄고 종료한다
                SetLightsActive(false);
                return;
            }

            // 그래서 OverlapBox를 사용해 시작 시 플레이어가 트리거 안에 있는지 검사한다
            Collider[] colliders = Physics.OverlapBox(
                transform.position,
                box.size / 2f,
                transform.rotation
            );

            // 겹친 콜라이더 중 플레이어가 있는지 확인한다
            foreach (Collider col in colliders)
            {
                if (col.CompareTag("Player"))
                {
                    playerInside = true;
                    break;
                }
            }

            // 만약 플레이어가 안에 있다면 라이트를 켜고,
            // 그렇지 않으면 라이트를 끈 상태로 유지한다
            SetLightsActive(playerInside);
        }

        // 플레이어가 트리거를 벗어났을 때 호출된다
        private void OnTriggerExit(Collider other)
        {
            
        }

        #endregion


        #region Custom Method

        // 라이트와 에미션의 전체 ON / OFF 상태를 제어한다
        public void SetLightsActive(bool state)
        {
            // 만약 이미 영구 OFF 상태인데 다시 켜려고 한다면
            if (lightsPermanentlyOff && state)
                return; // 무시하고 끝낸다

            Vector3 triggerCenter = transform.position;

            // 라이트 제어 처리
            if (lights != null)
            {
                foreach (Light l in lights)
                {
                    // 만약 라이트가 null이면 건너뛴다
                    if (l == null)
                        continue;

                    // 트리거 중심과 라이트 사이의 거리를 계산한다
                    float distance = Vector3.Distance(l.transform.position, triggerCenter);

                    // 만약 켜는 상태이고 && 거리 조건을 만족하면
                    if (state && distance <= triggerLightDistance)
                    {
                        // 그래서 라이트를 활성화한다
                        l.enabled = true;
                        l.range = activeRange;
                    }
                    else
                    {
                        // 그렇지 않으면 라이트를 끈다
                        l.enabled = false;
                        l.range = inactiveRange;
                    }
                }
            }

            // 에미션 상태도 함께 제어한다
            SetEmissionState(state, triggerCenter);
        }

        // 씬 시작 시 모든 에미션을 강제로 OFF로 만드는 초기화 함수
        private void ForceDisableAllEmission()
        {
            // 만약 에미션 대상이 없다면
            if (emissiveRenderers == null)
                return;

            // 모든 렌더러에 대해 반복한다
            foreach (Renderer rend in emissiveRenderers)
            {
                if (rend == null)
                    continue;

                // 모든 머티리얼을 대상으로 처리한다
                foreach (Material mat in rend.materials)
                {
                    if (mat == null)
                        continue;

                    // 그래서 에미션 키워드를 끄고
                    mat.DisableKeyword("_EMISSION");

                    // 간접광에 영향이 가지 않도록 설정하고
                    mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;

                    // 에미션 색상을 검정으로 만든다
                    mat.SetColor("_EmissionColor", Color.black);
                }
            }
        }

        // 트리거 중심과의 거리 기준으로 에미션을 켜거나 끈다
        private void SetEmissionState(bool state, Vector3 triggerCenter)
        {
            // 만약 에미션 대상이 없다면
            if (emissiveRenderers == null)
                return;

            foreach (Renderer rend in emissiveRenderers)
            {
                if (rend == null)
                    continue;

                // 트리거 중심과의 거리 계산
                float distance = Vector3.Distance(rend.transform.position, triggerCenter);

                foreach (Material mat in rend.materials)
                {
                    if (mat == null)
                        continue;

                    // 만약 켜는 상태이고 && 거리 조건을 만족하면
                    if (state && distance <= triggerLightDistance)
                    {
                        // 그래서 에미션을 켠다
                        mat.EnableKeyword("_EMISSION");
                        mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                        mat.SetColor("_EmissionColor", Color.white * 1.5f);
                    }
                    else
                    {
                        // 그렇지 않으면 에미션을 끈다
                        mat.DisableKeyword("_EMISSION");
                        mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
                        mat.SetColor("_EmissionColor", Color.black);
                    }
                }
            }
        }
        public void SetPermanentlyOff()
        {
            lightsPermanentlyOff = true;
            SetLightsActive(false);
        }
        private bool IsTutorialCompleted()
        {
            var data = SaveSystem.LoadPreview();
            return data != null && data.tutorialCompleted;
        }
        #endregion
    }
}
