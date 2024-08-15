using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.UI;
using System;

namespace HeartCardGame
{
    public class HT_UserRegistration : MonoBehaviour
    {
        [Header("===== Object Scripts =====")]
        [SerializeField] private HT_SocketHandler socketHandler;
        [SerializeField] private HT_DashboardManager dashboardManager;
        [SerializeField] private HT_DailyRewardHandler dailyRewardHandler;
        [SerializeField] private HT_DailySpinHandler dailySpinHandler;
        [SerializeField] private HT_UiManager uiManager;

        [Header("===== Dashboard Object =====")]
        public Button dailySpeenBtn;
        public Button leaderboardBtn;

        [Header("===== Profile Data =====")]
        public string userId;
        public string userName, accessToken, profilePic;
        public bool isFTUE, isUseBOt;

        [Header("===== Model Class =====")]
        [SerializeField] private UserRegisterRes userRegisterResponse;

        public void UserRegister(string playerName, Action<bool> action = null)
        {
            string url = socketHandler.serverUrl[(int)socketHandler.serverType];
            url += HT_StaticData.UserRegister;
            StartCoroutine(HT_APIManager.RequestWithPostData(url, HT_APIEventManager.UserRegister(playerName), (data) =>
            {
                userRegisterResponse = JsonConvert.DeserializeObject<UserRegisterRes>(data);
                if (userRegisterResponse.success)
                {
                    action?.Invoke(true);
                    UserDataSet();
                    dashboardManager.PanelOnOff(dashboardManager.enterNamePanel, false);
                    dashboardManager.PanelOnOff(dashboardManager.dashboardPanel, true);
                    if (!userRegisterResponse.data.isClaimedDailyReward)
                    {
                        dailyRewardHandler.GetDailyReward();
                        dashboardManager.PanelOnOff(dashboardManager.dailyRewardPanel, true);
                    }

                    if (!userRegisterResponse.data.isClaimedDailyWheel)
                        dailySpeenBtn.interactable = true;
                }
            }, (error) => uiManager.ApiError(error, true)));
        }

        public void UserDataSet()
        {
            dashboardManager.AllPanelOff();
            userId = userRegisterResponse.data.userData._id;
            userName = userRegisterResponse.data.userData.userName;
            accessToken = userRegisterResponse.data.userData.token;
            PlayerPrefs.SetString("Token", accessToken);
            profilePic = userRegisterResponse.data.userData.profileImage;
            isFTUE = userRegisterResponse.data.userData.isFTUE;
            isUseBOt = userRegisterResponse.data.userData.isBot;
            dashboardManager.UserDataSetting(userRegisterResponse.data.userData.userName, userRegisterResponse.data.userData.coins, userRegisterResponse.data.userData.profileImage, true);
            dailySpinHandler.GetDailySpinBonus();
        }
    }

    #region UserRegisterModelClass
    [System.Serializable]
    public class UserRegisterData
    {
        public string deviceId;
        public string token;
        public string deviceType;
        public string userName;
    }

    [System.Serializable]
    public class UserRegister
    {
        public UserRegisterData data;
    }

    [System.Serializable]
    public class UserRegisterResData
    {
        public UserData userData;
        public bool isClaimedDailyReward;
        public bool isClaimedDailyWheel;
        public bool isUserFound;
        public string message;
    }

    [System.Serializable]
    public class UserRegisterRes
    {
        public string message;
        public string status;
        public int statusCode;
        public bool success;
        public UserRegisterResData data;
    }

    [System.Serializable]
    public class UserData
    {
        public string userName;
        public string profileImage;
        public bool isFTUE;
        public bool isBot;
        public int coins;
        public string useAvatar;
        public List<string> purchaseAvatars;
        public string _id;
        public string token;
    }
    #endregion
}