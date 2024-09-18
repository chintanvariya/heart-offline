using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Newtonsoft.Json;

namespace FGSOfflineHeart
{
    public class HT_CardDeckController : MonoBehaviour
    {
        public float s1, s2;

        [Header("===== Card Data =====")]
        [SerializeField] private HT_CardController cardPrefab;
        public HT_PlayerController myPlayer;
        public Sprite cardBackSprite;
        public Transform cardGenerateTransform, cardDestinationTransform;
        public List<string> cardListName;
        public List<HT_CardController> cardsList;
        public Ease ease;

        [Header("===== Opponent Card Data =====")]
        public List<HT_PlayerController> opponentCardTransformList;
        [SerializeField] private List<HT_CardController> dummyCardList;

        [Header("===== Card Distribution Data =====")]
        public bool isCardDistribution;

        [Header("==== Object Script =====")]
        public HT_AllCardDetails allCardDetails;
        [SerializeField] private HT_EventManager eventManager;
        [SerializeField] private HT_GameManager gameManager;
        [SerializeField] private HT_UiManager uiManager;

        [SerializeField] private HT_AudioManager audioManager;
        [SerializeField] private HT_OfflineCardDistributor offlineCardDistributor;

        [Header("===== Card Response =====")]
        [SerializeField] private ShowCardResponse showCardResponse;

        [Header("===== Coroutine =====")]
        Coroutine cardAnimation, showCardAnimation, spriteAnimation;

        [Space(10)]
        public bool isAnimationStart;

        private void OnEnable() => eventManager.RegisterEvent(SocketEvents.SHOW_MY_CARDS, ShowCardFromServer);

        private void OnDisable() => eventManager.UnregisterEvent(SocketEvents.SHOW_MY_CARDS, ShowCardFromServer);

        private void Start()
        {
            gameManager.RoundReset += ResetCardDeckController;
            gameManager.GameReset += ResetGameCardDeckController;
        }

        private void ShowCardFromServer(string data)
        {
            showCardResponse = JsonConvert.DeserializeObject<ShowCardResponse>(data);
            CardShow(showCardResponse.data.cards);
            gameManager.tableState = TableState.START_DEALING_CARD;
        }

        public void CardShow(List<string> cardList)
        {
            uiManager.leaveBtn.interactable = true;
            ShowCardAnimation(cardList);
            uiManager.OtherPanelOpen(uiManager.winningPanel, false);
            uiManager.AllPopupOff();
        }

        public void ShowCardAnimation(List<string> cardList)
        {
            GetCardPositionAndRotationForOtherPlayer(cardList.Count, myPlayer, out myPlayer.allVectors, out myPlayer.rotationVectors);
            CardGenerate(cardList);
            cardAnimation = StartCoroutine(CardAnimation(cardList));
        }

        IEnumerator CardAnimation(List<string> cardList)
        {
            for (int i = 0; i < myPlayer.cardControllers.Count; i++)
            {
                var card = myPlayer.cardControllers[i];
                card.siblingIndex = i;
                card.transform.SetParent(cardDestinationTransform);
                audioManager.GamePlayAudioSetting(audioManager.cardDistributionClip);
                card.myRectTransform.DOLocalMove(myPlayer.allVectors[i], 2f).SetEase(ease, s1, s2);
                yield return new WaitForSeconds(0.1f);
            }
            myPlayer.HeartSpadeObjReset();
            showCardAnimation = StartCoroutine(ShowCardAnimationOpponent(cardList));
            StopCoroutine(cardAnimation);
        }

