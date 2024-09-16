using FGSOfflineCallBreak;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FGSOfflineCallBreak
{

    public class CallBreakRegisterUserController : MonoBehaviour
    {
        public RectTransform selectedImage;

        public List<RectTransform> allProfile;

        public List<Image> allProfileImages;

        public TMPro.TMP_InputField registerUserName;

        public UnityEngine.UI.Button registerButton;

        public void OpenScreen()
        {
            for (int i = 0; i < allProfileImages.Count; i++)
                allProfileImages[i].sprite = CallBreakGameManager.instance.allProfileSprite[i];

            registerUserName.text = string.Empty;
            registerButton.interactable = false;

            CallBreakGameManager.instance.selfUserDetails.userAvatarIndex = 0;

            gameObject.SetActive(true);
        }

        public void ProfileSelectBtn(int profileIndex)
        {
            CallBreakGameManager.instance.selfUserDetails.userAvatarIndex = profileIndex;

            selectedImage.SetParent(allProfile[profileIndex], true);
            selectedImage.SetAsFirstSibling();
            selectedImage.anchoredPosition = new Vector2(0, -15);
        }

        public void OnValueChange()
        {
            if (registerUserName.text.Length > 5 && registerUserName.text.Length < 10)
                registerButton.interactable = true;
            else
                registerButton.interactable = false;
        }

        public void OnValueEnd()
        {
            if (registerUserName.text.Length > 5 && registerUserName.text.Length < 10)
                registerButton.interactable = true;
            else
            {
                registerButton.interactable = false;
                CallBreakUIManager.Instance.toolTipsController.OpenToolTips("AdsIsNotReady", "Enter at least 5 characters", "");
            }

            CallBreakGameManager.instance.selfUserDetails.userName = registerUserName.text;
            CallBreakGameManager.instance.selfUserDetails.userChips = 500;
        }

        public void CloseScreen()
        {
            Debug.Log($"{CallBreakConstants.UserDetialsJsonString }");

            CallBreakConstants.UserDetialsJsonString = CallBreakUtilities.ReturnJsonString(CallBreakGameManager.instance.selfUserDetails);

            Debug.Log($"{CallBreakConstants.UserDetialsJsonString }");

            CallBreakGameManager.instance.selfUserDetails = CallBreakUtilities.ReturnUserDetails(CallBreakConstants.UserDetialsJsonString);

            CallBreakGameManager.profilePicture = CallBreakGameManager.instance.allProfileSprite[CallBreakGameManager.instance.selfUserDetails.userAvatarIndex];

            CallBreakUIManager.Instance.dashboardController.OpenScreen();

            this.gameObject.SetActive(false);
        }

    }
}
