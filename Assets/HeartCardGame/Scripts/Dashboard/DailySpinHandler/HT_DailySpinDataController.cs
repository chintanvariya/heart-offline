using UnityEngine;
using TMPro;

namespace HeartCardGame
{
    public class HT_DailySpinDataController : MonoBehaviour
    {
        [Header("===== Spinner Data =====")]
        public TextMeshProUGUI dataTxt;
        public int index;
        public float originAngle;
        [SerializeField] private GameObject hardLuckObj, coinDataObj;

        public void SetDailySpinData(string coinsValue, int indexOnSpinner, string bonusType)
        {
            index = indexOnSpinner;
            if (bonusType.Contains("Bonus Coin"))
            {
                dataTxt.SetText($"{coinsValue}");
                hardLuckObj.SetActive(false);
                coinDataObj.SetActive(true);
            }
            if (bonusType.Contains("Hard Luck"))
            {
                coinDataObj.SetActive(false);
                hardLuckObj.SetActive(true);
            }
        }
    }
}