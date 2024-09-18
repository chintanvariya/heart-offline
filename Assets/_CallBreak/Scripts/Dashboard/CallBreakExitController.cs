using FGSOfflineHeart;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FGSOfflineCallBreak
{
    public class CallBreakExitController : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI titleText;
        public TMPro.TextMeshProUGUI descriptionText;
        public void OpenScreen(string title, string description)
        {
            titleText.text = title;
            descriptionText.text = description;
            gameObject.SetActive(true);

            Time.timeScale = 0;
        }

        public void OnButtonClicked(string buttonName)
        {
            switch (buttonName)
            {
                case "Yes":
                    if (CallBreakGameManager.isInGamePlay)
                    {
                        CallBreakUIManager.Instance.gamePlayController.CloseScreen();
                        CallBreakUIManager.Instance.dashboardController.OpenScreen();
                        CloseScreen();

                        HT_GameManager.instance.RoundReset();
                        HT_GameManager.instance.GameReset();

                        CallBreakGameManager.isInGamePlay = false;
                        HT_OfflineGameHandler.instance.gamePlay.SetActive(false);
                        CallBreakUIManager.Instance.dashboardController.OpenScreen();
                        Time.timeScale = 1;
                    }
                    else
                    {
#if !UNITY_EDITOR
                        Application.Quit();
#elif  UNITY_EDITOR
                        EditorApplication.isPlaying = false;
#endif
                    }
                    break;
                case "No":
                    Time.timeScale = 1;
                    CloseScreen();
                    break;
            }
        }

        public void CloseScreen()
        {
            gameObject.SetActive(false);
        }


    }
}
