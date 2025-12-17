using UnityEngine;
namespace FaintFear
{
    /// <summary>
    /// 문서 아이템을 줍기
    /// </summary>
    public class PickupDocument : Interactive
    {
        #region Variables
        private PlayerMove playerMove;

        public GameObject DocumentUI;

        #endregion
        private void Awake()
        {
            //참조
            playerMove = GameObject.Find("Player").GetComponent<PlayerMove>();
        }
        public override void Interaction()
        {
            //플레이어 움직임 막기
            playerMove.enabled = false;
            //문서 UI 보이게
            DocumentUI.SetActive(true); 
            //정신력 시스템 off
            PlayerStatus.Instance.isMentalSystemActive = false;
            //배터리 시스템 off
            PlayerStatus.Instance.isBatteryActive = false;
        }
    }
}