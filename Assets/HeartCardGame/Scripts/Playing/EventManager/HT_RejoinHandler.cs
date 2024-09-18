using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

namespace FGSOfflineHeart
{
    public class HT_RejoinHandler : MonoBehaviour
    {
        [Header("===== Object Script =====")]
        [SerializeField] private HT_EventManager eventManager;
        [SerializeField] private HT_JoinTableHandler joinTableHandler;
        [SerializeField] private HT_GameManager gameManager;
        [SerializeField] private HT_GameStartTimerManager gameStartTimerManager;
        [SerializeField] private HT_CardDeckController cardDeckController;
        [SerializeField] private HT_TurnInfoManager turnInfoManager;
        [SerializeField] private HT_CardPassManager cardPassManager;
        [SerializeField] private HT_UiManager uiManager;

        [Header("===== Prefab =====")]
        [SerializeField] private HT_CardController cardPrefab;

        [Header("===== Model Class =====")]
        [SerializeField] private SignupResponse signupResponse;

        private void OnEnable() => eventManager.RegisterEvent(SocketEvents.SIGNUP, SignUpDataSetting);

        private void OnDisable() => eventManager.UnregisterEvent(SocketEvents.SIGNUP, SignUpDataSetting);

        private void SignUpDataSetting(string data)
        {
            signupResponse = JsonConvert.DeserializeObject<SignupResponse>(data);
            var signupData = signupResponse.data;
            HT_GameManager.instance.SetTableState(signupData.GAME_TABLE_INFO.tableState);
            var playerData = signupData.SIGNUP;
            HT_PlayerController player = joinTableHandler.GetMyPlayer();
            player.SetPlayerInfo(playerData.userId, signupData.GAME_TABLE_INFO.tableId, signupData.SIGNUP.pp);
            Debug.Log($"HT_RejoinHandler || SignUpDataSetting || Name {player.userName} {signupData.GAME_TABLE_INFO.isRejoin}");
            if (signupData.GAME_TABLE_INFO.isRejoin)
                RejoinDataSetting();
        }

        public void RejoinDataSetting()
        {
            var signupData = signupResponse.data;
            Debug.Log($"HT_RejoinHandler || RejoinDataSetting || Table state {gameManager.tableState}");
            gameManager.tableId = signupData.GAME_TABLE_INFO.tableId;
            gameManager.tableTxt.text = "#" + gameManager.tableId;
            joinTableHandler.PlayerSetOnTable(signupData.GAME_TABLE_INFO.seats);
            gameManager.SetTableState(signupData.GAME_TABLE_INFO.tableState);

            if (gameManager.tableState == TableState.SHOOTING_MOON)
                uiManager.CommonTooltipSet(signupData.GAME_TABLE_INFO.massage, true, true);

            if ((gameManager.tableState == TableState.ROUND_TIMER_STARTED || gameManager.tableState == TableState.LOCK_IN_PERIOD) && signupData.GAME_TABLE_INFO.currentGameStartTimer >= 1)
                gameStartTimerManager.GameStartTimer(signupData.GAME_TABLE_INFO.currentGameStartTimer, true);

            if (gameManager.tableState == TableState.START_DEALING_CARD || gameManager.tableState == TableState.CARD_PASS_ROUND_STARTED || gameManager.tableState == TableState.ROUND_STARTED || gameManager.tableState == TableState.CARD_MOVE_ROUND_STARTED)
                CardSetAtRejoin(signupData.GAME_TABLE_INFO, gameManager.mySeatIndex);

            //uiManager.reconnectionPanel.SetActive(false);
        }

