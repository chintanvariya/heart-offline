using GoogleMobileAds.Api;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace FGSOfflineCallBreak
{
    public class CallBreakScoreBoardController : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI currentRoundText;

        public List<TMPro.TextMeshProUGUI> playerOneRoundsText;
        public List<TMPro.TextMeshProUGUI> playerTwoRoundsText;
        public List<TMPro.TextMeshProUGUI> playerThreeRoundsText;
        public List<TMPro.TextMeshProUGUI> playerFourRoundsText;

        public List<TMPro.TextMeshProUGUI> allPlayerName;
        public List<Image> allPlayerProfilePicture;
        public List<GameObject> allPlayerCrown;

        public GameObject closeButton;
        public GameObject continueButton;

        public void ResetScoreBoardData()
        {
            ResetDataOfText(playerOneRoundsText);
            ResetDataOfText(playerTwoRoundsText);
            ResetDataOfText(playerThreeRoundsText);
            ResetDataOfText(playerFourRoundsText);

            for (int i = 0; i < allPlayerCrown.Count; i++)
                allPlayerCrown[i].SetActive(false);

        }

        public void ResetDataOfText(List<TMPro.TextMeshProUGUI> currentList)
        {
            for (int i = 0; i < playerOneRoundsText.Count; i++)
            {
                currentList[i].color = Color.white;
                currentList[i].text = "0";
            }
        }

        public void OpenCurrentRoundScoreBoard()
        {
            closeButton.SetActive(true);
            continueButton.SetActive(false);
            gameObject.SetActive(true);
        }

        public void OpenScreen(int winnerIndex)
        {
            allPlayerCrown[winnerIndex].SetActive(true);
            switch (winnerIndex)
            {
                case 0:
                    playerOneRoundsText[CallBreakGameManager.instance.currentRound - 1].color = Color.green;
                    playerOneRoundsText[5].color = Color.green;
                    break;
                case 1:
                    playerTwoRoundsText[CallBreakGameManager.instance.currentRound - 1].color = Color.green;
                    playerTwoRoundsText[5].color = Color.green;
                    break;
                case 2:
                    playerThreeRoundsText[CallBreakGameManager.instance.currentRound - 1].color = Color.green;
                    playerThreeRoundsText[5].color = Color.green;
                    break;
                case 3:
                    playerFourRoundsText[CallBreakGameManager.instance.currentRound - 1].color = Color.green;
                    playerFourRoundsText[5].color = Color.green;
                    break;
            }

            currentRoundText.text = $"Round : {CallBreakGameManager.instance.currentRound}";

            allPlayerName[0].text = CallBreakGameManager.instance.selfUserDetails.userName;
            allPlayerProfilePicture[0].sprite = CallBreakGameManager.instance.allProfileSprite[CallBreakGameManager.instance.selfUserDetails.userAvatarIndex];

            for (int i = 0; i < playerOneRoundsText.Count - 1; i++)
                playerOneRoundsText[i].text = CallBreakUIManager.Instance.gamePlayController.allPlayer[0].roundScore[i].ToString();

            for (int i = 0; i < playerTwoRoundsText.Count - 1; i++)
            {
                playerTwoRoundsText[i].text = CallBreakUIManager.Instance.gamePlayController.allPlayer[1].roundScore[i].ToString();
                allPlayerName[1].text = CallBreakUIManager.Instance.gamePlayController.allPlayer[1].botDetails.userName;
                allPlayerProfilePicture[1].sprite = CallBreakUIManager.Instance.gamePlayController.allPlayer[1].profilePicture.sprite;
            }
            for (int i = 0; i < playerThreeRoundsText.Count - 1; i++)
            {
                playerThreeRoundsText[i].text = CallBreakUIManager.Instance.gamePlayController.allPlayer[2].roundScore[i].ToString();
                allPlayerName[2].text = CallBreakUIManager.Instance.gamePlayController.allPlayer[2].botDetails.userName;
                allPlayerProfilePicture[2].sprite = CallBreakUIManager.Instance.gamePlayController.allPlayer[2].profilePicture.sprite;
            }
            for (int i = 0; i < playerFourRoundsText.Count - 1; i++)
            {
                playerFourRoundsText[i].text = CallBreakUIManager.Instance.gamePlayController.allPlayer[3].roundScore[i].ToString();
                allPlayerName[3].text = CallBreakUIManager.Instance.gamePlayController.allPlayer[3].botDetails.userName;
                allPlayerProfilePicture[3].sprite = CallBreakUIManager.Instance.gamePlayController.allPlayer[3].profilePicture.sprite;
            }

            playerOneRoundsText[5].text = CallBreakUIManager.Instance.gamePlayController.allPlayer[0].roundScore.Sum().ToString("F1");
            playerTwoRoundsText[5].text = CallBreakUIManager.Instance.gamePlayController.allPlayer[1].roundScore.Sum().ToString("F1");
            playerThreeRoundsText[5].text = CallBreakUIManager.Instance.gamePlayController.allPlayer[2].roundScore.Sum().ToString("F1");
            playerFourRoundsText[5].text = CallBreakUIManager.Instance.gamePlayController.allPlayer[3].roundScore.Sum().ToString("F1");

            continueButton.SetActive(true);
            gameObject.SetActive(true);
        }

        public void OnButtonClicked(string buttonName)
        {
            switch (buttonName)
            {
                case "Continue":
                    if (CallBreakConstants.callBreakRemoteConfig.adsDetails.isShowInterstitialAdsOnScoreBoard)
                    {
                        CallBreakUIManager.Instance.preLoaderController.OpenPreloader();
                        GoogleMobileAds.Sample.InterstitialAdController.ShowInterstitialAd();
                    }
                    else
                    {
                        OnAdFullScreenContentClosedHandler();
                    }
                    break;

                default:
                    break;
            }
        }

        private void OnEnable()
        {
            GoogleMobileAds.Sample.InterstitialAdController.OnInterstitialAdFullScreenContentClosed += OnAdFullScreenContentClosedHandler;
            GoogleMobileAds.Sample.InterstitialAdController.OnInterstitialAdFullScreenContentFailed += OnRewardedAdFullScreenContentFailed;
            GoogleMobileAds.Sample.InterstitialAdController.OnInterstitialAdNotReady += OnInterstitialAdNotReady;
        }
        private void OnDisable()
        {
            GoogleMobileAds.Sample.InterstitialAdController.OnInterstitialAdFullScreenContentClosed -= OnAdFullScreenContentClosedHandler;
            GoogleMobileAds.Sample.InterstitialAdController.OnInterstitialAdFullScreenContentFailed -= OnRewardedAdFullScreenContentFailed;
            GoogleMobileAds.Sample.InterstitialAdController.OnInterstitialAdNotReady -= OnInterstitialAdNotReady;
        }

        private void OnInterstitialAdNotReady()
        {
            //CallBreakUIManager.Instance.toolTipsController.OpenToolTips("AdsIsNotReady", "Ad is not ready yet !!", "");
            CloseScreen();
            CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
            CallBreakGameManager.instance.StartNewRoundAfterScoreboard();
        }

        private void OnRewardedAdFullScreenContentFailed(AdError error)
        {
            CloseScreen();
            CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
            CallBreakGameManager.instance.StartNewRoundAfterScoreboard();
        }

        private void OnAdFullScreenContentClosedHandler()
        {
            Debug.Log("OnAdFullScreenContentClosedHandler || ");
            CloseScreen();
            CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
            CallBreakGameManager.instance.StartNewRoundAfterScoreboard();
        }

        public void CloseScreen()
        {
            continueButton.SetActive(false);
            closeButton.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
