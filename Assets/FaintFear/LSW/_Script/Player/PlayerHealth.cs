using FaintFear;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, IDamageable
{

    public float mental;
    private bool isDeath = false;

    public UnityAction onDamage;
    public UnityAction onDie;
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
