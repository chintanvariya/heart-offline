using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEditor.Experimental;
using GoogleMobileAds.Api;
using System;
using UnityEditor;
using FGSOfflineHeart;

namespace FGSOfflineCallBreak
{
    public enum LobbyType { Rookie, Newbie, Experienced, Gifted }
    public class CallBreakDashboardController : MonoBehaviour
    {
        [Header("HourlyRewardController")]
        public CallBreakHourlyRewardController hourlyRewardController;
        public CallBreakHourlyRewardController spinnerHourlyRewardController;

        [Header("Dashborad ICONS")]
        public List<GameObject> dashBoardIcons;
        public List<GameObject> allIcons;
        public List<GameObject> quickRoundObj;

        public Image standardModeBtn, quickModeBtn;
        public Sprite modeSelectBG, normalBG;
        public Color modeSelectColor, normalColor;
        public GameObject roundInfoToolTip;

        public TextMeshProUGUI userIDText;

        private const int DefaultRound = 5;

        public CallBreakProfileUiController profileUiController;

        [Header("LEVEL BUTTONS")]
        public List<Button> allLevelTypeButtons;
        public List<Image> allLevelType;
        public List<TextMeshProUGUI> allLevelTypeText;

        public CallBreakLobbyUiController lobbyPrefab;
        public Transform parentOfLobby;

        [Header("ALL LOBBIES")]
        public List<CallBreakLobbyUiController> allLobbies;
        public List<int> allLobbyAmount = new List<int>();
        public List<Sprite> practiesAndCoins;

        public Sprite freeLobbyBg;
        public Sprite coinLoobyBG;

        public void OpenScreen()
        {
            try
            {


                this.enabled = true;
                RoundBtnClick(DefaultRound);
                CallBreakGameManager.instance.selfUserDetails.userId = SystemInfo.deviceUniqueIdentifier.Substring(0, 8);
                userIDText.text = "User ID : " + CallBreakGameManager.instance.selfUserDetails.userId;

                profileUiController.OpenScreen();

                foreach (var item in allIcons)
                    item.transform.localScale = Vector3.one;

                allIcons[2].transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);


                allLobbyAmount = new List<int>();

                if (CallBreakConstants.callBreakRemoteConfig.flagDetails.isSuccess)
                {
                    allLobbyAmount = CallBreakConstants.callBreakRemoteConfig.levelDetails.allLobbyAmount;
                    CallBreakConstants.coinsToClearLevel = CallBreakConstants.callBreakRemoteConfig.levelDetails.coinsToClearLevel;
                }
                else
                {
                    List<int> lobbyValue1 = new List<int> { 0, 10, 20, 30, 40, 50, 100, 200, 300, 400, 500 };
                    allLobbyAmount = new List<int>(lobbyValue1);
                    int lobby = 1000;
                    for (int i = 0; i < 39; i++)
                    {
                        allLobbyAmount.Add(lobby);
                        lobby += 500;
                    }
                }

                Debug.Log("===============>" + allLobbies.Count);
                if (allLobbies.Count == 0)
                {
                    for (int i = 0; i < allLobbyAmount.Count; i++)
                    {
                        //Debug.Log("===============>" + i);
                        CallBreakLobbyUiController cloneOfLobby = Instantiate(lobbyPrefab, parentOfLobby);
                        cloneOfLobby.dashboardController = this;
                        allLobbies.Add(cloneOfLobby);
                    }
                }

                UpdateTheLobbiesDetails();

                SelectedLobbyType("Rookie");

                for (int i = 0; i < allLevelTypeButtons.Count; i++)
                    allLevelTypeButtons[i].interactable = false;

                for (int i = 0; i < CallBreakGameManager.instance.selfUserDetails.level; i++)
                    allLevelTypeButtons[i].interactable = true;

            }
            catch (Exception ex)
            {
                Debug.Log("=========>" + ex.ToString());
                throw;
            }
        }

