using UnityEngine;
using System.Linq;
using DG.Tweening;

namespace HeartCardGame
{
    public class HT_OfflinePlayerTurnController : MonoBehaviour
    {
        [Header("===== Object Scripts =====")]
        [SerializeField] private HT_TurnInfoManager turnInfoManager;
        [SerializeField] private HT_JoinTableHandler joinTableHandler;
        [SerializeField] private HT_WinOfRound winOfRound;
        [SerializeField] private HT_OfflineWinnerHandler offlineWinnerHandler;
        [SerializeField] private HT_ShootingAnimation shootingAnimation;
        [SerializeField] private HT_GameManager gameManager;
        [SerializeField] private HT_UiManager uiManager;
        public HT_ThrowCardHandler throwCardHandler;

        [Header("===== Turn Data =====")]
        public int turnPlayer;
        public string turnCardSequence;
        public bool isBreakingHearts;
        public bool clubTwoPresent;

        public int PlayerTurnHandler()
        {
            bool isFirst = joinTableHandler.playerData.Any(player => player.cardControllers.Any(controller => controller.myName == "C-2"));
            clubTwoPresent = isFirst;

            HT_PlayerController isFirstPlayer = joinTableHandler.playerData.Find(player => player.cardControllers.Any(controller => controller.myName == "C-2"));

            if (isFirst) return turnPlayer = isFirstPlayer.mySeatIndex;

            return turnPlayer = turnPlayer == 3 ? 0 : turnPlayer + 1;
        }

        public void OfflineTurnHandler()
        {
            int playerTurn = PlayerTurnHandler();
            if (HT_OfflineGameHandler.instance.IsShootingMoon())
            {
                Debug.Log($"<color=yellow>Shooting Moon</color>");
                shootingAnimation.ShootingMoonAnimation();
                DOVirtual.DelayedCall(10f, () => offlineWinnerHandler.OfflineWinnerDeclare());
            }
            else if (HT_OfflineGameHandler.instance.CheckScoreBoard())
            {
                Debug.Log($"<color=green>Scoreboard Declare</color>");
                offlineWinnerHandler.OfflineWinnerDeclare();
                return;
            }
            else
                turnInfoManager.UserTurnHandle(playerTurn, 10, turnCardSequence, isBreakingHearts);
            HT_PlayerController player = joinTableHandler.playerData[playerTurn];

            if (!HT_OfflineGameHandler.instance.IsShootingMoon() && !HT_OfflineGameHandler.instance.CheckScoreBoard())
            {
                if (!player.isMyPlayer)
                {
                    CardMoving(player, 2f);
                    player.AllCardTransparentImageOnOff(false);
                }
                else
                    gameManager.CardTargetRaycastOnOff(true);
            }
        }

        public void CardMoving(HT_PlayerController player, float waitSecond)
        {
            HT_CardController card = null;

            card = Card(player);
            if (card.cardType == CardType.H) isBreakingHearts = true;
            DOVirtual.DelayedCall(waitSecond, () =>
            {
                throwCardHandler.UserCardThrow(player, card, isBreakingHearts);
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    if (uiManager.heartBrokenPanel.activeInHierarchy)
                    {
                        DOVirtual.DelayedCall(0.7f, () =>
                        {
                            if (!player.isMyPlayer)
                                if (turnCardSequence != "N") OfflineTurnHandler();
                        });
                    }
                    else
                    {
                        if (!player.isMyPlayer)
                            if (turnCardSequence != "N") OfflineTurnHandler();
                    }
                });
            });
        }

        HT_CardController Card(HT_PlayerController player)
        {
            HT_CardController card = null;

            int randCard = 0;
            if (clubTwoPresent)
                card = player.cardControllers.Find(x => x.myName == "C-2");
            else
            {
                System.Collections.Generic.List<HT_CardController> cards = new(turnInfoManager.GetThrowableCards(player, turnCardSequence, isBreakingHearts));
                Debug.Log($"HT_OfflinePlayerTurnController || Card || Selected Cards Count {cards.Count}");
                randCard = Random.Range(0, cards.Count);
                card = cards[randCard];
            }

            return card;
        }

        public void TurnAfterWinOfRound() => DOVirtual.DelayedCall(1f, () => OfflineTurnHandler());
    }
}