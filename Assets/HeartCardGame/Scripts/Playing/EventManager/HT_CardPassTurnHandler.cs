using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace FGSOfflineHeart
{
    public class HT_CardPassTurnHandler : MonoBehaviour
    {
        [Header("===== Object Script =====")]
        [SerializeField] private HT_EventManager eventManager;
        [SerializeField] private HT_CardPassManager cardPassManager;
        [SerializeField] private HT_CardDeckController cardDeckController;
        [SerializeField] private HT_AudioManager audioManager;

        [Header("===== Model Class =====")]
        [SerializeField] private CardPassTurnResponse cardPassTurnResponse;

        private void OnEnable() => eventManager.RegisterEvent(SocketEvents.USER_CARD_PASS_TURN, CardPassTurnSetting);

        private void OnDisable() => eventManager.UnregisterEvent(SocketEvents.USER_CARD_PASS_TURN, CardPassTurnSetting);

        private void CardPassTurnSetting(string arg0)
        {
            cardPassTurnResponse = JsonConvert.DeserializeObject<CardPassTurnResponse>(arg0);
            cardPassManager.cardControllerList.Clear();
            cardPassManager.PassCardDataSetting(cardPassTurnResponse.data.cardMoveSide, cardPassTurnResponse.data.currentRound, cardPassTurnResponse.data.time);
            Debug.Log($"HT_CardPassTurnHandler || CardPassTurnSetting || Card Count {cardDeckController.myPlayer.cardControllers.Count}");
            cardDeckController.myPlayer.AllCardTransparentImageOnOff(false);
            audioManager.GamePlayAudioSetting(audioManager.userTurnClip);
        }
    }

    [System.Serializable]    
    public class CardPassPlayersDatum
    {
        public string userId;
        public int si;
    }

    [System.Serializable]
    public class CardPassTurnResponseData
    {
        public List<CardPassPlayersDatum> cardPassPlayersData;
        public int time;
        public string cardMoveSide;
        public int currentRound;
    }

    [System.Serializable]
    public class CardPassTurnResponse
    {
        public string en;
        public CardPassTurnResponseData data;
    }
}
