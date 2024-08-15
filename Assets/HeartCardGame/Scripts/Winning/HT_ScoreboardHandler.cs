using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HeartCardGame
{
    public class HT_ScoreboardHandler : MonoBehaviour
    {
        [Header("====== Winning Data =====")]
        [SerializeField] private Image profileImg;
        [SerializeField] TextMeshProUGUI nameTxt, totalTxt;
        [SerializeField] private GameObject loaderObj, leftObj;

        public void ScoreboardDataSetting(int totalCount, string profilePic, string userName, bool isLeft, Sprite sprite = null)
        {
            totalTxt.SetText($"{totalCount}");
            nameTxt.SetText($"{userName}");
            leftObj.SetActive(isLeft);
            if (HT_GameManager.instance.isOffline)
            {
                loaderObj.SetActive(false);
                profileImg.sprite = sprite;
            }
            else
                HT_GameManager.instance.uiManager.StartCoroutine(HT_GameManager.instance.uiManager.GetTexture(profilePic, loaderObj, (sprite) => profileImg.sprite = sprite));
        }
    }
}