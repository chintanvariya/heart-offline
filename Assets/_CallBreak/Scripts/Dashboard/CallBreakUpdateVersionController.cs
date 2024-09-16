using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace FGSOfflineCallBreak
{
    public class CallBreakUpdateVersionController : MonoBehaviour
    {
        private void Awake()
        {
            Debug.LogError(Application.identifier);
            //Debug.LogError($"{PlayerSettings.Android.bundleVersionCode}");
        }

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
            CallBreakSoundManager.PlaySoundEvent(SoundEffects.Click);
            switch (buttonName)
            {
                case "Yes":
                    string storeLink = string.Empty;
#if UNITY_ANDROID && !UNITY_EDITOR
                    string playStoreLink = $"https://play.google.com/store/apps/details?id=";
                    storeLink = playStoreLink + Application.identifier;
#elif UNITY_IPHONE
                    storeLink = "";
#else
                    string playStoreLink = $"https://play.google.com/store/apps/details?id=";
                    storeLink = playStoreLink + Application.identifier;
#endif
                    Application.OpenURL(storeLink);
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