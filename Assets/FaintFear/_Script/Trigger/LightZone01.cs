using UnityEngine;

namespace FaintFear
{
    public class LightZone01 : MonoBehaviour
    {
        #region Variables

        [SerializeField] private GameObject allPointLights;
        [SerializeField] private GameObject allEmissiveObjects;
        [SerializeField] private float activeRange = 8f;
        [SerializeField] private float inactiveRange = 0f;
        [SerializeField] private float triggerLightDistance = 15f;

        private Light[] lights;
        private Renderer[] emissiveRenderers;
        private bool lightsPermanentlyOff = false;

        #endregion

        #region Unity Event Method

        private void Awake()
        {
            if (allPointLights != null)
                lights = allPointLights.GetComponentsInChildren<Light>(true);

            if (allEmissiveObjects != null)
                emissiveRenderers = allEmissiveObjects.GetComponentsInChildren<Renderer>(true);

            ForceDisableAllEmission();
        }

        private void Start()
        {
            if (lights == null && emissiveRenderers == null)
                return;

            // ⭐ 저장된 조명 상태 복원
            var data = SaveSystem.LoadPreview();
            if (data != null && data.lightsPermaOff)
            {
                Debug.Log("[LightZone01] Loading saved state - lights permanently OFF");
                lightsPermanentlyOff = true;
                SetLightsActive(false);
                return;
            }

            // ⭐ 튜토리얼 중에는 무조건 조명 켜기
            bool tutorialCompleted = data != null && data.tutorialCompleted;

            if (!tutorialCompleted && !GameManager.TutorialCompleted)
            {
                Debug.Log("[LightZone01] Tutorial not completed - lights ON by default");
                SetLightsActive(true);
                return;
            }

            // ⭐ 튜토리얼 완료 후 플레이어 위치 확인
            bool playerInside = CheckPlayerInside();
            SetLightsActive(playerInside);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                SetLightsActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                SetLightsActive(false);
            }
        }

        #endregion

        #region Custom Method

        private bool CheckPlayerInside()
        {
            BoxCollider box = GetComponent<BoxCollider>();
            if (box == null)
            {
                Debug.LogWarning("[LightZone01] BoxCollider not found");
                return false;
            }

            Collider[] colliders = Physics.OverlapBox(
                transform.position,
                box.size / 2f,
                transform.rotation
            );

            foreach (Collider col in colliders)
            {
                if (col.CompareTag("Player"))
                {
                    Debug.Log("[LightZone01] Player found inside trigger");
                    return true;
                }
            }

            return false;
        }

        public void SetLightsActive(bool state)
        {
            if (lightsPermanentlyOff && state)
            {
                Debug.Log("[LightZone01] Lights are permanently off, ignoring activation");
                return;
            }

            Vector3 triggerCenter = transform.position;
            int lightsControlled = 0;

            if (lights != null)
            {
                foreach (Light l in lights)
                {
                    if (l == null) continue;

                    float distance = Vector3.Distance(l.transform.position, triggerCenter);

                    if (state && distance <= triggerLightDistance)
                    {
                        l.enabled = true;
                        l.range = activeRange;
                        lightsControlled++;
                    }
                    else
                    {
                        l.enabled = false;
                        l.range = inactiveRange;
                    }
                }
            }

            SetEmissionState(state, triggerCenter);

            Debug.Log($"[LightZone01] SetLightsActive({state}) - {lightsControlled}/{lights?.Length ?? 0} lights controlled");
        }

        private void ForceDisableAllEmission()
        {
            if (emissiveRenderers == null) return;

            foreach (Renderer rend in emissiveRenderers)
            {
                if (rend == null) continue;

                foreach (Material mat in rend.materials)
                {
                    if (mat == null) continue;

                    mat.DisableKeyword("_EMISSION");
                    mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
                    mat.SetColor("_EmissionColor", Color.black);
                }
            }
        }

        private void SetEmissionState(bool state, Vector3 triggerCenter)
        {
            if (emissiveRenderers == null) return;

            foreach (Renderer rend in emissiveRenderers)
            {
                if (rend == null) continue;

                float distance = Vector3.Distance(rend.transform.position, triggerCenter);

                foreach (Material mat in rend.materials)
                {
                    if (mat == null) continue;

                    if (state && distance <= triggerLightDistance)
                    {
                        mat.EnableKeyword("_EMISSION");
                        mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                        mat.SetColor("_EmissionColor", Color.white * 1.5f);
                    }
                    else
                    {
                        mat.DisableKeyword("_EMISSION");
                        mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
                        mat.SetColor("_EmissionColor", Color.black);
                    }
                }
            }
        }

        public void SetPermanentlyOff()
        {
            Debug.Log("[LightZone01] Setting lights permanently off");
            lightsPermanentlyOff = true;
            SetLightsActive(false);
        }

        #endregion
    }
}