using UnityEngine;
using Newtonsoft.Json;
using DG.Tweening;
using System.Collections.Generic;

namespace FGSOfflineHeart
{
    public class HT_WinOfRound : MonoBehaviour
    {
        [Header("===== Object Script =====")]
        [SerializeField] private HT_EventManager eventManager;
        [SerializeField] private HT_JoinTableHandler joinTableHandler;
        [SerializeField] private HT_GameManager gameManager;
        [SerializeField] private HT_OfflineWinOfRoundHandler offlineWinOfRoundHandler;

        [Header("===== Card Data =====")]
        public List<HT_CardController> handedCards;
        [SerializeField] private List<HT_HeartSpadeAnimationHandler> heartSpadesAnimation;

        [Header("===== Model Class =====")]
        [SerializeField] private WinOfRoundResponse winOfRoundResponse;

        private void OnEnable() => eventManager.RegisterEvent(SocketEvents.WIN_OF_ROUND, WinOfRoundSetting);

        private void OnDisable() => eventManager.UnregisterEvent(SocketEvents.WIN_OF_ROUND, WinOfRoundSetting);

        private void Start()
        {
            gameManager.GameReset += DestroyHandedCards;
            gameManager.GameReset += DestroyHeartSpadeObject;
        }

        private void WinOfRoundSetting(string arg0)
        {
            winOfRoundResponse = JsonConvert.DeserializeObject<WinOfRoundResponse>(arg0);
            HT_PlayerController player = joinTableHandler.GetAnyPlayer(winOfRoundResponse.data.seatIndex);
            WinOfRoundSetting(player);
        }

        public void WinOfRoundSetting(HT_PlayerController player)
        {
            DestroyHandedCards();

            foreach (var players in joinTableHandler.playerData)
            {
                handedCards.Add(players.handCard);
            }
            FindHeartSpadeAnimation();
            if (gameManager.isOffline)
                HeartAndSpadeAnimation(player);
            else
                HeartAndSpadeAnimation(player, winOfRoundResponse.data.heartPoint, winOfRoundResponse.data.spadePoint);
            Debug.Log($"HT_WinOfRound || WinOfRoundSetting || Player Name {player.mySeatIndex} || TURN INDEX {HT_OfflineGameHandler.instance.offlinePlayerTurnController.turnPlayer}");
            player.handCard.myRectTransform?.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.3f).OnComplete(() =>
            {
                foreach (var players in joinTableHandler.playerData)
                {
                    var rect = players.handCard.myRectTransform;
                    rect.SetParent(player.handCardStore);
                    rect.DOLocalMove(Vector3.zero, 0.2f);
                }
            });
            offlineWinOfRoundHandler.DestroyHandedCardInvoke();
        }

        void FindHeartSpadeAnimation()
        {
            DestroyHeartSpadeObject();
            foreach (var card in handedCards)
            {
                if (card.cardType == CardType.H)
                {
                    card.heartAnimationObj.gameObject.SetActive(true);
                    heartSpadesAnimation.Add(card.heartAnimationObj);
                }
                if (card.cardType == CardType.S && card.myName == "S-12")
                {
                    card.spadeAnimationObj.gameObject.SetActive(true);
                    heartSpadesAnimation.Add(card.spadeAnimationObj);
                }
            }
        }

        void HeartAndSpadeAnimation(HT_PlayerController player, int heartPoint = 0, int spadePoint = 0)
        {
            foreach (var obj in heartSpadesAnimation)
            {
                if (obj.cardType == CardType.H)
                {
                    Debug.Log($"HT_WinOfRound || HeartAndSpadeAnimation || H CARD TYPE {obj.cardType} || Parent : {player.heartInfoObj.name}");
                    obj.heartSpadeRectTransform.SetParent(player.heartInfoObj.transform);
                    player.HeartSpadeObjectActive(player.heartInfoObj.gameObject, true);
                    if (gameManager.isOffline) heartPoint = player.roundHeartPoint += 1;
                    obj.heartSpadeRectTransform.DOLocalMove(Vector3.zero, 0.7f).OnComplete(() =>
                    {
                        obj.gameObject.SetActive(false);
                        HeartSpadeReachedAnimation(player.heartInfoObj);
                        player.HeartSpadeInfoSet(heartPoint, spadePoint);
                    });
                }
                if (obj.cardType == CardType.S)
                {
                    Debug.Log($"HT_WinOfRound || HeartAndSpadeAnimation || S CARD TYPE {obj.cardType} || Parent : {player.spadeInfoObj.name}");
                    obj.heartSpadeRectTransform.SetParent(player.spadeInfoObj.transform);
                    player.HeartSpadeObjectActive(player.spadeInfoObj.gameObject, true);
                    if (gameManager.isOffline) spadePoint = player.roundSpadePoint = 13;
                    obj.heartSpadeRectTransform.DOLocalMove(Vector3.zero, 0.7f).OnComplete(() =>
                    {
                        obj.gameObject.SetActive(false);
                        HeartSpadeReachedAnimation(player.spadeInfoObj);
                        player.HeartSpadeInfoSet(heartPoint, spadePoint);
                    });
                }
            }
        }

        public void HeartSpadeReachedAnimation(RectTransform rectTransform) => rectTransform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.05f).OnComplete(() => rectTransform.DOScale(Vector3.one, 0.05f));

        void DestroyHandedCards()
        {
            foreach (var item in handedCards)
            {
                if (item != null)
                {
                    item.myRectTransform.DOKill();
                    Destroy(item.gameObject);
                }
            }
            handedCards.Clear();
        }

        void DestroyHeartSpadeObject()
        {
            foreach (var item in heartSpadesAnimation)
            {
                if (item != null)
                {
                    item.heartSpadeRectTransform.DOKill();
                    Destroy(item.gameObject);
                }
            }
            heartSpadesAnimation.Clear();
        }
    }

    [System.Serializable]
    public class WinOfRoundResponseData
    {
        public int seatIndex;
        public int handCount;
        public int penaltyPoint;
        public int spadePoint;
        public int heartPoint;
        public int currentSpadePoint;
        public int currentHeartPoint;
        public bool isBreakingHearts;
    }

    [System.Serializable]
    public class WinOfRoundResponse
    {
        public string en;
        public WinOfRoundResponseData data;
    }
}