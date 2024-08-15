using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Newtonsoft.Json;

namespace HeartCardGame
{
    public class HT_CollectBootManager : MonoBehaviour
    {
        [Header("===== Profile =====")]
        [SerializeField] private Transform generateTransform;
        [SerializeField] private List<RectTransform> playerList;
        [SerializeField] private List<HT_CollectBootHandler> collectBootObjList;

        [Header("===== Boot Data =====")]
        [SerializeField] private HT_CollectBootHandler collectBootHandler;

        [Header("===== Object Scripts =====")]
        [SerializeField] private HT_EventManager eventManager;
        [SerializeField] private HT_GameManager gameManager;
        [SerializeField] private HT_AudioManager audioManager;
        [SerializeField] private HT_LeaveGameHandler leaveGameHandler;

        [Header("===== Model Class =====")]
        [SerializeField] private CollectBootValueResponse collectBootValueResponse;

        private void OnEnable() => eventManager.RegisterEvent(SocketEvents.COLLECT_BOOT_VALUE, CollectBootValue);

        private void OnDisable() => eventManager.UnregisterEvent(SocketEvents.COLLECT_BOOT_VALUE, CollectBootValue);

        private void Start() => gameManager.GameReset += ResetCollectBootManager; // 4 % 5= ?

        private void CollectBootValue(string arg0)
        {
            collectBootValueResponse = JsonConvert.DeserializeObject<CollectBootValueResponse>(arg0);
            CollectBootAnimation(collectBootValueResponse.data.entryFee);
            gameManager.tableState = TableState.COLLECTING_BOOT_VALUE;
        }

        public void CollectBootAnimation(int entreFee)
        {
            leaveGameHandler.LeaveButtonNoPressed();
            for (int i = 0; i < playerList.Count; i++)
            {
                HT_CollectBootHandler bootClone = Instantiate(collectBootHandler, generateTransform);
                bootClone.SetPosition(playerList[i].anchoredPosition);
                bootClone.SetBootValue(entreFee);
                collectBootObjList.Add(bootClone);
                bootClone.rectTransform.DOLocalMove(new Vector3(0, 150, 0), 1f).OnComplete(() =>
                {
                    bootClone.rectTransform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.5f);
                    bootClone.SetBootValue(entreFee * playerList.Count);
                    bootClone.bootImg.DOFade(0, 1f);
                    bootClone.bootTxt.DOFade(0, 1f).OnComplete(() => { ResetCollectBootManager(); });
                });
            }
            audioManager.GamePlayAudioSetting(audioManager.collectBootClip);
        }

        public void ResetCollectBootManager()
        {
            foreach (var item in collectBootObjList)
            {
                if (item != null)
                {
                    item.rectTransform.DOKill();
                    Destroy(item.gameObject);
                }
            }
            collectBootObjList.Clear();
        }
    }

    [Serializable]
    public class BalanceDatum
    {
        public int balance;
        public string userId;
    }

    [Serializable]
    public class CollectBootValueResponseData
    {
        public int entryFee;
        public List<string> userIds;
        public List<BalanceDatum> balanceData;
    }

    [Serializable]
    public class CollectBootValueResponse
    {
        public string en;
        public CollectBootValueResponseData data;
    }
}