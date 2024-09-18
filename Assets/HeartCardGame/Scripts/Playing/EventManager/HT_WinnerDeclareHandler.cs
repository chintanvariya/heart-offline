using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using FGSOfflineCallBreak;
using GoogleMobileAds.Api;
using UnityEngine.UI;

namespace FGSOfflineHeart
{
    public class HT_WinnerDeclareHandler : MonoBehaviour
    {
        [Header("===== Object Script =====")]
        [SerializeField] private HT_EventManager eventManager;
        [SerializeField] private HT_GameManager gameManager;
        [SerializeField] private HT_UiManager uiManager;
        [SerializeField] private HT_TurnInfoManager turnInfoManager;
        [SerializeField] private HT_WinOfRound winOfRound;

        [SerializeField] private HT_JoinTableHandler joinTableHandler;
        [SerializeField] private HT_AudioManager audioManager;

        [SerializeField] private HT_OfflineCardDistributor offlineCardDistributor;
        [SerializeField] private HT_CardDeckController cardDeckController;
        [SerializeField] private HT_OfflineGameHandler offlineGameHandler;

        [Header("===== Winning Data =====")]
        [SerializeField] private List<HT_WinnerHandler> winnerHandlers;
        public HT_WinnerHandler winnerHandler;
        public RectTransform winnerDataGenerator;
        [SerializeField] private TMPro.TextMeshProUGUI waitingTxt;
        [SerializeField] private Button continueBtn;
        [SerializeField] private GameObject nextGameObject;
        private int timeOfNextRound;

        [Header("===== Model Class =====")]
        [SerializeField] private WinnerDeclareResponse winnerDeclareResponse;

        private void OnEnable()
        {
            eventManager.RegisterEvent(SocketEvents.WINNER_DECLARE, WinnerDeclare);
        }

        private void OnDisable()
        {
            eventManager.UnregisterEvent(SocketEvents.WINNER_DECLARE, WinnerDeclare);
        }

        private void Start() => gameManager.RoundReset += WinnerReset;

        private void WinnerDeclare(string data)
        {
            winnerDeclareResponse = JsonConvert.DeserializeObject<WinnerDeclareResponse>(data);
            gameManager.RoundReset?.Invoke();
            WinnerPreSetting(winnerDeclareResponse.data.nextRound);
            int round = winnerDeclareResponse.data.roundScoreHistory.scores.Count - 1;
            timeOfNextRound = winnerDeclareResponse.data.timer;
            for (int i = 0; i < winnerDeclareResponse.data.roundScoreHistory.scores[round].score.Count; i++)
            {
                var totalScore = winnerDeclareResponse.data.roundScoreHistory.total[i];
                var scoreData = winnerDeclareResponse.data.roundScoreHistory.scores[round].score.Find(x => x.seatIndex == totalScore.seatIndex);
                var user = winnerDeclareResponse.data.roundScoreHistory.users.Find(x => x.seatIndex == totalScore.seatIndex);
                HT_WinnerHandler winnerClone = Instantiate(winnerHandler, winnerDataGenerator);
                string userName = user.seatIndex == gameManager.mySeatIndex ? "You" : user.username;
                bool isWinner = false;
                bool isLeft = user.userStatus.Contains("LEFT");
                if (winnerDeclareResponse.data.winner.Count > 0)
                {
                    audioManager.backgroundAudioSource.mute = true;
                    isWinner = winnerDeclareResponse.data.winner.Contains(user.seatIndex);
                }
                winnerClone.WinnerDataSetting(scoreData.spadePoint, scoreData.heartPoint, totalScore.totalPoint, user.profilePicture, userName, isWinner, isLeft);
                winnerHandlers.Add(winnerClone);
            }
            FinalWinnerInfoSet(winnerDeclareResponse.data.winner.Count > 0, winnerDeclareResponse.data.timer, winnerDeclareResponse.data.winner.Contains(cardDeckController.myPlayer.mySeatIndex));

            if (winnerDeclareResponse.data.winner.Count > 0)
            {
                if (winnerDeclareResponse.data.winner.Contains(cardDeckController.myPlayer.mySeatIndex))
                    HT_GameManager.instance.audioManager.GamePlayAudioSetting(HT_GameManager.instance.audioManager.winClip);
                else
                    HT_GameManager.instance.audioManager.GamePlayAudioSetting(HT_GameManager.instance.audioManager.lossClip);
            }

            foreach (var player in joinTableHandler.playerData)
            {
                player.HeartSpadeObjReset();
            }
        }

