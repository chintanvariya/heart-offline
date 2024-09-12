using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Collections;
using System;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace HeartCardGame
{
    public class HT_UiManager : MonoBehaviour
    {
        [Header("===== Object Script =====")]
        [SerializeField] private HT_GameManager gameManager;
        //[SerializeField] private HT_UserRegistration userRegistration;
        //[SerializeField] private HT_DashboardManager dashboardManager;

        [Header("===== Panel And Popup =====")]
        public GameObject gameStartTimerPanel;
        public GameObject gamePlayPanel, cardPassPanel, settingPanel, leavePanel, winningPanel, scoreboardPanel, reconnectionPanel, noInternetPanel, shootingMoonPanel, heartBrokenPanel, gamePanel;
        [SerializeField] private GameObject alertPopup;

        [Header("===== Rect Transform =====")]
        public RectTransform gameStartTransform;
        public RectTransform lockInPeriodTransform, commonTooltipTransform;

        [Header("===== Texts =====")]
        public TextMeshProUGUI alertTxt;
        public TextMeshProUGUI btnTxt, titleTxt, gameStartTxt, lockInPeriodTxt, commonTooltipTxt;

        [Header("===== Buttons =====")]
        public Button scoreboardBtn;
        public Button leaveBtn;
        public GameObject noInternetCloseBtn;

        private void Start() => gameManager.GameReset += ResetUIManager;

        public void AlertPopupOnOff(string alertMsg, string btnMsg, string title, bool isOpen)
        {
            if (isOpen)
            {
                alertPopup.SetActive(true);
                alertTxt.SetText(alertMsg);
                btnTxt.SetText(btnMsg);
                titleTxt.SetText(title);
                alertPopup.transform.GetChild(0).DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            }
            else
            {
                alertPopup.transform.GetChild(0).DOScale(Vector3.zero, 0.3f).OnComplete(() =>
                {
                    alertPopup.SetActive(false);
                });
            }
        }

        public void AllPopupOff()
        {
            gameStartTransform.localScale = Vector3.zero;
            lockInPeriodTransform.localScale = Vector3.zero;
            commonTooltipTransform.localScale = Vector3.zero;
        }

        public void LockInPeriodSetting(string msg)
        {
            lockInPeriodTransform.localScale = Vector3.zero;
            lockInPeriodTxt.SetText(msg);
            lockInPeriodTransform.DOScale(Vector2.one, 0.5f);
        }

        public void OtherPanelOpen(GameObject panel, bool isOpen)
        {
            if (isOpen)
            {
                panel.transform.GetChild(0).localScale = Vector3.zero;
                panel.SetActive(true);
                panel.transform.GetChild(0).DOScale(Vector3.one, 0.3f);
            }
            else
                panel.SetActive(false);
        }

        public void CommonTooltipSet(string msg, bool isOpen, bool isCloseble)
        {
            if (isOpen)
            {
                commonTooltipTransform.localScale = Vector3.zero;
                commonTooltipTxt.SetText(msg);
                commonTooltipTransform.DOScale(Vector3.one, 1f).OnComplete(() =>
                {
                    if (isCloseble)
                        DOVirtual.DelayedCall(0.5f, () => commonTooltipTransform.DOScale(Vector3.zero, 0.5f));
                });
            }
            else
                commonTooltipTransform.localScale = Vector3.zero;
        }

        public void SettingPanelClick()
        {
            OtherPanelOpen(settingPanel, true);
        }

        public void NoInternetPanelOnOff(bool isOpen, bool isCloseBtn)
        {
            if (isOpen && !noInternetPanel.activeInHierarchy)
            {
                noInternetPanel.transform.GetChild(0).localScale = Vector3.zero;
                noInternetCloseBtn.SetActive(isCloseBtn);
                noInternetPanel.SetActive(true);
                noInternetPanel.transform.GetChild(0).DOScale(Vector3.one, 0.3f);
            }
            else if (!isOpen)
                noInternetPanel.SetActive(false);
        }

        public IEnumerator GetTexture(string profileURL, GameObject loader, Action<Sprite> getSprite)
        {
            loader.SetActive(true);
            UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(profileURL);
            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.result != UnityWebRequest.Result.Success)
                Debug.LogError($"HT_UiManager || GetTexture || Error : {unityWebRequest.error}");
            else
            {
                Texture2D myTexture = ((DownloadHandlerTexture)unityWebRequest.downloadHandler).texture;
                Sprite mySprite = Sprite.Create(myTexture, new Rect(0.0f, 0.0f, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
                getSprite?.Invoke(mySprite);
                loader.SetActive(false);
            }
        }

        public void ApiError(string error, bool isRunning = false)
        {
            if (isRunning)
            {
                Debug.Log($"Running");
                if (PlayerPrefs.HasKey("UserName"))
                {
                    Debug.Log($"First");
                    //dashboardManager.PanelOnOff(dashboardManager.dashboardPanel, true);
                    //dashboardManager.PanelOnOff(dashboardManager.enterNamePanel, false);
                    //dashboardManager.userNameTxt.SetText($"{PlayerPrefs.GetString("UserName")}");
                    return;
                }
                else
                {
                    Debug.Log($"Second");
                    //dashboardManager.enterNamePanel.SetActive(true);
                    return;
                }
            }
            //StartCoroutine(Network.InterConnectionCheck((isNetworkError) =>
            //{
            //    if (isNetworkError)
            //    {
            //        Debug.Log($"IsNetwork Error");
            //        dashboardManager.errorType = ErrorType.Error;
            //        dashboardManager.PopupOnOff(dashboardManager.commonPopup, true);
            //        dashboardManager.commonPopupTxt.SetText($"Something is went wrong. Please try again after some time.");
            //    }
            //    else
            //    {
            //        Debug.Log($"Is InternetConnection Issue");
            //        NoInternetPanelOnOff(true, true);
            //    }
            //}));
        }

        public void NoInternetClose()
        {
            NoInternetPanelOnOff(false, false);
            ////userRegistration.UserRegister(PlayerPrefs.GetString("UserName"));
        }

        public void ResetUIManager()
        {
            AllPopupOff();
            cardPassPanel.SetActive(false);
            settingPanel.SetActive(false);
            leavePanel.SetActive(false);
            alertPopup.SetActive(false);
            winningPanel.SetActive(false);
            scoreboardPanel.SetActive(false);
            gameStartTxt.SetText("");
            lockInPeriodTxt.SetText("");
            commonTooltipTxt.SetText("");
            leaveBtn.interactable = true;
        }
    }
}