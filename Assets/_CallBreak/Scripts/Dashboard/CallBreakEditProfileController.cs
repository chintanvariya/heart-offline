using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FGSOfflineCallBreak
{
    public class CallBreakEditProfileController : MonoBehaviour
    {
        [Header("Game Statistics")]
        public TextMeshProUGUI gamePlayedText;
        public TextMeshProUGUI gameWonText;
        public TextMeshProUGUI gameLossText;

        [Header("User Details")]
        public TextMeshProUGUI useIdText;
        public TextMeshProUGUI useBalanceText;
        public Image profilePicture;

        [Header("User Name InputField")]
        public TMP_InputField inputFieldUserName;


        public void OpenScreen(UserDetails selfUserDetails)
        {
            gamePlayedText.text = selfUserDetails.userGameDetails.GamePlayed.ToString();
            gameWonText.text = selfUserDetails.userGameDetails.GameWon.ToString();
            gameLossText.text = selfUserDetails.userGameDetails.GameLoss.ToString();

            UpdateMyProfilePicture();

            inputFieldUserName.text = selfUserDetails.userName;
            useIdText.text = "ID :- " + selfUserDetails.userId;
            useBalanceText.text = CallBreakUtilities.AbbreviateNumber(selfUserDetails.userChips);

            gameObject.SetActive(true);
        }

        public void UpdateMyProfilePicture() => profilePicture.sprite = CallBreakGameManager.profilePicture;

        public void OnValueChangeEnd()
        {
            if (inputFieldUserName.text.Length > 5)
            {
                CallBreakGameManager.instance.selfUserDetails.userName = inputFieldUserName.text;
                CallBreakConstants.UserDetialsJsonString = CallBreakUtilities.ReturnJsonString(CallBreakGameManager.instance.selfUserDetails);
                CallBreakUIManager.Instance.dashboardController.profileUiController.UpdateUserName();
            }
            else
            {
                inputFieldUserName.text = CallBreakGameManager.instance.selfUserDetails.userName;
            }
        }

        public void OnButtonClicked(string buttonName)
        {
            switch (buttonName)
            {
                case "ChangeAvatar":
                    CallBreakUIManager.Instance.profileSelectionController.OpenScreen();
                    break;
                case "CoinStore":
                    CallBreakUIManager.Instance.itemPurchase.OpenScreen();
                    break;
                default:
                    break;
            }
        }

        public void CloseScreen()
        {
            gameObject.SetActive(false);
        }
    }
}
