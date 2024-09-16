using UnityEngine;
using Newtonsoft.Json;

namespace HeartCardGame
{
    public class HT_TimeOutLeaveHandler : MonoBehaviour
    {
        [Header("===== Object Script =====")]
        [SerializeField] private HT_EventManager eventManager;
        [SerializeField] private HT_UiManager uiManager;


        [SerializeField] private HT_CardDeckController cardDeckController;
        [SerializeField] private HT_JoinTableHandler joinTableHandler;
        [SerializeField] private HT_GameManager gameManager;
        [SerializeField] private HT_LeaveGameHandler leaveGameHandler;

        [Header("===== Model Class =====")]
        [SerializeField] TimeOutLeaveTablePopupResponse timeOutLeaveTablePopupResponse;
        [SerializeField] BackInGamePlayResponse backInGamePlayResponse;

        private void OnEnable()
        {
            eventManager.RegisterEvent(SocketEvents.TIME_OUT_LEAVE_TABLE_POPUP, TimeOutLeavePopupSetting);
            eventManager.RegisterEvent(SocketEvents.BACK_IN_GAME_PLAYING, BackInGamePlay);
        }

        private void OnDisable()
        {
            eventManager.UnregisterEvent(SocketEvents.TIME_OUT_LEAVE_TABLE_POPUP, TimeOutLeavePopupSetting);
            eventManager.UnregisterEvent(SocketEvents.BACK_IN_GAME_PLAYING, BackInGamePlay);
        }

        private void TimeOutLeavePopupSetting(string arg0)
        {
            timeOutLeaveTablePopupResponse = JsonConvert.DeserializeObject<TimeOutLeaveTablePopupResponse>(arg0);
            uiManager.AlertPopupOnOff(timeOutLeaveTablePopupResponse.data.msg, "Ok", timeOutLeaveTablePopupResponse.data.title, true);
        }

        private void BackInGamePlay(string arg0)
        {
            backInGamePlayResponse = JsonConvert.DeserializeObject<BackInGamePlayResponse>(arg0);
            if (cardDeckController.myPlayer.mySeatIndex == backInGamePlayResponse.data.seatIndex)
                uiManager.AlertPopupOnOff("", "", "", false);
            HT_PlayerController player = joinTableHandler.GetAnyPlayer(backInGamePlayResponse.data.seatIndex);
            player.DisconnectedObjOnOff(false);
        }

        public void BackInGamePlayReqSend()
        {
            if (uiManager.titleTxt.text.Contains("Insufficient Balance!"))
            {
                Debug.Log($"Come");
                gameManager.ReloadScene();
                return;
            }

            //if (gameManager.tableState == TableState.ROUND_STARTED || gameManager.tableState == TableState.START_DEALING_CARD || gameManager.tableState == TableState.CARD_MOVE_ROUND_STARTED || gameManager.tableState == TableState.CARD_PASS_ROUND_STARTED)
            //    socketHandler.DataSendToSocket(SocketEvents.BACK_IN_GAME_PLAYING.ToString(), socketEventManager.BackInGamePlayRequest());

            if (gameManager.tableState == TableState.NONE || gameManager.tableState == TableState.DASHBOARD)
            {
                Debug.Log($"Come 2");
                leaveGameHandler.PlayerLeave();
            }
        }
    }

    [System.Serializable]
    public class TimeOutLeaveTablePopupResponseData
    {
        public string title;
        public string msg;
    }

    [System.Serializable]
    public class TimeOutLeaveTablePopupResponse
    {
        public string en;
        public TimeOutLeaveTablePopupResponseData data;
    }

    [System.Serializable]
    public class BackInGamePlayRequestData { }

    [System.Serializable]
    public class BackInGamePlayRequest
    {
        public BackInGamePlayRequestData data;
    }

    [System.Serializable]
    public class BackInGamePlayResponseData
    {
        public int seatIndex;
    }

    [System.Serializable]
    public class BackInGamePlayResponse
    {
        public string en;
        public BackInGamePlayResponseData data;
    }
}