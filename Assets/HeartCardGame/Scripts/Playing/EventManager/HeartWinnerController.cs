using GoogleMobileAds.Api;
using System;
using UnityEngine;

namespace FGSOfflineHeart
{
    public class HeartWinnerController : MonoBehaviour
    {
        [Header("Winner Declare Handler")]
        public HT_WinnerDeclareHandler winnerDeclareHandler;

        public void ShowAds()
        {
            if (CallBreakConstants.callBreakRemoteConfig.flagDetails.isAds)
            {
                if (CallBreakConstants.callBreakRemoteConfig.adsDetails.isShowInterstitialAdsOnScoreBoard)
                    GoogleMobileAds.Sample.InterstitialAdController.ShowInterstitialAd();
                else
                    winnerDeclareHandler.ContinueBtn();
            }
            else
                winnerDeclareHandler.ContinueBtn();
        }
        private void OnEnable()
        {
            GoogleMobileAds.Sample.InterstitialAdController.OnInterstitialAdFullScreenContentClosed += OnAdFullScreenContentClosedHandler;
            GoogleMobileAds.Sample.InterstitialAdController.OnInterstitialAdFullScreenContentFailed += OnAdFullScreenContentFailed;
            GoogleMobileAds.Sample.InterstitialAdController.OnInterstitialAdNotReady += OnInterstitialAdNotReady;
        }
        private void OnDisable()
        {
            GoogleMobileAds.Sample.InterstitialAdController.OnInterstitialAdFullScreenContentClosed -= OnAdFullScreenContentClosedHandler;
            GoogleMobileAds.Sample.InterstitialAdController.OnInterstitialAdFullScreenContentFailed -= OnAdFullScreenContentFailed;
            GoogleMobileAds.Sample.InterstitialAdController.OnInterstitialAdNotReady -= OnInterstitialAdNotReady;
        }

        private void OnAdFullScreenContentFailed(AdError error)
        {
            winnerDeclareHandler.ContinueBtn();
        }

        private void OnAdFullScreenContentClosedHandler()
        {
            winnerDeclareHandler.ContinueBtn();
        }

        private void OnInterstitialAdNotReady()
        {
            winnerDeclareHandler.ContinueBtn();
        }

    }
}