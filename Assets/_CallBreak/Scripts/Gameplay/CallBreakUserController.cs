using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;
namespace FGSOfflineCallBreak
{
    public class CallBreakUserController : MonoBehaviour
    {
        public BotDetails botDetails;

        public int staticSeatIndex;

        [Space(10)]
        public Transform myCardPos, myThrowCardPos;

        public Image profilePicture;

        public TextMeshProUGUI userNameText;
        public TextMeshProUGUI bidText;
        public TextMeshProUGUI userChipsText;

        //public GameObject crownObj;
        public GameObject bidObjectToolTip;
        public GameObject dealerIcon;

        [Space(10)]
        public List<CallBreakCardController> myCards = new List<CallBreakCardController>();

        public List<CallBreakCardController> spadeCards = new List<CallBreakCardController>();
        public List<CallBreakCardController> heartCards = new List<CallBreakCardController>();
        public List<CallBreakCardController> clubCards = new List<CallBreakCardController>();
        public List<CallBreakCardController> diamondCards = new List<CallBreakCardController>();

        public List<float> roundScore = new List<float>();

        internal bool isMyTurnComplete;
        internal int totalBid, currentBidScore;
        internal float finalScore;
        public int totalScore;
        public bool isSelfPlayer;

        public CallBreakUserTurnTimer turnTimer;

        public Transform emojiTransform;
        private void OnEnable()
        {
            //CallBreakGameManager.ResetPlayerDataEvent += ResetPlayerData;
            CallBreakGameManager.ScoreBoardDataEvent += ScoreBoardUpdate;
        }

        private void OnDisable()
        {
            //CallBreakGameManager.ResetPlayerDataEvent -= ResetPlayerData;
            CallBreakGameManager.ScoreBoardDataEvent -= ScoreBoardUpdate;
        }

        internal void SetMyBid() => bidText.text = currentBidScore + "/" + totalBid.ToString();

        internal void ProfileAndNameDataSet()
        {
            if (isSelfPlayer)
            {
                Debug.Log($"CallBreakGameManager.instance.selfUserDetails.userName {CallBreakGameManager.instance.selfUserDetails.userName}");
                userNameText.text = CallBreakGameManager.instance.selfUserDetails.userName;
                profilePicture.sprite = CallBreakGameManager.profilePicture;
                userChipsText.text = CallBreakUtilities.AbbreviateNumber(CallBreakGameManager.instance.selfUserDetails.userChips);
            }
            else
            {
                profilePicture.sprite = CallBreakGameManager.instance.generateTheBots.allBotSprite[botDetails.userAvatarIndex];
                userNameText.text = botDetails.userName;
            }
            SetMyBid();
        }

        public void UserTurnStarted()
        {
            turnTimer.TimerObjectDeActivate();
            if (CallBreakGameManager.instance.currentCard.cardDetail.cardType == CardType.Heart && heartCards.Count != 0)
            {
                heartCards[0].ThrowCardAnimation(myThrowCardPos, staticSeatIndex);
            }
            else if (CallBreakGameManager.instance.currentCard.cardDetail.cardType == CardType.Club && clubCards.Count != 0)
            {
                clubCards[0].ThrowCardAnimation(myThrowCardPos, staticSeatIndex);
            }
            else if (CallBreakGameManager.instance.currentCard.cardDetail.cardType == CardType.Diamond && diamondCards.Count != 0)
            {
                diamondCards[0].ThrowCardAnimation(myThrowCardPos, staticSeatIndex);
            }
            else if (spadeCards.Count != 0)
            {
                spadeCards[0].ThrowCardAnimation(myThrowCardPos, staticSeatIndex);
            }
            else
            {
                if (myCards.Count != 0)
                {
                    myCards.Sort((a, b) => b.cardDetail.value.CompareTo(a.cardDetail.value));
                    myCards[myCards.Count - 1].ThrowCardAnimation(myThrowCardPos, staticSeatIndex);
                }
            }
        }

        void ResetCardList()
        {
            spadeCards.Clear();
            heartCards.Clear();
            clubCards.Clear();
            diamondCards.Clear();
        }

        public void UpdateCardInGroup()
        {
            ResetCardList();
            for (int i = 0; i < myCards.Count; i++)
            {
                if (myCards[i].cardDetail.cardType == CardType.Spade)
                    spadeCards.Add(myCards[i]);
                else if (myCards[i].cardDetail.cardType == CardType.Heart)
                    heartCards.Add(myCards[i]);
                else if (myCards[i].cardDetail.cardType == CardType.Club)
                    clubCards.Add(myCards[i]);
                else if (myCards[i].cardDetail.cardType == CardType.Diamond)
                    diamondCards.Add(myCards[i]);
            }

            SortCardData(spadeCards);
            SortCardData(heartCards);
            SortCardData(clubCards);
            SortCardData(diamondCards);

            myCards.Clear();
            myCards.AddRange(spadeCards);
            myCards.AddRange(heartCards);
            myCards.AddRange(diamondCards);
            myCards.AddRange(clubCards);
        }

