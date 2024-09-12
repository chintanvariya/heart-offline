using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HeartCardGame
{
    public class HT_PreCardPass : MonoBehaviour
    {
        [Header("===== Object Script =====")]
        [SerializeField] private HT_EventManager eventManager;
        [SerializeField] private HT_GameManager gameManager;
        [SerializeField] private HT_CardDeckController cardDeckController;
        [SerializeField] private HT_AudioManager audioManager;

        [Header("===== Model Class =====")]
        [SerializeField] private PreSelectCardResponse preSelectCardResponse;

        private void OnEnable() => eventManager.RegisterEvent(SocketEvents.PRE_CARD_PASS_SELECT, PreCardSelectSetting);

        private void OnDisable() => eventManager.UnregisterEvent(SocketEvents.PRE_CARD_PASS_SELECT, PreCardSelectSetting);

        private void PreCardSelectSetting(string arg0)
        {
            preSelectCardResponse = JsonConvert.DeserializeObject<PreSelectCardResponse>(arg0);
            HT_CardController card = cardDeckController.myPlayer.cardControllers.Find(x => x.myName == preSelectCardResponse.data.card);
            if (preSelectCardResponse.data.forwardCardMove)
            {
                gameManager.cardPassManager.cardControllerList.Add(card);
                gameManager.cardPassManager.CardSetOnEmptyBox(card, 0.2f);
            }
            else
            {
                card = gameManager.cardPassManager.cardControllerList.Find(x => x.myName == preSelectCardResponse.data.card);
                gameManager.cardPassManager.cardControllerList.Remove(card);
                gameManager.cardPassManager.CardResetOnOldPosition(card);
            }
            cardDeckController.CardListUpdate();
            cardDeckController.UpdateCardPosition(cardDeckController.myPlayer, true, 0.2f);
            if (gameManager.cardPassManager.cardControllerList.Count == 3)
            {
                Debug.Log($"CARD COUNT {cardDeckController.myPlayer.cardControllers.Count}");
                cardDeckController.myPlayer.AllCardTransparentImageOnOff(true);
            }
            audioManager.GamePlayAudioSetting(audioManager.cardDistributionClip);
        }

        public void PreCardSelectRequest(string cardName, bool isForward)
        {
            cardDeckController.myPlayer.AllCardTransparentImageOnOff(true);
            //socketHandler.DataSendToSocket(SocketEvents.PRE_CARD_PASS_SELECT.ToString(), socketEventManager.PreCardPassSelect(cardName, isForward, gameManager.tableId, gameManager.userId));
        }
    }

    [System.Serializable]
    public class PreSelectCardRequestData
    {
        public string userId;
        public string tableId;
        public string card;
        public bool forwardCardMove;
    }

    [System.Serializable]
    public class PreSelectCardRequest
    {
        public PreSelectCardRequestData data;
    }

    [System.Serializable]
    public class PreSelectCardResponseData
    {
        public List<string> passCards;
        public string card;
        public bool forwardCardMove;
        public string userId;
        public int si;
    }

    [System.Serializable]
    public class PreSelectCardResponse
    {
        public string en;
        public PreSelectCardResponseData data;
    }
}