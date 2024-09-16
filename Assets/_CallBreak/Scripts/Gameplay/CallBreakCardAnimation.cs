using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;
using System.Linq;
using System;

namespace FGSOfflineCallBreak
{
    public sealed class CallBreakCardAnimation : MonoBehaviour
    {
        public CallBreakGamePlayController gamePlayController;

        public static CallBreakCardAnimation instance;

        public static bool isSplashShow = true;

        public List<Sprite> allStaticCard = new List<Sprite>();
        public List<Sprite> allShuffleCard = new List<Sprite>();

        [Space(10)]
        public CallBreakCardController prefabCard;
        public Transform cardSpawnPoint;

        private const float cardSize = 1.4f;

        public Transform table;

        public List<CardDetail> allCardDetails = new List<CardDetail>();
        public List<CallBreakCardController> allCardsObject = new List<CallBreakCardController>();

        private void Awake()
        {
            if (instance == null) instance = this;
            StopAllCoroutines();
            DOTween.KillAll();
        }

        public void StopAllCoroutinesOnGamePlay()
        {
            CallBreakGameManager.instance.TurnDataReset();
            CallBreakUIManager.Instance.userTurnTimer.ResetUserTimer();

            for (int i = 0; i < CallBreakUIManager.Instance.gamePlayController.allPlayer.Count; i++)
                CallBreakUIManager.Instance.gamePlayController.allPlayer[i].ResetPlayerData();

            CallBreakUIManager.Instance.bidSelectionController.CloseScreen();
            CallBreakUIManager.Instance.scoreBoardController.CloseScreen();

            StopCoroutine(setAndStartGamePlay);
            StopCoroutine(cardDistributeAnimation);
            StopCoroutine(selfPlayerCard);

            StopAllCoroutines();
            DOTween.KillAll();
        }

        public Coroutine setAndStartGamePlay;
        public Coroutine cardDistributeAnimation;
        public Coroutine selfPlayerCard;

        internal void StartGamePlay(float timeToExecute)
        {
            setAndStartGamePlay = StartCoroutine(SetAndStartGamePlay(timeToExecute));
        }

        internal IEnumerator SetAndStartGamePlay(float timeToExecute)
        {
            CallBreakGameManager.instance.currentRound++;
            CallBreakGameManager.instance.clubCardCounter = 0;

            gamePlayController.allPlayer.ForEach(c => c.dealerIcon.SetActive(false));
            gamePlayController.UpdateTheRoundText();

            yield return new WaitForSeconds(timeToExecute);

            CallBreakGameManager.currentPlayerIndex = UnityEngine.Random.Range(0, 4);
            CallBreakGameManager.currentPlayerIndex = (CallBreakGameManager.currentPlayerIndex) % 4;

            gamePlayController.allPlayer[CallBreakGameManager.currentPlayerIndex].dealerIcon.SetActive(true);

            CallBreakUIManager.Instance.scoreBoardController.CloseScreen();

            CardPositionSet.instance.height = 60;
            CardPositionSet.instance.width = 800;

            //CallBreakUIManager.Instance.scoreBoardController.ResetScoreBoardData();

            allCardDetails.Clear();
            // Shuffle Card
            allShuffleCard = allStaticCard.OrderBy(a => Guid.NewGuid()).ToList();

            for (int i = 0; i < allStaticCard.Count; i++)
            {
                CardDetail cardDetail = new CardDetail();
                string[] allStaticCardSplit = allShuffleCard[i].name.Split("-");
                cardDetail.cardType = CallBreakUtilities.ReturnMyCardType(allStaticCardSplit[0]);
                cardDetail.value = CallBreakUtilities.ReturnMyCardValue(allStaticCardSplit[1]);
                cardDetail.cardName = allShuffleCard[i].name;
                allCardDetails.Add(cardDetail);
            }

            // Instantiate All 52 Card
            for (int i = 0; i < 52; i++)
            {
                CallBreakCardController cardController = Instantiate(prefabCard, gamePlayController.allPlayer[CallBreakGameManager.currentPlayerIndex].myThrowCardPos);
                cardController.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                cardController.table = table;
                cardController.name = allCardDetails[i].cardName;
                cardController.cardDetail = allCardDetails[i];
                allCardsObject.Add(cardController);
            }

            // Add 13 cards to each player.
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    gamePlayController.allPlayer[i].myCards.Add(allCardsObject[0]);
                    allCardsObject.RemoveAt(0);
                }
            }

            yield return new WaitForSeconds(1f);

            AddSelfPlayerCard();

            // Distribution Animation 
            for (int i = 0; i < 4; i++)
            {
                cardDistributeAnimation = StartCoroutine(CardDistributeAnimation(i));
                yield return new WaitForSeconds(0.1f);
            }

