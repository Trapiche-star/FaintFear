using FaintFear;
using UnityEngine;

public class PlayerPushInteraction : MonoBehaviour
{
    private PlayerMove playerMove;
    private void Awake()
    {
        // 같은 오브젝트에 있는 PlayerMove 컴포넌트 가져오기
        playerMove = GetComponent<PlayerMove>();
    }

    private void OnEnable()
    {

        if (playerMove != null)
        {
            playerMove.OnPushEvent += OnPush;
        }
    }
    private void OnDisable()
    {
        if (playerMove != null)
        {
            playerMove.OnPushEvent -= OnPush;
        }
    }

    private void OnPush()
    {
        //여기서 코드 구현하시면됩니다.
        
        Debug.Log("v키 입력");
    }

}
