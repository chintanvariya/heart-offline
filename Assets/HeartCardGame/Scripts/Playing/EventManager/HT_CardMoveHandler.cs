using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

namespace FGSOfflineHeart
{
    public class HT_CardMoveHandler : MonoBehaviour
    {
        [Header("===== Object Script =====")]
        [SerializeField] private HT_EventManager eventManager;
        [SerializeField] private HT_JoinTableHandler joinTableHandler;
        [SerializeField] private HT_CardPassManager cardPassManager;
        [SerializeField] private HT_GameManager gameManager;
        [SerializeField] private HT_AudioManager audioManager;

        [Header("===== Move Card Info =====")]
        [SerializeField] private Transform cardPassTransform;
        [SerializeField] private Transform remainingCardTransform;
        [SerializeField] private List<HT_CardController> remainingCards;
        public List<string> filteredCards;
        public List<string> cardsAll;

        [Header("===== Model Class =====")]
        [SerializeField] private PassCardResponse passCardResponse;

        private void OnEnable() => eventManager.RegisterEvent(SocketEvents.CARD_MOVE, CardPassSetting);

        private void OnDisable() => eventManager.UnregisterEvent(SocketEvents.CARD_MOVE, CardPassSetting);

        private void Start()
        {
            gameManager.RoundReset += ResetCardMoveHandler;
            gameManager.GameReset += ResetGameCardMoveHandler;
        }

        public void ResetCardMoveHandler()
        {
            remainingCards.Clear();
            filteredCards.Clear();
            cardsAll.Clear();
        }

        void CardPassSetting(string data)
        {
            filteredCards = new();
            passCardResponse = JsonConvert.DeserializeObject<PassCardResponse>(data);
            HT_PlayerController player = joinTableHandler.GetMyPlayer();
            List<string> cardMoveList = passCardResponse.data.playersCards.Find(x => x.userId == player.userId).cards.Select(x => x.card).ToList();
            CardSettingForMove(cardMoveList);
        }

        public void CardSettingForMove(List<string> cardMoveList)
        {
            cardsAll = cardMoveList;
            OtherPlayerCardMove();
            gameManager.isCardPassed = true;
        }

        void CardSorting(HT_PlayerController player)
        {
            cardPassManager.cardControllerList.Clear();
            foreach (Transform child in cardPassTransform)
            {
                foreach (Transform item in child)
                    remainingCards.Add(item.GetComponent<HT_CardController>());
            }

            foreach (var item in remainingCards)
            {
                item.TransparentOnOff(false);
                player.cardControllers.Remove(item);
            }

            player.cardControllers.AddRange(remainingCards);
        }

        public void SeatIndexWiseCardMove()
        {
            HT_PlayerController player = joinTableHandler.GetMyPlayer();
            PlayersCard playersCard = passCardResponse.data.playersCards.Find(x => x.userSI == player.mySeatIndex);
        }

        void OtherPlayerCardMove()
        {
            if (!gameManager.isOffline)
            {
                for (int i = 0; i < passCardResponse.data.playersCards.Count; i++)
                {
                    PlayersCard playersCard = passCardResponse.data.playersCards[i];
                    HT_PlayerController player = joinTableHandler.GetAnyPlayer(playersCard.userSI);
                    HT_PlayerController destinationPlayer = joinTableHandler.GetAnyPlayer(playersCard.destinationSI);
                    MoveCard(player, destinationPlayer);
                }
            }
            for (int i = 0; i < joinTableHandler.playerData.Count; i++)
            {
                var player = joinTableHandler.playerData[i];
                if (!player.isMyPlayer)
                {
                    for (int j = 0; j < player.cardControllers.Count; j++)
                    {
                        var card = player.cardControllers[j];
                        card.myRectTransform.SetParent(player.cardHolderTransform);
                        if (gameManager.isOffline)
                        {
                            int value = int.Parse(card.myName.Substring(2));
                            card.cardValue = value == 1 ? 14 : value;
                        }
                    }
                    player.CardSetting();
                }
                else
                    player.CardPassGenerate(cardsAll);
            }
            audioManager.GamePlayAudioSetting(audioManager.cardPassClip);
        }

        void MoveCard(HT_PlayerController fromPlayer, HT_PlayerController toPlayer)
        {
            List<HT_CardController> removeCard = new();
            if (fromPlayer.isMyPlayer)
            {
                CardSorting(fromPlayer);
                removeCard = remainingCards;
            }
            else
                removeCard = fromPlayer.cardControllers.Take(3).ToList();

            fromPlayer.cardControllers = fromPlayer.cardControllers.Except(removeCard).ToList();
            toPlayer.cardControllers.AddRange(removeCard);
        }

        public void ResetGameCardMoveHandler()
        {
            foreach (var item in remainingCards)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }
            remainingCards.Clear();
        }
    }

    [System.Serializable]
    public class PassCard
    {
        public string card;
        public bool isAlready;
    }

    [System.Serializable]
    public class PassCardResponseData
    {
        public List<PlayersCard> playersCards;
    }

    [System.Serializable]
    public class PlayersCard
    {
        public List<PassCard> cards;
        public string userId;
        public int userSI;
        public int destinationSI;
    }

    [System.Serializable]
    public class PassCardResponse
    {
        public string en;
        public PassCardResponseData data;
    }
}