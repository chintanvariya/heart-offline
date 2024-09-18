using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FGSOfflineHeart
{
    public class HT_WinnerHandler : MonoBehaviour
    {
        [Header("====== Winning Data =====")]
        [SerializeField] private Image profileImg;
        [SerializeField] TextMeshProUGUI nameTxt, spadeTxt, heartTxt, totalTxt;
        [SerializeField] private GameObject heartInfoObj, spadeInfoObj, loaderObj, winObj, leftObj;

        public void WinnerDataSetting(int spadeCount, int heartCount, int totalCount, string profilePic, string userName, bool isWinner, bool isLeft, Sprite sprite = null)
        {
            spadeTxt.SetText($"{spadeCount}");
            heartTxt.SetText($"{heartCount}");
            totalTxt.SetText($"{totalCount}");
            nameTxt.SetText($"{userName}");

            leftObj.SetActive(isLeft);

            if (isWinner)
                winObj.SetActive(true);
            if (heartCount > 0)
                heartInfoObj.SetActive(true);
            if (spadeCount > 0)
                spadeInfoObj.SetActive(true);
            profileImg.sprite = sprite;
            //if (HT_GameManager.instance.isOffline)
            //{
            //    loaderObj.SetActive(false);
            //}
            //else
            //    HT_GameManager.instance.uiManager.StartCoroutine(HT_GameManager.instance.uiManager.GetTexture(profilePic, loaderObj, (sprite) => profileImg.sprite = sprite));
        }
    }
}