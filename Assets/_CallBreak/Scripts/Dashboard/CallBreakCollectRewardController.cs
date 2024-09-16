using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;

namespace FGSOfflineCallBreak
{
    public class CallBreakCollectRewardController : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI collectRewardedText;
        public TMPro.TextMeshProUGUI collectRewardedTitleText;
        public TMPro.TextMeshProUGUI collectRewardedDespText;

        public bool adsIsNotReady;

        public int rewardedCoins;

        public Button collect2XButton;
        public Button collectButton;

        public Image timerSlider;
        public TMPro.TextMeshProUGUI timerSliderText;

        public ulong runningTimer;
        public float maxTime;

        public CallBreakHourlyRewardController hourlyRewardController;

        public void OpenCollectReward(string title, int reward, CallBreakHourlyRewardController controller)
        {
            collectRewardedText.text = title;
            rewardedCoins = reward;
            collectRewardedText.text = CallBreakUtilities.AbbreviateNumber(rewardedCoins);

            CallBreakUIManager.Instance.dashboardController.CloseScreen();
            hourlyRewardController = controller;

            if (hourlyRewardController.Ready())
            {
                collect2XButton.interactable = true;
                collectButton.interactable = true;
            }
            else
            {
                collect2XButton.interactable = false;
                collectButton.interactable = false;
            }

            this.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (hourlyRewardController.Ready())
            {
                //collect2XButton.interactable = true;
                //collectButton.interactable = true;
                //timerSlider.transform.parent.gameObject.SetActive(false);
                return;
            }
            else
            {

            }
            ulong diff = ((ulong)DateTime.Now.Ticks - hourlyRewardController.lastTimeClicked);
            ulong m = diff / TimeSpan.TicksPerMillisecond;
            float secondsLeft = (float)(hourlyRewardController.miliSecondsToWait - m) / 1000.0f;
            string timerString = "";
            timerString += ((int)secondsLeft / 3600).ToString() + " : ";
            secondsLeft -= ((int)secondsLeft / 3600) * 3600;
            timerString += ((int)secondsLeft / 60).ToString("00") + " : ";
            timerString += (secondsLeft % 60).ToString("00");
            timerSliderText.text = timerString;
        }

        public void OnButtonClicked(string buttonName)
        {
            Debug.Log(buttonName);
            switch (buttonName)
            {
                case "Collect":
                    CollectChips(1);
                    hourlyRewardController.OnButtonClicked();
                    break;
                case "Collect2X":
                    CallBreakUIManager.Instance.preLoaderController.OpenPreloader();
                    GoogleMobileAds.Sample.RewardedAdController.ShowRewardedAd();
                    break;
                case "Close":
                    CallBreakUIManager.Instance.dashboardController.OpenScreen();
                    gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }



        public void CollectChips(int multiplier)
        {
            hourlyRewardController.OnButtonClicked();
            rewardedCoins = rewardedCoins * multiplier;
            CallBreakUIManager.Instance.rewardCoinAnimation.CollectCoinAnimation("CollectReward");
        }

        private void OnEnable()
        {
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdFullScreenContentClosed += OnAdFullScreenContentClosedHandler;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdFullScreenContentFailed += OnRewardedAdFullScreenContentFailed;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdNotReady += OnRewardedAdNotReady;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdGranted += OnRewardedAdGranted;
        }
        private void OnDisable()
        {
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdFullScreenContentClosed -= OnAdFullScreenContentClosedHandler;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdFullScreenContentFailed -= OnRewardedAdFullScreenContentFailed;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdNotReady -= OnRewardedAdNotReady;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdGranted -= OnRewardedAdGranted;
        }

        public void OnRewardedAdNotReady()
        {
            Debug.Log("OnRewardedAdNotReady || ");
            CallBreakUIManager.Instance.toolTipsController.OpenToolTips("AdsIsNotReady", "Ad is not ready yet !!", "");
            CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
            collect2XButton.interactable = false;
        }

        public void OnRewardedAdFullScreenContentFailed(AdError error)
        {
            CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
        }

        void OnRewardedAdGranted()
        {
            Debug.Log("OnAdFullScreenContentClosedHandler || ");
            CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
            CollectChips(2);
            CallBreakUIManager.Instance.dashboardController.OpenScreen();
        }

        private void OnAdFullScreenContentClosedHandler()
        {
            CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
        }
    }
}
