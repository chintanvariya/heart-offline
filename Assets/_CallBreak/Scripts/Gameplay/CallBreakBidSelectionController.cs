using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace FGSOfflineCallBreak
{

    public class CallBreakBidSelectionController : MonoBehaviour
    {
        public Button confirmBidButton;

        public Image timerSlider;

        public int totalTimer;

        public List<Image> allButtonImages;

        public Sprite normalButton, selectedButton;

        public void OpenScreen()
        {
            for (int i = 0; i < allButtonImages.Count; i++)
                allButtonImages[i].sprite = normalButton;

            confirmBidButton.interactable = false;

            timerSlider.fillAmount = 1;
            timerSlider.DOFillAmount(0, totalTimer).SetEase(Ease.Linear).OnComplete(() =>
            {
                OnConfirmBidButton();
            });
            gameObject.SetActive(true);
        }


        public void OnButtonBidSelection(int countOfBid)
        {
            timerSlider.DOKill();

            CallBreakSoundManager.PlaySoundEvent(SoundEffects.Click);
            for (int i = 0; i < allButtonImages.Count; i++)
                allButtonImages[i].sprite = normalButton;
            allButtonImages[countOfBid - 1].sprite = selectedButton;

            CallBreakUIManager.Instance.gamePlayController.allPlayer[0].totalBid = countOfBid;
            confirmBidButton.interactable = true;
        }

        public void CloseScreen() => gameObject.SetActive(false);

        public void OnConfirmBidButton()
        {
            CallBreakSoundManager.PlaySoundEvent(SoundEffects.Click);

            CallBreakUIManager.Instance.gamePlayController.allPlayer[0].SetMyBid();

            CallBreakCardAnimation.instance.gamePlayController.allPlayer[0].BidToolTipAnimation();

            CallBreakCardAnimation.instance.OnBidToolTipClose();

            CloseScreen();
        }

    }
}
