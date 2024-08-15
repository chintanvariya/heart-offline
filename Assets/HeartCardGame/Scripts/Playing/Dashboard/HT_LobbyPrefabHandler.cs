using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HeartCardGame
{
    public class HT_LobbyPrefabHandler : MonoBehaviour
    {
        [Header("===== Lobby Data =====")]
        [SerializeField] private TextMeshProUGUI coinTxt;
        [SerializeField] private TextMeshProUGUI winningAmountTxt;
        public Button playBtn;
        public bool isUseBot;

        public void LobbbyDataSetting(string entryFee, string winningAmount, bool isCanPlay, bool isBot)
        {
            coinTxt.SetText($"₹ {entryFee}");
            winningAmountTxt.SetText($"{winningAmount}");

            isUseBot = isBot;

            if (!isCanPlay)
                coinTxt.SetText($"Add Chips");
        }
    }
}