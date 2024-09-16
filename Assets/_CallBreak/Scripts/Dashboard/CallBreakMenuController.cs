using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGSOfflineCallBreak
{
    public class CallBreakMenuController : MonoBehaviour
    {
        [Header("DashBoard RefenceTransfrom")]
        public Transform dashboardMenuBg;
        public Transform dashboardCloseSettingButton;
        public Transform dashboardCloseMenuButton;

        [Header("GamePlay RefenceTransfrom")]
        public Transform gamePlayMenuBg;
        public Transform gamePlayCloseSettingButton;
        public Transform gamePlayCloseMenuButton;


        [Header("Menu Root Bg")]
        public RectTransform menuRoot;
        [Header("Menu")]
        public GameObject menuText;
        public Transform allMenuButtonRoot;
        public RectTransform closeMenuButton;

        [Header("Setting")]
        public GameObject settingText;
        public Transform allSettingButtonRoot;
        public RectTransform closeSettingButton;

        public void ChangeTheAnchorPos(Vector2 anchorPos)
        {
            menuRoot.anchorMax = anchorPos;
            menuRoot.anchorMin = anchorPos;
            menuRoot.pivot = anchorPos;

            closeMenuButton.anchorMax = anchorPos;
            closeMenuButton.anchorMin = anchorPos;
            closeMenuButton.pivot = anchorPos;

            closeSettingButton.anchorMax = anchorPos;
            closeSettingButton.anchorMin = anchorPos;
            closeSettingButton.pivot = anchorPos;
        }

        public void OpenScreen(string menuFrom)
        {
            switch (menuFrom)
            {
                case "DashBoardMenu":

                    ChangeTheAnchorPos(Vector2.one);

                    menuRoot.transform.position = dashboardMenuBg.position;
                    closeMenuButton.transform.position = dashboardCloseMenuButton.position;
                    closeSettingButton.transform.position = dashboardCloseSettingButton.position;
                    break;
                case "GamePlaydMenu":
                    ChangeTheAnchorPos(new Vector2(0, 1));

                    menuRoot.transform.position = gamePlayMenuBg.position;
                    closeMenuButton.transform.position = gamePlayCloseMenuButton.position;
                    closeSettingButton.transform.position = gamePlayCloseSettingButton.position;
                    break;

                default:
                    break;
            }
            OpenCloseMenuScreen(true);
            OpenCloseSettingScreen(false);

            menuRoot.localScale = Vector3.zero;
            gameObject.SetActive(true);
            menuRoot.DOScale(Vector3.one, 0.25f).SetEase(Ease.Linear);
        }

        public void OpenCloseMenuScreen(bool isActive)
        {
            closeMenuButton.gameObject.SetActive(isActive);
            allMenuButtonRoot.gameObject.SetActive(isActive);
            menuText.SetActive(isActive);
        }

        public void OpenCloseSettingScreen(bool isActive)
        {
            closeSettingButton.gameObject.SetActive(isActive);
            settingText.SetActive(isActive);
            allSettingButtonRoot.gameObject.SetActive(isActive);
        }

        public void OnButtonClicked(string buttonName)
        {
            CallBreakSoundManager.PlaySoundEvent(SoundEffects.Click);
            switch (buttonName)
            {
                case "Setting":
                    OpenCloseMenuScreen(false);
                    OpenCloseSettingScreen(true);
                    break;
                case "Rules":
                    menuRoot.DOScale(Vector3.zero, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        CallBreakUIManager.Instance.rulesController.OpenScreen();
                        gameObject.SetActive(false);
                    });
                    break;
                case "Tutorial":
                    menuRoot.DOScale(Vector3.zero, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        gameObject.SetActive(false);
                        CallBreakUIManager.Instance.howToPlay.OpenScreen();
                    });
                    break;
                case "Privacy":
                    Application.OpenURL("https://finixgamesstudio.com/privacy-policy/");
                    break;
                case "Quit":
                    if (CallBreakGameManager.isInGamePlay)
                    {
                        CallBreakUIManager.Instance.exitController.OpenScreen("Exit", CallBreakConstants.ExitFromGamePlayMessage);
                    }
                    else
                    {
                        CallBreakUIManager.Instance.exitController.OpenScreen("Exit", CallBreakConstants.ExitFromDashboardPlayMessage);
                    }
                    menuRoot.DOScale(Vector3.zero, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        gameObject.SetActive(false);
                    });

                    break;
                case "SettingClose":
                    OpenCloseMenuScreen(true);
                    OpenCloseSettingScreen(false);
                    break;
                default:
                    break;
            }
        }

        public void CloseScreen()
        {
            menuRoot.DOScale(Vector3.zero, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }

    }
}
