using UnityEngine;
using DG.Tweening;

namespace HeartCardGame
{
    public class HT_HeartBrokenAnimation : MonoBehaviour
    {
        [Header("===== Heart Animation Properties =====")]
        [SerializeField] private RectTransform heartObjRectTransform;
        [SerializeField] private ParticleSystem heartBroken;

        [SerializeField] private HT_UiManager uiManager;
        [SerializeField] private HT_GameManager gameManager;
        [SerializeField] private HT_AudioManager audioManager;

        private void OnEnable() => HeartBrokenAnimation();

        public void HeartBrokenAnimation()
        {
            gameManager.isHeartAnimationShow = false;
            audioManager.GamePlayAudioSetting(audioManager.heartBrokenClip);

            heartBroken.gameObject.SetActive(true);
            heartBroken.Play();
            Invoke(nameof(ResetHeartBrokenAnimation), 1f);
            //heartObjRectTransform.DOShakePosition(1f, 20, 50, 200).OnComplete(() =>
            //{
            //    uiManager.heartBrokenPanel.SetActive(false);
            //});
        }

        void ResetHeartBrokenAnimation()
        {
            uiManager.heartBrokenPanel.SetActive(false);
            heartBroken.gameObject.SetActive(true);
            heartBroken.Play();
        }
    }
}