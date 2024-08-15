using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace HeartCardGame
{
    public class HT_ProfileHandler : MonoBehaviour
    {
        [Header("===== Object Scripts =====")]
        [SerializeField] private HT_DashboardManager dashboardManager;
        [SerializeField] private HT_UiManager uiManager;
        [SerializeField] private HT_SocketHandler socketHandler;
        [SerializeField] private HT_UserRegistration userRegistration;

        [Header("===== Lobby Handle Data =====")]
        [SerializeField] private Image profileImage;
        [SerializeField] private TextMeshProUGUI userNameTxt, balanceTxt;
        [SerializeField] private GameObject loader;
        [SerializeField] private ScrollRect scrollbar;

        [Header("===== Profile Edit Data =====")]
        [SerializeField] TMP_InputField nameInputField;
        [SerializeField] TextMeshProUGUI nameTxt, warningTxt;
        public TextMeshProUGUI totalGamePlayTxt, gameWinTxt, gameLossTxt;
        [SerializeField] HT_ProfilePicHandler profilePicHandler;
        [SerializeField] List<HT_ProfilePicHandler> profilePicHandlers;
        [SerializeField] private RectTransform profileGenerator;
        [SerializeField] private Button editBtn;

        [Header("===== Model Class =====")]
        [SerializeField] private ProfileEditDataRes profileEditDataResponse;
        [SerializeField] private AvatarResponse avatarResponse;

        private const string matchNamePattern = "^[a-zA-Z]+[a-zA-Z0-9]+$";

        Coroutine TextureCor;

        private void Start()
        {
            dashboardManager.UserNameAction += UserNameSetting;
            dashboardManager.BalanceAction += BalanceSetting;
            dashboardManager.ProfileAction += ProfileSetting;
        }

        private void ProfileSetting(string profileURL)
        {
            TextureCor = StartCoroutine(uiManager.GetTexture(profileURL, loader, (sprite) =>
            {
                profileImage.sprite = sprite;
                if (TextureCor != null)
                    StopCoroutine(TextureCor);
            }));
        }

        private void BalanceSetting(float obj) => balanceTxt.SetText($"{obj}");

        private void UserNameSetting(string obj) => userNameTxt.SetText($"{obj}");

        public void EditProfileName()
        {
            string changedName = nameInputField.text;
            editBtn.interactable = true;
            userNameTxt.gameObject.SetActive(true);
            nameInputField.gameObject.SetActive(false);
            if (IsUserNameValid(changedName, warningTxt))
            {
                nameInputField.DeactivateInputField();
                nameInputField.text = "";
                warningTxt.SetText($"");
                string url = socketHandler.serverUrl[(int)socketHandler.serverType];
                url += HT_StaticData.EditProfile;
                if (dashboardManager.IsAccessTokenAvailable())
                    EditNameHandle(url, changedName);
                else
                    userRegistration.UserRegister(PlayerPrefs.GetString("UserName"), (success) => EditNameHandle(url, changedName));
            }
        }

        void EditNameHandle(string url, string changedName)
        {
            StartCoroutine(HT_APIManager.RequestWithPostData(url, HT_APIEventManager.ProfileEdit(changedName), (data) =>
            {
                profileEditDataResponse = JsonConvert.DeserializeObject<ProfileEditDataRes>(data);
                dashboardManager.UserDataSetting(profileEditDataResponse.data.userName, profileEditDataResponse.data.coins, profileEditDataResponse.data.profileImage, false);
            }, (error) => uiManager.ApiError(error)));
        }

        public void ClickOnEditNameBtn()
        {
            userNameTxt.gameObject.SetActive(false);
            nameInputField.gameObject.SetActive(true);
            nameInputField.text = userNameTxt.text;
            editBtn.interactable = false;
            nameInputField.ActivateInputField();
        }

        public bool IsUserNameValid(string inputString, TMP_Text warningText)
        {
            bool isIFValid = true;
            Debug.Log($"Name {inputString.Length}");
            foreach (var item in inputString)
            {
                Debug.Log($"I {item}");
            }
            if (string.IsNullOrEmpty(inputString))
            {
                warningText.text = "Field should not be blank";
                isIFValid = false;
            }
            else
            {
                if (inputString.Length < 2)
                {
                    warningText.text = "Minimum 2 characters required.";
                    isIFValid = false;
                }
                else if (inputString.Length > 10)
                {
                    warningText.text = "Maximum 10 characters allow.";
                    isIFValid = false;
                }
                else if (!Regex.IsMatch(inputString, matchNamePattern))
                {
                    warningText.text = "Enter valid characters";
                    isIFValid = false;
                }
            }

            return isIFValid;
        }

        public void EditProfilePic()
        {
            DestroyAvatar();
            dashboardManager.PopupOnOff(dashboardManager.allAvatarPanel, true);
            dashboardManager.profilePanel.SetActive(false);
            scrollbar.content.anchoredPosition = new Vector2(scrollbar.content.anchoredPosition.x, 0);
            for (int i = 0; i < dashboardManager.profilePics.Count; i++)
            {
                var profileURL = dashboardManager.profilePics[i];
                HT_ProfilePicHandler profilePicClone = Instantiate(profilePicHandler, profileGenerator);
                profilePicClone.SetProfileImage(profileURL.avatarImage, !profileURL.isPurchase, profileURL.isCanBuy, profileURL.isFree, profileURL.isUsedAvatar, profileURL.coins);
                profilePicClone.avatarBtn.onClick.AddListener(() =>
                {
                    ClickOnAvatar(profileURL._id, profilePicClone.lockObj.activeInHierarchy, profilePicClone);
                });
                profilePicHandlers.Add(profilePicClone);
            }
        }

        public void ClickOnAvatar(string avatarId, bool isBuy, HT_ProfilePicHandler profilePicHandler)
        {
            if (isBuy)
            {
                string url = socketHandler.serverUrl[(int)socketHandler.serverType];
                url += HT_StaticData.BuyAvatar;
                if (dashboardManager.IsAccessTokenAvailable())
                    SetAvatarHandle(url, avatarId, profilePicHandler);
                else
                    userRegistration.UserRegister(PlayerPrefs.GetString("UserName"), (success) => SetAvatarHandle(url, avatarId, profilePicHandler));
            }
            else
            {
                string url = socketHandler.serverUrl[(int)socketHandler.serverType];
                url += HT_StaticData.UseAvatar;
                if (dashboardManager.IsAccessTokenAvailable())
                    UseAvatarHandle(url, avatarId);
                else
                    userRegistration.UserRegister(PlayerPrefs.GetString("UserName"), (success) => UseAvatarHandle(url, avatarId));
            }
        }

        void SetAvatarHandle(string url, string avatarId, HT_ProfilePicHandler profilePicHandler)
        {
            StartCoroutine(HT_APIManager.RequestWithPostData(url, HT_APIEventManager.AvatarSet(avatarId), (data) =>
            {
                avatarResponse = JsonConvert.DeserializeObject<AvatarResponse>(data);
                if (avatarResponse.success)
                {
                    profilePicHandler.SetProfileAfterPurchase();
                    dashboardManager.UserDataSetting(avatarResponse.data.userName, avatarResponse.data.coins, avatarResponse.data.profileImage, false);
                    dashboardManager.PopupOnOff(dashboardManager.commonPopup, true);
                    dashboardManager.commonPopupTxt.SetText($"{avatarResponse.message}");
                    dashboardManager.PanelOnOff(dashboardManager.allAvatarPanel, false);
                }
            }, (error) => uiManager.ApiError(error)));
        }

        void UseAvatarHandle(string url, string avatarId)
        {
            StartCoroutine(HT_APIManager.RequestWithPostData(url, HT_APIEventManager.AvatarSet(avatarId), (data) =>
            {
                avatarResponse = JsonConvert.DeserializeObject<AvatarResponse>(data);
                if (avatarResponse.success)
                {
                    dashboardManager.UserDataSetting(avatarResponse.data.userName, avatarResponse.data.coins, avatarResponse.data.profileImage, true);
                    dashboardManager.PopupOnOff(dashboardManager.commonPopup, true);
                    dashboardManager.commonPopupTxt.SetText($"{avatarResponse.message}");
                    dashboardManager.PanelOnOff(dashboardManager.allAvatarPanel, false);
                }
            }, (error) => uiManager.ApiError(error)));
        }

        void DestroyAvatar()
        {
            foreach (var item in profilePicHandlers)
            {
                item.avatarBtn.onClick.RemoveAllListeners();
                Destroy(item.gameObject);
            }
            profilePicHandlers.Clear();
        }
    }

    #region ProfileModelClass
    [System.Serializable]
    public class ProfileEditData
    {
        public string userName;
    }

    [System.Serializable]
    public class ProfileEditDataResData
    {
        public string userName;
        public string profileImage;
        public bool isFTUE;
        public bool isBot;
        public int coins;
        public string useAvatar;
        public List<string> purchaseAvatars;
        public string _id;
        public string token;
    }

    [System.Serializable]
    public class ProfileEditDataRes
    {
        public string message;
        public string status;
        public int statusCode;
        public bool success;
        public ProfileEditDataResData data;
    }
    #endregion

    #region AvatarModelClass
    [System.Serializable]
    public class AvatarResponseData
    {
        public string profileImage;
        public string userName;
        public int coins;
    }

    [System.Serializable]
    public class AvatarResponse
    {
        public string message;
        public string status;
        public int statusCode;
        public bool success;
        public AvatarResponseData data;
    }
    #endregion
}