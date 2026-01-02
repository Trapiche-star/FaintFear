using UnityEngine;

public class ItemGlowPulse : MonoBehaviour
{
    public Color emissionColor = Color.white;
    public float intensity = 0.5f;
    public float pulseSpeed = 1f;

    private Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        mat.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        float emission = Mathf.PingPong(Time.time * pulseSpeed, 1f);
        Color finalColor = emissionColor * (emission * intensity);

        mat.SetColor("_EmissionColor", finalColor);
    }
}