        internal void SortCardData(List<CallBreakCardController> cardControllers)
        {
            cardControllers.Sort((a, b) => b.cardDetail.value.CompareTo(a.cardDetail.value));
        }

        internal void RemoveSuitCard(CallBreakCardController cardController)
        {
            if (heartCards.Contains(cardController)) heartCards.Remove(cardController);
            else if (clubCards.Contains(cardController)) clubCards.Remove(cardController);
            else if (diamondCards.Contains(cardController)) diamondCards.Remove(cardController);
            else if (spadeCards.Contains(cardController)) spadeCards.Remove(cardController);
        }

        internal void HighLightCard()
        {
            Debug.LogError(" PlayerData || HighLightCard ");

            ActiveAllSelfPlayerCard(true);
            if (CallBreakGameManager.instance.currentCard.cardDetail.cardType == CardType.Heart && heartCards.Count != 0)
            {
                CallBreakUIManager.Instance.cardSymbolToolTip.sprite = CallBreakUIManager.Instance.heartSymbol;
                heartCards.ForEach(c => c.transform.GetChild(0).gameObject.SetActive(false));
            }
            else if (CallBreakGameManager.instance.currentCard.cardDetail.cardType == CardType.Club && clubCards.Count != 0)
            {
                CallBreakUIManager.Instance.cardSymbolToolTip.sprite = CallBreakUIManager.Instance.clubSymbol;
                clubCards.ForEach(c => c.transform.GetChild(0).gameObject.SetActive(false));
            }
            else if (CallBreakGameManager.instance.currentCard.cardDetail.cardType == CardType.Diamond && diamondCards.Count != 0)
            {
                CallBreakUIManager.Instance.cardSymbolToolTip.sprite = CallBreakUIManager.Instance.diamondSymbol;
                diamondCards.ForEach(c => c.transform.GetChild(0).gameObject.SetActive(false));
            }
            else if (spadeCards.Count != 0)
            {
                CallBreakUIManager.Instance.cardSymbolToolTip.sprite = CallBreakUIManager.Instance.spadeSymbol;
                spadeCards.ForEach(c => c.transform.GetChild(0).gameObject.SetActive(false));
            }
            else
            {
                ActiveAllSelfPlayerCard(false);
            }
        }

        internal void ScoreBoardUpdate()
        {
            if (currentBidScore == totalBid)
            {
                roundScore[CallBreakGameManager.instance.currentRound - 1] = totalBid;
            }
            else if (currentBidScore > totalBid)
            {
                float tempScore = currentBidScore - totalBid;
                roundScore[CallBreakGameManager.instance.currentRound - 1] = totalBid + (tempScore / 10);
            }
            else
            {
                roundScore[CallBreakGameManager.instance.currentRound - 1] = -totalBid;
            }
        }



        internal void ActiveAllSelfPlayerCard(bool isActive)
        {
            foreach (var item in myCards)
            {
                item.transform.GetChild(0).gameObject.SetActive(isActive);
            }
        }

        internal void BidToolTipAnimation()
        {
            Debug.LogError($"=========>{2121}");
            SetMyBid();
            bidObjectToolTip.SetActive(true);
            bidObjectToolTip.transform.localScale = Vector3.zero;
            bidObjectToolTip.transform.DOScale(Vector3.one, .3f).SetEase(Ease.OutExpo);
            bidObjectToolTip.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Bid " + totalBid;
        }

        internal void BidSelection()
        {
            totalScore = 0;
            for (int i = 0; i < myCards.Count; i++)
            {
                if (myCards[i].cardDetail.value >= 10)
                {
                    totalScore += myCards[i].cardDetail.value;
                }
            }
            if (totalScore >= 80)
            {
                totalBid = 6;
            }
            else if (totalScore >= 60)
            {
                totalBid = 5;
            }
            else if (totalScore >= 40)
            {
                totalBid = 3;
            }
            else if (totalScore >= 20)
            {
                totalBid = 2;
            }
            else
            {
                totalBid = 1;
            }
        }

        internal void ResetPlayerData()
        {
            foreach (var card in myCards)
                Destroy(card.gameObject);

            myCards.Clear();
            myCards = new List<CallBreakCardController>();
            currentBidScore = 0;
            totalBid = 0;
            SetMyBid();
            bidObjectToolTip.SetActive(false);

            spadeCards = new List<CallBreakCardController>();
            heartCards = new List<CallBreakCardController>();
            clubCards = new List<CallBreakCardController>();
            diamondCards = new List<CallBreakCardController>();
            ResetUserTimer();
        }

        public void ResetUserTimer() => turnTimer.TimerObjectDeActivate();
    }
}





