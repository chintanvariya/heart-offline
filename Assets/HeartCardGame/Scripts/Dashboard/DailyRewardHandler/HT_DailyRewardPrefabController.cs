using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace HeartCardGame
{
    public class HT_DailyRewardPrefabController : MonoBehaviour
    {
        [Header("===== Daily Reward Data =====")]
        [SerializeField] TextMeshProUGUI dayTxt;
        [SerializeField] TextMeshProUGUI coinTxt;
        [SerializeField] GameObject lockObj, claimedObj, lostObj;
        public string dailyRewardId;
        public Button claimBtn;
        public GameObject particlesSystem;

        public void DailyRewardSetting(string id, int dayNumber, int price, bool isClaimed, bool isLostClaim, bool isCurrentClaimed, bool isAnimationRequired)
        {
            dayTxt.SetText($"Day {dayNumber}");
            coinTxt.SetText($"{price}");

            if (!isCurrentClaimed && !isClaimed && !isLostClaim)
            {
                claimBtn.interactable = false;
                lockObj.SetActive(true);
            }

            if (isLostClaim)
            {
                claimBtn.interactable = false;
                lostObj.SetActive(true);
            }

            if (isClaimed)
            {
                claimedObj.SetActive(true);
                claimBtn.interactable = false;
            }

            if (isAnimationRequired && isCurrentClaimed)
                particlesSystem.SetActive(true);

            dailyRewardId = id;
        }
    }
}