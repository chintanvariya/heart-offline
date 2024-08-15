using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

namespace HeartCardGame
{
    public class HT_ThrowCardHandler : MonoBehaviour
    {
        [Header("===== Object Scripts =====")]
        [SerializeField] private HT_EventManager eventManager;
        [SerializeField] private HT_JoinTableHandler joinTableHandler;
        [SerializeField] private HT_AudioManager audioManager;
        [SerializeField] private HT_GameManager gameManager;
        [SerializeField] private HT_OfflinePlayerTurnController offlinePlayerTurnController;
        [SerializeField] private HT_OfflineWinOfRoundHandler offlineWinOfRoundHandler;

        [Header("===== Model Class =====")]
        [SerializeField] private ThrowCardResponse throwCardResponse;

        private void OnEnable() => eventManager.RegisterEvent(SocketEvents.USER_THROW_CARD_SHOW, UserThrowCard);

        private void OnDisable() => eventManager.UnregisterEvent(SocketEvents.USER_THROW_CARD_SHOW, UserThrowCard);

        private void UserThrowCard(string arg0)
        {
            throwCardResponse = JsonConvert.DeserializeObject<ThrowCardResponse>(arg0);
            HT_PlayerController player = joinTableHandler.GetAnyPlayer(throwCardResponse.data.seatIndex);
            HT_CardController card;
            if (player.isMyPlayer)
            {
                card = player.cardControllers.Find(x => x.myName == throwCardResponse.data.card);
                Debug.Log($"MY Player Card {card.name}");
            }
            else
            {
                card = player.cardControllers.LastOrDefault();
                card.myName = throwCardResponse.data.card;
                card.cardType = card.GetCardType(card.myName);
                Debug.Log($"Opponent Player card {card}");
            }
            UserCardThrow(player, card, throwCardResponse.data.isBreakingHearts);
        }

        public void UserCardThrow(HT_PlayerController player, HT_CardController card, bool isBreakingHearts)
        {
            player.CardThrow(card, isBreakingHearts);
            audioManager.GamePlayAudioSetting(audioManager.cardDistributionClip);
            if (gameManager.isOffline)
            {
                offlineWinOfRoundHandler.handedCards.Add(card);
                if (offlineWinOfRoundHandler.handedCards.Count == 1) offlinePlayerTurnController.turnCardSequence = card.myName.Trim('-')[0].ToString();
                if (offlineWinOfRoundHandler.handedCards.Count > 3) offlineWinOfRoundHandler.WinOfRound();
            }
        }        
    }

    [System.Serializable]
    public class ThrowCardResponseData
    {
        public int seatIndex;
        public string card;
        public bool isBreakingHearts;
        public bool turnTimeout;
    }

    [System.Serializable]
    public class ThrowCardResponse
    {
        public string en;
        public ThrowCardResponseData data;
    }

    [System.Serializable]
    public class ThrowCardRequestData
    {
        public string card;
    }

    [System.Serializable]
    public class ThrowCardRequest
    {
        public ThrowCardRequestData data;
    }
}