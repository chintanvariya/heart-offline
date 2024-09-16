using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace FGSOfflineCallBreak
{
    public class CallBreakEmojiUiController : MonoBehaviour
    {
        public CallBreakEmojiController emojiController;
        public RectTransform rectTransform;
        public int indexOfEmoji;
        public void OnButtonClicked() => emojiController.ClickedOnEmojiIndex(indexOfEmoji);

        public void AnimationOnUserProfile(Transform from, Transform to)
        {
            rectTransform.sizeDelta = new Vector2(128, 128);


            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            transform.position = from.position;
            transform.SetParent(to.transform);
            rectTransform.DOAnchorPos(Vector2.zero, 1f).SetEase(Ease.InSine).OnComplete(() =>
            {
                Destroy(gameObject, 2.5f);
            });
        }
    }
}
