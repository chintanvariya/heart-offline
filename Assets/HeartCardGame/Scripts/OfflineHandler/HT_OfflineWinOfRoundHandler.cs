using UnityEngine;
using System.Linq;
using DG.Tweening;

namespace HeartCardGame
{
    public class HT_OfflineWinOfRoundHandler : MonoBehaviour
    {
        [Header("===== Object Scripts =====")]
        public HT_JoinTableHandler joinTableHandler;
        [SerializeField] private HT_WinOfRound winOfRound;
        [SerializeField] private HT_OfflinePlayerTurnController offlinePlayerTurnController;
        [SerializeField] private HT_GameManager gameManager;

        [Header("===== Card Data =====")]
        public System.Collections.Generic.List<HT_CardController> handedCards;

        private void Start() => gameManager.RoundReset += DestroyHandedCards;

        public void WinOfRound()
        {
            HT_CardController maxCard = null;

            maxCard = handedCards
                .Where(card => card.cardType.ToString() == offlinePlayerTurnController.turnCardSequence)
                .OrderBy(card => card.cardValue)
                .Last();

            offlinePlayerTurnController.turnCardSequence = "N";

            Debug.Log($"HT_OfflineWinOfRoundHandler || WinOfRound || MAXCARD {maxCard}");

            DOVirtual.DelayedCall(0.5f, () =>
            {
                HT_PlayerController player = joinTableHandler.playerData.Find(x => x.handCard.myName == maxCard.myName);
                winOfRound.WinOfRoundSetting(player);
                offlinePlayerTurnController.turnPlayer = player.mySeatIndex;
                offlinePlayerTurnController.turnPlayer = offlinePlayerTurnController.turnPlayer == 0 ? 3 : player.mySeatIndex - 1;
                offlinePlayerTurnController.TurnAfterWinOfRound();
            });
        }

        public void DestroyHandedCards()
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

        public void DestroyHandedCardInvoke()
        {
            CancelInvoke(nameof(DestroyHandedCards));
            Invoke(nameof(DestroyHandedCards), 0.5f);
        }
    }
}