using UnityEngine;
using UnityEngine.UI;

namespace HeartCardGame
{
    public class HT_ProfileAvatarHandler : MonoBehaviour
    {
        [Header("===== Object Scripts =====")]
        [SerializeField] private HT_DashboardManager dashboardManager;
        [SerializeField] private HT_UiManager uiManager;

        [Header("===== Profile Data =====")]
        [SerializeField] private Image profileImage;
        [SerializeField] private GameObject loader;

        Coroutine TextureCor;

        private void Start()
        {
            dashboardManager.ProfileAction += ProfileSetting;
        }

        private void ProfileSetting(string profileURL)
        {
            TextureCor = StartCoroutine(uiManager.GetTexture(profileURL, loader, (sprite) =>
            {
                profileImage.sprite = sprite;
                if (TextureCor != null)
                    StopCoroutine(TextureCor);
            }));
        }
    }

    [System.Serializable]
    public class AvatarBuyOrSet
    {
        public string avatarId;
    }
}