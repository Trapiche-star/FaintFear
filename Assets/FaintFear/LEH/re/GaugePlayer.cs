using FaintFear;
using UnityEngine;

public class GaugePlayer : Interactive
{
    public GaugeUI gaugeUI;
    public float maxChargeTime = 2f;
    public float drainSpeed = 1f;
    public PlayerMove playerMove;

    float currentCharge;
    bool isCharging;
    bool isTouchingPushItem;

    Push currentItem;

    void Update()
    {
        HandleGauge();
        playerMove.enabled = currentCharge < 1f;

        HandlePushMove();
    }


    public void SetPushItem(Push item)
    {
        if (item == null || item.isCleared) return;

        isTouchingPushItem = true;
        currentItem = item;
    }

    public void ClearPushItem()
    {
        isTouchingPushItem = false;
        currentItem = null;
        currentCharge = 0f;
    }

    void HandleGauge()
    {
        // V키 누르고 + 밀기 오브젝트에 닿아있을 때만 충전
        if (Input.GetKey(KeyCode.V) && isTouchingPushItem && !currentItem.isCleared)
        {
            isCharging = true;
            currentCharge += Time.deltaTime / maxChargeTime;
            currentCharge = Mathf.Clamp01(currentCharge);
        }
        else
        {
            // V키 안 누르면 감소
            isCharging = false;
            currentCharge -= Time.deltaTime * drainSpeed;
            currentCharge = Mathf.Clamp01(currentCharge);
        }

        gaugeUI.SetGauge(currentCharge);
        gaugeUI.Show(isTouchingPushItem && !currentItem.isCleared);
    }

    void HandlePushMove()
    {
        // 게이지가 꽉 찼을 때만 이동 가능
        if (currentCharge < 1f || currentItem == null) return;

        //다른 InputAction 잠금 구간
        // (이 구간에서는 PlayerMove 비활성화 추천)

        if (Input.GetKey(KeyCode.A))
            currentItem.Move(-1);

        if (Input.GetKey(KeyCode.D))
            currentItem.Move(1);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("PushItem"))
        {
            Push item = hit.collider.GetComponent<Push>();
            if (item != null && !item.isCleared)
            {
                isTouchingPushItem = true;
                currentItem = item;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("PushItem"))
        {
            isTouchingPushItem = false;
            currentItem = null;
            currentCharge = 0f;
        }
    }
}
