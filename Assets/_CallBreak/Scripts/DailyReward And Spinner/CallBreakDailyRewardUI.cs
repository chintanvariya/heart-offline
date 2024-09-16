using UnityEngine;
using UnityEngine.UI;

namespace FGSOfflineCallBreak
{
    public class CallBreakDailyRewardUI : MonoBehaviour
    {
        public GameObject lockObject;
        public GameObject claimedObject;

        public GameObject coinImage;
        public GameObject keyImage;

        public TMPro.TextMeshProUGUI coinValueText;
        public TMPro.TextMeshProUGUI coinKeysText;

        public TMPro.TextMeshProUGUI dayText;
        public Image dayImageHud;

        public void UpdateTheValue(Sprite dayHud, int day, int coin, int keys)
        {
            UpdateDayHud(dayHud);
            dayText.text = $"Day {day} ";
            coinValueText.text = $"{CallBreakUtilities.AbbreviateNumber(coin)}";
            coinKeysText.text = $"{CallBreakUtilities.AbbreviateNumber(keys)}";

            coinImage.SetActive(true);
            keyImage.SetActive(true);

            if (coin == 0)
                coinImage.SetActive(false);
            if (keys == 0)
                keyImage.SetActive(false);
        }

        public void UpdateDayHud(Sprite dayHud) => dayImageHud.sprite = dayHud;
        public void UpdateAndHideLocked(bool isActive) => lockObject.SetActive(!isActive);
        public void UpdateAndHideClaimed(bool isActive) => claimedObject.SetActive(isActive);
    }
}
