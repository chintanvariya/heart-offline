using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using FGSOfflineCallBreak;

namespace HeartCardGame
{
    public class HT_PlayerController : MonoBehaviour
    {
        [Header("===== Player Data =====")]
        public PlayerState playerState;
        public string userId, userName, currentHeartCount, totalHeartCount;
        public bool isMyPlayer;
        public int mySeatIndex;
        public Image mySprite;
        public Sprite defaultSprite;
        public GameObject disconnectedObj, leftObj;
        Coroutine textureCoroutine;

        [Header("===== Heart Spade Info =====")]
        public int roundHeartPoint;
        public int roundSpadePoint, totalHeartPoint, totalSpadePoint, totalPoint;

        [Header("===== Player Properties =====")]
        public GameObject player;
        public Transform handCardStore;
        public RectTransform heartInfoObj, spadeInfoObj;
        public TextMeshProUGUI nameTxt, heartInfoTxt, spadeInfoTxt, chipsTxt;

        [Header("===== Player Card Data =====")]
        public List<HT_CardController> cardControllers;
        public List<HT_CardController> passCardList;
        public HT_CardController handCard;
        public Transform cardHolderTransform;
        public RectTransform deckCardHolder;

        [Header("===== Turn Data =====")]
        public bool isTurnStarted;

        [Header("===== Object Script =====")]
        public HT_TurnTimerController turnTimerController;

        [Header("===== Shooting Moon Animation =====")]
        [SerializeField] private RectTransform heartAnimationObj;
        [SerializeField] private RectTransform spadeAnimationObj;
        public List<RectTransform> heartRects, spadeRects;
        Coroutine shootingMoonCor;

        [Header("===== Model Class =====")]
        [SerializeField] private SignupResponse signupResponse;

        [Header("===== Card Setting Data =====")]
        public float multiWidth;
        public float fixHeight, height, width, yPerCard, zDistance, moveDuration;
        public bool isAnimationStart, isHOrizontal;
        [Range(0f, 90f)] public float maxCardAngle;// 5f;
        public List<Vector3> allVectors;
        public List<Vector3> rotationVectors;

        public void SetPlayerState(string currentState) => playerState = (PlayerState)Enum.Parse(typeof(PlayerState), currentState);

        public void SetPlayerInfo(string playerId, string tableId, string profilrURL)
        {
            userId = playerId;
            HT_GameManager.instance.userId = playerId;
            HT_GameManager.instance.tableId = tableId;
            HT_GameManager.instance.tableTxt.text = "#" + tableId;
        }

        //public void BlurImgOnOff(bool isOpen) => blurImgObj.SetActive(isOpen);

        public void DisconnectedObjOnOff(bool isOpen) => disconnectedObj.SetActive(isOpen);

        public void LeftObjectOnOff(bool isOpen) => leftObj.SetActive(isOpen);

        public void PlayerDataSet(Seat playerData)
        {
            if (!isMyPlayer)
                nameTxt.SetText(playerData.name);
            userName = playerData.name;
            mySeatIndex = playerData.si;
            userId = playerData.userId;
            SetPlayerState(playerData.userState);

            //if (mySprite.sprite == defaultSprite)
            //    GetTexture(playerData.pp);
        }

        public void OfflinePlayerDataset(string playerName, int si, Sprite playerProfile)
        {
            if (isMyPlayer)
            {
                chipsTxt.text = CallBreakUtilities.AbbreviateNumber(CallBreakGameManager.instance.selfUserDetails.userChips);
            }
            nameTxt.SetText(playerName);
            userName = playerName;
            mySeatIndex = si;
            mySprite.sprite = playerProfile;
        }

        public void HeartSpadeInfoSet(int heartCount, int spadeCount)
        {
            Debug.Log($"HT_PlayerController {mySeatIndex} || HeartSpadeInfoSet || heartCount {heartCount} || spadeCount {spadeCount}");
            if (heartCount > 0)
            {
                HeartSpadeObjectActive(heartInfoObj.gameObject, true);
                heartInfoTxt.SetText(heartCount.ToString());
            }
            if (spadeCount > 0)
            {
                HeartSpadeObjectActive(spadeInfoObj.gameObject, true);
                spadeInfoTxt.SetText(spadeCount.ToString());
            }
        }

        public void HeartSpadeObjectActive(GameObject obj, bool isOpen) => obj.SetActive(isOpen);

