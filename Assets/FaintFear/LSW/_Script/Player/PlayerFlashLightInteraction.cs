using FaintFear;
using UnityEngine;

public class PlayerFlashLightInteraction : MonoBehaviour
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
            playerMove.OnFlashLightEvent += OnFlash;
        }
    }
    private void OnDisable()
    {
        if (playerMove != null)
        {
            playerMove.OnFlashLightEvent -= OnFlash;
        }
    }

    private void OnFlash()
    {
        //여기서 코드 구현하시면됩니다.
        Debug.Log("f키 입력");
    }


}
