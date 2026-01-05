using UnityEngine;

public class ItemGlowPulse : MonoBehaviour
{
    public Color emissionColor = Color.white;
    public float intensity = 0.5f;
    public float pulseSpeed = 1f;

    private Material mat;
    private Renderer targetRenderer;

    void Start()
    {
        // 자신에게서 먼저 찾고, 없으면 자식에게서 찾기
        targetRenderer = GetComponent<Renderer>();

        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<Renderer>();
        }

        // Renderer를 찾지 못한 경우
        if (targetRenderer == null)
        {
            Debug.LogWarning($"[ItemGlowPulse] '{gameObject.name}'에 Renderer가 없습니다. 스크립트를 비활성화합니다.");
            enabled = false;
            return;
        }

        // Material 설정
        mat = targetRenderer.material;
        mat.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        // mat이 null인 경우 방어 코드
        if (mat == null) return;

        float emission = Mathf.PingPong(Time.time * pulseSpeed, 1f);
        Color finalColor = emissionColor * (emission * intensity);
        mat.SetColor("_EmissionColor", finalColor);
    }

    // 오브젝트가 파괴될 때 생성된 Material 정리
    void OnDestroy()
    {
        if (mat != null)
        {
            Destroy(mat);
        }
    }
}