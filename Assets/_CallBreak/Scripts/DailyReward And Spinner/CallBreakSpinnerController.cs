using GoogleMobileAds.Api;
using I2.MiniGames;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace FGSOfflineCallBreak
{
    public class CallBreakSpinnerController : MonoBehaviour
    {
        public Button adsSpinButton;
        public Button spinButton;

        public CallBreakHourlyRewardController hourlyRewardController;

        public MiniGame_Controller miniGame_Controller;

        public void OpenScreen(CallBreakHourlyRewardController controller)
        {
            adsSpinButton.gameObject.SetActive(false);
            spinButton.gameObject.SetActive(true);

            adsSpinButton.interactable = true;
            isFromAds = false;
            hourlyRewardController = controller;
            gameObject.SetActive(true);
        }
        public bool isFromAds;

        public static System.Action<float> GetReward;

        public float rewardOfSpinner;

        public void GetRewardOnSpinComplete(float coins)
        {
            hourlyRewardController.OnButtonClicked();

            rewardOfSpinner = coins;
            Debug.LogError($"<color><b>CallBreakSpinnerController || GetRewardOnSpinComplete {coins}</b></color>");

            adsSpinButton.gameObject.SetActive(true);

            CallBreakUIManager.Instance.rewardCoinAnimation.CollectCoinAnimation("Spinner");
        }

        public void SpinButtonClicked() => hourlyRewardController.OnButtonClicked();

        public void ShowAds()
        {
            isFromAds = true;
            adsSpinButton.interactable = false;
            GoogleMobileAds.Sample.RewardedAdController.ShowRewardedAd();
        }

        public void CloseScreen()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            GetReward += GetRewardOnSpinComplete;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdFullScreenContentClosed += OnAdFullScreenContentClosedHandler;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdFullScreenContentFailed += OnRewardedAdFullScreenContentFailed;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdNotReady += OnRewardedAdNotReady;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdGranted += OnRewardedAdGranted;
        }
        private void OnDisable()
        {
            GetReward -= GetRewardOnSpinComplete;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdFullScreenContentClosed -= OnAdFullScreenContentClosedHandler;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdFullScreenContentFailed -= OnRewardedAdFullScreenContentFailed;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdNotReady -= OnRewardedAdNotReady;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdGranted -= OnRewardedAdGranted;
        }

        public void OnRewardedAdNotReady()
        {
            CallBreakUIManager.Instance.toolTipsController.OpenToolTips("AdsIsNotReady", "Ad is not ready yet !!", "");
            CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
            CloseScreen();
        }

        public void OnRewardedAdFullScreenContentFailed(AdError error)
        {
            CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
            CloseScreen();
        }

        void OnRewardedAdGranted()
        {
            Debug.Log("OnRewardedAdGranted || ");
            CallBreakUIManager.Instance.preLoaderController.ClosePreloader();

            adsSpinButton.gameObject.SetActive(false);
            spinButton.gameObject.SetActive(true);

        }

        private void OnAdFullScreenContentClosedHandler()
        {
            Debug.Log("OnAdFullScreenContentClosedHandler || ");

        }


    }
}