using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using static FGSOfflineCallBreak.CallBreakRemoteConfigClass;

namespace FGSOfflineCallBreak
{
    [CreateAssetMenu(fileName = "ManagerData", menuName = "ManagerData/BotsDetails", order = 1)]
    [Serializable]
    public class ManageData : ScriptableObject
    {
        public List<BotDetails> allBotDetails;
    }

    [Serializable]
    public class UserDetails
    {
        public string userName;
        public string userId;
        public float userChips;
        public float userKeys;
        public int userAvatarIndex;
        public int level = 1;
        public float levelProgress;
        public int removeAds;
        public UserGameDetails userGameDetails;
    }

    [Serializable]
    public class UserGameDetails
    {
        public int GamePlayed;
        public int GameWon;
        public int GameLoss;
    }

    [Serializable]
    public class BotDetails
    {
        public string userName;
        public string userId;
        public long userChips;
        public long userKeys;
        public int userAvatarIndex;
    }

    public sealed class CallBreakGameManager : MonoBehaviour
    {
        public bool isLogOff;

        [SerializeField]
        public UserDetails selfUserDetails;
        public static Sprite profilePicture;

        public static bool isInGamePlay;

        public static Action UpdateUserDetails;

        public GenerateTheBots generateTheBots;
        //public ManageData manageData;
        //public SpriteData spriteData;

        public float lobbyAmount;

        public ParticleSystem particlesOfHukum;

        public List<Sprite> allBotSprite = new List<Sprite>();

        private void Awake()
        {
            if (instance == null) instance = this;

            StopAllCoroutines();

            Application.targetFrameRate = 70;
            Input.multiTouchEnabled = false;
            Time.timeScale = 1f;

            float sizeOfInt = sizeof(float);

            // Print the size
            Debug.Log("Size of integer: " + sizeOfInt + " bytes");
            Debug.unityLogger.logEnabled = isLogOff;
        }

        internal CallBreakCardController currentCard;
        internal List<CallBreakCardController> allTableCards = new List<CallBreakCardController>();


        public static Action ScoreBoardDataEvent;

        public static CallBreakGameManager instance;

        public static int currentPlayerIndex;
        [Header("CURRENT ROUND")]
        public int currentRound = 0, totalRound;
        public static int currentLobbyAmount;

        private const float WinCardScale = 1.5f;
        private float delayTime = 0.1f;

        [Space(5)]

        public List<Sprite> allProfileSprite = new List<Sprite>();
        public List<GameObject> particles = new List<GameObject>();

        public int clubCardCounter;

        public CallBreakGamePlayController gamePlayController;

        internal IEnumerator NextUserTurn()
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % 4;

            Debug.Log(" Next PlayerTurn ====> " + currentPlayerIndex);

            if (allTableCards.Count == 4)
            {
                gamePlayController.allPlayer[0].ActiveAllSelfPlayerCard(false);

                List<CallBreakCardController> spadeCards = new();
                List<CallBreakCardController> otherTypeCards = new();

                for (int i = 0; i < allTableCards.Count; i++)
                {
                    if (allTableCards[i].cardDetail.cardType == CardType.Spade)
                    {
                        spadeCards.Add(allTableCards[i]);
                        Debug.Log(" Spade Add " + allTableCards[i].name);
                    }
                }

                allTableCards.Sort((a, b) => b.cardDetail.value.CompareTo(a.cardDetail.value));
                spadeCards.Sort((a, b) => b.cardDetail.value.CompareTo(a.cardDetail.value));

                for (int i = 0; i < allTableCards.Count; i++)
                {
                    Debug.LogWarning(allTableCards[i].cardDetail.cardType + " ===== " + currentCard.cardDetail.cardType);

                    if (allTableCards[i].cardDetail.cardType != currentCard.cardDetail.cardType)
                    {
                        otherTypeCards.Add(allTableCards[i]);
                        Debug.Log(allTableCards[i].name);
                    }
                }

                for (int i = 0; i < otherTypeCards.Count; i++)
                {
                    if (allTableCards.Contains(otherTypeCards[i])) allTableCards.Remove(otherTypeCards[i]);
                }

                CallBreakCardController winCard = (spadeCards.Count > 0) ? spadeCards[0] : allTableCards[0];
                yield return new WaitForSeconds(0.2f);

                Debug.Log(" WINCARD == " + winCard.name);
                Tweener scaleAnimation = winCard.transform.DOScale(new Vector3(WinCardScale, WinCardScale, WinCardScale), 0.2f).SetEase(Ease.Linear).SetAutoKill(false);
                Transform playerData = winCard.cardThrowParent;

                //for (int i = 0; i < allTableCards.Count; i++)
                //    allTableCards[i].cardThrowParent = playerData;

                yield return new WaitForSeconds(0.3f);

                scaleAnimation.PlayBackwards();

                yield return new WaitForSeconds(0.5f);

                for (int i = 0; i < otherTypeCards.Count; i++)
                {
                    allTableCards.Add(otherTypeCards[i]);
                }

                foreach (var item in particles)
                {
                    item.SetActive(true);
                    item.transform.localPosition = Vector3.zero;
                }

                foreach (var item in allTableCards)
                {
                    item.transform.DOScale(Vector3.zero, .3f).SetEase(Ease.Linear);
                    item.transform.DOMove(gamePlayController.allPlayer[winCard.playerIndex].myCardPos.position, .3f).SetEase(Ease.Linear);
                }

                foreach (var item in particles)
                {
                    item.transform.DOMove(gamePlayController.allPlayer[winCard.playerIndex].myCardPos.position, .3f).SetEase(Ease.Linear);
                }


                yield return new WaitForSeconds(0.5f);

                foreach (var item in particles)
                {
                    item.SetActive(false);
                }

                gamePlayController.allPlayer[winCard.playerIndex].currentBidScore++;
                gamePlayController.allPlayer[winCard.playerIndex].SetMyBid();
                TurnDataReset();
                currentPlayerIndex = gamePlayController.allPlayer.IndexOf(gamePlayController.allPlayer[winCard.playerIndex]);

                WinnerTurn();

                if (gamePlayController.allPlayer.TrueForAll(i => i.myCards.Count == 0))
                {
                    ScoreBoardDataEvent();
                    Invoke(nameof(RestartGame), 1.5f);
                }
            }
            else
            {
                //float delayTime = UnityEngine.Random.Range(0.1f, 0.4f);
                if (currentPlayerIndex == 0) delayTime = 0;
                else delayTime = 0.3f;
                yield return new WaitForSeconds(delayTime);

                if (currentPlayerIndex == 0)
                {
                    gamePlayController.allPlayer[0].HighLightCard();
                    ThrowSelfPlayerCardAuto();
                }
                else
                {
                    gamePlayController.allPlayer[currentPlayerIndex].turnTimer.SelfUserTimer();//ThrowCard()
                }
            }
        }



