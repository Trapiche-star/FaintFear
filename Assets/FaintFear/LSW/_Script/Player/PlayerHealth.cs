using FaintFear;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    //참조
    private Flashlight flashlight;

    private float mental;
    private bool isDeath = false;

    [SerializeField]
    private float damagePerSecond = 3f;
    [SerializeField]
    private float healPerSecond = 5f;


    public UnityAction onDamage;
    public UnityAction onDie;

    private void Awake()
    {
        flashlight = GetComponentInChildren<Flashlight>();
    }

    private void Start()
    {
        mental = PlayerStatus.Instance.currentMentalPower;
    }

    private void Update()
    {
        //손전등 사용 가능 후부터 정신력 깎이게
        if (isDeath || !PlayerStatus.Instance.isMentalSystemActive) return;

        if (flashlight != null && flashlight.IsOn)
        {
            RecoverMental();
        }
        else
        {
            DrainMental();
        }
    }
    //정신력 감소
    void DrainMental()
    {
        mental -= damagePerSecond * Time.deltaTime;
        PlayerStatus.Instance.SetHealth(mental);

        if (mental <= 0f && isDeath == false)
        {
            Die();
        }
    }
    //정신력 증가
    void RecoverMental()
    {
        mental += healPerSecond * Time.deltaTime;
        PlayerStatus.Instance.SetHealth(mental);
    }

    public void TakeDamage(float damage)
    {
        mental -= damage;

        PlayerStatus.Instance.SetHealth(mental);

        onDamage?.Invoke();

        if (mental <= 0f && isDeath == false)
        {
            Die();
        }
    }

    private void Die()
    {
        isDeath = true;

        onDie?.Invoke();
    }
}
