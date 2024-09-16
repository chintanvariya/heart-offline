using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using Newtonsoft.Json;
using UnityEngine;
using static FGSOfflineCallBreak.EmojiRequestResponseClass;

namespace FGSOfflineCallBreak
{
    public class EmojiRequestResponseClass
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        [System.Serializable]
        public class EmojiEventData
        {
            public int indexOfEmoji;
            public int toSendSeatIndex;
            public int fromToSendSeatIndex;
            public string tableId;
        }

        [System.Serializable]
        public class EmojiEvent
        {
            public string en;
            public EmojiEventData data;
        }
    }

    public class CallBreakEmojiController : MonoBehaviour
    {

        [SerializeField] private List<CallBreakEmojiUiController> allEmojies;

        public RectTransform root;
        public RectTransform startPosition;
        public RectTransform targetPosition;

        public EmojiEvent emojiResponse;

        public int selectToSendIndex;

        public void OpenEmojiScreen(int selectToSend)
        {
            selectToSendIndex = selectToSend;
            gameObject.SetActive(true);
            for (int i = 0; i < allEmojies.Count; i++)
                allEmojies[i].indexOfEmoji = i;
            root.transform.position = startPosition.position;
            root.DOMove(targetPosition.position, 0.5f);
        }

        public void CloseEmojiScreen()
        {
            root.DOMove(startPosition.position, 0.5f).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }

        public void OnEmojiResponse(string response)
        {
            root.transform.position = startPosition.position;
            emojiResponse = JsonConvert.DeserializeObject<EmojiEvent>(response);
            EmojiAnimation(emojiResponse.data.fromToSendSeatIndex, emojiResponse.data.toSendSeatIndex, emojiResponse.data.indexOfEmoji);
        }

        public void EmojiAnimation(int from, int to, int emojiIndex)
        {
            this.gameObject.SetActive(true);
            root.transform.position = targetPosition.position;
            CallBreakUserController fromHomeController = CallBreakUIManager.Instance.gamePlayController.allPlayer.Find(player => player.staticSeatIndex == from);
            CallBreakUserController toHomeController = CallBreakUIManager.Instance.gamePlayController.allPlayer.Find(player => player.staticSeatIndex == to);

            CallBreakEmojiUiController emojiUiController = Instantiate(allEmojies[emojiIndex], transform);
            emojiUiController.AnimationOnUserProfile(fromHomeController.emojiTransform, toHomeController.emojiTransform);
            //SoundManager.instance.EmojiSoundPlay(SoundManager.instance.emojiSoundClip, emojiIndex);
            this.gameObject.SetActive(false);
        }

        public void ClickedOnEmojiIndex(int emojiNo) => OnEmojiResponse(EmojiRequestData(emojiNo, selectToSendIndex));
        private void EmojiAcknowledgement(string expectAcknowledgement) => Debug.Log("EmojiAcknowledgement || expectAcknowledgement  " + expectAcknowledgement);

        public string EmojiRequestData(int emojiNumber, int toSendSeatIndex)
        {
            EmojiEvent emojiRequest = new EmojiEvent();
            EmojiEventData emojiRequestData = new EmojiEventData();
            emojiRequest.en = "EMOJI";

            emojiRequestData.toSendSeatIndex = toSendSeatIndex;
            emojiRequestData.fromToSendSeatIndex = 0;
            emojiRequestData.indexOfEmoji = emojiNumber;

            emojiRequest.data = emojiRequestData;
            string json = JsonConvert.SerializeObject(emojiRequest);
            return json;
        }
    }
}
