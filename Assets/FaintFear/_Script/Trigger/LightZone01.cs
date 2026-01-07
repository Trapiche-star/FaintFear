using UnityEngine;

namespace FaintFear
{
    [RequireComponent(typeof(BoxCollider))]
    public class LightZone01 : MonoBehaviour
    {
        #region Variables

        [Header("Light Objects")]
        [SerializeField] private GameObject allPointLights;
        [SerializeField] private GameObject allEmissiveObjects;

        [Header("Light Settings")]
        [SerializeField] private float activeRange = 8f;
        [SerializeField] private float inactiveRange = 0f;
        [SerializeField] private float triggerLightDistance = 15f;

        [Header("Overlap Settings")]
        [SerializeField] private float borderTolerance = 0.3f; // ⭐ 여유 구간

        private Light[] lights;
        private Renderer[] emissiveRenderers;
        private BoxCollider box;

        private bool lightsPermanentlyOff = false;
        private bool isPlayerInside = false;

        #endregion

        #region Unity

        private void Awake()
        {
            box = GetComponent<BoxCollider>();
            box.isTrigger = true;

            if (allPointLights != null)
                lights = allPointLights.GetComponentsInChildren<Light>(true);

            if (allEmissiveObjects != null)
                emissiveRenderers = allEmissiveObjects.GetComponentsInChildren<Renderer>(true);

            ForceDisableAllEmission();
        }

        private void Start()
        {
            var data = SaveSystem.LoadPreview();

            if (data != null && data.lightsPermaOff)
            {
                lightsPermanentlyOff = true;
                SetLightsActive(false);
                enabled = false;
                return;
            }

            bool tutorialCompleted =
                (data != null && data.tutorialCompleted) || GameManager.TutorialCompleted;

            if (!tutorialCompleted)
            {
                isPlayerInside = true;
                SetLightsActive(true);
            }
        }

        private void Update()
        {
            if (lightsPermanentlyOff) return;

            // ⭐ Restrict 체크
            TriggerRestrict restrict = FindObjectOfType<TriggerRestrict>();
            if (restrict != null && restrict.IsRestricting) return;

            bool insideNow = CheckPlayerInsideStable();

            if (insideNow == isPlayerInside) return; // 상태 변화 없으면 무시

            isPlayerInside = insideNow;
            SetLightsActive(isPlayerInside);
        }

        #endregion

        #region Core

        // ⭐ 여유 구간(Hysteresis) 적용
        private bool CheckPlayerInsideStable()
        {
            Vector3 center = transform.TransformPoint(box.center);
            Vector3 halfSize = box.size * 0.5f - Vector3.one * borderTolerance;

            Collider[] hits = Physics.OverlapBox(
                center,
                halfSize,
                transform.rotation
            );

            foreach (Collider col in hits)
            {
                if (col.CompareTag("Player"))
                    return true;
            }

            return false;
        }

        public void SetLightsActive(bool state)
        {
            if (lightsPermanentlyOff && state) return;

            Vector3 center = transform.position;

            if (lights != null)
            {
                foreach (Light l in lights)
                {
                    if (l == null) continue;

                    float dist = Vector3.Distance(l.transform.position, center);
                    bool enable = state && dist <= triggerLightDistance;

                    l.enabled = enable;
                    l.range = enable ? activeRange : inactiveRange;
                }
            }

            SetEmissionState(state, center);
        }

        private void ForceDisableAllEmission()
        {
            if (emissiveRenderers == null) return;

            foreach (Renderer r in emissiveRenderers)
            {
                foreach (Material mat in r.materials)
                {
                    mat.DisableKeyword("_EMISSION");
                    mat.SetColor("_EmissionColor", Color.black);
                }
            }
        }

        private void SetEmissionState(bool state, Vector3 center)
        {
            if (emissiveRenderers == null) return;

            foreach (Renderer r in emissiveRenderers)
            {
                float dist = Vector3.Distance(r.transform.position, center);

                foreach (Material mat in r.materials)
                {
                    if (state && dist <= triggerLightDistance)
                    {
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", Color.white * 1.5f);
                    }
                    else
                    {
                        mat.DisableKeyword("_EMISSION");
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

        #endregion
    }
}
