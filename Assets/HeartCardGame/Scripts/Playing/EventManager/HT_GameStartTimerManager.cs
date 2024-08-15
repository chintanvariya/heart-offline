using UnityEngine;
using DG.Tweening;
using Newtonsoft.Json;

namespace HeartCardGame
{
    public class HT_GameStartTimerManager : MonoBehaviour
    {
        [Header("===== Timer Data =====")]
        [SerializeField] private int time;

        [Header("===== Object Script =====")]
        [SerializeField] private HT_EventManager eventManager;
        [SerializeField] private HT_UiManager uiManager;
        [SerializeField] private HT_GameManager gameManager;
        [SerializeField] private HT_AudioManager audioManager;
        [SerializeField] private HT_OfflineCardDistributor offlineCardDistributor;

        [Header("===== Model Class =====")]
        [SerializeField] private RoundTimerResponse roundTimerResponse;
        [SerializeField] private LockInPeriodResponse lockInPeriodResponse;

        private void Start() => gameManager.GameReset += ResetGameStartTimerManage;

        private void OnEnable()
        {
            eventManager.RegisterEvent(SocketEvents.LOCK_IN_PERIOD, LockInPeriod);
            eventManager.RegisterEvent(SocketEvents.ROUND_TIMER_STARTED, GameStartTimer);
        }

        private void OnDisable()
        {
            eventManager.UnregisterEvent(SocketEvents.LOCK_IN_PERIOD, LockInPeriod);
            eventManager.UnregisterEvent(SocketEvents.ROUND_TIMER_STARTED, GameStartTimer);
        }

        public void GameStartTimer(string data)
        {
            roundTimerResponse = JsonConvert.DeserializeObject<RoundTimerResponse>(data);
            GameStartTimer(roundTimerResponse.data.timer, true);
            audioManager.BackgroundMusicOnOff();
            gameManager.tableState = TableState.ROUND_TIMER_STARTED;
        }

        private void LockInPeriod(string data)
        {
            lockInPeriodResponse = JsonConvert.DeserializeObject<LockInPeriodResponse>(data);
            uiManager.leaveBtn.interactable = false;
            gameManager.tableState = TableState.LOCK_IN_PERIOD;
        }

        public void GameStartTimer(int timer, bool isTimer)
        {
            Debug.Log($"HT_GameStartTimerManager || GameStartTimer || timer {timer}");
            CancelInvoke(nameof(TimeInvoke));           
            time = timer;
            if (isTimer)
                InvokeRepeating(nameof(TimeInvoke), 0.01f, 1f);
            else
                uiManager.gameStartTxt.SetText("Wait for other players...");
            if (uiManager.gameStartTransform.localScale != Vector3.one)
                uiManager.gameStartTransform.localScale = Vector3.zero;
            uiManager.gameStartTransform.DOScale(Vector3.one, 0.5f);
        }

        void TimeInvoke()
        {
            uiManager.gameStartTxt.SetText("Game Start In " + time + " Seconds...");
            time--;
            if (time < 0)
                ResetGameStartTimerManage();
        }

        public void ResetGameStartTimerManage()
        {
            uiManager.gameStartTransform.localScale = Vector3.zero;
            CancelInvoke(nameof(TimeInvoke));
            if (gameManager.isOffline)
                offlineCardDistributor.CardDistribution();
        }
    }

    [System.Serializable]
    public class RoundTimerResponseData
    {
        public int timer;
    }

    [System.Serializable]
    public class RoundTimerResponse
    {
        public string en;
        public RoundTimerResponseData data;
    }

    [System.Serializable]
    public class LockInPeriodResponseData
    {
        public string msg;
    }

    [System.Serializable]
    public class LockInPeriodResponse
    {
        public string en;
        public LockInPeriodResponseData data;
    }
}