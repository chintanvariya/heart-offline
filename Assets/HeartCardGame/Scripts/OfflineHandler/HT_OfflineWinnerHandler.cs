using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace FGSOfflineHeart
{
    public class HT_OfflineWinnerHandler : MonoBehaviour
    {
        [Header("===== Object Script =====")]
        [SerializeField] private HT_WinnerDeclareHandler winnerDeclareHandler;
        [SerializeField] private HT_OfflineGameHandler offlineGameHandler;
        [SerializeField] private HT_JoinTableHandler joinTableHandler;
        [SerializeField] private HT_GameManager gameManager;
        [SerializeField] private HT_CardPassManager cardPassManager;
        [SerializeField] private HT_CardDeckController cardDeckController;

        [Header("===== Winner Data ====")]
        [SerializeField] private List<HT_WinnerHandler> winnerHandlers;

        public void OfflineWinnerDeclare()
        {
            try
            {


                DestroyWinnerHandler();
                cardPassManager.cardPassBtn.interactable = false;
                joinTableHandler.playerData.ForEach(player =>
                {
                    player.totalHeartPoint += player.roundHeartPoint;
                    player.totalSpadePoint += player.roundSpadePoint;
                    player.totalPoint += player.roundHeartPoint + player.roundSpadePoint;
                    Debug.Log($"<color=cyan>HT_OfflineWinnerHandler || OfflineWinnerDeclare || Total Of Player {player.totalPoint}</color>");
                });
                bool isFinal = joinTableHandler.playerData.Any(x => x.totalPoint >= 100);
                offlineGameHandler.roundNum++;
                winnerDeclareHandler.WinnerPreSetting(offlineGameHandler.roundNum);
                List<HT_PlayerController> players = new(joinTableHandler.playerData.OrderBy(x => x.totalPoint).ToList());
                Debug.Log($"COUNT OF PLAYER {players.Count} {joinTableHandler.playerData.Count}");
                int minScore = players.Select(x => x.totalPoint).Min();
                for (int i = 0; i < players.Count; i++)
                {
                    var player = players[i];
                    Debug.Log($"Player {player.mySeatIndex}");
                    HT_WinnerHandler winnerClone = Instantiate(winnerDeclareHandler.winnerHandler, winnerDeclareHandler.winnerDataGenerator);
                    bool isWinner = false;
                    if (isFinal) isWinner = player.totalPoint == minScore;
                    winnerClone.WinnerDataSetting(player.roundSpadePoint, player.roundHeartPoint, player.totalPoint, "", player.userName, isWinner, false, player.mySprite.sprite);
                    winnerHandlers.Add(winnerClone);
                }
                gameManager.RoundReset?.Invoke();
                bool isWinSound = cardDeckController.myPlayer.totalPoint == minScore;
                winnerDeclareHandler.FinalWinnerInfoSet(isFinal, 10, isWinSound);
            }
            catch (System.Exception ex)
            {
                Debug.Log("=============> " + ex.ToString());
                throw;
            }
        }

        void DestroyWinnerHandler()
        {
            foreach (var winner in winnerHandlers)
            {
                if (winner != null)
                    Destroy(winner.gameObject);
            }
            winnerHandlers.Clear();
        }
    }
}