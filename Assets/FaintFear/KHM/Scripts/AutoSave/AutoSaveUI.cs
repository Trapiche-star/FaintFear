using UnityEngine;
namespace FaintFear
{
    /// <summary>
    /// 자동 저장 시 나오는 UI를 관리하는 클래스
    /// </summary>
    public class AutoSaveUI : Singleton<AutoSaveUI>
    {
        public GameObject root;

        // 씬이 로드될 때마다 실행
        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        //UI 보여주기
        public void Show()
        {
            root.SetActive(true);
        }

        //UI 숨기기
        public void Hide()
        {
            root.SetActive(false);
        }
    }
}