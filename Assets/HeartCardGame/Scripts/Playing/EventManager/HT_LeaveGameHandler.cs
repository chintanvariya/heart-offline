using UnityEngine;
using Newtonsoft.Json;

namespace HeartCardGame
{
    public class HT_LeaveGameHandler : MonoBehaviour
    {
        [Header("===== Object Script =====")]
        [SerializeField] private HT_UiManager uiManager;
        [SerializeField] private HT_SocketEventManager socketEventManager;
        [SerializeField] private HT_GameManager gameManager;
        [SerializeField] private HT_EventManager eventManager;
        [SerializeField] private HT_SocketHandler socketHandler;
        [SerializeField] private HT_JoinTableHandler joinTableHandler;
        [SerializeField] private HT_GameStartTimerManager gameStartTimerManager;
        [SerializeField] private HT_LobbyHandler lobbyHandler;
        [SerializeField] private HT_DashboardManager dashboardManager;
        [SerializeField] private HT_AudioManager audioManager;

        [Header("===== Model Class =====")]
        [SerializeField] private LeaveGameResponse leaveGameResponse;

        private void OnEnable() => eventManager.RegisterEvent(SocketEvents.LEAVE_TABLE, LeaveGameResponseSetting);

        private void OnDisable() => eventManager.UnregisterEvent(SocketEvents.LEAVE_TABLE, LeaveGameResponseSetting);

        private void LeaveGameResponseSetting(string arg0)
        {
            leaveGameResponse = JsonConvert.DeserializeObject<LeaveGameResponse>(arg0);
            if (leaveGameResponse.data.seatIndex == gameManager.mySeatIndex && leaveGameResponse.data.msg != "DISCONNECTED")
                GetLobbyOnGamePlay();
            else
            {
                HT_PlayerController player = joinTableHandler.GetAnyPlayer(leaveGameResponse.data.seatIndex);
                if (leaveGameResponse.data.msg == "DISCONNECTED")
                    player.DisconnectedObjOnOff(true);
                if (leaveGameResponse.data.msg == "LEFT" && (gameManager.tableState != TableState.WAITING_FOR_PLAYERS || gameManager.tableState != TableState.ROUND_TIMER_STARTED))
                {
                    player.LeftObjectOnOff(true);
                    player.DisconnectedObjOnOff(false);
                }

                if (gameManager.tableState == TableState.WAITING_FOR_PLAYERS || gameManager.tableState == TableState.ROUND_TIMER_STARTED)
                {
                    player.ResetPlayers();
                    if (leaveGameResponse.data.currentPlayerInTable < 4)
                        gameStartTimerManager.GameStartTimer(15, false);
                }
            }
        }

        public void ClickOnErrorPopup()
        {
            if (dashboardManager.errorType == ErrorType.Error)
                PlayerLeave();
            if (dashboardManager.errorType == ErrorType.Alert)
                dashboardManager.commonPopup.SetActive(false);
        }

        public void PlayerLeave()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
#if UNITY_ANDROID
            Application.Quit();
#endif
        }

        public void GetLobbyOnGamePlay()
        {
            socketHandler.CancelInvoke(nameof(socketHandler.InternetChecking));
            audioManager.backgroundAudioSource.mute = true;
            socketHandler.ForcefullySocketDisconnect();
            gameManager.cardPassManager.cardPassBtn.interactable = false;
            gameManager.turnInfoManager.isFirstTurn = true;
            gameManager.isHeartAnimationShow = true;
            gameManager.isCardPassed = false;
            gameManager.tableState = TableState.DASHBOARD;
            lobbyHandler.GetLobby();
            uiManager.gamePanel.SetActive(false);
            dashboardManager.lobbyPanel.SetActive(true);
            uiManager.dashboardPanel.SetActive(true);
        }

        public void LeaveButtonPressed() => uiManager.OtherPanelOpen(uiManager.leavePanel, true);

        public void LeaveButtonNoPressed() => uiManager.OtherPanelOpen(uiManager.leavePanel, false);

        public void LeaveGame()
        {
            if (gameManager.isOffline)
            {
                Debug.Log($"Leave Button Pressed");
                gameManager.ReloadScene();
            }
            else
                socketHandler.DataSendToSocket(SocketEvents.LEAVE_TABLE.ToString(), socketEventManager.LeaveGameRequest(gameManager.tableId, gameManager.userId));
        }
    }

    [System.Serializable]
    public class LeaveGameRequestData
    {
        public string userId;
        public string tableId;
        public bool isLeaveFromScoreBoard;
    }

    [System.Serializable]
    public class LeaveGameRequest
    {
        public LeaveGameRequestData data;
    }

    [System.Serializable]
    public class LeaveGameResponseData
    {
        public int seatIndex;
        public int currentPlayerInTable;
        public bool playerLeave;
        public string msg;
    }

    [System.Serializable]
    public class LeaveGameResponse
    {
        public string en;
        public LeaveGameResponseData data;
    }
}