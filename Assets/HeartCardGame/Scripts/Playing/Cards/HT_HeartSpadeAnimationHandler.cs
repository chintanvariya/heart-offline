using UnityEngine;
using DG.Tweening;

namespace HeartCardGame
{
    public class HT_HeartSpadeAnimationHandler : MonoBehaviour
    {
        [Header("Heart Spade Object =====")]
        public RectTransform heartSpadeRectTransform;
        public CardType cardType;

        private void OnEnable()
        {
            heartSpadeRectTransform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.5f).SetLoops(int.MaxValue, LoopType.Yoyo);
        }
    }
}