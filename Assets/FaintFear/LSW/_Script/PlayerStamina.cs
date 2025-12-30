using System.Collections;
using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f; // 최대 스테미나
    [SerializeField] private float currentStamina = 0f;

    [Header("Rate Settings")]
    [SerializeField] private float consumeRate = 20f; // 초당 소모량
    [SerializeField] private float recoverRate = 15f; // 초당 회복량

    private bool onShift = false;
    private bool recoveryCoolDown = false;

    private void Awake()
    {
        currentStamina = maxStamina;
    }

    private void Update()
    {
        if(currentStamina < 0)
        {
            StartCoroutine(RecoveryCoolDown());
        }
        if(!recoveryCoolDown)
        {
            if (onShift && currentStamina > 0)
            {
                ConsumeStamina();
            }
            else
            {
                RecoverStamina();
            }
        }
      
    }

    private void ConsumeStamina()
    {
        currentStamina -= consumeRate * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }

    private void RecoverStamina()
    {
        currentStamina += recoverRate * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }

    IEnumerator RecoveryCoolDown()
    {
        recoveryCoolDown = true;
        yield return new WaitForSeconds(3f);
        recoveryCoolDown = false;
    }
}