        IEnumerator ShowCardAnimationOpponent(List<string> cardList)
        {
            DummyCardGenerate(39);
            int k = 0;
            int endPoint = 13;
            for (int i = 0; i < opponentCardTransformList.Count; i++)
            {
                int n = 0;
                var opponentTransform = opponentCardTransformList[i];
                opponentTransform.allVectors.Clear();
                opponentTransform.rotationVectors.Clear();
                GetCardPositionAndRotationForOtherPlayer(13, opponentTransform, out opponentTransform.allVectors, out opponentTransform.rotationVectors);
                opponentTransform.HeartSpadeObjReset();
                for (int j = k; j < endPoint; j++)
                {
                    var card = dummyCardList[j];
                    if (gameManager.isOffline)
                    {
                        string cardName = CardNameForOffline();
                        card.myName = cardName;
                        card.name = cardName;
                        card.cardType = card.GetCardType(cardName);
                        int value = int.Parse(card.myName.Substring(2));
                        card.cardValue = value == 1 ? 14 : value;
                    }
                    opponentTransform.cardControllers.Add(card);
                    card.transform.SetParent(opponentTransform.cardHolderTransform);
                    audioManager.GamePlayAudioSetting(audioManager.cardDistributionClip);
                    card.myRectTransform.DOLocalMove(opponentTransform.allVectors[n], 0.2f);
                    n++;
                    yield return new WaitForSeconds(0.1f);
                }
                k += 13;
                endPoint += 13;
            }

            SetSpriteOfCard(cardList);
            StopCoroutine(showCardAnimation);
        }

        void DummyCardGenerate(int cardCount)
        {
            dummyCardList.Clear();
            for (int j = 0; j < cardCount; j++)
            {
                HT_CardController cardClone = Instantiate(cardPrefab, cardGenerateTransform);
                cardClone.cardSprite.raycastTarget = false;
                dummyCardList.Add(cardClone);
            }
        }

        void SetSpriteOfCard(List<string> cardsName) => spriteAnimation = StartCoroutine(SetSpriteAnimation(cardsName));

        IEnumerator SetSpriteAnimation(List<string> cardsName)
        {
            for (int i = 0; i < cardsName.Count; i++)
            {
                var cardName = cardsName[i];
                var card = myPlayer.cardControllers[i];
                card.TransparentOnOff(true);
                Sprite cardSprite = allCardDetails.GetSpriteOfCard(cardName);
                card.myRectTransform.DOLocalRotate(new Vector3(0, 90, 0), 0.09f).OnComplete(() =>
                {
                    card.SetMySprite(cardSprite);
                    card.myRectTransform.DOLocalRotate(new Vector3(0, 0, 0), 0.09f).OnComplete(() =>
                    {
                        if ((gameManager.isOffline && uiManager.cardPassPanel.activeInHierarchy) || (!gameManager.isOffline && gameManager.tableState == TableState.CARD_PASS_ROUND_STARTED))
                            card.TransparentOnOff(false);
                    });
                });
                yield return new WaitForSeconds(0.1f);
            }
            gameManager.CardTargetRaycastOnOff(true);
            Debug.Log($"<color=green>HT_CardDeckController || SetSpriteAnimation || ALL SET</color>");
            if (gameManager.isOffline)
                StopCoroutine(spriteAnimation);
        }

        public void CardListUpdate()
        {
            myPlayer.cardControllers.Clear();
            foreach (Transform item in cardDestinationTransform)
            {
                myPlayer.cardControllers.Add(item.GetComponent<HT_CardController>());
            }
        }

        public void GetCardPositionAndRotationForOtherPlayer(int cardsCount, HT_PlayerController player, out List<Vector3> positionList, out List<Vector3> rotationList)
        {
            player.width = cardsCount * player.multiWidth;
            player.height = player.fixHeight;
            player.yPerCard = -0.005f;

            float radius = Mathf.Abs(player.height) < 0.001f ? player.width * player.width / 0.001f * Mathf.Sign(player.height) : player.height / 2f + player.width * player.width / (8f * player.height);
            float angle = 2f * Mathf.Asin(0.5f * player.width / radius) * Mathf.Rad2Deg;
            angle = Mathf.Sign(angle) * Mathf.Min(Mathf.Abs(angle), player.maxCardAngle * (cardsCount - 1));
            float cardAngle = cardsCount == 1 ? 0f : angle / (cardsCount - 1f);
            positionList = new List<Vector3>();
            rotationList = new List<Vector3>();
            for (int i = 0; i < cardsCount; i++)
            {
                Vector3 position = new Vector3(0f, radius, 0f);
                position = Quaternion.Euler(0f, 0f, angle / 2f - cardAngle * i) * position;
                position.y += myPlayer.height - radius;
                position += i * new Vector3(0f, player.yPerCard, player.zDistance);
                position = !player.isHOrizontal ? new Vector3(position.y, position.x, position.z) : position;

                positionList.Add(position);
                rotationList.Add(new Vector3(0f, 0f, angle / 2f - cardAngle * i));
            }
        }

