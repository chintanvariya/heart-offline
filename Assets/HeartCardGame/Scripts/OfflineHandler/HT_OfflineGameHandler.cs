using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

namespace HeartCardGame
{
    public class HT_OfflineGameHandler : MonoBehaviour
    {
        public static HT_OfflineGameHandler instance;

        [Header("===== Object Scripts =====")]
        [SerializeField] private HT_JoinTableHandler joinTableHandler;
        [SerializeField] private HT_GameStartTimerManager gameStartTimerManager;
        [SerializeField] private HT_UiManager uiManager;
        [SerializeField] private HT_GameManager gameManager;
        [SerializeField] private HT_CardMoveHandler cardMoveHandler;
        [SerializeField] private HT_OfflineCardDistributor offlineCardDistributor;
        [SerializeField] private HT_CardPassHandler cardPassHandler;
        [SerializeField] private HT_AudioManager audioManager;
        public HT_OfflinePlayerTurnController offlinePlayerTurnController;
        public HT_OfflineWinOfRoundHandler offlineWinOfRoundHandler;
        public HT_CardDeckController cardDeckController;
        public HT_CardPassManager cardPassManager;

        [Header("===== Game Info =====")]
        public int roundNum;
        public string directionString;

        [Header("===== Card List =====")]
        public List<string> cardNameList;

        [Header("===== Sprite List =====")]
        public List<Sprite> spritesList;

        Dictionary<int, int> mapping = new Dictionary<int, int>
        {
            { 0, 2 },
            { 2, 0 },
            { 1, 3 },
            { 3, 1 }
        };

        private void Awake()
        {
            if (instance != null)
                Destroy(instance);
            instance = this;
        }

        private void Start()
        {
            PlayerSetup();
            gameManager.RoundReset += ResetOfflineGameHandler;
        }

        public void PlayerSetup()
        {
            roundNum = 1;
            gameManager.isOffline = true;
            gameManager.tableState = TableState.OFFLINE;
            uiManager.dashboardPanel.SetActive(false);
            uiManager.scoreboardBtn.interactable = false;
            uiManager.gamePanel.SetActive(true);
            audioManager.BackgroundMusicOnOff();
            joinTableHandler.playerData = new(joinTableHandler.playerDataOrigin);
            for (int i = 0; i < joinTableHandler.playerData.Count; i++)
            {
                var player = joinTableHandler.playerData[i];
                if (!player.isMyPlayer)
                {
                    string playerName = $"Player {i}";
                    int randomSprite = Random.Range(0, spritesList.Count);
                    player.OfflinePlayerDataset(playerName, i, spritesList[randomSprite]);
                }
            }
            if (gameManager.myUserSprite != null)
                cardDeckController.myPlayer.mySprite.sprite = gameManager.myUserSprite;
            else
            {
                int randomSprite = Random.Range(0, spritesList.Count);
                cardDeckController.myPlayer.mySprite.sprite = spritesList[randomSprite];
            }
            gameStartTimerManager.GameStartTimer(5, true);
        }

        public void CardPassHandle()
        {
            if (roundNum % 4 != 0)
            {
                gameManager.isCardPassed = false;
                uiManager.cardPassPanel.SetActive(true);
                cardPassManager.cardControllerList.Clear();
                cardPassManager.PassCardDataSetting(RoundDirection(), roundNum, 10);
                Invoke(nameof(CardPass), 10f);
            }
            else
            {
                gameManager.isCardPassed = true;
                uiManager.CommonTooltipSet("Round 4 has no passing.", true, true);
                DOVirtual.DelayedCall(2.5f, () => GameStart());
            }
        }

