using UnityEngine;

public class PushCheck : MonoBehaviour
{
    public GaugePlayer player; // 반드시 인스펙터에서 연결

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PushItem"))
        {
            player.SetPushItem(other.GetComponent<Push>());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PushItem"))
        {
            player.ClearPushItem();
        }
    }
}