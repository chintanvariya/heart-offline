using UnityEngine;
using System;
using UnityEngine.UI;

namespace HeartCardGame
{
    public class HT_GameManager : MonoBehaviour
    {
        public static HT_GameManager instance;

        [Header("===== Table Info =====")]
        public TableState tableState;
        public string tableId;
        public Text tableTxt, timeTxt;

        [Header("===== Object Scripts =====")]
        public HT_AllCardDetails allCardDetails;
        public HT_CardPassManager cardPassManager;
        public HT_CardDeckController cardDeckController;
        public HT_SocketEventManager socketEventManager;
        public HT_SocketHandler socketHandler;
        public HT_UiManager uiManager;
        public HT_PreCardPass preCardPassHandler;
        public HT_TurnInfoManager turnInfoManager;
        public HT_AudioManager audioManager;
        [SerializeField] private HT_OfflinePlayerTurnController offlinePlayerTurnController;
        [SerializeField] private HT_JoinTableHandler joinTableHandler;

        [Header("===== Player Info =====")]
        public int mySeatIndex;
        public string userId;
        public bool isCardPassed, isHeartAnimationShow;
        public Sprite myUserSprite;

        [Header("===== Actions =====")]
        public Action RoundReset;
        public Action GameReset;

        [Header("===== Other =====")]
        public Text fpsText;
        public bool isOffline;

        private void Awake()
        {
            if (instance != null)
                Destroy(instance);
            instance = this;
            Input.multiTouchEnabled = false;
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        private void Start()
        {
            RoundReset += ResetAllPlayer;
            GameReset += ResetGamePlayers;
            //InvokeRepeating(nameof(UpdateFPS), 0f, 1f);
        }

        //private void Update() => timeTxt.text = DateTime.Now.ToString("hh:mm:ss fff");

        public void SetTableState(string currentState) => tableState = (TableState)Enum.Parse(typeof(TableState), currentState);

        public void CardTargetRaycastOnOff(bool isEnable)
        {
            foreach (var card in cardDeckController.myPlayer.cardControllers)
            {
                card.cardSprite.raycastTarget = isEnable;
            }
        }

        public void ReloadScene()
        {
            audioManager.backgroundAudioSource.mute = true;
            socketHandler.ForcefullySocketDisconnect();
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }

        //private void UpdateFPS() => fpsText.text = (int)(1 / Time.unscaledDeltaTime) + "";

        public void ResetAllPlayer()
        {
            offlinePlayerTurnController.isBreakingHearts = false;
            foreach (var player in joinTableHandler.playerData)
            {
                player.ResetPlayerController();
            }
        }

        public void ResetGamePlayers()
        {
            foreach (var player in joinTableHandler.playerData)
            {
                player.ResetPlayers();
            }
        }
    }

    public enum TableState
    {
        NONE,
        WAITING_FOR_PLAYERS,
        ROUND_TIMER_STARTED,
        LOCK_IN_PERIOD,
        COLLECTING_BOOT_VALUE,
        START_DEALING_CARD,
        SHOOTING_MOON,
        CARD_PASS_ROUND_STARTED,
        CARD_MOVE_ROUND_STARTED,
        ROUND_STARTED,
        WINNER_DECLARED,
        SCOREBOARD_DECLARED,
        DASHBOARD,
        OFFLINE
    }
}