using System;
using UnityEngine;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Sample
{
    /// <summary>
    /// Demonstrates how to use Google Mobile Ads interstitial ads.
    /// </summary>
    [AddComponentMenu("GoogleMobileAds/Samples/InterstitialAdController")]
    public class InterstitialAdController : MonoBehaviour
    {
        /// <summary>
        /// UI element activated when an ad is ready to show.
        /// </summary>
        //public GameObject AdLoadedStatus;

        // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
        private const string _adUnitId = "ca-app-pub-5918737477932362/9904746265";
#elif UNITY_IPHONE
        private const string _adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        private const string _adUnitId = "unused";
#endif

        private static InterstitialAd _interstitialAd;

        /// <summary>
        /// Loads the ad.
        /// </summary>
        public static void LoadInterstitialAd()
        {

            // Clean up the old ad before loading a new one.
            if (_interstitialAd != null)
            {
                DestroyAd();
            }

            Debug.Log("Loading interstitial ad.");

            // Create our request used to load the ad.
            var adRequest = new AdRequest();
            string adUnitId = string.Empty;

            if (CallBreakConstants.callBreakRemoteConfig.flagDetails.isSuccess)
#if UNITY_ANDROID
                adUnitId = CallBreakConstants.callBreakRemoteConfig.adsDetails.androidAdsIds.callBreakInterstitial;
#elif UNITY_IPHONE
                adUnitId = CallBreakConstants.callBreakRemoteConfig.adsDetails.iosAdsIds.callBreakInterstitial;
#else

#endif
            // Send the request to load the ad.
            else
                adUnitId = _adUnitId;

            InterstitialAd.Load(_adUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
            {
                // If the operation failed with a reason.
                if (error != null)
                {
                    Debug.LogError("Interstitial ad failed to load an ad with error : " + error);
                    return;
                }
                // If the operation failed for unknown reasons.
                // This is an unexpected error, please report this bug if it happens.
                if (ad == null)
                {
                    Debug.LogError("Unexpected error: Interstitial load event fired with null ad and null error.");
                    return;
                }

                // The operation completed successfully.
                Debug.Log("Interstitial ad loaded with response : " + ad.GetResponseInfo());
                _interstitialAd = ad;

                // Register to ad events to extend functionality.
                RegisterEventHandlers(ad);

                // Inform the UI that the ad is ready.
                //AdLoadedStatus?.SetActive(true);
            });
        }

        /// <summary>
        /// Shows the ad.
        /// </summary>
        public static void ShowInterstitialAd()
        {
            if (_interstitialAd != null && _interstitialAd.CanShowAd())
            {
                Debug.Log("Showing interstitial ad.");
                _interstitialAd.Show();
            }
            else
            {
                OnInterstitialAdNotReady?.Invoke();
                Debug.LogError("Interstitial ad is not ready yet.");
            }

            // Inform the UI that the ad is not ready.
            //AdLoadedStatus?.SetActive(false);
        }

        /// <summary>
        /// Destroys the ad.
        /// </summary>
        public static void DestroyAd()
        {
            if (_interstitialAd != null)
            {
                Debug.Log("Destroying interstitial ad.");
                _interstitialAd.Destroy();
                _interstitialAd = null;
            }

            // Inform the UI that the ad is not ready.
            //AdLoadedStatus?.SetActive(false);
        }

        /// <summary>
        /// Logs the ResponseInfo.
        /// </summary>
        public void LogResponseInfo()
        {
            if (_interstitialAd != null)
            {
                var responseInfo = _interstitialAd.GetResponseInfo();
                UnityEngine.Debug.Log(responseInfo);
            }
        }

        // Raised when the ad is estimated to have earned money.
        public static event Action<AdValue> OnInterstitialAdPaid;
        // Raised when an impression is recorded for an ad.
        public static event Action OnInterstitialAdImpressionRecorded;
        // Raised when a click is recorded for an ad.
        public static event Action OnInterstitialAdClicked;
        // Raised when an ad opened full screen content.
        public static event Action OnInterstitialAdFullScreenContentOpened;
        // Raised when the ad closed full screen content.
        public static event Action OnInterstitialAdFullScreenContentClosed;
        // Raised when the ad failed to open full screen content.
        public static event Action<AdError> OnInterstitialAdFullScreenContentFailed;
        // Raise when the ad in not ready
        public static event Action OnInterstitialAdNotReady;

        private static void RegisterEventHandlers(InterstitialAd ad)
        {
            ad.OnAdPaid += (AdValue adValue) => OnInterstitialAdPaid?.Invoke(adValue);
            ad.OnAdImpressionRecorded += () => OnInterstitialAdImpressionRecorded?.Invoke();
            ad.OnAdClicked += () => OnInterstitialAdClicked?.Invoke();
            ad.OnAdFullScreenContentOpened += () => OnInterstitialAdFullScreenContentOpened?.Invoke();
            ad.OnAdFullScreenContentClosed += () =>
            {
                OnInterstitialAdFullScreenContentClosed?.Invoke();
                LoadInterstitialAd(); // Reload ad after it's closed
            };
            ad.OnAdFullScreenContentFailed += (AdError error) => OnInterstitialAdFullScreenContentFailed?.Invoke(error);
        }
    }
}