        public void RoundSectionActivate(bool isActive)
        {
            foreach (var item in quickRoundObj)
            {
                item.SetActive(isActive);
            }
        }
        public void RoundBtnClick(int roundNumber)
        {
            RoundInfoObj(false);
            CallBreakSoundManager.PlaySoundEvent(SoundEffects.Click);
            string roundText = string.Empty;
            if (roundNumber == DefaultRound)
            {
                standardModeBtn.sprite = modeSelectBG;
                quickModeBtn.sprite = normalBG;
                RoundSectionActivate(true);
                roundText = "Standard";
            }
            else
            {
                quickModeBtn.sprite = modeSelectBG;
                standardModeBtn.sprite = normalBG;
                RoundSectionActivate(false);
                roundText = "Quick";
            }
            CallBreakGameManager.instance.totalRound = roundNumber;

            foreach (var item in allLobbies)
                item.UpdateRoundText(roundText);
        }



        public void OnButtonClicked(int buttonIndex)
        {
            foreach (var item in allIcons)
                item.transform.localScale = Vector3.one;

            allIcons[buttonIndex].transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

            switch (buttonIndex)
            {
                case 0:
                    CallBreakUIManager.Instance.dailyRewardManager.OpenScreen();
                    FirebaseController.instance.FirelogEvent("DailyBonus", "DashboardController", "CallBreak");
                    break;
                case 1:
                    CallBreakUIManager.Instance.itemPurchase.OpenScreen();
                    FirebaseController.instance.FirelogEvent("StoreStore", "DashboardController", "CallBreak");
                    break;
                case 2:
                    CallBreakUIManager.Instance.preLoaderController.OpenPreloader();
                    GoogleMobileAds.Sample.RewardedAdController.ShowRewardedAd();
                    FirebaseController.instance.FirelogEvent("100FreeCoins", "DashboardController", "CallBreak");
                    break;
                case 3:
                    CloseScreen();
                    if (spinnerHourlyRewardController.Ready())
                        CallBreakUIManager.Instance.spinnerController.OpenScreen(spinnerHourlyRewardController);
                    FirebaseController.instance.FirelogEvent("LuckySpin", "DashboardController", "CallBreak");
                    break;
                case 4:
                    CallBreakUIManager.Instance.collectRewardController.OpenCollectReward("Free Coins", 200, hourlyRewardController);
                    FirebaseController.instance.FirelogEvent("FreeCoins", "DashboardController", "CallBreak");
                    break;
                default:
                    break;
            }
        }

        public void InfoBtn(string roundState)
        {
            RoundInfoObj(true);

            if (roundState == "Standard")
            {
                roundInfoToolTip.GetComponent<RectTransform>().anchoredPosition = new Vector2(-175, -125);
                roundInfoToolTip.GetComponentInChildren<TextMeshProUGUI>().text = CallBreakConstants.StandardRoundInfoDes;
            }
            else
            {
                roundInfoToolTip.GetComponent<RectTransform>().anchoredPosition = new Vector2(115, -125);
                roundInfoToolTip.GetComponentInChildren<TextMeshProUGUI>().text = CallBreakConstants.QuickRoundInfoDes;
            }
        }

        public void RoundInfoObj(bool isActive)
        {
            roundInfoToolTip.SetActive(isActive);
        }


