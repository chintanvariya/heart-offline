using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace FGSOfflineCallBreak
{
    public class CallBreakCollectRewardCoinAnimation : MonoBehaviour
    {
        public CallBreakCoinPrefabController chipsPrefab;

        public Transform parent;
        public Transform target;


        [Range(-500, 500)]
        public float jumpMultiplayer;

        [Range(0, 10)]
        public int jumpCount;

        [Range(0, 10)]
        public float jumpTime;

        public float offSetX = 0.7f;
        public float offSetY = 0.7f;

        List<CallBreakCoinPrefabController> allObjects = new List<CallBreakCoinPrefabController>();

        public int counterOFAnimationDone;

        public string doAnimationOfThisScreen;

        public void CollectCoinAnimation(string comingFrom)
        {
            doAnimationOfThisScreen = comingFrom;

            switch (doAnimationOfThisScreen)
            {
                case "Spinner":
                    if (CallBreakUIManager.Instance.spinnerController.rewardOfSpinner == 0)
                    {
                        CallBreakUIManager.Instance.soundManager.PlaySoundEffect(SoundEffects.Lose);
                        if (CallBreakUIManager.Instance.spinnerController.isFromAds)
                            CallBreakUIManager.Instance.spinnerController.CloseScreen();
                        return;
                    }
                    break;
                case "CollectReward": break;
                case "WinnerLoser": break;
                case "100Coins": break;
                default:
                    break;
            }

            foreach (Transform child in parent)
                Destroy(child.gameObject);
            allObjects.Clear();

            for (int i = 0; i < 10; i++)
            {
                CallBreakCoinPrefabController clone = Instantiate(chipsPrefab, parent);
                allObjects.Add(clone);
            }

            counterOFAnimationDone = 0;
            StartCoroutine(Animation());
        }


        IEnumerator Animation()
        {
            for (int i = 0; i < allObjects.Count; i++)
            {
                yield return new WaitForSeconds(0.02f);
                Vector3 target = new Vector3(allObjects[i].transform.position.x + UnityEngine.Random.Range(-offSetX, offSetX), allObjects[i].transform.position.y + UnityEngine.Random.Range(-offSetY, offSetY), 0);
                allObjects[i].transform.DOMove(target, 0.1f).SetEase(Ease.OutBack);
            }

            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < allObjects.Count; i++)
            {
                yield return new WaitForSeconds(0.15f);
                allObjects[i].rectTransform.DOSizeDelta(target.GetComponent<RectTransform>().sizeDelta, jumpTime);
                allObjects[i].MoveAnimationToTarget(this, target, jumpMultiplayer, jumpCount, jumpTime);
            }
        }

        public void CompleteAnimation()
        {
            float rewardOfCoin = 0;
            float rewardOfKeys = 0;
            counterOFAnimationDone++;
            if (counterOFAnimationDone == allObjects.Count)
            {
                for (int i = 0; i < allObjects.Count; i++)
                {
                    Destroy(allObjects[i].gameObject);
                }
                switch (doAnimationOfThisScreen)
                {
                    case "Spinner":
                        rewardOfCoin = CallBreakUIManager.Instance.spinnerController.rewardOfSpinner;
                        if (CallBreakUIManager.Instance.spinnerController.isFromAds)
                            CallBreakUIManager.Instance.spinnerController.CloseScreen();
                        break;
                    case "CollectReward":
                        rewardOfCoin = CallBreakUIManager.Instance.collectRewardController.rewardedCoins;
                        CallBreakUIManager.Instance.collectRewardController.OnButtonClicked("Close");
                        break;
                    case "WinnerLoser":
                        rewardOfCoin = CallBreakUIManager.Instance.winnerLoserController.rewardedCoins;
                        CallBreakUIManager.Instance.winnerLoserController.OnButtonClicked("Close");
                        break;
                    case "100Coins":
                        rewardOfCoin = 100;
                        break;
                    case "CoinStore":
                        rewardOfCoin = CallBreakIAPManager.Instance.coinStore;
                        rewardOfKeys = CallBreakIAPManager.Instance.keys;
                        break;
                    default:
                        break;
                }

                CallBreakGameManager.instance.selfUserDetails.userChips += rewardOfCoin;
                CallBreakGameManager.instance.selfUserDetails.userKeys += rewardOfKeys;
                CallBreakConstants.UserDetialsJsonString = CallBreakUtilities.ReturnJsonString(CallBreakGameManager.instance.selfUserDetails);
                CallBreakUIManager.Instance.dashboardController.profileUiController.UpdateUserChips();
                CallBreakUIManager.Instance.dashboardController.profileUiController.UpdateUserKeys();
                CallBreakUIManager.Instance.dashboardController.OpenScreen();

                if (doAnimationOfThisScreen == "Spinner" && !CallBreakUIManager.Instance.spinnerController.isFromAds)
                    CallBreakUIManager.Instance.dashboardController.CloseScreen();

            }
        }
    }
}