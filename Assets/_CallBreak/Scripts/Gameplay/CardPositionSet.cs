using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

namespace FGSOfflineCallBreak
{
    public class CardPositionSet : MonoBehaviour
    {
        public static CardPositionSet instance;
        public List<GameObject> cards = new List<GameObject>();
        // public List<Transform> cardsHolders = new List<Transform>();
        readonly List<GameObject> forceSetPosition = new List<GameObject>();
        public List<GameObject> Cards => new List<GameObject>(cards);
        [Space(5)]
        public float height = 0.5f;
        public float width = 1f;
        [Range(0f, 90f)] public float maxCardAngle = 5f;
        public float yPerCard = -0.005f;
        public float zDistance;
        public float moveDuration = 0.5f;
        public Transform cardHolderPrefab;
        //   public event Action<int> OnCountChanged;
        //  private bool updatePositions;

        private void Awake()
        {
            instance = this;
        }
        public void UpdateCardPosition()
        {
            float radius = Mathf.Abs(height) < 0.001f ? width * width / 0.001f * Mathf.Sign(height) : height / 2f + width * width / (8f * height);

            float angle = 2f * Mathf.Asin(0.5f * width / radius) * Mathf.Rad2Deg;
            angle = Mathf.Sign(angle) * Mathf.Min(Mathf.Abs(angle), maxCardAngle * (cards.Count - 1));
            float cardAngle = cards.Count == 1 ? 0f : angle / (cards.Count - 1f);

            for (int i = 0; i < cards.Count; i++)
            {
                //  cards[i].transform.SetParent(cardHolderPrefab, true);

                Vector3 position = new Vector3(0f, radius, 0f);
                position = Quaternion.Euler(0f, 0f, angle / 2f - cardAngle * i) * position;
                position.y += height - radius;
                position += i * new Vector3(0f, yPerCard, zDistance);

                /*  cardsHolders[i].transform.localPosition = position;
                  cardsHolders[i].transform.localEulerAngles = new Vector3(0f, 0f, angle / 2f - cardAngle * i);

                  cards[i].transform.SetParent(cardsHolders[i].transform, true);*/

                if (!forceSetPosition.Contains(cards[i]))
                {
                    cards[i].transform.DOKill();
                    cards[i].transform.DOLocalMove(position, moveDuration);
                    cards[i].transform.DOLocalRotate(new Vector3(0f, 0f, angle / 2f - cardAngle * i), moveDuration);
                    // cards[i].transform.DOScale(Vector3.one, moveDuration);
                }
                else
                {
                    forceSetPosition.Remove(cards[i]);

                    cards[i].transform.localPosition = position;
                    cards[i].transform.localEulerAngles = new Vector3(0f, 0f, angle / 2f - cardAngle * i);
                    // cards[i].transform.localScale = Vector3.one;
                }
            }

        }
        public void Add(GameObject card, bool moveAnimation = true) => Add(card, -1, moveAnimation);

        public void Add(GameObject card, int index, bool moveAnimation = true)
        {
            //Transform cardHolder = GetCardHolder();

            if (index == -1)
            {
                cards.Add(card);
                //  cardsHolders.Add(cardHolder);
            }
            else
            {
                cards.Insert(index, card);
                // cardsHolders.Insert(index, cardHolder);
            }

            //updatePositions = true;

            if (!moveAnimation)
                forceSetPosition.Add(card);

            // OnCountChanged?.Invoke(cards.Count);
        }

        Transform GetCardHolder()
        {
            Transform cardHolder = Instantiate(cardHolderPrefab, transform, false);
            return cardHolder;
        }
        [Range(0, 800)] public int widthAdjustment;
        [Range(0, 60)] public int heightAdjustment;

        public void RemoveCard(GameObject cardIndex)
        {
            if (Cards.Count == 0)
                return;

            //  GameObject card = Cards[cardIndex];
            Remove(cardIndex);
            if (CallBreakCardAnimation.instance.gamePlayController.allPlayer[0].myCards.Count < 13 && CallBreakCardAnimation.instance.gamePlayController.allPlayer[0].myCards.Count > 10)
            {
                width = 720;
                height = 60;
                maxCardAngle = 60;
            }
            else if (CallBreakCardAnimation.instance.gamePlayController.allPlayer[0].myCards.Count < 10 && CallBreakCardAnimation.instance.gamePlayController.allPlayer[0].myCards.Count > 6)
            {
                width = 400;
                height = 50;
                maxCardAngle = 50;
            }
            else if (CallBreakCardAnimation.instance.gamePlayController.allPlayer[0].myCards.Count < 6 && CallBreakCardAnimation.instance.gamePlayController.allPlayer[0].myCards.Count > 3)
            {
                width = 250;
                height = 20;
                maxCardAngle = 40;
            }
            else if (CallBreakCardAnimation.instance.gamePlayController.allPlayer[0].myCards.Count == 3)
            {
                width = 180;
                height = 20;
                maxCardAngle = 30;
            }
            else if (CallBreakCardAnimation.instance.gamePlayController.allPlayer[0].myCards.Count <= 2)
            {
                width = 100;
                height = 5;
                maxCardAngle = 20;
            }
            UpdateCardPosition();
        }
        public void Remove(GameObject card)
        {
            if (!cards.Contains(card))
                return;

            /*   Transform cardHolder = cardsHolders[cards.IndexOf(card)];
               cardsHolders.Remove(cardHolder);
               Destroy(cardHolder.gameObject);
    */
            cards.Remove(card);
            card.transform.DOKill();
            //   card.transform.SetParent(null);
            //updatePositions = true;

            // OnCountChanged?.Invoke(cards.Count);

        }
        void OnValidate()
        {
            // updatePositions = true;
        }

        public void ResetCardData()
        {
            /* foreach (var item in cardsHolders)
             {
                 item.transform.GetChild(0).transform.SetParent(null);
                 Destroy(item.gameObject);
             }*/
            cards.Clear(); Cards.Clear();
        }
    }
}
