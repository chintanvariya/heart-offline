using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace FGSOfflineHeart
{
    public class HT_TurnInfoManager : MonoBehaviour
    {
        [Header("===== Object Script =====")]
        [SerializeField] private HT_EventManager eventManager;
        [SerializeField] private HT_JoinTableHandler joinTableHandler;
        [SerializeField] private HT_GameManager gameManager;
        [SerializeField] private HT_AudioManager audioManager;
        [SerializeField] private HT_SettingPanelController settingPanelController;

        [Header("===== Turn Data =====")]
        public bool isFirstTurn;

        [Header("===== Model Class =====")]
        [SerializeField] private TurnInfoResponse turnInfoResponse;

        private void OnEnable() => eventManager.RegisterEvent(SocketEvents.USER_TURN_STARTED, UserTurnStart);

        private void OnDisable() => eventManager.UnregisterEvent(SocketEvents.USER_TURN_STARTED, UserTurnStart);

        private void UserTurnStart(string data)
        {
            turnInfoResponse = JsonConvert.DeserializeObject<TurnInfoResponse>(data);
            gameManager.isCardPassed = true;
            UserTurnHandle(turnInfoResponse.data.currentTurnSI, turnInfoResponse.data.userTurnTimer, turnInfoResponse.data.turnCardSequence, turnInfoResponse.data.isBreakingHearts);
        }

        public void UserTurnHandle(int currentSI, float turnTime, string turnCardSequence, bool isBreakingHearts)
        {
            AllPlayerTimerOff();
            var player = joinTableHandler.playerData[currentSI];
            player.turnTimerController.TurnTimer(turnTime, 10);
            CardsOpacityHandle(player, turnCardSequence, isBreakingHearts);
        }

        public void CardsOpacityHandle(HT_PlayerController player, string turnCardSequence, bool isBreakingHeart)
        {
            if (player.isMyPlayer)
            {
                audioManager.GamePlayAudioSetting(audioManager.userTurnClip);
                if (settingPanelController.vibrate)
                    Handheld.Vibrate();
                player.cardControllers.ForEach(card => card.TransparentOnOff(true));
                gameManager.CardTargetRaycastOnOff(true);
                List<HT_CardController> cards = new(GetThrowableCards(player, turnCardSequence, isBreakingHeart));
                Debug.Log($"HT_TurnInfoManager || CardsOpacityHandle || Card count {cards.Count}");
                cards.ForEach(card => card.TransparentOnOff(false));
            }
            else
                joinTableHandler.playerData[gameManager.mySeatIndex].AllCardTransparentImageOnOff(true);
        }

        public List<HT_CardController> GetThrowableCards(HT_PlayerController player, string turnCardSequence, bool isBreakingHeart)
        {
            List<HT_CardController> cards = new();
            if (isAllCardHeart(player.cardControllers))
            {
                player.cardControllers.ForEach(card => card.TransparentOnOff(false));
                return player.cardControllers;
            }
            if (turnCardSequence == "N")
            {
                if (isBreakingHeart)
                    cards = player.cardControllers; // all card open
                else
                {
                    if (isFirstTurn)
                        cards = player.cardControllers.Where(card => card.myName == "C-2").ToList(); // heart all card and queen of spades close and all are open
                    else
                        cards = player.cardControllers.Where(card => card.cardType != CardType.H).ToList(); // heart card close
                }
            }
            else
            {
                if (player.cardControllers.Find(x => x.cardType == GetCardType(turnCardSequence)))
                    cards = player.cardControllers.Where(card => card.cardType == GetCardType(turnCardSequence)).ToList(); // only pass cards open
                else if (isFirstTurn)
                    cards = player.cardControllers.Where(item => item.cardType != CardType.H && item.myName != "S-12").ToList(); // when first turn but sequnce card is not in our cards
                else
                    cards = player.cardControllers; // all card open
            }
            if (player.isMyPlayer)
                isFirstTurn = false;
            return cards;
        }

        CardType GetCardType(string cardName)
        {
            CardType cardType = (CardType)Enum.Parse(typeof(CardType), cardName);
            return cardType;
        }

        public void AllPlayerTimerOff()
        {
            foreach (var player in joinTableHandler.playerData)
            {
                player.turnTimerController.ResetTime();
            }
        }

        bool isAllCardHeart(List<HT_CardController> cardControllers)
        {
            return cardControllers.TrueForAll(card => card.cardType == CardType.H);
        }
    }

    [Serializable]
    public class TurnInfoResponseData
    {
        public string currentTurnUserId;
        public int currentTurnSI;
        public int currentRound;
        public int userTurnTimer;
        public bool isBreakingHearts;
        public string turnCardSequence;
        public List<string> turnCurrentCards;
        public string tableId;
    }

    [Serializable]
    public class TurnInfoResponse
    {
        public string en;
        public TurnInfoResponseData data;
    }
}