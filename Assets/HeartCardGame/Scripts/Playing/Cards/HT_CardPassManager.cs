using UnityEngine;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;
using UnityEngine.UI;

namespace HeartCardGame
{
    public class HT_CardPassManager : MonoBehaviour
    {
        [Header("===== Pass Info Data =====")]
        [SerializeField] private TextMeshProUGUI passBtnTxt;
        [SerializeField] private TextMeshProUGUI passCardTxt, roundNumTxt, timeTxt;
        public Button cardPassBtn;
        public List<RectTransform> cardPassTransforms;
        int timer;

        [Header("===== Card Info =====")]
        public List<HT_CardController> cardControllerList;

        [Header("===== Object Script =====")]

        [SerializeField] private HT_GameManager gameManager;
        [SerializeField] private HT_CardDeckController cardDeckController;
        [SerializeField] private HT_PlayerController myPlayer;
        [SerializeField] private HT_JoinTableHandler joinTableHandler;
        [SerializeField] private HT_UiManager uiManager;

        private void Start() => gameManager.GameReset += ResetCardPassManager;

        public void PassCardDataSetting(string passBtn, int roundNum, int time)
        {
            passBtnTxt.SetText($"Pass {passBtn.ToLower()}");
            passCardTxt.SetText($"Pass 3 card {passBtn.ToLower()}");
            roundNumTxt.SetText($"{roundNum}");
            gameManager.tableState = gameManager.isOffline ? TableState.OFFLINE : TableState.CARD_PASS_ROUND_STARTED;
            uiManager.cardPassPanel.SetActive(true);
            var player = joinTableHandler.GetMyPlayer();
            if (cardControllerList.Count != 3)
                player.AllCardTransparentImageOnOff(false);
            timer = time;
            InvokeRepeating(nameof(TimerStart), 0f, 1f);
        }

        void TimerStart()
        {
            if (timer <= 1 && gameManager.isOffline)
                CardSettingOnPassCard();
            if (timer >= 0)
            {
                timeTxt.SetText($"Cards will passed after {timer} seconds...");
                timer--;
            }
            else CancelInvoke(nameof(TimerStart));
        }

        public void CardSetOnEmptyBox(HT_CardController card, float speed, bool isAuto = false)
        {
            for (int i = 0; i < cardPassTransforms.Count; i++)
            {
                var transform = cardPassTransforms[i];
                if (transform.childCount < 1)
                {
                    myPlayer.AllCardTransparentImageOnOff(true);
                    Debug.Log($"HT_CardPassManager || CardSetOnEmptyBox || Card {card}");
                    card.myRectTransform.SetParent(transform);
                    card.myRectTransform.DOLocalMove(Vector3.zero, speed).OnComplete(() =>
                    {
                        if (cardControllerList.Count != 3)
                        {
                            myPlayer.AllCardTransparentImageOnOff(false);
                            cardPassBtn.interactable = false;
                        }
                        else
                            cardPassBtn.interactable = true;
                        if (!isAuto)
                            card.TransparentOnOff(false);
                    });
                    break;
                }
            }
        }

        public void CardResetOnOldPosition(HT_CardController card)
        {
            Debug.Log($"HT_CardPassManager || CardResetOnOldPosition || CARD NAME {card.myName}");
            card.myRectTransform.SetParent(cardDeckController.cardDestinationTransform);
            card.myRectTransform.SetSiblingIndex(card.siblingIndex);
            cardDeckController.myPlayer.AllCardTransparentImageOnOff(false);
            cardPassBtn.interactable = false;
        }

        public void CardPassList()
        {
            if (cardControllerList.Count == 3)
            {
                myPlayer.AllCardTransparentImageOnOff(true);
                cardPassBtn.interactable = true;
            }
            else
            {
                myPlayer.AllCardTransparentImageOnOff(false);
                cardPassBtn.interactable = false;
            }
        }

        public void CardPassRequest()
        {
            if (gameManager.isOffline)
                CardSettingOnPassCard();
            else
            {
                List<string> stringList = cardControllerList.Select(obj => obj.myName).ToList();
                //socketHandler.DataSendToSocket(SocketEvents.CARD_PASS.ToString(), socketEventManager.PassCardRequest(gameManager.tableId, gameManager.userId, stringList));
            }
        }

        void CardSettingOnPassCard()
        {
            cardControllerList.ForEach(card => card.TransparentOnOff(true));
            cardDeckController.myPlayer.cardControllers.ForEach(card => card.TransparentOnOff(true));
        }

        public void ResetCardPassManager()
        {
            passBtnTxt.SetText("");
            passCardTxt.SetText("");
            roundNumTxt.SetText("");
            CancelInvoke(nameof(TimerStart));
            cardPassBtn.interactable = false;
            foreach (var item in cardControllerList)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }
            cardControllerList.Clear();
        }
    }
}