        void CardPass()
        {
            List<int> seats = new();
            HT_PlayerController player = joinTableHandler.GetMyPlayer();
            List<string> cardMoveList = GetRandomList(player);

            ListUpdateOfPlayerForCardMove(player, cardMoveList);

            HT_PlayerController otherPlayer = GetToAndFromPlayer(player);
            seats.Add(otherPlayer.mySeatIndex);

            for (int i = 0; i < cardDeckController.opponentCardTransformList.Count; i++)
            {
                HT_PlayerController oppositePlayer = cardDeckController.opponentCardTransformList[i];
                List<string> cardMovelist = new(GetRandomList(oppositePlayer));
                ListUpdateOfPlayerForCardMove(oppositePlayer, cardMovelist);
                HT_PlayerController oppPlayer = GetToAndFromPlayer(oppositePlayer);
                seats.Add(oppPlayer.mySeatIndex);
            }

            cardPassManager.cardControllerList.AddRange(player.passCardList.Where(card => cardMoveList.Contains(card.myName)));
            player.passCardList.Clear();
            cardPassHandler.CardPassAnimation(cardMoveList, true);

            for (int i = 0; i < seats.Count; i++)
                joinTableHandler.playerData[seats[i]].cardControllers.AddRange(joinTableHandler.playerData[i].passCardList);

            cardDeckController.opponentCardTransformList.ForEach(x => x.cardControllers
                .ForEach(x => x.cardSprite.raycastTarget = false));

            DOVirtual.DelayedCall(1f, () =>
            {
                List<string> newCard = offlineCardDistributor.CardSequencing(player.cardControllers.Select(x => x.myName).ToList());
                cardMoveHandler.CardSettingForMove(newCard);

                Invoke(nameof(GameStart), 1f);
                joinTableHandler.playerData.Where(x => x.passCardList.Count > 0)
                    .ToList()
                    .ForEach(x => x.passCardList.Clear());
            });
        }

        public bool CheckScoreBoard()
        {
            int totalPoints = joinTableHandler.playerData.Sum(player => player.roundHeartPoint + player.roundSpadePoint);
            return totalPoints == 26;
        }

        public bool IsShootingMoon()
        {
            return joinTableHandler.playerData.Any(player => player.roundHeartPoint + player.roundSpadePoint == 26);
        }

        void GameStart() => offlinePlayerTurnController.OfflineTurnHandler();

        List<string> GetRandomList(HT_PlayerController player)
        {
            List<string> cardMoveList = new();
            int remainingCard = 3;
            if (player.isMyPlayer)
            {
                remainingCard = 3 - player.passCardList.Count;
                cardMoveList = player.passCardList.Select(x => x.myName).ToList();
            }

            for (int i = 0; i < remainingCard; i++)
            {
                Recheck:
                int temp = Random.Range(0, player.cardControllers.Count);
                HT_CardController cardController = player.cardControllers[temp];
                if (cardMoveList.Contains(cardController.myName))
                    goto Recheck;
                cardMoveList.Add(cardController.myName);
            }

            return cardMoveList;
        }

        void ListUpdateOfPlayerForCardMove(HT_PlayerController player, List<string> cardMoveList)
        {
            player.passCardList = player.cardControllers
                .Where(controller => cardMoveList.Contains(controller.myName))
                .ToList();
            player.cardControllers.RemoveAll(controller => cardMoveList.Any(passCard => passCard == controller.myName));
        }

        HT_PlayerController GetToAndFromPlayer(HT_PlayerController toPlayer)
        {
            int round = roundNum % 4;
            int playerIndex = toPlayer.mySeatIndex;
            int index = toPlayer.mySeatIndex;

            switch (round)
            {
                case 1:
                    playerIndex = playerIndex == 3 ? 0 : index + 1;
                    break;

                case 2:
                    playerIndex = playerIndex == 0 ? 3 : index - 1;
                    break;

                case 3:
                    playerIndex = mapping[index];
                    break;

                case 4:
                    uiManager.CommonTooltipSet("Round 4 has no passing.", true, true);
                    break;

                default:
                    break;
            }

            return joinTableHandler.playerData[playerIndex];
        }

        string RoundDirection()
        {
            int round = roundNum % 4;

            if (round == 1) return "Left";
            else if (round == 2) return "Right";
            else if (round == 3) return "Across";
            else return "";
        }

        void ResetOfflineGameHandler()
        {
            CancelInvoke(nameof(CardPass));
            CancelInvoke(nameof(GameStart));
        }
    }
}
