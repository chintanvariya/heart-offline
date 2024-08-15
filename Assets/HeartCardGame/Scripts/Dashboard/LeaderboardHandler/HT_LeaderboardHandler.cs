using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace HeartCardGame
{
    public class HT_LeaderboardHandler : MonoBehaviour
    {
        [Header("===== Object Scripts =====")]
        [SerializeField] private HT_SocketHandler socketHandler;
        [SerializeField] private HT_DashboardManager dashboardManager;
        [SerializeField] private HT_UiManager uiManager;
        [SerializeField] private HT_UserRegistration userRegistration;

        [Header("===== Leaderboard Data =====")]
        [SerializeField] private Transform leaderboardDataContainer;
        [SerializeField] private HT_LeaderboardPrefabController leaderboardPrefabController;
        [SerializeField] private HT_LeaderboardPrefabController myDataForLeaderboard;
        [SerializeField] private List<HT_LeaderboardPrefabController> leaderboardPrefabControllers;

        [Header("===== Model Class =====")]
        [SerializeField] private LeaderboardResponse leaderboardResponse;

        public void GetLeaderboard()
        {
            string url = socketHandler.serverUrl[(int)socketHandler.serverType];
            url += HT_StaticData.GetLeaderboard;
            if (dashboardManager.IsAccessTokenAvailable())
                LeaderboardHandle(url);
            else
                userRegistration.UserRegister(PlayerPrefs.GetString("UserName"), (success) => LeaderboardHandle(url));
        }

        void LeaderboardHandle(string url)
        {
            StartCoroutine(HT_APIManager.RequestWithPostData(url, "", (data) =>
            {
                leaderboardResponse = JsonConvert.DeserializeObject<LeaderboardResponse>(data);
                dashboardManager.profilePanel.SetActive(false);
                dashboardManager.dailySpinPanel.SetActive(false);
                dashboardManager.lobbyPanel.SetActive(false);
                dashboardManager.PopupOnOff(dashboardManager.leaderboardPanel, true);
                SetLeaderboardData();
            }, (error) => uiManager.ApiError(error)));
        }

        void SetLeaderboardData()
        {
            DestroyLeaderboardData();
            LeaderBoardDatum data = leaderboardResponse.data.leaderBoardData.LastOrDefault();
            myDataForLeaderboard.LeaderboardSetting(data.rank, data.userName, data.winGames, data.profileImage);
            for (int i = 0; i < leaderboardResponse.data.leaderBoardData.Count; i++)
            {
                var leaderboardData = leaderboardResponse.data.leaderBoardData[i];
                if (leaderboardData.rank != data.rank)
                {
                    HT_LeaderboardPrefabController leadeboardDataClone = Instantiate(leaderboardPrefabController, leaderboardDataContainer);
                    leadeboardDataClone.LeaderboardSetting(leaderboardData.rank, leaderboardData.userName, leaderboardData.winGames, leaderboardData.profileImage);
                    leaderboardPrefabControllers.Add(leadeboardDataClone);
                }
            }
        }

        void DestroyLeaderboardData()
        {
            foreach (var item in leaderboardPrefabControllers)
            {
                Destroy(item.gameObject);
            }
            leaderboardPrefabControllers.Clear();
        }
    }

    #region LeaderboardModelClass
    [System.Serializable]
    public class LeaderboardResponseData
    {
        public List<LeaderBoardDatum> leaderBoardData;
    }

    [System.Serializable]
    public class LeaderBoardDatum
    {
        public string _id;
        public string profileImage;
        public string userName;
        public int coins;
        public int winGames;
        public int rank;
    }

    [System.Serializable]
    public class LeaderboardResponse
    {
        public string message;
        public string status;
        public int statusCode;
        public bool success;
        public LeaderboardResponseData data;
    }
    #endregion
}