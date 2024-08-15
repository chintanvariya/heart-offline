using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace HeartCardGame
{
    public class HT_LobbyHandler : MonoBehaviour
    {
        [Header("===== Object Scripts =====")]
        [SerializeField] private HT_DashboardManager dashboardManager;
        [SerializeField] private HT_UiManager uiManager;
        [SerializeField] private HT_SocketHandler socketHandler;
        [SerializeField] private HT_UserRegistration userRegistration;
        [SerializeField] private HT_ChipsStoreHandler chipsStoreHandler;

        [Header("===== Lobby Handle Data =====")]
        [SerializeField] private TextMeshProUGUI balanceTxt;

        [Header("===== Lobby Generator Data =====")]
        [SerializeField] private HT_LobbyPrefabHandler lobbyPrefab;
        [SerializeField] private Transform lobbyDataGenerator;
        [SerializeField] private List<HT_LobbyPrefabHandler> lobbyPrefabs;

        [Header("===== Lobby data for playing =====")]
        public int entryFees;
        public int winninAmount;
        public string userId, userName, profilePic, lobbyId;
        public bool isUseBot, isFTUE;

        [Header("===== Model Class =====")]
        public GetLobbyDataRes getLobbyDataResponse;

        private void Start()
        {
            dashboardManager.BalanceAction += BalanceSetting;
        }

        private void BalanceSetting(float obj) => balanceTxt.SetText($"{obj}");

        public void GetLobby()
        {
            string url = socketHandler.serverUrl[(int)socketHandler.serverType];
            url += HT_StaticData.GetLobby;
            if (dashboardManager.IsAccessTokenAvailable())
                LobbyHandle(url);
            else
                userRegistration.UserRegister(PlayerPrefs.GetString("UserName"), (success) => LobbyHandle(url));
        }

        void LobbyHandle(string url)
        {
            StartCoroutine(HT_APIManager.RequestWithPostData(url, HT_APIEventManager.GetLobbys(), (data) =>
            {
                getLobbyDataResponse = JsonConvert.DeserializeObject<GetLobbyDataRes>(data);
                if (getLobbyDataResponse.success)
                    LobbyDataSetting();
            }, (error) => uiManager.ApiError(error)));
        }

        public void LobbyDataSetting()
        {
            dashboardManager.PanelOnOff(dashboardManager.profilePanel, false);
            dashboardManager.PanelOnOff(dashboardManager.leaderboardPanel, false);
            dashboardManager.PanelOnOff(dashboardManager.dailySpinPanel, false);
            DestroyLobbyData();
            dashboardManager.PanelOnOff(dashboardManager.lobbyPanel, true);
            dashboardManager.UserDataSetting(getLobbyDataResponse.data.userData.userName, getLobbyDataResponse.data.userData.coins, getLobbyDataResponse.data.userData.profileImage, false);
            for (int i = 0; i < getLobbyDataResponse.data.lobbyList.Count; i++)
            {
                HT_LobbyPrefabHandler lobbyDataClone = Instantiate(lobbyPrefab, lobbyDataGenerator);
                var lobbyData = getLobbyDataResponse.data.lobbyList[i];
                lobbyDataClone.LobbbyDataSetting(lobbyData.entryfee.ToString(), lobbyData.winningPrice.ToString(), lobbyData.isCanPlay, lobbyData.isUseBot);
                lobbyDataClone.playBtn.onClick.AddListener(() =>
                {
                    ClickOnPlayOnLobby(lobbyData.entryfee, lobbyData.winningPrice, lobbyData._id, lobbyData.isCanPlay, lobbyDataClone.isUseBot);
                });
                lobbyPrefabs.Add(lobbyDataClone);
            }
        }

        void ClickOnPlayOnLobby(int entryFee, int winningPrize, string _lobbyId, bool isCanPlay, bool isBot)
        {
            if (isCanPlay)
            {
                entryFees = entryFee;
                winninAmount = winningPrize;
                userId = userRegistration.userId;
                userName = userRegistration.userName;
                profilePic = userRegistration.profilePic;
                isUseBot = isBot;
                isFTUE = userRegistration.isFTUE;
                lobbyId = _lobbyId;
                socketHandler.InternetCheckInitiate();
            }
            else
                chipsStoreHandler.GetChipsStoreData();
        }

        public void BackButton()
        {
            dashboardManager.PanelOnOff(dashboardManager.dashboardPanel, true);
            dashboardManager.PanelOnOff(dashboardManager.lobbyPanel, false);
            DestroyLobbyData();
        }

        void DestroyLobbyData()
        {
            foreach (var lobbyData in lobbyPrefabs)
            {
                lobbyData.playBtn.onClick.RemoveAllListeners();
                Destroy(lobbyData.gameObject);
            }
            lobbyPrefabs.Clear();
        }
    }

    [System.Serializable]
    public class GetLobbyData
    {
        public bool isCash;
        public int maxPlayer;
    }

    [System.Serializable]
    public class LobbyList
    {
        public string _id;
        public int entryfee;
        public int winningPrice;
        public bool isFTUE;
        public bool isCanPlay;
        public bool isUseBot;
    }

    [System.Serializable]
    public class GetLobbyDataResData
    {
        public List<LobbyList> lobbyList;
        public UserDataForLobby userData;
    }

    [System.Serializable]
    public class GetLobbyDataRes
    {
        public string message;
        public string status;
        public int statusCode;
        public bool success;
        public GetLobbyDataResData data;
    }

    [System.Serializable]
    public class UserDataForLobby
    {
        public string userName;
        public int coins;
        public string profileImage;
    }
}