        public void CardThrow(HT_CardController cardController, bool isBreakingHeart)
        {
            Debug.Log($"HT_PlayerController || CardThrow || CARD NAME : {cardController.name} {isMyPlayer}");
            handCard = null;
            cardController.myRectTransform.SetParent(deckCardHolder);
            handCard = cardController;
            if (isMyPlayer)
            {
                cardController.myRectTransform.DOSizeDelta(deckCardHolder.sizeDelta, 0.2f);
                cardController.myRectTransform.DOLocalMove(Vector3.zero, 0.2f).OnStart(() =>
                {
                    AllCardTransparentImageOnOff(true);
                    turnTimerController.ResetTime();
                    HT_GameManager.instance.cardDeckController.UpdateCardPosition(this, true, 0.2f);
                    cardController.SpadeParticles();
                }).OnComplete(() =>
                {
                    if (isBreakingHeart && HT_GameManager.instance.isHeartAnimationShow)
                        HT_GameManager.instance.uiManager.heartBrokenPanel.SetActive(true);
                    if (HT_GameManager.instance.uiManager.heartBrokenPanel.activeInHierarchy)
                    {
                        DOVirtual.DelayedCall(1f, () =>
                        {
                            if (HT_GameManager.instance.isOffline && HT_OfflineGameHandler.instance.offlinePlayerTurnController.turnCardSequence != "N" && HT_OfflineGameHandler.instance.offlineWinOfRoundHandler.handedCards.Count < 4)
                                HT_OfflineGameHandler.instance.offlinePlayerTurnController.OfflineTurnHandler();
                        });
                    }
                    else
                    {
                        if (HT_GameManager.instance.isOffline && HT_OfflineGameHandler.instance.offlinePlayerTurnController.turnCardSequence != "N" && HT_OfflineGameHandler.instance.offlineWinOfRoundHandler.handedCards.Count < 4)
                            HT_OfflineGameHandler.instance.offlinePlayerTurnController.OfflineTurnHandler();
                    }
                });
            }
            else
            {
                cardController.myRectTransform.DOSizeDelta(deckCardHolder.sizeDelta, 0.2f);
                cardController.myRectTransform.DOLocalMove(Vector3.zero, 0.2f).OnStart(() =>
                {
                    Debug.Log($"Opponent card move");
                    HT_GameManager.instance.cardDeckController.UpdateCardPosition(this, true, 0.2f);
                    HT_GameManager.instance.turnInfoManager.AllPlayerTimerOff();
                    cardController.SpadeParticles();
                }).OnComplete(() =>
                {
                    if (isBreakingHeart && HT_GameManager.instance.isHeartAnimationShow)
                        HT_GameManager.instance.uiManager.heartBrokenPanel.SetActive(true);
                });
                cardController.myRectTransform.DOLocalRotate(new Vector3(0, 90, 0), 0.1f).OnComplete(() =>
                {
                    Sprite sprite = HT_GameManager.instance.allCardDetails.GetSpriteOfCard(cardController.myName);
                    Debug.Log($"Rotation Complete {sprite.name}");
                    cardController.SetMySprite(sprite);
                    cardController.myRectTransform.DOLocalRotate(Vector3.zero, 0.1f);
                });
            }
            cardControllers.Remove(cardController);
        }

        public void CardPassGenerate(List<string> _CardControllers)
        {
            Debug.Log($"HT_PlayerController || CardPassGenerate || Count {_CardControllers.Count} AND COUNT {cardControllers.Count}");
            for (int i = 0; i < cardControllers.Count; i++)
            {
                HT_CardController card = cardControllers[i];
                card.name = _CardControllers[i];
                card.myRectTransform.eulerAngles = Vector3.zero;
                Sprite cardSprite = HT_GameManager.instance.allCardDetails.GetSpriteOfCard(_CardControllers[i]);
                card.SetMySprite(cardSprite);
                HT_GameManager.instance.cardPassManager.CardSetOnEmptyBox(card, 0.2f);
                card.cardSprite.raycastTarget = true;
                card.TransparentOnOff(true);
                if (HT_GameManager.instance.isOffline)
                {
                    int value = int.Parse(_CardControllers[i].Substring(2));
                    card.cardValue = value == 1 ? 14 : value;
                }
            }

            for (int i = 0; i < cardControllers.Count; i++)
            {
                var card = cardControllers[i];
                card.myRectTransform.SetParent(HT_GameManager.instance.cardDeckController.cardDestinationTransform);
                card.myRectTransform.SetSiblingIndex(i);
            }
            HT_GameManager.instance.cardDeckController.UpdateCardPosition(this, true, 0.3f);
            HT_GameManager.instance.uiManager.cardPassPanel.SetActive(false);
        }

        public void CardSetting()
        {
            for (int i = 0; i < cardControllers.Count; i++)
            {
                var card = cardControllers[i];
                card.myRectTransform.DOLocalRotate(new Vector3(0, 180, 0), 0.3f);
                card.TransparentOnOff(false);
                card.cardSprite.sprite = HT_GameManager.instance.cardDeckController.cardBackSprite;
            }
            HT_GameManager.instance.cardDeckController.UpdateCardPosition(this, true, 0.3f);
        }

        public void HandCardGenerate(HT_CardController cardClone, string currentCard)
        {
            cardClone.name = currentCard;
            cardClone.myName = currentCard;
            Sprite sprite = HT_GameManager.instance.allCardDetails.GetSpriteOfCard(currentCard);
            cardClone.SetMySprite(sprite);
            cardClone.myRectTransform.eulerAngles = Vector3.zero;
            handCard = cardClone;
        }

