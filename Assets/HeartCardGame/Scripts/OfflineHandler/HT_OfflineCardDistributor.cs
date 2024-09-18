using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace FGSOfflineHeart
{
    public class HT_OfflineCardDistributor : MonoBehaviour
    {
        [Header("===== Object Scripts =====")]
        [SerializeField] private HT_CardDeckController cardDeckController;
        [SerializeField] private HT_OfflineGameHandler offlineGameHandler;
        [SerializeField] private HT_GameManager gameManager;

        [Header("===== Cards Name List =====")]
        public List<string> cardsOriginList;
        public List<string> cardsName;

        private void Start() => gameManager.RoundReset += ResetOfflineCardDistributor;

        public void CardDistribution()
        {
            Debug.Log($"HT_OfflineCardDistributor || CardDistribution ");
            cardsName = new List<string>(cardsOriginList);
            List<string> myPlayerCard = new();

            for (int i = 0; i < 13; i++)
            {
                int randNum = Random.Range(0, cardsName.Count);
                var card = cardsName[randNum];
                myPlayerCard.Add(card);
                cardsName.Remove(card);
            }

            var sortedCards = CardSequencing(myPlayerCard);

            cardDeckController.CardShow(sortedCards);
            Invoke(nameof(Temp), 7.2f);
        }

        void Temp() => offlineGameHandler.CardPassHandle();

        public List<string> CardSequencing(List<string> cards)
        {
            List<string> suitOrder = new() { "C", "D", "S", "H" };
            return cards
                .OrderBy(card => suitOrder.IndexOf(card.Substring(0, 1)))
                .ThenBy(card => (int.Parse(card.Substring(2))) == 1 ? 14 : int.Parse(card.Substring(2)))
                .ToList();
        }

        void ResetOfflineCardDistributor() => CancelInvoke(nameof(Temp));
    }
}