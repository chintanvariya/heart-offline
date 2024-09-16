using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;

namespace FGSOfflineCallBreak
{
    public enum AvatarDetails { Free, Coins, Video }
    public class CallBreakProfileSelectionController : MonoBehaviour
    {
        public RectTransform profileSelectedImg;

        public List<CallBreakProfileSelectionUi> allProfile;

        private int profileIndex;

        public static System.Action PlayerRewardEvent;

        public static System.Action<int> profileSelection;

        public AvatarPurchaseDetails avatarPurchaseDetails;
        private void OnEnable()
        {
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdFullScreenContentClosed += OnAdFullScreenContentClosedHandler;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdFullScreenContentFailed += OnAdFullScreenContentFailed;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdNotReady += OnRewardedAdNotReady;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdGranted += OnRewardedAdGranted;
        }

        private void OnDisable()
        {
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdFullScreenContentClosed -= OnAdFullScreenContentClosedHandler;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdFullScreenContentFailed -= OnAdFullScreenContentFailed;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdNotReady -= OnRewardedAdNotReady;
            GoogleMobileAds.Sample.RewardedAdController.OnRewardedAdGranted -= OnRewardedAdGranted;
        }

        public void OpenScreen()
        {
            if (!CallBreakConstants.ItHasPurchaseDataOrNot())
            {
                for (int i = 0; i < 20; i++)
                    avatarPurchaseDetails.PurchasedOrNot.Add(false);

                CallBreakConstants.AvatarPurchaseJsonString = CallBreakUtilities.ReturnJsonOfAvatarPurchaseDetails(avatarPurchaseDetails);
            }

            avatarPurchaseDetails = CallBreakUtilities.ReturnAvatarDetails(CallBreakConstants.AvatarPurchaseJsonString);

            UpdateTheLabelText();

            FreeAvatarSelected(allProfile[CallBreakGameManager.instance.selfUserDetails.userAvatarIndex]);

            gameObject.SetActive(true);
        }

        public void CloseScreen()
        {
            gameObject.SetActive(false);
        }

        public void UpdateTheLabelText()
        {
            for (int i = 0; i < allProfile.Count; i++)
            {
                allProfile[i].indexOfAvatar = i;
                allProfile[i].profileSelectionController = this;
                allProfile[i].profilePicture.sprite = CallBreakGameManager.instance.allProfileSprite[i];
                allProfile[i].buttonLabel.text = "FREE";

                switch (allProfile[i].avatarDetails)
                {
                    case AvatarDetails.Free:
                        avatarPurchaseDetails.PurchasedOrNot[i] = true;
                        break;
                    case AvatarDetails.Coins:
                        if (!avatarPurchaseDetails.PurchasedOrNot[i])
                            allProfile[i].buttonLabel.text = $"{allProfile[i].avatarValue}";
                        else
                            allProfile[i].coinImage.SetActive(false);
                        break;
                    case AvatarDetails.Video:
                        if (!avatarPurchaseDetails.PurchasedOrNot[i])
                            allProfile[i].buttonLabel.text = "WATCH";
                        break;
                    default:
                        break;
                }
            }
        }

        public void PurchaseAvatarSelected(CallBreakProfileSelectionUi profileSelectionUi)
        {
            if (profileSelectionUi.avatarValue <= CallBreakGameManager.instance.selfUserDetails.userChips)
            {
                CallBreakGameManager.instance.selfUserDetails.userChips -= profileSelectionUi.avatarValue;
                FreeAvatarSelected(profileSelectionUi);
            }
            else
            {
                Debug.LogError($"NOT MONEY");
                if (avatarPurchaseDetails.PurchasedOrNot[profileSelectionUi.indexOfAvatar])
                    FreeAvatarSelected(profileSelectionUi);
                else
                    Debug.LogError($"NOT MONEY OR NOT PURCHASE");
            }
        }


        public void FreeAvatarSelected(CallBreakProfileSelectionUi profileSelectionUi)
        {
            UpdateTheLabelText();

            profileSelectionUi.buttonLabel.text = "SELECTED";

            if (profileSelectionUi.coinImage != null)
                profileSelectionUi.coinImage.SetActive(false);

            CallBreakGameManager.instance.selfUserDetails.userAvatarIndex = profileSelectionUi.indexOfAvatar;

            CallBreakConstants.UserDetialsJsonString = CallBreakUtilities.ReturnJsonString(CallBreakGameManager.instance.selfUserDetails);

            CallBreakGameManager.profilePicture = CallBreakGameManager.instance.allProfileSprite[CallBreakGameManager.instance.selfUserDetails.userAvatarIndex];

            CallBreakUIManager.Instance.editProfileController.UpdateMyProfilePicture();
            CallBreakUIManager.Instance.dashboardController.profileUiController.UpdateMyProfilePicture();

            profileSelectedImg.transform.SetParent(profileSelectionUi.parent, true);
            profileSelectedImg.anchoredPosition = new Vector2(0, 0);

            avatarPurchaseDetails.PurchasedOrNot[profileSelectionUi.indexOfAvatar] = true;

            CallBreakConstants.AvatarPurchaseJsonString = CallBreakUtilities.ReturnJsonOfAvatarPurchaseDetails(avatarPurchaseDetails);

        }

        public CallBreakProfileSelectionUi profileSelect;

        public void AdsSelectedProfile(CallBreakProfileSelectionUi _profileSelectionUi)
        {
            profileSelect = _profileSelectionUi;
            CallBreakUIManager.Instance.preLoaderController.OpenPreloader();
            GoogleMobileAds.Sample.RewardedAdController.ShowRewardedAd();
        }

        public void OnRewardedAdGranted()
        {
            Debug.Log("CallBreakProfileSelectionController || OnRewardedAdGranted ");
            Debug.Log($"UserDetialsJsonString  => ");
            CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
            FreeAvatarSelected(profileSelect);
        }

        public void OnAdFullScreenContentClosedHandler()
        {
            Debug.Log($"OnAdFullScreenContentClosedHandler || UserDetialsJsonString  => ");
            CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
        }
        public void OnAdFullScreenContentFailed(AdError error)
        {
            CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
        }

        public void OnRewardedAdNotReady()
        {
            CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
        }
    }
    [System.Serializable]
    public class AvatarPurchaseDetails
    {
        public List<bool> PurchasedOrNot;
    }
}
