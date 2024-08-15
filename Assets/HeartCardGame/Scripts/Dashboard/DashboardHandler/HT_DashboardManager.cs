using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using DG.Tweening;

namespace HeartCardGame
{
    public class HT_DashboardManager : MonoBehaviour
    {
        [Header("===== Profile Data =====")]
        [SerializeField] private Image profileImg;
        public TextMeshProUGUI userNameTxt, balanceTxt;
        [SerializeField] private GameObject loader;
        public List<AvatarDatum> profilePics;

        [Header("===== Panels =====")]
        public GameObject lobbyPanel;
        public GameObject dashboardPanel, profilePanel, allAvatarPanel, chipsStorePanel, dailySpinPanel, enterNamePanel, leaderboardPanel, dailyRewardPanel, commonPopup, howToPlayPanel;
        public List<GameObject> allPanels;
        public TextMeshProUGUI commonPopupTxt;

        [Header("===== Object Scripts =====")]
        [SerializeField] private HT_UiManager uiManager;
        [SerializeField] private HT_SocketHandler socketHandler;
        [SerializeField] private HT_UserRegistration userRegistration;
        [SerializeField] private HT_ProfileHandler profileHandler;
        [SerializeField] private HT_LobbyHandler lobbyHandler;
        [SerializeField] private HT_DailySpinHandler dailySpinHandler;
        [SerializeField] private HT_GameManager gameManager;

        [Header("===== Buttons =====")]
        [SerializeField] private Button settingBtn;

        [Header("===== Model Class =====")]
        [SerializeField] private GetAvatarResponse getAvatarResponse;
        [SerializeField] private RunningGameResponse runningGameResponse;
        [SerializeField] private GetChipsStore getChipsStore;

        [Header("===== Actions =====")]
        public Action<string> UserNameAction;
        public Action<float> BalanceAction;
        public Action<string> ProfileAction;

        [Header("===== Enums =====")]
        public ErrorType errorType;

        Coroutine TextureCor;

        private void Start()
        {
            UserNameAction += UserNameSetting;
            BalanceAction += BalanceSetting;
            ProfileAction += ProfilePicSetting;

            RunningGame();
        }

        private void BalanceSetting(float balance) => balanceTxt.SetText($"{balance}");

        private void UserNameSetting(string userName) => userNameTxt.SetText($"{userName}");

        public void UserDataSetting(string name, float coin, string profileURL, bool isRequiredProfile)
        {
            UserNameAction?.Invoke(name);
            BalanceAction?.Invoke(coin);
            if (isRequiredProfile)
                ProfileAction?.Invoke(profileURL);
            userRegistration.userName = name;
            userRegistration.profilePic = profileURL;
        }

        public void RunningGame()
        {
            string url = socketHandler.serverUrl[(int)socketHandler.serverType];
            url += HT_StaticData.RunningGame;

            if (!socketHandler.IsInternetConnectionAvailable())
            {
                gameManager.myUserSprite = HT_OfflineGameHandler.instance.spritesList[0];
                profileImg.sprite = gameManager.myUserSprite;
                loader.SetActive(false);
            }

            StartCoroutine(HT_APIManager.RequestWithPostData(url, "", (data) =>
            {
                runningGameResponse = JsonConvert.DeserializeObject<RunningGameResponse>(data);
                if (runningGameResponse.data.isRunningGame)
                {
                    PlayerPrefs.SetString("Token", runningGameResponse.data.tableData.userDetail.accessToken);
                    SignUpDataSetting(); // Redirect into game
                    socketHandler.InternetCheckInitiate();
                }

                if (!runningGameResponse.data.isRunningGame && !runningGameResponse.data.isRegister)
                {
                    enterNamePanel.SetActive(true); // Open Register Panel
                    return;
                }

                if (!runningGameResponse.data.isRunningGame)
                    userRegistration.UserRegister(PlayerPrefs.GetString("UserName")); // Open Dashboard
            }, (error) => uiManager.ApiError(error, true)));
        }

        public void ProfilePicSetting(string profileURL)
        {
            TextureCor = StartCoroutine(uiManager.GetTexture(profileURL, loader, (sprite) =>
            {
                profileImg.sprite = sprite;
                gameManager.myUserSprite = sprite;
                if (TextureCor != null)
                    StopCoroutine(TextureCor);
            }));
        }

        public void PanelOnOff(GameObject panel, bool active) => panel.SetActive(active);

        public void ClickOnProfile()
        {
            string url = socketHandler.serverUrl[(int)socketHandler.serverType];
            url += HT_StaticData.GetAvatars;
            if (IsAccessTokenAvailable())
                GetAvatarHandle(url);
            else
                userRegistration.UserRegister(PlayerPrefs.GetString("UserName"), (success) => GetAvatarHandle(url));
        }

