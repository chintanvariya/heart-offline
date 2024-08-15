using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using DG.Tweening;

namespace HeartCardGame
{
    public class HT_DailyRewardHandler : MonoBehaviour
    {
        [Header("===== Object Scripts =====")]
        [SerializeField] private HT_SocketHandler socketHandler;
        [SerializeField] private List<HT_DailyRewardPrefabController> dailyRewardPrefabControllers;
        [SerializeField] private HT_DashboardManager dashboardManager;
        [SerializeField] private HT_UserRegistration userRegistration;
        [SerializeField] private HT_UiManager uiManager;
        [SerializeField] private HT_AudioManager audioManager;

        [Header("===== Model Class =====")]
        [SerializeField] private GetDailyRewardResponse getDailyRewardResponse;
        [SerializeField] private ClaimRewardResponse claimRewardResponse;

        public void GetDailyReward()
        {
            string url = socketHandler.serverUrl[(int)socketHandler.serverType];
            url += HT_StaticData.GetDailyRewardList;
            StartCoroutine(HT_APIManager.RequestWithPostData(url, "", (data) =>
            {
                getDailyRewardResponse = JsonConvert.DeserializeObject<GetDailyRewardResponse>(data);
                SetDailyReward();
            }, (error) => uiManager.ApiError(error)));
        }

        void SetDailyReward()
        {
            if (getDailyRewardResponse.success)
            {
                for (int i = 0; i < getDailyRewardResponse.data.Count; i++)
                {
                    var dailyRewardData = getDailyRewardResponse.data[i];
                    HT_DailyRewardPrefabController dailyRewardClone = dailyRewardPrefabControllers[i];
                    dailyRewardClone.DailyRewardSetting(dailyRewardData._id, dailyRewardData.day, dailyRewardData.coins, dailyRewardData.isClaimed, dailyRewardData.isLostClaim, dailyRewardData.isCurrentClaimed, false);
                    dailyRewardClone.claimBtn.onClick.AddListener(() =>
                    {
                        dailyRewardClone.particlesSystem.SetActive(true);
                        ClaimDailyReward();
                        audioManager.GamePlayAudioSetting(audioManager.spinResultCLip);
                        DOVirtual.DelayedCall(1.5f, () => dailyRewardClone.particlesSystem.SetActive(false));
                    });
                }
            }
        }

        public void ClaimDailyReward()
        {
            string url = socketHandler.serverUrl[(int)socketHandler.serverType];
            url += HT_StaticData.ClaimReward;
            StartCoroutine(HT_APIManager.RequestWithPostData(url, "", (data) =>
            {
                claimRewardResponse = JsonConvert.DeserializeObject<ClaimRewardResponse>(data);
                if (claimRewardResponse.success)
                {
                    uiManager.reconnectionPanel.SetActive(false);
                    dashboardManager.UserDataSetting(userRegistration.userName, claimRewardResponse.data.userCoins, userRegistration.profilePic, false);
                    for (int i = 0; i < claimRewardResponse.data.userDailyClaimData.Count; i++)
                    {
                        var claimRewardData = claimRewardResponse.data.userDailyClaimData[i];
                        HT_DailyRewardPrefabController dailyRewardClone = dailyRewardPrefabControllers[i];
                        dailyRewardClone.DailyRewardSetting(claimRewardData._id, claimRewardData.day, claimRewardData.coins, claimRewardData.isClaimed, claimRewardData.isLostClaim, claimRewardData.isCurrentClaimed, true);
                    }
                    Invoke(nameof(ClaimRewardPopupManage), 2f);
                }
                else
                    ClaimRewardPopupManage();
            }, (error) => uiManager.ApiError(error)));
        }

        void ClaimRewardPopupManage()
        {
            dashboardManager.PopupOnOff(dashboardManager.commonPopup, true);
            dashboardManager.commonPopupTxt.SetText($"{claimRewardResponse.message}");
            dashboardManager.PanelOnOff(dashboardManager.dailyRewardPanel, false);
        }
    }

    #region GetDailyRewardModelClass
    [System.Serializable]
    public class GetDailyRewardResponseData
    {
        public int day;
        public bool isClaimed;
        public bool isLostClaim;
        public string _id;
        public string id;
        public bool isCurrentClaimed;
        public int coins;
    }

    [System.Serializable]
    public class GetDailyRewardResponse
    {
        public string message;
        public string status;
        public int statusCode;
        public bool success;
        public List<GetDailyRewardResponseData> data;
    }
    #endregion

    #region ClaimRewardModelClass
    [System.Serializable]
    public class ClaimRewardResponseData
    {
        public List<UserDailyClaimDatum> userDailyClaimData;
        public int userCoins;
        public int claimDay;
    }

    [System.Serializable]
    public class ClaimRewardResponse
    {
        public string message;
        public string status;
        public int statusCode;
        public bool success;
        public ClaimRewardResponseData data;
    }

    [System.Serializable]
    public class UserDailyClaimDatum
    {
        public int day;
        public bool isClaimed;
        public bool isLostClaim;
        public string _id;
        public string id;
        public bool isCurrentClaimed;
        public int coins;
    }
    #endregion
}