            // Card Distribution Animation Of Self Player  
            selfPlayerCard = StartCoroutine(SelfPlayerCard());
        }

        public void AddSelfPlayerCard()
        {
            for (int i = 0; i < 4; i++)
                gamePlayController.allPlayer[i].UpdateCardInGroup();

            CardPositionSet.instance.ResetCardData();
            for (int i = 0; i < gamePlayController.allPlayer[0].myCards.Count; i++)
            {
                CardPositionSet.instance.Add(gamePlayController.allPlayer[0].myCards[i].gameObject, false);
            }

            for (int i = 0; i < 13; i++)
                gamePlayController.allPlayer[0].myCards[i].transform.SetSiblingIndex(i);
        }

        public IEnumerator CardDistributeAnimation(int playerIndex)
        {
            // Move Cards To All Player 
            for (int i = 0; i < 13; i++)
            {
                CallBreakSoundManager.PlaySoundEvent(SoundEffects.Deal);
                gamePlayController.allPlayer[playerIndex].myCards[i].transform.DORotate(new Vector3(0, 0, -120f), 0.5f).SetEase(Ease.Linear);
                gamePlayController.allPlayer[playerIndex].myCards[i].transform.DOMove(gamePlayController.allPlayer[playerIndex].myCardPos.position, 0.5f).SetEase(Ease.Linear);
                gamePlayController.allPlayer[playerIndex].myCards[i].transform.DOScale(Vector3.zero, 0.02f).SetDelay(0.5f);
                gamePlayController.allPlayer[playerIndex].myCards[i].transform.SetParent(gamePlayController.allPlayer[playerIndex].myCardPos);
                yield return new WaitForSeconds(0.02f);
            }
        }

        public Sprite ReturnCardSprite(string cardName)
        {
            Sprite sprite = null;
            for (int i = 0; i < allShuffleCard.Count; i++)
            {
                if (allShuffleCard[i].name == cardName)
                    sprite = allShuffleCard[i];
            }
            return sprite;
        }

        internal IEnumerator SelfPlayerCard()
        {
            // Set Card To Static Position And Scale Animation
            yield return new WaitForSeconds(1f);
            CardPositionSet.instance.UpdateCardPosition();

            for (int i = 0; i < 13; i++)
            {
                gamePlayController.allPlayer[0].myCards[i].transform.DOScale(new Vector3(cardSize, cardSize, cardSize), 0.5f).SetEase(Ease.OutElastic);
                yield return new WaitForSeconds(0.05f);
            }

            yield return new WaitForSeconds(0.3f);

            // Move All Card To Middle

            for (int i = 0; i < 13; i++)
            {
                gamePlayController.allPlayer[0].myCards[i].transform.DOMove(gamePlayController.allPlayer[0].myCards[6].transform.position, 0.1f).SetEase(Ease.Linear);
            }

            yield return new WaitForSeconds(0.2f);
            CardPositionSet.instance.UpdateCardPosition();

            // Move All Card To Their Position And Assign Sprite And Card Data

            AddSelfPlayerCard();

            yield return new WaitForSeconds(0.5f);

            // Shake Card
            for (int i = 0; i < 13; i++)
            {
                gamePlayController.allPlayer[0].myCards[i].transform.DOShakeRotation(0.25f, 100, 300);
            }

            yield return new WaitForSeconds(0.5f);
            CardPositionSet.instance.UpdateCardPosition();

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 13; j++)
                    gamePlayController.allPlayer[i].myCards[j].cardImage.sprite = ReturnCardSprite(gamePlayController.allPlayer[i].myCards[j].cardDetail.cardName);

            yield return new WaitForSeconds(0.2f);

            for (int i = 0; i < gamePlayController.allPlayer.Count; i++)
            {
                gamePlayController.allPlayer[i].BidSelection();
                yield return new WaitForSeconds(0.35f);
                if (!gamePlayController.allPlayer[i].isSelfPlayer)
                    gamePlayController.allPlayer[i].BidToolTipAnimation();
            }

            CallBreakUIManager.Instance.bidSelectionController.OpenScreen();
        }

        internal void CardRaycast(bool isActive)
        {
            gamePlayController.allPlayer[0].myCards.ForEach(c => c.GetComponent<Image>().raycastTarget = isActive);
        }

        public void SelfPlayerTempObjActive(bool isActive)
        {
            gamePlayController.allPlayer[0].myCards.ForEach(c => c.transform.GetChild(1).gameObject.SetActive(isActive));
        }


        public void StartGame(int lobbyAmount)
        {
            Debug.LogError("===========================");
            CallBreakSoundManager.PlaySoundEvent(SoundEffects.Click);

            if (CallBreakGameManager.instance.selfUserDetails.userChips >= lobbyAmount)
            {
                Debug.LogError("===========================");
                CallBreakGameManager.currentLobbyAmount = lobbyAmount;
                StartCoroutine(SetAndStartGamePlay(1f));
            }
            else
            {

            }
        }

        public void PlayBtnFromTutorial()
        {
            StartGame(CallBreakGameManager.currentLobbyAmount);
        }

        public void OnBidToolTipClose() => StartCoroutine(BidToolTipClose());

        public IEnumerator BidToolTipClose()
        {
            yield return new WaitForSeconds(1f);

            foreach (var item in gamePlayController.allPlayer)
            {
                item.bidObjectToolTip.transform.DOScale(Vector3.zero, .3f);
            }
            yield return new WaitForSeconds(1f);
            CallBreakGameManager.instance.WinnerTurn();

        }
    }
}
