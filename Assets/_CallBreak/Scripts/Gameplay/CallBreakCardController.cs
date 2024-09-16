using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FGSOfflineCallBreak
{
    public class CallBreakCardController : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        public CardDetail cardDetail;
        public Image cardImage;

        public int playerIndex;
        //internal CallBreakUserController myPlayerData;
        public Transform cardThrowParent;
        public void OnPointerDown(PointerEventData eventData) { }
        public void OnPointerUp(PointerEventData eventData)
        {
            CallBreakUIManager.Instance.gamePlayController.allPlayer[0].ActiveAllSelfPlayerCard(false);
            CallBreakCardAnimation.instance.CardRaycast(false);
            CallBreakCardAnimation.instance.SelfPlayerTempObjActive(true);

            CallBreakUIManager.Instance.gamePlayController.allPlayer[0].myCards.Remove(this);
            transform.SetAsLastSibling();
            ThrowCardAnimation(CallBreakUIManager.Instance.gamePlayController.allPlayer[0].myThrowCardPos, 0);
        }

        internal void ThrowCardAnimation(Transform cardThrow, int indexOfPlayer)
        {
            cardThrowParent = cardThrow;
            playerIndex = indexOfPlayer;
            CallBreakSoundManager.PlaySoundEvent(SoundEffects.ThrowCard);

            CallBreakGameManager.instance.allTableCards.Add(this);

            Debug.Log(" ALL Table Cards  =====" + CallBreakGameManager.instance.allTableCards.Count);

            if (CallBreakGameManager.instance.allTableCards.Count == 1)
                CallBreakGameManager.instance.currentCard = this;

            CallBreakGameManager.currentPlayerIndex = CallBreakCardAnimation.instance.gamePlayController.allPlayer.IndexOf(CallBreakUIManager.Instance.gamePlayController.allPlayer[playerIndex]);

            Debug.Log(" Current PlayerTurn ====> " + CallBreakGameManager.currentPlayerIndex + " CARD " + gameObject.name);

            if (cardDetail.cardType == CardType.Spade)
                CallBreakGameManager.instance.clubCardCounter++;

            CardScaleAndMove(0.85f, CallBreakUIManager.Instance.gamePlayController.allPlayer[playerIndex].myThrowCardPos);

            CallBreakUIManager.Instance.gamePlayController.allPlayer[playerIndex].RemoveSuitCard(this);

            CallBreakUIManager.Instance.gamePlayController.allPlayer[playerIndex].myCards.Remove(this);

            CallBreakUIManager.Instance.gamePlayController.allPlayer[0].turnTimer.TimerObjectDeActivate();
        }

        private const float duration = 0.3f;

        public Transform table;

        internal void CardScaleAndMove(float scale, Transform destination)
        {
            if (CallBreakGameManager.instance.clubCardCounter == 1 && cardDetail.cardType == CardType.Spade)
            {
                this.transform.DOScale(Vector3.one * 4, 0.5f).SetEase(Ease.Linear).
                OnStart(() =>
                {
                    table.DOScale(Vector3.one * .75f, 0.5f).SetEase(Ease.Linear);
                }).OnUpdate(() =>
                {
                    this.transform.DORotate(new Vector3(0, 100, 0), 0.2f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
                }).OnComplete(() =>
                {
                    this.transform.DOKill();
                    this.transform.DORotate(Vector3.zero, 0.0f).SetEase(Ease.Linear);
                    DOVirtual.DelayedCall(1f, () =>
                    {
                        this.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.Linear);
                        this.transform.DOJump(cardThrowParent.position, 1f, 1, 0.2f).SetEase(Ease.Linear).
                        OnStart(() =>
                        {
                            table.DOScale(Vector3.one, 0.5f).SetEase(Ease.Linear);
                            this.transform.DORotate(new Vector3(0, 0, 0), 0.2f);

                        }).OnComplete(() =>
                        {
                            CallBreakGameManager.instance.particlesOfHukum.gameObject.SetActive(true);
                            CallBreakGameManager.instance.particlesOfHukum.Play();

                            DOVirtual.DelayedCall(1f, () =>
                            {

                                table.transform.DOShakePosition(0.2f, 100, 300).OnComplete(() =>
                                {
                                    Debug.Log("Animation compeleted here");
                                    if (CallBreakGameManager.currentPlayerIndex == 0)
                                    {
                                        int removeIndex = CallBreakUIManager.Instance.gamePlayController.allPlayer[0].myCards.IndexOf(this);
                                        CardPositionSet.instance.RemoveCard(gameObject);
                                        CallBreakGameManager.instance.particlesOfHukum.gameObject.SetActive(false);
                                    }
                                    StartCoroutine(CallBreakGameManager.instance.NextUserTurn());
                                });
                            });
                        });
                    });
                });
                Debug.LogError("ANIMATION HERE");
            }
            else
            {
                transform.SetParent(cardThrowParent);
                transform.DORotate(new Vector3(0, 0, 0), duration);
                transform.DOScale(new Vector3(scale, scale, scale), duration).SetEase(Ease.Linear);
                transform.DOMove(cardThrowParent.position, duration).SetEase(Ease.Linear).OnComplete(() =>
                {
                    if (CallBreakGameManager.currentPlayerIndex == 0)
                    {
                        int removeIndex = CallBreakUIManager.Instance.gamePlayController.allPlayer[0].myCards.IndexOf(this);
                        CardPositionSet.instance.RemoveCard(gameObject);
                    }
                    StartCoroutine(CallBreakGameManager.instance.NextUserTurn());
                });
            }
        }

    }

    [System.Serializable]
    public class CardDetail
    {
        public int value;
        public string cardName;
        public CardType cardType;
    }
    public enum CardType
    {
        Spade, Diamond, Heart, Club
    }
}
