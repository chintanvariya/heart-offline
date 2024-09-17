using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using TMPro;
using System.Linq;

namespace HeartCardGame
{
    public class HT_ShowScoreboard : MonoBehaviour
    {
        [Header("===== Object Script =====")]
        [SerializeField] private HT_EventManager eventManager;

        [SerializeField] private HT_GameManager gameManager;
        [SerializeField] private HT_UiManager uiManager;
        [SerializeField] private HT_OfflineGameHandler offlineGameHandler;
        [SerializeField] private HT_JoinTableHandler joinTableHandler;

        [Header("===== Scoreboard Data =====")]
        [SerializeField] private List<HT_ScoreboardHandler> scoreboardHandlers;
        [SerializeField] private HT_ScoreboardHandler scoreboardHandler;
        [SerializeField] private TextMeshProUGUI roundNumTxt;
        [SerializeField] private RectTransform scoreboardDataGenerator;

        [Header("===== Model Class =====")]
        [SerializeField] private ShowScoreboardResponse showScoreboardResponse;

        private void OnEnable() => eventManager.RegisterEvent(SocketEvents.SHOW_SCORE_BOARD, ShowScoreboardResponse);

        private void OnDisable() => eventManager.UnregisterEvent(SocketEvents.SHOW_SCORE_BOARD, ShowScoreboardResponse);

        private void Start() => gameManager.GameReset += ResetShowScoreboard;

        private void ShowScoreboardResponse(string arg0)
        {
            showScoreboardResponse = JsonConvert.DeserializeObject<ShowScoreboardResponse>(arg0);
            ResetShowScoreboard();
            roundNumTxt.SetText($"{showScoreboardResponse.data.currentRound}");
            for (int i = 0; i < showScoreboardResponse.data.scoreHistory.total.Count; i++)
            {
                var totalScore = showScoreboardResponse.data.scoreHistory.total[i];
                var playerData = showScoreboardResponse.data.scoreHistory.users.Find(x => x.seatIndex == totalScore.seatIndex);
                HT_ScoreboardHandler scoreboardClone = Instantiate(scoreboardHandler, scoreboardDataGenerator);
                string userName = playerData.seatIndex == gameManager.mySeatIndex ? "You" : playerData.username;
                bool isLeft = playerData.userStatus.Contains("LEFT");
                scoreboardClone.ScoreboardDataSetting(totalScore.totalPoint, playerData.profilePicture, userName, isLeft);
                scoreboardHandlers.Add(scoreboardClone);
                uiManager.OtherPanelOpen(uiManager.scoreboardPanel, true);
            }
        }

        public void ScoreboardShow()
        {
            if (gameManager.isOffline)
            {
                ResetShowScoreboard();
                roundNumTxt.SetText($"{offlineGameHandler.roundNum}");
                offlineGameHandler.roundText.SetText($"{offlineGameHandler.roundNum}");
                List<HT_PlayerController> players = joinTableHandler.playerData.OrderBy(x => x.totalPoint).ToList();
                for (int i = 0; i < players.Count; i++)
                {
                    var player = players[i];
                    HT_ScoreboardHandler scoreboardClone = Instantiate(scoreboardHandler, scoreboardDataGenerator);
                    scoreboardClone.ScoreboardDataSetting(player.totalPoint, "", player.userName, false, player.mySprite.sprite);
                    scoreboardHandlers.Add(scoreboardClone);
                }
                uiManager.OtherPanelOpen(uiManager.scoreboardPanel, true);
            }
            //else
            //    socketHandler.DataSendToSocket(SocketEvents.SHOW_SCORE_BOARD.ToString(), socketEventManager.ShowScoreboard(gameManager.tableId));
        }

        public void ResetShowScoreboard()
        {
            foreach (var item in scoreboardHandlers)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }
            scoreboardHandlers.Clear();
        }
    }

    [System.Serializable]
    public class ShowScoreboardRequestData
    {
        public string tableId;
    }

    [System.Serializable]
    public class ShowScoreboardRequest
    {
        public ShowScoreboardRequestData data;
    }

    [System.Serializable]
    public class ShowScoreboardResponseData
    {
        public List<object> winner;
        public ScoreHistory scoreHistory;
        public string roundTableId;
        public int currentRound;
    }

    [System.Serializable]
    public class ShowScoreboardResponse
    {
        public string en;
        public ShowScoreboardResponseData data;
    }

    [System.Serializable]
    public class ScoreHistory
    {
        public List<Total> total;
        public List<User> users;
    }
}