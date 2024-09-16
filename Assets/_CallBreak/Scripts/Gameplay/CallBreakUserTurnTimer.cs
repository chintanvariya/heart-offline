using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;

namespace FGSOfflineCallBreak
{
    public class CallBreakUserTurnTimer : MonoBehaviour
    {

        public Image userFillImage;

        public TextMeshProUGUI userTimerText;
        public GameObject dotObj;
        public GameObject timerObject;

        private int userTimerDuration = 20;

        private static Tweener userFillImageAnimation;
        private static Tweener userCounterAnimation;
        private static Tweener dotRotationAnimation;

        public void ResetUserTimer() => TimerObjectDeActivate();

        public void SelfUserTimer()
        {
            userTimerDuration = 20;
            userFillImage.fillAmount = 0;
            dotObj.transform.eulerAngles = Vector3.zero;

            timerObject.SetActive(true);

            if (!CallBreakCardAnimation.instance.gamePlayController.allPlayer[CallBreakGameManager.currentPlayerIndex].isSelfPlayer)
            {
                Invoke(nameof(ThrowCardOfBot), UnityEngine.Random.Range(0, 1));
            }
            else
            {
                CallBreakCardAnimation.instance.SelfPlayerTempObjActive(false);
                CallBreakSoundManager.PlaySoundEvent(SoundEffects.YourTurn);
                CallBreakSoundManager.PlayVibrationEvent();
                CallBreakCardAnimation.instance.CardRaycast(true);
                userFillImageAnimation = userFillImage.DOFillAmount(1, userTimerDuration).SetEase(Ease.Linear).OnComplete(() =>
                {
                     CallBreakCardAnimation.instance.gamePlayController.allPlayer[CallBreakGameManager.currentPlayerIndex].UserTurnStarted();
                });
            }

            dotRotationAnimation = dotObj.transform.DORotate(new Vector3(0, 0, -360), userTimerDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear);
            userCounterAnimation = DOTween.To(() => userTimerDuration, x => userTimerText.text = Mathf.Round(x).ToString(), 0, userTimerDuration).SetEase(Ease.Linear).OnUpdate(() =>
            {
                if (userTimerText.text == "1")
                {
                    CallBreakCardAnimation.instance.CardRaycast(false);
                }
            });
        }

        public void ThrowCardOfBot()
        {
            Debug.Log($"ThrowCardOfBot ");
            CallBreakCardAnimation.instance.gamePlayController.allPlayer[CallBreakGameManager.currentPlayerIndex].UserTurnStarted();
        }

        public void TimerObjectDeActivate()
        {
            userCounterAnimation.Kill(); userFillImageAnimation.Kill(); dotRotationAnimation.Kill();
            timerObject.SetActive(false);
        }

    }
}
