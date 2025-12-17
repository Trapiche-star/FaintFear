using UnityEngine;

public class PowerGaugePlayer : MonoBehaviour
{
    public PushGaugeUI gaugeUI;

    public float maxChargeTime = 2f;
    public float chargeSpeed = 1f;
    public float drainSpeed = 1f;

    private float currentCharge;
    private bool isTouchingPushItem;

    void Update()
    {
        if (!isTouchingPushItem)
        {
            DrainGauge();
            return;
        }

        if (Input.GetKey(KeyCode.V))
        {
            gaugeUI.gameObject.SetActive(true);

            currentCharge += Time.deltaTime * chargeSpeed;
            currentCharge = Mathf.Clamp(currentCharge, 0, maxChargeTime);
        }
        else
        {
            DrainGauge();
        }

        gaugeUI.SetGauge(currentCharge / maxChargeTime);

        if (Input.GetKeyUp(KeyCode.V) && currentCharge >= maxChargeTime)
        {
            PushObject();
            currentCharge = 0f;
            gaugeUI.gameObject.SetActive(false);
        }
    }

    void DrainGauge()
    {
        currentCharge -= Time.deltaTime * drainSpeed;
        currentCharge = Mathf.Clamp(currentCharge, 0, maxChargeTime);

        if (currentCharge <= 0f)
            gaugeUI.gameObject.SetActive(false);
    }

    void PushObject()
    {
       //
    }

    //Trigger로 감지
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PushItem"))
        {
            isTouchingPushItem = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PushItem"))
        {
            isTouchingPushItem = false;
        }
    }
}


