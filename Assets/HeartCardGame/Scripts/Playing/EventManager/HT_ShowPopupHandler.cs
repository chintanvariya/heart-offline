using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FGSOfflineHeart
{
    public class HT_ShowPopupHandler : MonoBehaviour
    {
        [Header("===== Object Script =====")]
        [SerializeField] private HT_EventManager eventManager;
        [SerializeField] private HT_UiManager uiManager;

        [Header("===== Model Class =====")]
        [SerializeField] private ShowPopupResponse showPopupResponse;

        private void OnEnable() => eventManager.RegisterEvent(SocketEvents.SHOW_POPUP, PopupShowSetting);

        private void OnDisable() => eventManager.UnregisterEvent(SocketEvents.SHOW_POPUP, PopupShowSetting);

        private void PopupShowSetting(string arg0)
        {
            showPopupResponse = JsonConvert.DeserializeObject<ShowPopupResponse>(arg0);
            if (showPopupResponse.data.popupType == "toastPopup")
                uiManager.CommonTooltipSet(showPopupResponse.data.message, true, true);
            else
                uiManager.AlertPopupOnOff(showPopupResponse.data.message, showPopupResponse.data.button_text[0], showPopupResponse.data.title, true);
            //uiManager.reconnectionPanel.SetActive(false);
        }
    }

    [System.Serializable]
    public class ShowPopupResponseData
    {
        public bool isPopup;
        public string popupType;
        public string title;
        public string message;
        public int buttonCounts;
        public List<string> button_text;
        public List<string> button_color;
        public List<string> button_methods;
        public bool showLoader;
    }

    [System.Serializable]
    public class ShowPopupResponse
    {
        public string en;
        public ShowPopupResponseData data;
    }
}