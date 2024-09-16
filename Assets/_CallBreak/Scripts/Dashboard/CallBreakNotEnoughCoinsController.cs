using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FGSOfflineCallBreak
{

    public class CallBreakNotEnoughCoinsController : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI titleText;
        public TMPro.TextMeshProUGUI descriptionText;

        public int rewardCoins;

        public void OpenScreen(string title, string description, int reward)
        {
            rewardCoins = reward;
            titleText.text = title;
            descriptionText.text = description;
            gameObject.SetActive(true);
        }

        public void OnButtonClicked(string buttonName)
        {
            switch (buttonName)
            {
                case "Yes":

                    break;
                case "No":
                    CloseScreen();
                    break;
            }
        }

        public void OnButtonClicked()
        {
            CallBreakUIManager.Instance.preLoaderController.OpenPreloader();
            GoogleMobileAds.Sample.RewardedAdController.ShowRewardedAd();
        }

        private void OnEnable()
        {
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdFullScreenContentFailed += OnRewardedAdFullScreenContentFailed;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdFullScreenContentClosed += OnRewardedAdFullScreenContentClosedHandler;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdNotReady += OnRewardedAdNotReady;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdGranted += OnRewardedAdGranted;

        }
        private void OnDisable()
        {
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdFullScreenContentFailed -= OnRewardedAdFullScreenContentFailed;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdFullScreenContentClosed -= OnRewardedAdFullScreenContentClosedHandler;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdNotReady -= OnRewardedAdNotReady;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdGranted -= OnRewardedAdGranted;
        }

        public void OnRewardedAdFullScreenContentFailed(AdError adError)
        {
            CloseScreen();
        }

        public void OnRewardedAdGranted()
        {
            Debug.Log("CallBreakNotEnoughCoinsController || OnRewardedAdGranted ");
            CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
            CallBreakGameManager.instance.selfUserDetails.userChips += rewardCoins;
            CallBreakConstants.UserDetialsJsonString = CallBreakUtilities.ReturnJsonString(CallBreakGameManager.instance.selfUserDetails);
            CallBreakUIManager.Instance.dashboardController.profileUiController.UpdateUserChips();
            CallBreakUIManager.Instance.dashboardController.OpenScreen();
            gameObject.SetActive(false);
        }

        public void OnRewardedAdFullScreenContentClosedHandler()
        {
            CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
        }
        public void OnRewardedAdNotReady()
        {
            CallBreakUIManager.Instance.toolTipsController.OpenToolTips("AdsIsNotReady", "Ad is not ready yet !!", "");
            CloseScreen();
        }


        public void CloseScreen()
        {
            CallBreakUIManager.Instance.dashboardController.OpenScreen();
            CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
            gameObject.SetActive(false);
        }
    }
}
