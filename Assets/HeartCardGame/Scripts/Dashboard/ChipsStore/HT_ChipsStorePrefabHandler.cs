using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace HeartCardGame
{
    public class HT_ChipsStorePrefabHandler : MonoBehaviour
    {
        [Header("===== Chips Store Data =====")]
        [SerializeField] private TextMeshProUGUI coinTxt;
        [SerializeField] private TextMeshProUGUI priceTxt;
        [SerializeField] private string chipsStoreId;
        public Button addChipsBtn;

        public void SetChipsStoreData(int coins, int price, string id)
        {
            coinTxt.SetText($"{coins}");
            priceTxt.SetText($"₹ {price}");
            chipsStoreId = id;
        }
    }
}