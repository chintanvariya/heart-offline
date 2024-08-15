using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

namespace HeartCardGame
{
    public class HT_CardPassHandler : MonoBehaviour
    {
        [Header("===== Object Script =====")]
        [SerializeField] private HT_EventManager eventManager;
        [SerializeField] private HT_JoinTableHandler joinTableHandler;
        [SerializeField] private HT_CardPassManager cardPassManager;
        [SerializeField] private HT_GameManager gameManager;
        [SerializeField] private HT_UiManager uiManager;
        [SerializeField] private HT_CardDeckController cardDeckController;

        [Header("===== Model Class =====")]
        [SerializeField] private CardPassResponse cardPassResponse;

        private void OnEnable() => eventManager.RegisterEvent(SocketEvents.CARD_PASS, CardPassSetting);

        private void OnDisable() => eventManager.UnregisterEvent(SocketEvents.CARD_PASS, CardPassSetting);

        private void CardPassSetting(string data)
        {
            cardPassResponse = JsonConvert.DeserializeObject<CardPassResponse>(data);
            uiManager.cardPassPanel.SetActive(true);
            CardPassAnimation(cardPassResponse.data.cards);
            gameManager.tableState = TableState.CARD_PASS_ROUND_STARTED;
        }

        public void CardPassAnimation(List<string> movableCard, bool isAuto = false)
        {
            HT_PlayerController player = joinTableHandler.GetMyPlayer();
            if (player.isMyPlayer)
            {
                List<HT_CardController> cards = player.cardControllers.Concat(cardPassManager.cardControllerList).ToList();

                player.passCardList.Clear();
                foreach (var card in movableCard)
                {
                    player.passCardList.Add(cards.Find(x => x.myName == card));
                }
                Debug.Log($"HT_CardPassHandler || CardPassAnimation || CARD PASS COUNT {player.passCardList.Count} || CARD CONTROLLER COUNT {cardDeckController.myPlayer.cardControllers.Count}");
                foreach (var card in player.passCardList)
                {
                    if (card != null)
                        cardPassManager.CardSetOnEmptyBox(card, 0.2f, isAuto);
                }
                cardDeckController.CardListUpdate();
                cardDeckController.UpdateCardPosition(cardDeckController.myPlayer, true, 0.2f);
                if (cardPassManager.cardControllerList.Count == 3)
                    cardDeckController.myPlayer.AllCardTransparentImageOnOff(true);
            }
        }
    }

    [System.Serializable]
    public class CardPassRequestData
    {
        public string userId;
        public string tableId;
        public List<string> cards;
    }

    [System.Serializable]
    public class CardPassRequest
    {
        public CardPassRequestData data;
    }

    [System.Serializable]
    public class CardPassResponseData
    {
        public List<string> cards;
        public string userId;
        public int si;
    }

    [System.Serializable]
    public class CardPassResponse
    {
        public string en;
        public CardPassResponseData data;
    }
}
