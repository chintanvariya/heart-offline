
using UnityEngine;
using DG.Tweening;
using System.Collections;

namespace FGSOfflineCallBreak
{
    public class CallBreakSplashScreen : MonoBehaviour
    {
        public UnityEngine.UI.Image slider;
        public UnityEngine.UI.Text percentageText;

        internal void StartAnimation()
        {
            //yield return new WaitForSeconds(1f);

            slider.DOFillAmount(.98f, 5).SetEase(Ease.InOutCirc);
            CallBreakCardAnimation.isSplashShow = false;
            var loadingAnimation = percentageText.DOText("98%", 5f).SetEase(Ease.InOutCirc).OnComplete(() =>
            {
                OnCompletedAnimation();
            });

            loadingAnimation.OnUpdate(() =>
            {
                float progress = loadingAnimation.ElapsedPercentage() * 100f;
                percentageText.text = $"{progress:F0}%";
            });
        }

        public void OnCompletedAnimation()
        {
            if (CallBreakConstants.RegisterOrNot())
                LunchOnDashboard();
            else
                CallBreakUIManager.Instance.registerUserController.OpenScreen();
            gameObject.SetActive(false);
        }


        public void LunchOnDashboard()
        {
            CallBreakGameManager.instance.selfUserDetails = CallBreakUtilities.ReturnUserDetails(CallBreakConstants.UserDetialsJsonString);
            CallBreakGameManager.profilePicture = CallBreakGameManager.instance.allProfileSprite[CallBreakGameManager.instance.selfUserDetails.userAvatarIndex];
            this.gameObject.SetActive(false);
            CallBreakUIManager.Instance.dashboardController.OpenScreen();
        }
    }
}
