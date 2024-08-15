using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace HeartCardGame
{
    public class HT_SocketEventManager : MonoBehaviour
    {
        [Header("===== Object Scripts =====")]
        [SerializeField] private HT_LobbyHandler lobbyHandler;

        public bool isMyPlayer;

        public string SignUpRequest()
        {
            SignupRequest signupRequest = new SignupRequest();
            SignupRequestData data = new SignupRequestData();
            if (isMyPlayer)
            {
                data.userId = lobbyHandler.userId;
                data.userName = lobbyHandler.userName;
            }
            else
            {
                data.userId = Random.Range(10000, 100000).ToString();
                data.userName = data.userId;
            }
            data.accessToken = PlayerPrefs.GetString("Token");
            data.profilePic = lobbyHandler.profilePic;
            data.minPlayer = 2;
            data.noOfPlayer = 4;
            data.entryFee = lobbyHandler.entryFees;
            data.winningAmount = lobbyHandler.winninAmount.ToString();
            data.lobbyId = lobbyHandler.lobbyId;
            data.gameId = "65081c945a655d3a7c7db043";
            data.isUseBot = lobbyHandler.isUseBot;
            data.isFTUE = lobbyHandler.isFTUE;

            signupRequest.data = data;

            return JsonConvert.SerializeObject(signupRequest);
        }

        public string HeartbeatRequest()
        {
            HeartbeatRequest heartbeatRequest = new HeartbeatRequest();
            HeartbeatRequestData data = new HeartbeatRequestData();

            heartbeatRequest.data = data;

            return JsonConvert.SerializeObject(heartbeatRequest);
        }

        public string ThrowCardRequest(string cardName)
        {
            ThrowCardRequest throwCardRequest = new ThrowCardRequest();
            ThrowCardRequestData data = new ThrowCardRequestData();

            data.card = cardName;

            throwCardRequest.data = data;

            return JsonConvert.SerializeObject(throwCardRequest);
        }

        public string PassCardRequest(string tableId, string userId, List<string> cards)
        {
            CardPassRequest cardPassRequest = new CardPassRequest();
            CardPassRequestData data = new CardPassRequestData();

            data.tableId = tableId;
            data.userId = userId;
            data.cards = cards;

            cardPassRequest.data = data;

            return JsonConvert.SerializeObject(cardPassRequest);
        }

        public string LeaveGameRequest(string tableId, string userId)
        {
            LeaveGameRequest leaveGameRequest = new LeaveGameRequest();
            LeaveGameRequestData data = new LeaveGameRequestData();

            data.tableId = tableId;
            data.userId = userId;
            data.isLeaveFromScoreBoard = false;

            leaveGameRequest.data = data;

            return JsonConvert.SerializeObject(leaveGameRequest);
        }

        public string BackInGamePlayRequest()
        {
            BackInGamePlayRequest backInGamePlayRequest = new BackInGamePlayRequest();
            BackInGamePlayRequestData data = new BackInGamePlayRequestData();

            backInGamePlayRequest.data = data;

            return JsonConvert.SerializeObject(backInGamePlayRequest);
        }

        public string PreCardPassSelect(string cardName, bool forward, string tableId, string userId)
        {
            PreSelectCardRequest preSelectCardRequest = new PreSelectCardRequest();
            PreSelectCardRequestData data = new PreSelectCardRequestData();

            data.card = cardName;
            data.forwardCardMove = forward;
            data.tableId = tableId;
            data.userId = userId;

            preSelectCardRequest.data = data;

            return JsonConvert.SerializeObject(preSelectCardRequest);
        }

        public string ShowScoreboard(string tableId)
        {
            ShowScoreboardRequest showScoreboardRequest = new ShowScoreboardRequest();
            ShowScoreboardRequestData data = new ShowScoreboardRequestData();

            data.tableId = tableId;

            showScoreboardRequest.data = data;

            return JsonConvert.SerializeObject(showScoreboardRequest);
        } 
    }
}