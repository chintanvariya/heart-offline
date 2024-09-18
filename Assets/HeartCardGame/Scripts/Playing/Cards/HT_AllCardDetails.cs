using System.Collections.Generic;
using UnityEngine;

namespace FGSOfflineHeart
{
    public class HT_AllCardDetails : MonoBehaviour
    {
        [Header("===== All Card Sprite =====")]
        public List<Sprite> allCards;

        public Sprite GetSpriteOfCard(string cardName)
        {
            return allCards.Find(card => card.name == cardName);
        }
    }
}