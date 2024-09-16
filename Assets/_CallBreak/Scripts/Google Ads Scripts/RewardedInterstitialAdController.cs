using System;
using UnityEngine;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Sample
{
    /// <summary>
    /// Demonstrates how to use Google Mobile Ads rewarded interstitial ads.
    /// </summary>
    [AddComponentMenu("GoogleMobileAds/Samples/RewardedInterstitialAdController")]
    public class RewardedInterstitialAdController : MonoBehaviour
    {
        /// <summary>
        /// UI element activated when an ad is ready to show.
        /// </summary>
        //public GameObject AdLoadedStatus;

        // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
        private const string _adUnitId = "ca-app-pub-5918737477932362/5602346620";
#elif UNITY_IPHONE
        private const string _adUnitId = "ca-app-pub-3940256099942544/6978759866";
#else
        private const string _adUnitId = "unused";
#endif

        private static RewardedInterstitialAd _rewardedInterstitialAd;

        /// <summary>
        /// Loads the ad.
        /// </summary>
        public static void LoadRewardedInterstitialAd()
        {
            // Clean up the old ad before loading a new one.
            if (_rewardedInterstitialAd != null)
            {
                DestroyAd();
            }

            Debug.Log("Loading rewarded interstitial ad.");

            // Create our request used to load the ad.
            var adRequest = new AdRequest();

            string adUnitId = string.Empty;

            if (CallBreakConstants.callBreakRemoteConfig.flagDetails.isSuccess)
#if UNITY_ANDROID
                adUnitId = CallBreakConstants.callBreakRemoteConfig.adsDetails.androidAdsIds.callBreakRewardedInterstitial;
#elif UNITY_IPHONE
            adUnitId = CallBreakConstants.callBreakRemoteConfig.adsDetails.iosAdsIds.callBreakRewardedInterstitial;
#else

#endif
            // Send the request to load the ad.
            else
                adUnitId = _adUnitId;

            // Send the request to load the ad.
            RewardedInterstitialAd.Load(_adUnitId, adRequest,
                (RewardedInterstitialAd ad, LoadAdError error) =>
                {
                    // If the operation failed with a reason.
                    if (error != null)
                    {
                        Debug.LogError("Rewarded interstitial ad failed to load an ad with error : " + error);
                        return;
                    }
                    // If the operation failed for unknown reasons.
                    // This is an unexpexted error, please report this bug if it happens.
                    if (ad == null)
                    {
                        Debug.LogError("Unexpected error: Rewarded interstitial load event fired with null ad and null error.");
                        return;
                    }

                    // The operation completed successfully.
                    Debug.Log("Rewarded interstitial ad loaded with response : " + ad.GetResponseInfo());
                    _rewardedInterstitialAd = ad;

                    // Register to ad events to extend functionality.
                    RegisterEventHandlers(ad);

                    // Inform the UI that the ad is ready.
                    //AdLoadedStatus?.SetActive(true);
                });
        }

        /// <summary>
        /// Shows the ad.
        /// </summary>
        public static void ShowRewardedInterstitialAd()
        {
            if (_rewardedInterstitialAd != null && _rewardedInterstitialAd.CanShowAd())
            {
                _rewardedInterstitialAd.Show((Reward reward) =>
                {
                    Debug.Log("Rewarded interstitial ad rewarded : " + reward.Amount);
                });
            }
            else
            {
                OnRewardedInterstitialAdNotReady?.Invoke();
                Debug.LogError("Rewarded interstitial ad is not ready yet.");
            }

            // Inform the UI that the ad is not ready.
            //AdLoadedStatus?.SetActive(false);
        }

        /// <summary>
        /// Destroys the ad.
        /// </summary>
        public static void DestroyAd()
        {
            if (_rewardedInterstitialAd != null)
            {
                Debug.Log("Destroying rewarded interstitial ad.");
                _rewardedInterstitialAd.Destroy();
                _rewardedInterstitialAd = null;
            }

            // Inform the UI that the ad is not ready.
            //AdLoadedStatus?.SetActive(false);
        }

        /// <summary>
        /// Logs the ResponseInfo.
        /// </summary>
        public void LogResponseInfo()
        {
            if (_rewardedInterstitialAd != null)
            {
                var responseInfo = _rewardedInterstitialAd.GetResponseInfo();
                UnityEngine.Debug.Log(responseInfo);
            }
        }

        // Raised when the ad is estimated to have earned money.
        public static event Action<AdValue> OnRewardedInterstitialAdPaid;
        // Raised when an impression is recorded for an ad.
        public static event Action OnRewardedInterstitialAdImpressionRecorded;
        // Raised when a click is recorded for an ad.
        public static event Action OnRewardedInterstitialAdClicked;
        // Raised when an ad opened full screen content.
        public static event Action OnRewardedInterstitialAdFullScreenContentOpened;
        // Raised when the ad closed full screen content.
        public static event Action OnRewardedInterstitialAdFullScreenContentClosed;
        // Raised when the ad failed to open full screen content.
        public static event Action<AdError> OnRewardedInterstitialAdFullScreenContentFailed;
        // Raise when the ad in not ready
        public static event Action OnRewardedInterstitialAdNotReady;

        private static void RegisterEventHandlers(RewardedInterstitialAd ad)
        {
            ad.OnAdPaid += (AdValue adValue) => OnRewardedInterstitialAdPaid?.Invoke(adValue);
            ad.OnAdImpressionRecorded += () => OnRewardedInterstitialAdImpressionRecorded?.Invoke();
            ad.OnAdClicked += () => OnRewardedInterstitialAdClicked?.Invoke();
            ad.OnAdFullScreenContentOpened += () => OnRewardedInterstitialAdFullScreenContentOpened?.Invoke();
            ad.OnAdFullScreenContentClosed += () =>
            {
                OnRewardedInterstitialAdFullScreenContentClosed?.Invoke();
                LoadRewardedInterstitialAd(); // Reload ad after it's closed
            };
            ad.OnAdFullScreenContentFailed += (AdError error) => OnRewardedInterstitialAdFullScreenContentFailed?.Invoke(error);
        }

    }
}
