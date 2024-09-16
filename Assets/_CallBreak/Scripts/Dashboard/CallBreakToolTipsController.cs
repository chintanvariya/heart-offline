using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace FGSOfflineCallBreak
{
    public class CallBreakToolTipsController : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI firstText;
        public UnityEngine.UI.Image icon;
        public TMPro.TextMeshProUGUI secondText;

        public Transform startPosition;
        public Transform targetPosition;

        public Transform rootTransfrom;

        public void OpenToolTips(string toolTipsName, string message01, string message02)
        {
            firstText.gameObject.SetActive(false);
            icon.gameObject.SetActive(false);
            secondText.gameObject.SetActive(false);
            switch (toolTipsName)
            {
                case "AdsIsNotReady":
                    firstText.gameObject.SetActive(true);
                    firstText.text = message01;
                    break;
                case "GamePlay":
                    firstText.text = message01;
                    secondText.text = message02;

                    firstText.gameObject.SetActive(true);
                    icon.gameObject.SetActive(true);
                    secondText.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }
            //rootTransfrom.position = startPosition.position;
            rootTransfrom.position = targetPosition.position;
            rootTransfrom.localScale = Vector3.zero;
            gameObject.SetActive(true);
            //rootTransfrom.DOMove(targetPosition.position, 0.2f).SetEase(Ease.Linear);
            rootTransfrom.DOScale(Vector3.one, 0.2f).SetEase(Ease.Linear);
            CancelInvoke(nameof(CloseAnimation));
            Invoke(nameof(CloseAnimation), 1f);
        }
        void CloseAnimation()
        {
            //rootTransfrom.DOMove(startPosition.position, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
            rootTransfrom.DOScale(Vector3.zero, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
    }
}
