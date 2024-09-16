using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGSOfflineCallBreak
{
    public class CallBreakRemoteConfigClass
    {
        [Serializable]
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class AdsIds
        {
            public string callBreakAppOpen;
            public string callBreakBanner;
            public string callBreakInterstitial;
            public string callBreakReward;
            public string callBreakRewardedInterstitial;
        }

        [Serializable]
        public class CallBreakRemoteConfig
        {
            public FlagDetails flagDetails;
            public AdsDetails adsDetails;
            public LevelDetails levelDetails;
        }
        [Serializable]
        public class LevelDetails
        {
            public List<int> coinsToClearLevel = new List<int>();
            public List<int> allLobbyAmount = new List<int>();
        }
        [Serializable]
        public class FlagDetails
        {
            public bool isAds;
            public bool isSuccess;
            public bool isForceUpdate;
        }
        [Serializable]
        public class AdsDetails
        {
            public bool isShowInterstitialAdsOnLobby;
            public bool isShowInterstitialAdsOnScoreBoard;
            public int numberOfAds;
            public AdsIds androidAdsIds;
            public AdsIds iosAdsIds;
        }
    }
}