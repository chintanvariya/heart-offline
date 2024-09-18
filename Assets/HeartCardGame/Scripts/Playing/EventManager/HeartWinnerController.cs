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
            GoogleMobileAds.Sample.InterstitialAdController.ShowInterstitialAd();
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