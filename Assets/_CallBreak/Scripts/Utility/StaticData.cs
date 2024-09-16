using UnityEngine;

namespace FGSOfflineCallBreak
{
    public static class StaticData
    {
        private const string ProfilePicKey = "ProfileKey";
        public const string ProfileNameKey = "ProfileName";
        private const string CoinBalanceKey = "CoinBalance";
        private const string RemoveAdsKey = "RemoveAdsKey";



        public const string ClaimRewardDayKey = "ClaimRewardDayKey";
        public const string ProfileDataSaveKey = "ProfileDataSaveKey";
        public const string UserID_Key = "UserID";
        public const string StatisticsKey = "StatisticsKey";

        public static int RemoveAds
        {
            get => PlayerPrefs.GetInt(RemoveAdsKey, 0);
            set => PlayerPrefs.SetInt(RemoveAdsKey, value);
        }





        public static string ClaimRewardDay
        {
            get => PlayerPrefs.GetString(ClaimRewardDayKey);
            set => PlayerPrefs.SetString(ClaimRewardDayKey, value);
        }

        public static string UserStatisticsData
        {
            get => PlayerPrefs.GetString(StatisticsKey);
            set => PlayerPrefs.SetString(StatisticsKey, value);
        }
    }
}