        internal void WinnerTurn()
        {
            if (currentCard == null)
            {
                int highCard = gamePlayController.allPlayer[currentPlayerIndex].myCards.Count;

                if (highCard != 0) currentCard = gamePlayController.allPlayer[currentPlayerIndex].myCards[UnityEngine.Random.Range(0, highCard)].GetComponent<CallBreakCardController>();

                Debug.Log("====>" + currentCard.name);
            }
            if (currentPlayerIndex != 0)
            {
                Debug.LogError(" WINNER TURN ");
                gamePlayController.allPlayer[currentPlayerIndex].turnTimer.SelfUserTimer();// ThrowCard();
            }
            else
            {
                Debug.LogError(" YOUR TURN ");
                ThrowSelfPlayerCardAuto();
                gamePlayController.allPlayer[0].ActiveAllSelfPlayerCard(false);
            }
        }

        public void RestartGame()
        {
            for (int i = 0; i < gamePlayController.allPlayer.Count; i++)
                gamePlayController.allPlayer[i].finalScore = gamePlayController.allPlayer[i].roundScore.Sum();

            arrageUserInOrderToRoundHighScore = new List<CallBreakUserController>(gamePlayController.allPlayer);
            arrageUserInOrderToFinalHighScore = new List<CallBreakUserController>(gamePlayController.allPlayer);

            arrageUserInOrderToRoundHighScore = arrageUserInOrderToRoundHighScore.OrderByDescending(player => player.roundScore[currentRound - 1]).ToList();
            arrageUserInOrderToFinalHighScore = arrageUserInOrderToFinalHighScore.OrderByDescending(player => player.finalScore).ToList();

            if (currentRound == totalRound)
            {
                int selfUserRank = arrageUserInOrderToFinalHighScore.Select((player, index) => new { Player = player, Index = index }).FirstOrDefault(item => item.Player.isSelfPlayer)?.Index + 1 ?? 0;

                CallBreakUIManager.Instance.winnerLoserController.OpenWinnerAndLosserScreen(arrageUserInOrderToFinalHighScore[0].isSelfPlayer, selfUserRank);

                currentRound = 1;
            }
            else
            {
                clubCardCounter = 0;
                for (int i = 0; i < gamePlayController.allPlayer.Count; i++)
                    gamePlayController.allPlayer[i].ResetPlayerData();

                CallBreakUIManager.Instance.scoreBoardController.OpenScreen(arrageUserInOrderToRoundHighScore[0].staticSeatIndex);
            }
        }

        public List<CallBreakUserController> arrageUserInOrderToRoundHighScore;
        public List<CallBreakUserController> arrageUserInOrderToFinalHighScore;



        public void StartNewRoundAfterScoreboard()
        {
            CallBreakUIManager.Instance.gamePlayController.UpdateTheRoundText();
            StartCoroutine(CallBreakCardAnimation.instance.SetAndStartGamePlay(1f));
        }


        public void ThrowSelfPlayerCardAuto()
        {
            if (gamePlayController.allPlayer[0].myCards.Count == 0) return;

            if (gamePlayController.allPlayer[0].myCards.Count == 1)
            {
                gamePlayController.allPlayer[0].ActiveAllSelfPlayerCard(false);
                gamePlayController.allPlayer[currentPlayerIndex].turnTimer.SelfUserTimer();// ThrowCard();
            }
            else
            {
                gamePlayController.allPlayer[currentPlayerIndex].turnTimer.SelfUserTimer();
            }
        }

        public void TurnDataReset()
        {
            allTableCards.ForEach(c => c.gameObject.SetActive(false));
            allTableCards.ForEach(c => Destroy(c.gameObject));
            gamePlayController.allPlayer.ForEach(c => c.isMyTurnComplete = false);
            allTableCards.Clear();
        }

        public static Action<CallBreakRemoteConfig> updateTheConfigData;

        public CallBreakRemoteConfig UpdatedConfigData()
        {

            return new CallBreakRemoteConfig();
        }


    }
}
