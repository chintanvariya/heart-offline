using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGSOfflineCallBreak
{
    public class FreeCoinsController : MonoBehaviour
    {
        private const string LastRewardTimeKey = "LastRewardTime";

        void Start()
        {
            CheckRewardEligibility();
        }

        private void CheckRewardEligibility()
        {
            if (PlayerPrefs.HasKey(LastRewardTimeKey))
            {
                Debug.Log(PlayerPrefs.GetString(LastRewardTimeKey));
                long lastRewardTimeTicks = long.Parse(PlayerPrefs.GetString(LastRewardTimeKey));
                DateTime lastRewardTime = new DateTime(lastRewardTimeTicks);
                TimeSpan timeSinceLastReward = DateTime.Now - lastRewardTime;

                if (timeSinceLastReward.TotalHours >= 24)
                {
                    GrantReward();
                }
                else
                {
                    Debug.Log("Not enough time has passed since the last reward.");
                }
            }
            else
            {
                Debug.Log("First time user or no record found, grant reward");

                // First time user or no record found, grant reward
                GrantReward();
            }
        }

        private void GrantReward()
        {
            // Grant the reward to the user
            Debug.Log("Reward granted to the user!");

            // Update the last reward time
            PlayerPrefs.SetString(LastRewardTimeKey, DateTime.Now.Ticks.ToString());
            PlayerPrefs.Save();
        }

        public void ClaimReward()
        {
            //ADS
            GoogleMobileAds.Sample.InterstitialAdController.ShowInterstitialAd();
        }

        private void OnEnable()
        {
            GoogleMobileAds.Sample.InterstitialAdController.OnInterstitialAdFullScreenContentClosed += OnAdFullScreenContentClosedHandler;
        }

        private void OnDisable()
        {
            GoogleMobileAds.Sample.InterstitialAdController.OnInterstitialAdFullScreenContentClosed -= OnAdFullScreenContentClosedHandler;
        }

        public void OnAdFullScreenContentClosedHandler()
        {
            GrantReward();
        }
    }
}