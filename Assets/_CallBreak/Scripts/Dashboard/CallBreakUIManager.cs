using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

namespace FGSOfflineCallBreak
{
    public sealed class CallBreakUIManager : MonoBehaviour
    {
        public CallBreakPreLoaderController preLoaderController;

        public static CallBreakUIManager Instance;

        [Header("------------------ GameObjects ------------------")]

        public Sprite spadeSymbol;
        public Sprite diamondSymbol;
        public Sprite clubSymbol;
        public Sprite heartSymbol;

        [Header(" ------------------ UI Canvas ------------------")]
        [Space(10)]
        public Image cardSymbolToolTip;

        [Header(" ------------ List And Array ------------")]
        [Space(10)]

        public CallBreakSplashScreen splashScreen;
        public CallBreakRegisterUserController registerUserController;
        public CallBreakDashboardController dashboardController;
        public CallBreakCollectRewardController collectRewardController;
        public CallBreakNoInternetController noInternetController;
        public CallBreakEditProfileController editProfileController;
        public CallBreakItemPurchase itemPurchase;
        public CallBreakGamePlayController gamePlayController;
        public CallBreakProfileSelectionController profileSelectionController;
        public CallBreakScoreBoardController scoreBoardController;
        public CallBreakBidSelectionController bidSelectionController;
        public CallBreakWinnerLoserController winnerLoserController;
        public CallBreakDailyRewardManager dailyRewardManager;
        public CallBreakMenuController menuController;
        public CallBreakExitController exitController;
        public CallBreakRulesController rulesController;
        public CallBreakSpinnerController spinnerController;
        public CallBreakNotEnoughCoinsController notEnoughCoinsController;
        public CallBreakHowToPlay howToPlay;
        public CallBreakUserTurnTimer userTurnTimer;
        public CallBreakCollectRewardCoinAnimation rewardCoinAnimation;
        public CallBreakToolTipsController toolTipsController;

        public CallBreakSoundManager soundManager;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void QuitGame()
        {
            CallBreakSoundManager.PlaySoundEvent(SoundEffects.Click);
            Application.Quit();
        }

        public void PrivacyBtn()
        {
            CallBreakSoundManager.PlaySoundEvent(SoundEffects.Click);
            Application.OpenURL("https://finixgamesstudio.com/privacy-policy/");
        }

        public void RateUsBtn()
        {
            CallBreakSoundManager.PlaySoundEvent(SoundEffects.Click);
            string storeLink = string.Empty;
#if UNITY_ANDROID
            string playStoreLink = $"https://play.google.com/store/apps/details?id=";
            storeLink = playStoreLink + Application.identifier;
#elif UNITY_IPHONE
        
#else
        
#endif
            Application.OpenURL(storeLink);
        }
    }
}
