using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HeartCardGame
{
    public class HT_CardController : MonoBehaviour, IPointerDownHandler
    {
        [Header("===== Card Details =====")]
        public string myName;
        public int siblingIndex, cardValue;
        public CardType cardType;
        public RectTransform myRectTransform;
        public Image cardSprite;
        public GameObject transParentObj;
        public GameObject spadeParticles;

        [Header("===== Animation Object =====")]
        public HT_HeartSpadeAnimationHandler heartAnimationObj;
        public HT_HeartSpadeAnimationHandler spadeAnimationObj;

        public void TransparentOnOff(bool isOn) => transParentObj.SetActive(isOn);

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log($"HT_CardController || OnPointerDown || CARD NAME : {eventData.pointerCurrentRaycast.gameObject.name}");
            var gameManager = HT_GameManager.instance;
            if (!transParentObj.activeInHierarchy)
            {
                if (gameManager.isOffline)
                {
                    OfflineCardMoveAndPassHandler();
                    return;
                }
                if (!gameManager.isCardPassed)
                {
                    var preCarsPassHandler = gameManager.preCardPassHandler;
                    bool isForward = myRectTransform.transform.parent.name == "MyCardHolder";
                    preCarsPassHandler.PreCardSelectRequest(myName, isForward);
                }
                else if (!transParentObj.activeInHierarchy)
                {
                    gameManager.socketHandler.DataSendToSocket(SocketEvents.USER_THROW_CARD.ToString(), gameManager.socketEventManager.ThrowCardRequest(myName));
                    cardSprite.raycastTarget = false;
                }
            }
        }

        void OfflineCardMoveAndPassHandler()
        {
            var gameManager = HT_GameManager.instance;
            if (!transParentObj.activeInHierarchy)
            {
                if (!gameManager.isCardPassed)
                {
                    var preCarsPassHandler = gameManager.preCardPassHandler;
                    bool isForward = myRectTransform.transform.parent.name == "MyCardHolder";
                    if (!gameManager.cardDeckController.myPlayer.passCardList.Contains(this)) gameManager.cardDeckController.myPlayer.passCardList.Add(this);
                    if (isForward)
                    {
                        HT_OfflineGameHandler.instance.cardPassManager.cardControllerList.Add(this);
                        HT_OfflineGameHandler.instance.cardPassManager.CardSetOnEmptyBox(this, 0.2f);
                    }
                    else
                    {
                        gameManager.cardDeckController.myPlayer.passCardList.Remove(this);
                        gameManager.cardPassManager.cardControllerList.Remove(this);
                        gameManager.cardPassManager.CardResetOnOldPosition(this);
                    }
                    HT_OfflineGameHandler.instance.cardDeckController.CardListUpdate();
                    HT_OfflineGameHandler.instance.cardDeckController.UpdateCardPosition(HT_OfflineGameHandler.instance.cardDeckController.myPlayer, true, 0.2f);
                }
                else if (!transParentObj.activeInHierarchy)
                {
                    cardSprite.raycastTarget = false;
                    if (cardType == CardType.H) HT_OfflineGameHandler.instance.offlinePlayerTurnController.isBreakingHearts = true;
                    HT_OfflineGameHandler.instance.offlinePlayerTurnController.throwCardHandler.UserCardThrow(gameManager.cardDeckController.myPlayer, this, HT_OfflineGameHandler.instance.offlinePlayerTurnController.isBreakingHearts);
                }
            }
        }

        public void SetMySprite(Sprite mySprite)
        {
            cardSprite.sprite = mySprite;
            myName = mySprite.name;
            cardType = GetCardType(myName);
        }

        public CardType GetCardType(string cardName)
        {
            string card = cardName.Substring(0, 1);
            CardType cardType = (CardType)Enum.Parse(typeof(CardType), card);
            return cardType;
        }

        public void SpadeParticles()
        {
            if (myName.Equals("S-12"))
            {
                spadeParticles.SetActive(true);
                HT_GameManager.instance.audioManager.GamePlayAudioSetting(HT_GameManager.instance.audioManager.spadeQClip);
            }
        }
    }

    public enum CardType
    {
        H,
        S,
        C,
        D
    }
}