        void GetAvatarHandle(string url)
        {
            StartCoroutine(HT_APIManager.RequestWithPostData(url, "", (data) =>
            {
                getAvatarResponse = JsonConvert.DeserializeObject<GetAvatarResponse>(data);
                if (getAvatarResponse.success)
                {
                    GamePlayDataSet();
                    leaderboardPanel.SetActive(false);
                    dailySpinPanel.SetActive(false);
                    uiManager.settingPanel.SetActive(false);
                    PanelOnOff(profilePanel, true);
                    profilePics = new List<AvatarDatum>(getAvatarResponse.data.avatarData);
                }
            }, (error) => uiManager.ApiError(error)));
        }

        void GamePlayDataSet()
        {
            profileHandler.totalGamePlayTxt.SetText($"{getAvatarResponse.data.playedGamesData.totalGames}");
            profileHandler.gameWinTxt.SetText($"{getAvatarResponse.data.playedGamesData.win}");
            profileHandler.gameLossTxt.SetText($"{getAvatarResponse.data.playedGamesData.loss}");
        }

        public void PopupOnOff(GameObject popup, bool isOpen)
        {
            RectTransform rt = popup.transform.GetChild(0).GetComponent<RectTransform>();
            if (isOpen)
            {
                popup.SetActive(true);
                rt.localScale = Vector2.zero;
                rt.DOBlendableScaleBy(Vector2.one, 0.5f);
            }
            else
            {
                popup.SetActive(false);
                rt.localScale = Vector2.zero;
            }
        }

        void SignUpDataSetting()
        {
            var runningGameData = runningGameResponse.data.tableData;
            lobbyHandler.lobbyId = runningGameData.lobbyId;
            lobbyHandler.entryFees = runningGameData.entryFee;
            lobbyHandler.winninAmount = runningGameData.winningAmount;
            lobbyHandler.isUseBot = runningGameData.isUseBot;
            lobbyHandler.isFTUE = runningGameData.isFTUE;
            lobbyHandler.userId = runningGameData.userDetail.userId;
            lobbyHandler.userName = runningGameData.userDetail.name;
            lobbyHandler.profilePic = runningGameData.userDetail.pp;
        }

        public void BackButtonOnPopup(GameObject panel) => PanelOnOff(panel, false);

        public void ClickOnDailySpinBtn()
        {
            dailySpinPanel.SetActive(true);
            dailySpinHandler.spinBtn.interactable = true;
        }

        public void AllPanelOff()
        {
            foreach (var item in allPanels)
            {
                item.SetActive(false);
            }
        }

        public bool IsAccessTokenAvailable()
        {
            if (userRegistration.accessToken != string.Empty)
                return true;
            return false;
        }
    }

    public enum ErrorType
    {
        Alert,
        Error
    }

    #region GetAvatarResponse
    [Serializable]
    public class AvatarDatum
    {
        public string _id;
        public string avatarImage;
        public bool isFree;
        public int coins;
        public string id;
        public bool isCanBuy;
        public bool isPurchase;
        public bool isUsedAvatar;
    }

    [Serializable]
    public class PlayedGamesData
    {
        public int win;
        public int loss;
        public int tie;
        public int totalGames;
    }

    [Serializable]
    public class GetAvatarResponseData
    {
        public List<AvatarDatum> avatarData;
        public PlayedGamesData playedGamesData;
        public UserDataProfile userData;
    }

    [Serializable]
    public class UserDataProfile
    {
        public string userName;
        public int coins;
        public string profileImage;
    }

    [Serializable]
    public class GetAvatarResponse
    {
        public string message;
        public string status;
        public int statusCode;
        public bool success;
        public GetAvatarResponseData data;
    }
    #endregion

    #region RunningGame
    [Serializable]
    public class RunningGameResponseData
    {
        public bool isRunningGame;
        public bool isRegister;
        public TableData tableData;
    }

    [Serializable]
    public class RunningGameResponse
    {
        public string message;
        public string status;
        public int statusCode;
        public bool success;
        public RunningGameResponseData data;
    }

    [Serializable]
    public class TableData
    {
        public int minPlayer;
        public int maxPlayer;
        public string lobbyId;
        public string gameId;
        public int entryFee;
        public int winningAmount;
        public bool isUseBot;
        public bool isFTUE;
        public string gameType;
        public UserDetail userDetail;
    }

    [Serializable]
    public class UserDetail
    {
        public string userId;
        public int si;
        public string name;
        public string pp;
        public string userState;
        public int coins;
        public string accessToken;
    }
    #endregion
}