        public void HeartSpadeObjectGenerate(List<HT_PlayerController> players)
        {
            for (int i = 0; i < players.Count; i++)
            {
                var player = players[i];
                RectTransform rt = Instantiate(player.spadeAnimationObj, spadeInfoObj.transform);
                rt.anchoredPosition = Vector3.zero;
                player.spadeRects.Add(rt);
                for (int j = 0; j < 13; j++)
                {
                    RectTransform rectTransform = Instantiate(player.heartAnimationObj, heartInfoObj.transform);
                    rectTransform.anchoredPosition = Vector3.zero;
                    player.heartRects.Add(rectTransform);
                }
                player.HeartSpadeObjectActive(player.heartInfoObj.gameObject, true);
                player.HeartSpadeObjectActive(player.spadeInfoObj.gameObject, true);
            }
        }

        public void ShootingMoonAnimation(List<RectTransform> heartsRects, RectTransform heartSpadeRectTransform, bool isHeart, HT_PlayerController player) => shootingMoonCor = StartCoroutine(ShootingMoon(heartsRects, heartSpadeRectTransform, isHeart, player));

        IEnumerator ShootingMoon(List<RectTransform> rects, RectTransform heartSpadeRectTransform, bool isHeart, HT_PlayerController player)
        {
            int count = 0;
            int playerCount = 13;
            for (int i = 0; i < rects.Count; i++)
            {
                var rt = rects[i];
                rt.transform.SetParent(heartSpadeRectTransform);

                rt.DOLocalMove(Vector3.zero, 0.5f).OnComplete(() =>
                {
                    Destroy(rt.gameObject);
                    count++;
                    playerCount--;
                    if (isHeart)
                    {
                        player.heartInfoTxt.SetText($"{count}");
                        heartInfoTxt.SetText($"{playerCount}");
                    }
                    else
                    {
                        count = 13;
                        player.spadeInfoTxt.SetText($"{count}");
                        spadeInfoTxt.SetText($"0");
                    }
                    heartSpadeRectTransform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.05f).OnComplete(() => heartSpadeRectTransform.DOScale(Vector3.one, 0.2f));
                });
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(0.4f);
            if (isHeart)
                HeartSpadeObjectActive(heartInfoObj.gameObject, false);
            else
                HeartSpadeObjectActive(spadeInfoObj.gameObject, false);
            rects.Clear();
        }

        //public void GetTexture(string profileURL) => textureCoroutine = StartCoroutine(GetTexture(profileURL, loaderAnimator, (sprite) => mySprite.sprite = sprite));

        public IEnumerator GetTexture(string profileURL, GameObject loader, Action<Sprite> getSprite)
        {
            loader.SetActive(true);
            UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(profileURL);
            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.result != UnityWebRequest.Result.Success)
                Debug.LogError($"HT_UiManager || GetTexture || Error : {unityWebRequest.error}");
            else
            {
                Texture2D myTexture = ((DownloadHandlerTexture)unityWebRequest.downloadHandler).texture;
                Sprite mySprite = Sprite.Create(myTexture, new Rect(0.0f, 0.0f, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
                getSprite?.Invoke(mySprite);
                loader.SetActive(false);
            }
        }

        public void AllCardTransparentImageOnOff(bool isOn)
        {
            foreach (var card in cardControllers)
                card.TransparentOnOff(isOn);
        }

        public void ResetPlayerController()
        {
            foreach (var card in cardControllers)
            {
                if (card != null)
                {
                    card.myRectTransform.DOKill();
                    Destroy(card.gameObject);
                }
            }

            if (shootingMoonCor != null)
                StopCoroutine(shootingMoonCor);

            if (textureCoroutine != null)
                StopCoroutine(textureCoroutine);

            cardControllers.Clear();
            passCardList.Clear();

            if (handCard != null)
                Destroy(handCard.gameObject);
            handCard = null;

            allVectors.Clear();

            HeartSpadeObjectActive(heartInfoObj.gameObject, false);
            HeartSpadeObjectActive(spadeInfoObj.gameObject, false);

            roundHeartPoint = 0;
            roundSpadePoint = 0;

        }

        public void ResetPlayers()
        {
            foreach (var item in passCardList)
            {
                if (item != null)
                {
                    item.myRectTransform.DOKill();
                    Destroy(item.gameObject);
                }
            }

            if (textureCoroutine != null)
                StopCoroutine(textureCoroutine);


            passCardList.Clear();

            userId = string.Empty;
            userName = string.Empty;
            currentHeartCount = string.Empty;
            totalHeartCount = string.Empty;
            mySprite.sprite = defaultSprite;
            mySeatIndex = 0;

            LeftObjectOnOff(false);
            DisconnectedObjOnOff(false);

            if (!isMyPlayer)
                nameTxt.SetText($"");

            HeartSpadeObjReset();

            isTurnStarted = false;

            turnTimerController.ResetTime();

            totalPoint = 0;
            totalSpadePoint = 0;
            totalHeartPoint = 0;
        }

        public void HeartSpadeObjReset()
        {
            heartInfoTxt.SetText($"");
            spadeInfoTxt.SetText($"");
            HeartSpadeObjectActive(heartInfoObj.gameObject, false);
            HeartSpadeObjectActive(spadeInfoObj.gameObject, false);
        }
    }

    public enum PlayerState
    {
        PLAYING,
        LEFT,
        WON,
        TIE,
        NONE
    }
}