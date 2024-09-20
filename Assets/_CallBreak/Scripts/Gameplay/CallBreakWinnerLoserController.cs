using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using GoogleMobileAds.Api;
using System.Collections.Generic;
using System.Collections;
using FGSOfflineHeart;

namespace FGSOfflineCallBreak
{
    public class CallBreakWinnerLoserController : MonoBehaviour
    {
        public GameObject winSprite, loseSprite, drawSprite;

        public GameObject winCrown, winLabel;

        public TextMeshProUGUI rankText;

        public TextMeshProUGUI winCoinText;


        public GameObject bottomBarObj;

        private const string LOSE = "LOSE";
        private const string WINNER = "WINNER";
        private const int timerStart = 5;
        private string userStatus;
        private int totalWinAmount;
        private int totalWinPlayer = 1;

        public ParticleSystem winnerParticle01;
        public ParticleSystem winnerParticle02;



        public void DrawGame(int drawPlayer)
        {
            totalWinPlayer = drawPlayer;
        }


        public void OpenWinnerAndLosserScreen(bool isSelfPlayerWin, int myRank)
        {
            collectButton.gameObject.SetActive(false);
            collect2XButton.gameObject.SetActive(false);
            homeButton.gameObject.SetActive(false);

            CallBreakGameManager.instance.selfUserDetails.userKeys += float.Parse(CallBreakUIManager.Instance.dashboardController.currentLobbyPlay.keysAmount);
            CallBreakGameManager.instance.selfUserDetails.levelProgress += float.Parse(CallBreakUIManager.Instance.dashboardController.currentLobbyPlay.keysAmount);

            if (isSelfPlayerWin)
            {
                CallBreakGameManager.instance.selfUserDetails.userGameDetails.GameWon += 1;


                winSprite.SetActive(true); ;
                if (totalWinPlayer > 1)
                    drawSprite.SetActive(true); ;

                CallBreakSoundManager.PlaySoundEvent(SoundEffects.Win);
                WinObjActive(true);



                if (CallBreakUIManager.Instance.dashboardController.currentLobbyPlay.lobbyAmount <= 0)
                {
                    winCoinText.text = "Free";
                    homeButton.gameObject.SetActive(true);
                }
                else
                {
                    collectButton.gameObject.SetActive(true);
                    collect2XButton.gameObject.SetActive(true);
                    winCoinText.text = "+" + CallBreakUIManager.Instance.dashboardController.currentLobbyPlay.lobbyAmount * 4;
                }

                rankText.text = "1";

                rewardedCoins = CallBreakUIManager.Instance.dashboardController.currentLobbyPlay.lobbyAmount * 4;

                //HERE
                winnerParticle01.gameObject.SetActive(true);
                winnerParticle02.gameObject.SetActive(true);

                winnerParticle01.Play();
                winnerParticle02.Play();

                Invoke(nameof(ResetParticle), 1f);
            }
            else
            {
                CallBreakGameManager.instance.selfUserDetails.userGameDetails.GameLoss += 1;
                loseSprite.SetActive(true);
                WinObjActive(false);

                if (CallBreakUIManager.Instance.dashboardController.currentLobbyPlay.lobbyAmount <= 0)
                {
                    winCoinText.text = "Free";
                }
                else
                    winCoinText.text = "-" + CallBreakUIManager.Instance.dashboardController.currentLobbyPlay.lobbyAmount;

                rewardedCoins = 0;

                homeButton.gameObject.SetActive(true);

                rankText.text = myRank.ToString();
                CallBreakSoundManager.PlaySoundEvent(SoundEffects.Lose);
            }
            gameObject.SetActive(true);
        }


        void ResetParticle()
        {
            winnerParticle01.gameObject.SetActive(false);
            winnerParticle02.gameObject.SetActive(false);
        }

        private void WinObjActive(bool isActive)
        {
            winCrown.SetActive(isActive); winLabel.SetActive(isActive);
        }

