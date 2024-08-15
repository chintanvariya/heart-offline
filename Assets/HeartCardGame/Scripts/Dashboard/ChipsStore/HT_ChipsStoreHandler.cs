using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace HeartCardGame
{
    public class HT_ChipsStoreHandler : MonoBehaviour
    {
        [Header("===== Object Scripts =====")]
        [SerializeField] private HT_SocketHandler socketHandler;
        [SerializeField] private HT_DashboardManager dashboardManager;
        [SerializeField] private PAInAppPurchasing pAInAppPurchasing;
        [SerializeField] private HT_UiManager uiManager;
        [SerializeField] private HT_UserRegistration userRegistration;

        [Header("===== Chips Store Data =====")]
        [SerializeField] private Transform chipsStoreDataGenerator;
        [SerializeField] private HT_ChipsStorePrefabHandler chipsStorePrefabHandler;
        [SerializeField] private List<HT_ChipsStorePrefabHandler> chipsStorePrefabHandlers;

        [Header("===== Model Class =====")]
        public GetChipsStore getChipsStore;

        public void GetChipsStoreData()
        {
            string url = socketHandler.serverUrl[(int)socketHandler.serverType];
            url += HT_StaticData.GetChipsStore;
            if (dashboardManager.IsAccessTokenAvailable())
                ChipsStoreDataHandle(url);
            else
                userRegistration.UserRegister(PlayerPrefs.GetString("UserName"), (success) => ChipsStoreDataHandle(url));
        }

        void ChipsStoreDataHandle(string url)
        {
            StartCoroutine(HT_APIManager.RequestWithPostData(url, "", (data) =>
            {
                getChipsStore = JsonConvert.DeserializeObject<GetChipsStore>(data);
                pAInAppPurchasing.Inst();
                ChipsStoreSetting();
            }, (error) => uiManager.ApiError(error)));
        }

        void ChipsStoreSetting()
        {
            dashboardManager.PopupOnOff(dashboardManager.chipsStorePanel, true);
            for (int i = 0; i < getChipsStore.data.Count; i++)
            {
                var chipsStoreData = getChipsStore.data[i];
                HT_ChipsStorePrefabHandler chipsStoreClone = chipsStorePrefabHandlers[i];
                chipsStoreClone.SetChipsStoreData(chipsStoreData.coins, chipsStoreData.price, chipsStoreData._id);
            }
        }
    }

    #region GetLobbyModelClass
    [System.Serializable]
    public class GetChipsStoreData
    {
        public string _id;
        public string packageId;
        public int price;
        public int coins;
        public string inAppStoreImage;
        public bool isOffer;
    }

    [System.Serializable]
    public class GetChipsStore
    {
        public string message;
        public string status;
        public int statusCode;
        public bool success;
        public List<GetChipsStoreData> data;
    }
    #endregion
}