        public void UpdateTheLobbiesDetails()
        {
            for (int i = 0; i < allLobbies.Count; i++)
            {
                int lobbyAmount = allLobbyAmount[0];
                string keys = string.Empty;
                string round = "Standard";
                string playButton = string.Empty;
                string winAmount = string.Empty;

                Sprite practicesAndCoin;
                if (i == 0)
                {
                    keys = "Practice";
                    playButton = "Play";
                    winAmount = $"Free";
                    practicesAndCoin = practiesAndCoins[0];
                    allLobbies[i].bg.sprite = freeLobbyBg;
                }
                else
                {
                    lobbyAmount = allLobbyAmount[i];
                    keys = $"+{CallBreakUtilities.AbbreviateNumber(lobbyAmount / 2)}";
                    practicesAndCoin = practiesAndCoins[1];
                    playButton = $"Play {CallBreakUtilities.AbbreviateNumber(lobbyAmount)}";
                    winAmount = $"{CallBreakUtilities.AbbreviateNumber(lobbyAmount * 4)}";
                    allLobbies[i].bg.sprite = coinLoobyBG;
                }
                allLobbies[i].UpdateLobbyText(practicesAndCoin, lobbyAmount, keys, round, playButton, winAmount);
            }

            gameObject.SetActive(true);
        }



        public void SelectedLobbyType(string lobbyType)
        {
            foreach (var item in allLevelType)
                item.sprite = normalBG;
            foreach (var item in allLevelTypeText)
                item.color = normalColor;

            switch (lobbyType)
            {
                case "Rookie":
                    allLevelType[0].sprite = modeSelectBG;
                    allLevelTypeText[0].color = modeSelectColor;
                    ResetAllLobby(0, 6);
                    break;
                case "Newbie":
                    allLevelType[1].sprite = modeSelectBG;
                    allLevelTypeText[1].color = modeSelectColor;
                    ResetAllLobby(6, 11);
                    break;
                case "Experienced":
                    allLevelType[2].sprite = modeSelectBG;
                    allLevelTypeText[2].color = modeSelectColor;
                    ResetAllLobby(11, 16);
                    break;
                case "Gifted":
                    allLevelType[3].sprite = modeSelectBG;
                    allLevelTypeText[3].color = modeSelectColor;
                    ResetAllLobby(16, 21);
                    break;
                case "Expert":
                    allLevelType[4].sprite = modeSelectBG;
                    allLevelTypeText[4].color = modeSelectColor;
                    ResetAllLobby(21, 26);
                    break;
                case "HighRoller":
                    allLevelType[5].sprite = modeSelectBG;
                    allLevelTypeText[5].color = modeSelectColor;
                    ResetAllLobby(26, 31);
                    break;
                case "Millionaire":
                    allLevelType[6].sprite = modeSelectBG;
                    allLevelTypeText[6].color = modeSelectColor;
                    ResetAllLobby(31, 36);
                    break;
                case "Banker":
                    allLevelType[7].sprite = modeSelectBG;
                    allLevelTypeText[7].color = modeSelectColor;
                    ResetAllLobby(36, 41);
                    break;
                case "Tycoon":
                    allLevelType[8].sprite = modeSelectBG;
                    allLevelTypeText[8].color = modeSelectColor;
                    ResetAllLobby(41, 46);
                    break;
                case "Billonaire":
                    allLevelType[9].sprite = modeSelectBG;
                    allLevelTypeText[9].color = modeSelectColor;
                    ResetAllLobby(46, 50);
                    break;
                default:
                    break;
            }
        }

        public void ResetAllLobby(int start, int end)
        {
            foreach (var item in allLobbies)
                item.gameObject.SetActive(false);

            for (int i = start; i < end; i++)
                allLobbies[i].gameObject.SetActive(true);
        }

        public CallBreakLobbyUiController currentLobbyPlay;

        public void OnButtonPlayNow(CallBreakLobbyUiController lobbyUiController)
        {
            currentLobbyPlay = lobbyUiController;
            if (CallBreakConstants.callBreakRemoteConfig.adsDetails.isShowInterstitialAdsOnLobby)
            {
                CallBreakUIManager.Instance.preLoaderController.OpenPreloader();
                GoogleMobileAds.Sample.InterstitialAdController.ShowInterstitialAd();
            }
            else
                OnAdFullScreenContentClosedHandler();
        }

