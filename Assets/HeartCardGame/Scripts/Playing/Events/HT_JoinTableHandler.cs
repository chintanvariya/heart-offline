using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

namespace HeartCardGame
{
    public class HT_JoinTableHandler : MonoBehaviour
    {
        [Header("===== Object Script =====")]
        [SerializeField] private HT_EventManager eventManager;
        [SerializeField] private HT_GameStartTimerManager gameStartTimerManager;
        [SerializeField] private HT_UiManager uiManager;
        [SerializeField] private HT_GameManager gameManager;

        public List<HT_PlayerController> playerDataOrigin; // Static Origin List

        public List<HT_PlayerController> playerData; // Static List

        [HideInInspector] public string tableId = string.Empty;

        private void OnEnable() => eventManager.RegisterEvent(SocketEvents.JOIN_TABLE, JoinTableDataSetup);

        private void OnDisable() => eventManager.UnregisterEvent(SocketEvents.JOIN_TABLE, JoinTableDataSetup);

        private void JoinTableDataSetup(string data)
        {
            JoinTableResponse joinTableResponse = JsonConvert.DeserializeObject<JoinTableResponse>(data);
            PlayerSetOnTable(joinTableResponse.data.seats);
            uiManager.reconnectionPanel.SetActive(false);
            tableId = joinTableResponse.data.tableId; // Store tableId for event send
        }

        public HT_PlayerController GetMyPlayer()
        {
            return playerData.Find(player => player.isMyPlayer);
        }

        public void PlayerSetOnTable(List<Seat> seats)
        {
            uiManager.gameStartTimerPanel.SetActive(true);
            
            uiManager.gamePanel.SetActive(true);
            int mySeatIndex = seats.Find((player) => player.userId.Equals(GetMyPlayer().userId)).si;
            playerData = playerDataOrigin.Skip(playerDataOrigin.Count - mySeatIndex).Concat(playerDataOrigin.Take(playerDataOrigin.Count - mySeatIndex)).ToList();
            for (int i = 0; i < seats.Count; i++)
            {
                var playerInfo = seats[i];
                var playerdata = playerData[playerInfo.si];
                playerdata.PlayerDataSet(playerInfo);
                if (playerdata.isMyPlayer)
                    gameManager.mySeatIndex = playerInfo.si;
            }

            if (seats.Count < 4)
                gameStartTimerManager.GameStartTimer(15, false);
            else
                uiManager.AllPopupOff();
            gameManager.tableState = TableState.WAITING_FOR_PLAYERS;
        }

        public HT_PlayerController GetAnyPlayer(int seatIndex)
        {
            return playerData.Find(player => player.mySeatIndex == seatIndex);
        }
    }

    [System.Serializable]
    public class JoinTableResponseData
    {
        public int totalPlayers;
        public string tableId;
        public List<Seat> seats;
    }

    [System.Serializable]
    public class JoinTableResponse
    {
        public string en;
        public JoinTableResponseData data;
    }
}