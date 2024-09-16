using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FGSOfflineCallBreak
{
    public class CallBreakProfileUiController : MonoBehaviour
    {

        public TMPro.TextMeshProUGUI userNameText;
        public TMPro.TextMeshProUGUI userChipsText;
        public TMPro.TextMeshProUGUI userLevelText;
        public TMPro.TextMeshProUGUI userKeysText;

        public Image levelFillImage;

        public Image profilePicture;

        public void OpenScreen()
        {
            CallBreakGameManager.instance.selfUserDetails.levelProgress = CallBreakGameManager.instance.selfUserDetails.userKeys;

            Debug.Log($"TOTAL levelProgress => {CallBreakGameManager.instance.selfUserDetails.levelProgress}");

            int currentLevel = CallBreakUtilities.ReturnCurrentLevel(CallBreakGameManager.instance.selfUserDetails.levelProgress);

            CallBreakGameManager.instance.selfUserDetails.level = currentLevel;
            userLevelText.text = "Level " + CallBreakGameManager.instance.selfUserDetails.level;

            Debug.Log($"USER ON LEVEL TO CLEAR => {currentLevel}");

            float startOfLevel = Mathf.Abs(CallBreakConstants.coinsToClearLevel[currentLevel - 1] - CallBreakGameManager.instance.selfUserDetails.levelProgress);
            if (CallBreakGameManager.instance.selfUserDetails.levelProgress <= CallBreakConstants.coinsToClearLevel[currentLevel - 1])
                startOfLevel = CallBreakGameManager.instance.selfUserDetails.levelProgress;

            Debug.Log($"startOfLevel  {startOfLevel}");

            Debug.Log($"MAX VALUIE TO LEVEL CLEAR  {CallBreakConstants.coinsToClearLevel[currentLevel - 1]}");

            Debug.Log($"MAX VALUIE TO LEVEL CLEAR  {1 * startOfLevel / CallBreakConstants.coinsToClearLevel[currentLevel - 1]}");

            float fillAmount = 1 * startOfLevel / (float)CallBreakConstants.coinsToClearLevel[currentLevel - 1];

            Debug.Log($"fillAmount  {fillAmount}");

            levelFillImage.fillAmount = fillAmount;

            UpdateMyProfilePicture();
            UpdateUserName();
            UpdateUserChips();
            UpdateUserKeys();

            gameObject.SetActive(true);
        }

        public void UpdateMyProfilePicture() => profilePicture.sprite = CallBreakGameManager.profilePicture;
        public void UpdateUserName() => userNameText.text = CallBreakGameManager.instance.selfUserDetails.userName;
        public void UpdateUserChips() => userChipsText.text = CallBreakUtilities.AbbreviateNumber(CallBreakGameManager.instance.selfUserDetails.userChips);
        public void UpdateUserKeys()
        {
            float clearLevelCoins = CallBreakConstants.coinsToClearLevel[CallBreakGameManager.instance.selfUserDetails.level - 1];

            if (CallBreakGameManager.instance.selfUserDetails.levelProgress < CallBreakConstants.coinsToClearLevel[CallBreakGameManager.instance.selfUserDetails.level - 1])
                clearLevelCoins = CallBreakGameManager.instance.selfUserDetails.levelProgress;

            userKeysText.text = $"{CallBreakUtilities.AbbreviateNumber(clearLevelCoins)}";
        }

        public void OnButtonClicked(string buttonName)
        {
            switch (buttonName)
            {
                case "Profile":
                    CallBreakUIManager.Instance.editProfileController.OpenScreen(CallBreakGameManager.instance.selfUserDetails);
                    break;
                case "CoinStore":
                    CallBreakUIManager.Instance.itemPurchase.OpenScreen();
                    break;
                case "BuyKeys":
                    CallBreakUIManager.Instance.itemPurchase.OpenScreen();
                    break;
                case "Menu":
                    CallBreakUIManager.Instance.menuController.OpenScreen("DashBoardMenu");
                    break;
            }
        }

        public void CloseScreen() => gameObject.SetActive(false);

        private void OnEnable()
        {

        }

        private void OnDisable()
        {

        }
    }
}