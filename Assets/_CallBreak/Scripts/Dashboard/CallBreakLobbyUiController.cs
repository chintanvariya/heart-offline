using UnityEngine;
using UnityEngine.UI;

namespace FGSOfflineCallBreak
{

    public class CallBreakLobbyUiController : MonoBehaviour
    {
        [Header("BACKGROUND IMAGE")]
        public Image bg;

        [Header("TEXT")]
        public TMPro.TextMeshProUGUI lobbyType;
        public TMPro.TextMeshProUGUI roundText;
        public TMPro.TextMeshProUGUI playButtonText;
        public TMPro.TextMeshProUGUI winAmountText;
        [Header("IMAGE")]
        public Image practicesAndCoin;
        [Header("LOBBY AMOUNT")]
        public int lobbyAmount;
        public string keysAmount;
        [Header("DashboardHandler")]
        public CallBreakDashboardController dashboardController;

        public void UpdateLobbyText(Sprite _practicesAndCoin, int _lobbyAmount, string keys, string round, string playButton, string winAmount)
        {
            if (keys == "Practice")
                keysAmount = "0";
            else
                keysAmount = keys;

            lobbyAmount = _lobbyAmount;      //100
            lobbyType.text = keys;           //Practice || +10
            roundText.text = round;          //Standard
            playButtonText.text = playButton;//Play 10
            winAmountText.text = winAmount;  //40
            practicesAndCoin.sprite = _practicesAndCoin;
        }

        public void UpdateRoundText(string mode) => roundText.text = mode;

        public void OnOnClikedPlay() => dashboardController.OnButtonPlayNow(this);

    }
}