        public void CollectCoinBtn()
        {
            //AdmobManager.instance.ShowInterstitialAd();

            if (userStatus == WINNER)
            {
                CallBreakGameManager.instance.selfUserDetails.userChips += (CallBreakGameManager.currentLobbyAmount * 4);
                CallBreakGameManager.instance.selfUserDetails.levelProgress += (CallBreakGameManager.currentLobbyAmount);
            }

            bottomBarObj.SetActive(true);
            timer = timerStart;

            //0
            InvokeRepeating(nameof(timer), 1, 1);
            //DOTween.To(() => timerStart, x => nextTimeCounterText.text = "New game begins in <color=#F9FF00>" + Mathf.Round(x).ToString() + "</color> seconds.", 0, timerStart).SetEase(Ease.Linear).OnComplete(() =>
            //{

            //});
        }

        public int timer;

        void Timer()
        {
            timer--;
            //nextTimeCounterText.text = $"New game begins in <color=#F9FF00>{timer}</color> seconds.";
            if (timer == 0)
            {
                CallBreakGameManager.instance.selfUserDetails.userGameDetails.GamePlayed += 1;
                CallBreakUIManager.Instance.scoreBoardController.ResetScoreBoardData();
                bottomBarObj.SetActive(false);
                StartCoroutine(CallBreakCardAnimation.instance.SetAndStartGamePlay(0f));
            }
        }

        public void OnButtonClicked(string buttonName)
        {
            Debug.Log(buttonName);
            switch (buttonName)
            {
                case "Collect":
                    CollectChips(1);
                    break;
                case "Collect2X":
                    CallBreakUIManager.Instance.preLoaderController.OpenPreloader();
                    GoogleMobileAds.Sample.RewardedInterstitialAdController.ShowRewardedInterstitialAd();
                    break;
                case "Close":
                    CallBreakUIManager.Instance.dashboardController.OpenScreen();
                    CallBreakUIManager.Instance.gamePlayController.CloseScreen();
                    gameObject.SetActive(false);
                    HT_GameManager.instance.ReloadScene();
                    break;
                case "Home":
                    CallBreakUIManager.Instance.dashboardController.OpenScreen();
                    CallBreakUIManager.Instance.gamePlayController.CloseScreen();
                    gameObject.SetActive(false);
                    HT_GameManager.instance.ReloadScene();
                    break;
                default:
                    break;
            }
        }
        public float leftTimer;

        public Button collect2XButton;
        public Button collectButton;
        public Button homeButton;


        public int rewardedCoins;

        public void CollectChips(int multiplier)
        {
            CallBreakGameManager.instance.selfUserDetails.userChips += rewardedCoins * multiplier;
            CallBreakUIManager.Instance.rewardCoinAnimation.CollectCoinAnimation("WinnerLoser");
        }

        private void OnEnable()
        {
            GoogleMobileAds.Sample.RewardedInterstitialAdController.OnRewardedInterstitialAdFullScreenContentClosed += OnAdFullScreenContentClosedHandler;
            GoogleMobileAds.Sample.RewardedInterstitialAdController.OnRewardedInterstitialAdFullScreenContentFailed += OnRewardedAdFullScreenContentFailed;
            GoogleMobileAds.Sample.RewardedInterstitialAdController.OnRewardedInterstitialAdNotReady += OnRewardedInterstitialAdNotReady;
        }
        private void OnDisable()
        {
            GoogleMobileAds.Sample.RewardedInterstitialAdController.OnRewardedInterstitialAdFullScreenContentClosed -= OnAdFullScreenContentClosedHandler;
            GoogleMobileAds.Sample.RewardedInterstitialAdController.OnRewardedInterstitialAdFullScreenContentFailed -= OnRewardedAdFullScreenContentFailed;
            GoogleMobileAds.Sample.RewardedInterstitialAdController.OnRewardedInterstitialAdNotReady -= OnRewardedInterstitialAdNotReady;
        }



        public void OnRewardedInterstitialAdNotReady()
        {
            CallBreakUIManager.Instance.toolTipsController.OpenToolTips("AdsIsNotReady", "Ad is not ready yet !!", "");
            collect2XButton.interactable = false;
        }

        public void OnRewardedAdFullScreenContentFailed(AdError error)
        {
            CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
        }

        private void OnAdFullScreenContentClosedHandler()
        {
            Debug.Log("OnAdFullScreenContentClosedHandler || ");
            CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
            CollectChips(2);
            //CallBreakUIManager.Instance.dashboardController.OpenScreen();
        }




    }
}
