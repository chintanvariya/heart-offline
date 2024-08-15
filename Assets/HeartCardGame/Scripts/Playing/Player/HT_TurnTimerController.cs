using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace HeartCardGame
{
    public class HT_TurnTimerController : MonoBehaviour
    {
        [Header("===== Timer Data =====")]
        [SerializeField] private Image timerFillingImg;
        [SerializeField] private Transform dotParentTransform;
        Coroutine timerCoroutine;

        [Header("===== Object Script =====")]
        [SerializeField] private HT_PlayerController playerController;

        public void TurnTimer(float turnTime, float remainTime)
        {
            ResetTime();
            gameObject.SetActive(true);
            timerFillingImg.gameObject.SetActive(true);
            timerFillingImg.fillAmount = remainTime / turnTime;
            playerController.isTurnStarted = true;
            playerController.BlurImgOnOff(true);
            TimerCountingOnOff(turnTime, remainTime);
        }

        void TimerCountingOnOff(float turnTime, float remainTime)
        {
            timerCoroutine = StartCoroutine(TimerCounting(turnTime, remainTime));
        }

        IEnumerator TimerCounting(float currentTime, float totalTime)
        {
            while (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                timerFillingImg.fillAmount = currentTime / totalTime;
                RotationDotOnRing(currentTime, totalTime);
                if (currentTime <= 0.5f)
                    HT_GameManager.instance.CardTargetRaycastOnOff(false);
                yield return 0;
            }

            if (currentTime < 0)
            {
                if (playerController.isMyPlayer && HT_GameManager.instance.isOffline)
                    HT_OfflineGameHandler.instance.offlinePlayerTurnController.CardMoving(playerController, 0f);
                gameObject.SetActive(false);
            }
        }

        void RotationDotOnRing(float currentTime, float totalTime)
        {
            dotParentTransform.eulerAngles = new Vector3(0, 0, (currentTime / totalTime) * 360);
        }

        public void ResetTime()
        {
            if (timerCoroutine != null)
                StopCoroutine(timerCoroutine);
            dotParentTransform.eulerAngles = Vector3.zero;
            timerFillingImg.fillAmount = 1;
            playerController.isTurnStarted = false;
            playerController.BlurImgOnOff(false);
            gameObject.SetActive(false);
        }
    }
}