        public void UpdateCardPosition(HT_PlayerController player, bool moveAnimation, float moveDuration, Action SpriteTransformation = null)
        {
            isAnimationStart = true;
            player.width = player.cardControllers.Count * player.multiWidth;
            player.height = player.fixHeight;
            player.yPerCard = -0.005f;

            float radius = Mathf.Abs(player.height) < 0.001f ? player.width * player.width / 0.001f * Mathf.Sign(player.height) : player.height / 2f + player.width * player.width / (8f * player.height);
            float angle = 2f * Mathf.Asin(0.5f * player.width / radius) * Mathf.Rad2Deg;
            angle = Mathf.Sign(angle) * Mathf.Min(Mathf.Abs(angle), player.maxCardAngle * (player.cardControllers.Count - 1));
            float cardAngle = player.cardControllers.Count == 1 ? 0f : angle / (player.cardControllers.Count - 1f);

            for (int i = 0; i < player.cardControllers.Count; i++)
            {
                Vector3 position = new Vector3(0f, radius, 0f);
                position = Quaternion.Euler(0f, 0f, angle / 2f - cardAngle * i) * position;
                position.y += player.height - radius;
                position += i * new Vector3(0f, player.yPerCard, player.zDistance);
                position = !player.isHOrizontal ? new Vector3(position.y, position.x, position.z) : position;

                if (moveAnimation)
                {
                    player.cardControllers[i].myRectTransform.DOKill();
                    player.cardControllers[i].myRectTransform.DOLocalMove(position, moveDuration).OnComplete(() =>
                    {
                        isAnimationStart = false;
                        gameManager.tableState = gameManager.isOffline ? TableState.OFFLINE : TableState.ROUND_STARTED;
                    });
                }
                else
                {
                    player.cardControllers[i].myRectTransform.localPosition = position;
                    isAnimationStart = false;
                }
            }
            if (moveAnimation)
            {
                DOVirtual.DelayedCall(moveDuration / 2, () =>
                {
                    SpriteTransformation?.Invoke();
                });
            }
            else SpriteTransformation?.Invoke();
        }

        public void CardGenerate(List<string> cardNames)
        {
            for (int i = 0; i < cardNames.Count; i++)
            {
                HT_CardController cardClone = Instantiate(cardPrefab, cardGenerateTransform);
                string cardName = cardNames[i];
                cardClone.name = cardName;
                cardClone.myName = cardName;
                if (gameManager.isOffline)
                {
                    int value = int.Parse(cardName.Substring(2));
                    cardClone.cardValue = value == 1 ? 14 : value;
                }
                myPlayer.cardControllers.Add(cardClone);
            }
        }

        string CardNameForOffline()
        {
            string cardsName = "";
            int randNum = UnityEngine.Random.Range(0, offlineCardDistributor.cardsName.Count);
            cardsName = offlineCardDistributor.cardsName[randNum];
            offlineCardDistributor.cardsName.Remove(cardsName);
            return cardsName;
        }

        public void ResetCardDeckController()
        {
            StopAllCoroutines();

            isCardDistribution = false;

            if (cardAnimation != null)
                StopCoroutine(cardAnimation);
            if (spriteAnimation != null)
                StopCoroutine(spriteAnimation);
            if (showCardAnimation != null)
                StopCoroutine(showCardAnimation);
            ResetGameCardDeckController();
        }

        public void ResetGameCardDeckController()
        {
            foreach (var card in dummyCardList)
            {
                if (card != null)
                {
                    card.myRectTransform.DOKill();
                    Destroy(card.gameObject);
                }
            }
            dummyCardList.Clear();
        }
    }

    [Serializable]
    public class ShowCardResponseData
    {
        public List<string> cards;
        public int currentRound;
    }

    [Serializable]
    public class ShowCardResponse
    {
        public string en;
        public ShowCardResponseData data;
    }
}