        private void OnEnable()
        {
            GoogleMobileAds.Sample.InterstitialAdController.OnInterstitialAdFullScreenContentClosed += OnAdFullScreenContentClosedHandler;
            GoogleMobileAds.Sample.InterstitialAdController.OnInterstitialAdFullScreenContentFailed += OnAdFullScreenContentFailed;
            GoogleMobileAds.Sample.InterstitialAdController.OnInterstitialAdNotReady += OnInterstitialAdNotReady;

            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdFullScreenContentFailed += OnRewardedAdFullScreenContentFailed;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdFullScreenContentClosed += OnRewardedAdFullScreenContentClosedHandler;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdNotReady += OnRewardedAdNotReady;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdGranted += OnRewardedAdGranted;

        }
        private void OnDisable()
        {
            GoogleMobileAds.Sample.InterstitialAdController.OnInterstitialAdFullScreenContentClosed -= OnAdFullScreenContentClosedHandler;
            GoogleMobileAds.Sample.InterstitialAdController.OnInterstitialAdFullScreenContentFailed -= OnAdFullScreenContentFailed;
            GoogleMobileAds.Sample.InterstitialAdController.OnInterstitialAdNotReady -= OnInterstitialAdNotReady;

            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdFullScreenContentFailed -= OnRewardedAdFullScreenContentFailed;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdFullScreenContentClosed -= OnRewardedAdFullScreenContentClosedHandler;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdNotReady -= OnRewardedAdNotReady;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdGranted -= OnRewardedAdGranted;
        }

        public void OnInterstitialAdNotReady()
        {
            CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
            //CallBreakUIManager.Instance.toolTipsController.OpenToolTips("AdsIsNotReady", "Ad is not ready yet !!", "");

            CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
            CallBreakGameManager.instance.selfUserDetails.userChips -= currentLobbyPlay.lobbyAmount;
            CallBreakConstants.UserDetialsJsonString = CallBreakUtilities.ReturnJsonString(CallBreakGameManager.instance.selfUserDetails);
            CallBreakGameManager.instance.lobbyAmount = currentLobbyPlay.lobbyAmount;

            HT_OfflineGameHandler.instance.StartGamePlay();

            profileUiController.CloseScreen();
            CloseScreen();

            //AdsIsNotReady
        }


        public void OnAdFullScreenContentFailed(AdError error)
        {
            CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
            HT_OfflineGameHandler.instance.StartGamePlay();

        }

        private void OnAdFullScreenContentClosedHandler()
        {
            if (CallBreakGameManager.instance.selfUserDetails.userChips < currentLobbyPlay.lobbyAmount)
            {
                CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
                CloseScreen();
                CallBreakUIManager.Instance.notEnoughCoinsController.OpenScreen("Not Enough Coins", "Insufficient coins! Watch a video for 500 free coins!", 500);
            }
            else
            {
                CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
                CallBreakGameManager.instance.selfUserDetails.userChips -= currentLobbyPlay.lobbyAmount;
                CallBreakConstants.UserDetialsJsonString = CallBreakUtilities.ReturnJsonString(CallBreakGameManager.instance.selfUserDetails);
                CallBreakGameManager.instance.lobbyAmount = currentLobbyPlay.lobbyAmount;

                HT_OfflineGameHandler.instance.StartGamePlay();

                profileUiController.CloseScreen();
                CloseScreen();
            }
        }

        public void OnRewardedAdFullScreenContentFailed(AdError error)
        {
            CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
        }

        public void OnRewardedAdGranted()
        {
            Debug.Log("CallBreakDashboardController || OnRewardedAdGranted ");
            CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
            CallBreakUIManager.Instance.rewardCoinAnimation.CollectCoinAnimation("100Coins");
        }

        public void OnRewardedAdFullScreenContentClosedHandler()
        {

        }

        public void OnRewardedAdNotReady()
        {
            CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
            CallBreakUIManager.Instance.toolTipsController.OpenToolTips("AdsIsNotReady", "Ad is not ready yet !!", "");
        }


        public void CloseScreen() => this.enabled = false;

    }
}

