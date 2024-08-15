using UnityEngine;
using TMPro;

namespace HeartCardGame
{
    public class HT_LeaderboardPrefabController : MonoBehaviour
    {
        [Header("===== Leaderboard Data =====")]
        [SerializeField] private TextMeshProUGUI indexTxt;
        [SerializeField] private TextMeshProUGUI nameTxt, scoreTxt;
        [SerializeField] private GameObject loader;
        [SerializeField] private UnityEngine.UI.Image profileImg;
        [SerializeField] private UnityEngine.UI.Image leaderDataImg;
        [SerializeField] private Sprite firstUserSprite, otherSprite;
        [SerializeField] private bool isMyPlayer;

        Coroutine TextureCor;

        public void LeaderboardSetting(int rank, string userName, int score, string proffilePic)
        {
            indexTxt.SetText($"{rank}");
            nameTxt.SetText($"{userName}");
            scoreTxt.SetText($"Win Game : {score}");
            if (!isMyPlayer)
                leaderDataImg.sprite = rank == 1 ? firstUserSprite : otherSprite;
            TextureCor = StartCoroutine(HT_GameManager.instance.uiManager.GetTexture(proffilePic, loader, (sprite) =>
             {
                 profileImg.sprite = sprite;
                 StopCoroutine(TextureCor);
             }));
        }
    }
}