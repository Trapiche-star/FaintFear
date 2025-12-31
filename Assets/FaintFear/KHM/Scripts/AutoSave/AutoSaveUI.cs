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
        private void OnEnable()
        {
            AutoSaveManager.OnAutoSaveStart += Show;
            AutoSaveManager.OnAutoSaveEnd += Hide;
        }

        private void OnDisable()
        {
            AutoSaveManager.OnAutoSaveStart -= Show;
            AutoSaveManager.OnAutoSaveEnd -= Hide;
        }
        public void Show()
        {
            if (root == null) return;
            root.SetActive(true);
        }

        public void Hide()
        {
            if (root == null) return;
            root.SetActive(false);
        }
    }
}