        public void CardSetAtRejoin(GAMETABLEINFO gameTableInfo, int seatIndex)
        {
            var playersDetails = gameTableInfo.playersDetails;
            string userId = gameTableInfo.currentTurn;
            Debug.Log($"HT_RejoinHandler || CardSetAtRejoin || Seatindex {seatIndex}");
            for (int i = 0; i < playersDetails.Count; i++)
            {
                var player = playersDetails[i];
                HT_PlayerController playerController = joinTableHandler.playerData[player.seatIndex];
                Debug.Log($"HT_RejoinHandler || CardSetAtRejoin || Player Name {playerController.userName}");
                if (player.seatIndex == seatIndex)
                {
                    List<string> allCards = new List<string>();
                    if (gameManager.tableState == TableState.CARD_PASS_ROUND_STARTED)
                        allCards = player.currentCards.Concat(player.cardPassDetails.cards).ToList();
                    else
                        allCards = player.currentCards;
                    Debug.Log($"HT_RejoinHandler || CardSetAtRejoin || Cards Count {allCards.Count}");
                    cardDeckController.CardGenerate(allCards);
                    cardDeckController.GetCardPositionAndRotationForOtherPlayer(allCards.Count, cardDeckController.myPlayer, out cardDeckController.myPlayer.allVectors, out cardDeckController.myPlayer.rotationVectors);
                    for (int j = 0; j < cardDeckController.myPlayer.cardControllers.Count; j++)
                    {
                        var card = cardDeckController.myPlayer.cardControllers[j];
                        card.siblingIndex = j;
                        card.transform.SetParent(cardDeckController.cardDestinationTransform);
                        card.myRectTransform.anchoredPosition = cardDeckController.myPlayer.allVectors[j];
                        card.myRectTransform.eulerAngles = Vector3.zero;
                        card.cardSprite.raycastTarget = true;
                    }
                    for (int j = 0; j < allCards.Count; j++)
                    {
                        var cardName = allCards[j];
                        var card = cardDeckController.myPlayer.cardControllers[j];
                        card.TransparentOnOff(true);
                        Sprite cardSprite = cardDeckController.allCardDetails.GetSpriteOfCard(cardName);
                        card.SetMySprite(cardSprite);
                    }
                    cardDeckController.CardListUpdate();
                    if (gameManager.tableState != TableState.ROUND_STARTED && gameManager.tableState != TableState.CARD_MOVE_ROUND_STARTED && player.cardPassDetails.cards.Count > 0 || gameManager.tableState == TableState.CARD_PASS_ROUND_STARTED)
                    {
                        Debug.Log($"CardSetAtRejoin Called");
                        foreach (var cardName in player.cardPassDetails.cards)
                        {
                            HT_CardController card = cardDeckController.myPlayer.cardControllers.Find(x => x.myName == cardName);
                            gameManager.cardPassManager.CardSetOnEmptyBox(card, 0f);
                            cardDeckController.myPlayer.cardControllers.Remove(card);
                            gameManager.cardPassManager.cardControllerList.Add(card);
                        }
                        cardPassManager.PassCardDataSetting(gameTableInfo.cardMoveSide, gameTableInfo.currentRound, gameTableInfo.currentCardPassTimer);
                    }
                    cardDeckController.UpdateCardPosition(cardDeckController.myPlayer, false, 0.2f);
                }
                else
                {
                    foreach (var currentCard in player.currentCards)
                    {
                        HT_CardController cardClone = Instantiate(cardPrefab, playerController.cardHolderTransform);
                        playerController.cardControllers.Add(cardClone);
                    }

                    if (player.isLeft)
                        playerController.LeftObjectOnOff(true);
                    if (player.isAuto && !player.isLeft)
                        playerController.DisconnectedObjOnOff(true);

                    cardDeckController.UpdateCardPosition(playerController, false, 0f);
                }
                playerController.HeartSpadeInfoSet(player.heartPoint, player.spadePoint);
            }

            if (gameManager.tableState == TableState.ROUND_STARTED)
            {
                HT_PlayerController playerController = joinTableHandler.playerData.Find(x => x.userId == userId);
                if (gameTableInfo.currentUserTurnTimer > 0)
                    playerController.turnTimerController.TurnTimer(gameTableInfo.currentUserTurnTimer, gameTableInfo.userTurnTimer);
                turnInfoManager.isFirstTurn = playerController.cardControllers.Count == 13;
                turnInfoManager.CardsOpacityHandle(playerController, gameTableInfo.turnCardSequence, gameTableInfo.isBreakingHearts);
                gameManager.isCardPassed = true;
                for (int i = 0; i < gameTableInfo.turnCurrentCards.Count; i++)
                {
                    var currentCard = gameTableInfo.turnCurrentCards[i];
                    HT_PlayerController player = joinTableHandler.playerData[i];
                    if (!currentCard.Contains("U-0"))
                    {
                        Debug.Log($"Card Generate Place : {player.userName} and its card {currentCard}");
                        HT_CardController cardClone = Instantiate(cardPrefab, player.deckCardHolder);
                        player.HandCardGenerate(cardClone, currentCard);
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class CardPassDetails
    {
        public bool status;
        public List<string> cards;
    }

    [System.Serializable]
    public class SignupResponseData
    {
        public SIGNUP SIGNUP;
        public GAMETABLEINFO GAME_TABLE_INFO;
    }

    [System.Serializable]
    public class GAMETABLEINFO
    {
        public bool isRejoin;
        public int entryFee;
        public int userTurnTimer;
        public int currentUserTurnTimer;
        public int gameStartTimer;
        public int currentGameStartTimer;
        public int cardPassTimer;
        public int currentCardPassTimer;
        public int handCount;
        public string tableId;
        public int totalPlayers;
        public int minimumPlayers;
        public int currentRound;
        public string currentTurn;
        public string winnningAmonut;
        public List<Seat> seats;
        public string tableState;
        public string turnCardSequence;
        public bool isBreakingHearts;
        public string cardMoveSide;
        public List<string> turnCurrentCards;
        public string userId;
        public int seatIndex;
        public bool isFTUE;
        public List<PlayersDetail> playersDetails;
        public string massage;
    }

    [System.Serializable]
    public class PlayersDetail
    {
        public string _id;
        public string userId;
        public string username;
        public string profilePic;
        public int seatIndex;
        public string roundTableId;
        public string userStatus;
        public bool isFirstTurn;
        public string socketId;
        public List<string> currentCards;
        public int turnTimeout;
        public CardPassDetails cardPassDetails;
        public int hands;
        public int penaltyPoint;
        public int spadePoint;
        public int heartPoint;
        public int totalPoint;
        public bool isLeft;
        public bool isAuto;
        public bool isTurn;
        public bool isBot;
        public System.DateTime createdAt;
        public System.DateTime updatedAt;
    }

    [System.Serializable]
    public class SignupResponse
    {
        public string en;
        public SignupResponseData data;
        public string userId;
        public string tableId;
    }

    [System.Serializable]
    public class Seat
    {
        public string userId;
        public int si;
        public string name;
        public string pp;
        public string userState;
    }

    [System.Serializable]
    public class SIGNUP
    {
        public string _id;
        public string userId;
        public string name;
        public string pp;
        public int balance;
    }

    [System.Serializable]
    public class SignupRequestData
    {
        public string accessToken;
        public string userId;
        public string profilePic;
        public string userName;
        public int minPlayer;
        public int noOfPlayer;
        public int entryFee;
        public string winningAmount;
        public string lobbyId;
        public string gameId;
        public bool isUseBot;
        public bool isFTUE;
    }

    [System.Serializable]
    public class SignupRequest
    {
        public SignupRequestData data;
    }

    [System.Serializable]
    public class HeartbeatRequestData { }

    [System.Serializable]
    public class HeartbeatRequest
    {
        public HeartbeatRequestData data;
    }
}