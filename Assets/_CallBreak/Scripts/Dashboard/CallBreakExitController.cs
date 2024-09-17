using HeartCardGame;
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

                        CallBreakGameManager.isInGamePlay = false;
                        HT_OfflineGameHandler.instance.gamePlay.SetActive(false);
                        CallBreakUIManager.Instance.dashboardController.OpenScreen();
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
