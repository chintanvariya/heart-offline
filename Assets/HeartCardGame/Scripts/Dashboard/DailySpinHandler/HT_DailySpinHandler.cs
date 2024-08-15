using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using DG.Tweening;
using System.Collections;
using System.Linq;
using System;
using UnityEngine.UI;

namespace HeartCardGame
{
    public class HT_DailySpinHandler : MonoBehaviour
    {
        [Header("===== Object Scripts =====")]
        [SerializeField] private HT_DashboardManager dashboardManager;
        [SerializeField] private HT_SocketHandler socketHandler;
        [SerializeField] private HT_UserRegistration userRegistration;
        [SerializeField] private HT_UiManager uiManager;
        [SerializeField] private HT_AudioManager audioManager;

        [Header("===== Spinner Data =====")]
        [SerializeField] private List<HT_DailySpinDataController> dailySpinDataControllers;
        public Button spinBtn, closeBtn;
        public GameObject particles;

        [Header("===== Model Class =====")]
        [SerializeField] private GetDailyWheelReponse getDailyWheelReponse;
        [SerializeField] private ClaimDailyWheelResponse claimDailyWheelResponse;

        [Header("===== Coroutines =====")]
        Coroutine SpinAnimationCoroutine;

        private float timeDuration;
        private float speed;
        private float currentAngleZ;
        public Transform spinnerBase;

        public void GetDailySpinBonus()
        {
            string url = socketHandler.serverUrl[(int)socketHandler.serverType];
            url += HT_StaticData.GetDailyWheelBonus;
            if (dashboardManager.IsAccessTokenAvailable())
                GetDailySpinBonusHandle(url);
            else
                userRegistration.UserRegister(PlayerPrefs.GetString("UserName"), (success) => GetDailySpinBonusHandle(url));
        }

        void GetDailySpinBonusHandle(string url)
        {
            StartCoroutine(HT_APIManager.RequestWithPostData(url, "", (data) =>
            {
                getDailyWheelReponse = JsonConvert.DeserializeObject<GetDailyWheelReponse>(data);
                if (getDailyWheelReponse.success)
                    SetDailySpinBonusData();
            }, (error) => uiManager.ApiError(error)));
        }

        public void ClickOnSpin()
        {
            timeDuration = 5f;

            spinBtn.interactable = false;
            closeBtn.interactable = false;

            SpeedIncrease();
            SpinAnimationCoroutine = StartCoroutine(SpinAnimation());

            Invoke(nameof(SpeedDecrease), 2f);
        }

        private void SpeedIncrease() => DOTween.To(() => speed, x => speed = x, UnityEngine.Random.Range(800, 900), 1f).SetEase(Ease.Linear);

        private void SpeedDecrease() => DOTween.To(() => speed, x => speed = x, 50, 2f).SetEase(Ease.Linear);

        private IEnumerator SpinAnimation()
        {
            audioManager.GamePlayAudioSetting(audioManager.spinClip);
            while (timeDuration > 0)
            {
                spinnerBase.Rotate(-Vector3.forward * speed * Time.deltaTime);
                timeDuration -= Time.deltaTime;
                if (dailySpinDataControllers.Select(x => x.originAngle).ToList().Contains(spinnerBase.eulerAngles.z))
                    Debug.Log($"COme {spinnerBase.eulerAngles.z}");
                yield return 0;
            }

            currentAngleZ = spinnerBase.eulerAngles.z;
            float number = currentAngleZ;
            float closest = dailySpinDataControllers.Select(x => x.originAngle).ToList().Aggregate((x, y) => Math.Abs(x - number) < Math.Abs(y - number) ? x : y);
            spinnerBase.transform.DORotate(new Vector3(0, 0, closest), 1f, RotateMode.FastBeyond360).SetEase(Ease.OutBack).OnComplete(() => SpinClaimedReq(closest));
        }

        private void SpinClaimedReq(float closest)
        {
            if (SpinAnimationCoroutine != null)
                StopCoroutine(SpinAnimationCoroutine);

            Debug.Log($"HT_DailyRewardHandler || SpinClaimedReq {closest}");
            int winIndex = dailySpinDataControllers.Find(x => x.originAngle == closest).index;

            string url = socketHandler.serverUrl[(int)socketHandler.serverType];
            url += HT_StaticData.ClaimDailyWheelBonus;
            StartCoroutine(HT_APIManager.RequestWithPostData(url, HT_APIEventManager.ClaimDailyWheelBonus(getDailyWheelReponse.data.day, winIndex), (data) =>
            {
                claimDailyWheelResponse = JsonConvert.DeserializeObject<ClaimDailyWheelResponse>(data);
                uiManager.reconnectionPanel.SetActive(false);
                particles.SetActive(true);
                if (claimDailyWheelResponse.success)
                {
                    audioManager.GamePlayAudioSetting(audioManager.spinResultCLip);
                    dashboardManager.UserDataSetting(userRegistration.userName, claimDailyWheelResponse.data.updatedCoins, userRegistration.profilePic, false);
                    userRegistration.dailySpeenBtn.interactable = false;
                    DOVirtual.DelayedCall(2f, () => SpinPopupManage(claimDailyWheelResponse.data.message));
                }
                else
                    SpinPopupManage(claimDailyWheelResponse.message);
            }, (error) => uiManager.ApiError(error)));
        }

        void SpinPopupManage(string message)
        {
            particles.SetActive(false);
            dashboardManager.PopupOnOff(dashboardManager.commonPopup, true);
            dashboardManager.commonPopupTxt.SetText($"{message}");
            dashboardManager.PanelOnOff(dashboardManager.dailySpinPanel, false);
        }

        void SetDailySpinBonusData()
        {
            for (int i = 0; i < getDailyWheelReponse.data.dailyWheelBonus.Count; i++)
            {
                var dailySpinData = getDailyWheelReponse.data.dailyWheelBonus[i];
                var dailySpinController = dailySpinDataControllers[i];
                dailySpinController.SetDailySpinData(dailySpinData.value, dailySpinData.index, dailySpinData.bonusType);
            }
        }
    }

    #region GetDailySpinModelClass
    [Serializable]
    public class DailyWheelBonus
    {
        public string bonusType;
        public string value;
        public bool isDeductTds;
        public int index;
    }

    [Serializable]
    public class GetDailyWheelReponseData
    {
        public int day;
        public List<DailyWheelBonus> dailyWheelBonus;
    }

    [Serializable]
    public class GetDailyWheelReponse
    {
        public string message;
        public string status;
        public int statusCode;
        public bool success;
        public GetDailyWheelReponseData data;
    }
    #endregion

    #region ClaimDailyWheelRequest
    [Serializable]
    public class ClaimDailyWheelBonus
    {
        public int day;
        public int index;
    }
    #endregion

    #region ClaimDailyWheelResponse
    [Serializable]
    public class ClaimDailyWheelResponseData
    {
        public int updatedCoins;
        public bool isClaimedDailyWheel;
        public DateTime lastClaimedDailyWheelDate;
        public string message;
    }

    [Serializable]
    public class ClaimDailyWheelResponse
    {
        public string message;
        public string status;
        public int statusCode;
        public bool success;
        public ClaimDailyWheelResponseData data;
    }
    #endregion
}