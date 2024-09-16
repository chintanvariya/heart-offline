using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace FGSOfflineCallBreak
{
    public class CallBreakCoinPrefabController : MonoBehaviour
    {
        public RectTransform rectTransform;

        //public Transform target;

        public GameObject particleObject;

        //private void Start()
        //{
        //    Animation();
        //}

        //public void Animation()
        //{
        //    transform.DOJump(target.position, -1.5f, 1, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
        //    {
        //        transform.position = Vector3.zero;
        //        Invoke(nameof(Animation), 2f);
        //    });
        //}

        public void MoveAnimationToTarget(CallBreakCollectRewardCoinAnimation coinAnimation, Transform target, float jumpMultiplayer, int jumpCount, float jumpTime)
        {
            particleObject.SetActive(true);
            transform.DOJump(target.position, jumpMultiplayer, jumpCount, jumpTime).SetEase(Ease.Linear).OnComplete(() =>
            {
                coinAnimation.CompleteAnimation();
            });
        }
    }
}