        public void FinalWinnerInfoSet(bool isFinal, int timer, bool isWinner)
        {
            waitingTxt.gameObject.SetActive(false);
            uiManager.OtherPanelOpen(uiManager.winningPanel, true);
            timeOfNextRound = timer;
            if (isFinal)
            {
                //waitingTxt.gameObject.SetActive(false);
                nextGameObject.SetActive(true);
                continueBtn.gameObject.SetActive(false);
                HT_GameManager.instance.audioManager.GamePlayAudioSetting(isWinner ? HT_GameManager.instance.audioManager.winClip : HT_GameManager.instance.audioManager.lossClip);
            }
            else
            {
                continueBtn.gameObject.SetActive(true);
                nextGameObject.SetActive(false);
                //waitingTxt.gameObject.SetActive(true);
                //InvokeRepeating(nameof(TimerOnWinningPanel), 0f, 1f);
            }
        }

        public void WinnerPreSetting(int nextRound)
        {
            turnInfoManager.isFirstTurn = true;
            uiManager.shootingMoonPanel.SetActive(false);
            //dashboardManager.PanelOnOff(dashboardManager.howToPlayPanel, false);
            uiManager.scoreboardBtn.interactable = true;
            gameManager.isHeartAnimationShow = true;
            uiManager.settingPanel.SetActive(false);
            uiManager.leavePanel.SetActive(false);
            uiManager.scoreboardPanel.SetActive(false);

            if (nextRound % 4 != 0)
                gameManager.isCardPassed = false;
            else
                gameManager.isCardPassed = true;
            winOfRound.handedCards.Clear();
        }

        void TimerOnWinningPanel()
        {
            waitingTxt.SetText($"New round start in {timeOfNextRound} seconds...");
            timeOfNextRound--;
            if (timeOfNextRound < 0)
            {
                foreach (var player in joinTableHandler.playerData)
                {
                    player.HeartSpadeObjReset();
                }
                CancelInvoke(nameof(TimerOnWinningPanel));

                if (gameManager.isOffline)
                    offlineCardDistributor.CardDistribution();
            }
        }

        public void ContinueBtn()
        {
            foreach (var player in joinTableHandler.playerData)
                player.HeartSpadeObjReset();
            if (gameManager.isOffline)
                offlineCardDistributor.CardDistribution();
        }

        void WinnerReset()
        {
            foreach (var winner in winnerHandlers)
            {
                Destroy(winner.gameObject);
            }
            winnerHandlers.Clear();

            CancelInvoke(nameof(TimerOnWinningPanel));
            //dashboardManager.PanelOnOff(dashboardManager.howToPlayPanel, false);
        }

        public void NewRoundSetting()
        {
            if (gameManager.isOffline)
            {
                gameManager.GameReset?.Invoke();
                gameManager.RoundReset?.Invoke();
                offlineGameHandler.PlayerSetup();
            }
            else
            {
                //socketHandler.ForcefullySocketDisconnect();
                //socketHandler.InternetCheckInitiate();
            }
        }
    }

    [System.Serializable]
    public class WinnerDeclareResponseData
    {
        public int timer;
        public List<int> winner;
        public RoundScoreHistory roundScoreHistory;
        public List<WinningAmount> winningAmount;
        public string roundTableId;
        public int nextRound;
    }

    [System.Serializable]
    public class WinnerDeclareResponse
    {
        public string en;
        public WinnerDeclareResponseData data;
    }

    [System.Serializable]
    public class RoundScoreHistory
    {
        public List<Total> total;
        public List<Score> scores;
        public List<Roundwinner> roundwinner;
        public List<User> users;
    }

    [System.Serializable]
    public class Roundwinner
    {
        public string title;
        public List<RoundWinners> roundWinners;
    }

    [System.Serializable]
    public class RoundWinners
    {
        public string userId;
        public int seatIndex;
        public string profilePic;
    }

    [System.Serializable]
    public class Score
    {
        public string title;
        public List<Scores> score;
    }

    [System.Serializable]
    public class Scores
    {
        public int roundPoint;
        public int heartPoint;
        public int spadePoint;
        public int seatIndex;
    }

    [System.Serializable]
    public class Total
    {
        public int totalPoint;
        public int seatIndex;
    }

    [System.Serializable]
    public class User
    {
        public string username;
        public string profilePicture;
        public int seatIndex;
        public string userStatus;
    }

    [System.Serializable]
    public class WinningAmount
    {
        public int seatIndex;
        public string userId;
        public string winningAmount;
    }
}