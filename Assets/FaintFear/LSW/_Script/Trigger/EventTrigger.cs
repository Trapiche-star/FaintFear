using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    public PlayerMove playerMove;

    public void StartEvent()
    {
        playerMove.enabled = false; // 조작 비활성화
        // 이벤트 재생...
    }

    public void EndEvent()
    {
        playerMove.enabled = true; // 조작 다시 활성화
    }
}