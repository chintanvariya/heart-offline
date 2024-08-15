using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using Newtonsoft.Json;
using UnityEngine.Purchasing.Extension;

namespace HeartCardGame
{
    public class PAInAppPurchasing : MonoBehaviour, IDetailedStoreListener
    {
        [Header("===== Object Scripts =====")]
        [SerializeField] private HT_ChipsStoreHandler chipsStoreHandler;
        [SerializeField] private HT_SocketHandler socketHandler;
        [SerializeField] private HT_DashboardManager dashboardManager;
        [SerializeField] private HT_UserRegistration userRegistration;
        [SerializeField] private HT_UiManager uiManager;

        [Header("===== Model Class =====")]
        [SerializeField] AddCoinsResponse addCoinsResponse;

        public static PAInAppPurchasing instance;
        private Action OnPurchaseCompleted;
        private IStoreController StoreController;
        private IExtensionProvider ExtensionProvider;

        private void Awake()
        {
            if (instance == null)
                instance = this;
        }

        private void Start()
        {
            //Inst();
        }

        internal async void Inst()
        {
            InitializationOptions options = new InitializationOptions()
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                .SetEnvironmentName("test");
#else
            .SetEnvironmentName("production");
#endif
            await UnityServices.InitializeAsync(options);
            ResourceRequest operation = Resources.LoadAsync<TextAsset>("IAPProductCatalog");
            operation.completed += HandleIAPCatalogLoaded;
        }

        private void HandleIAPCatalogLoaded(AsyncOperation Operation)
        {
            ResourceRequest request = Operation as ResourceRequest;

            Debug.Log($"Loaded Asset: {request.asset}");
            ProductCatalog catalog = JsonUtility.FromJson<ProductCatalog>((request.asset as TextAsset).text);
            Debug.Log($"Loaded catalog with {catalog.allProducts.Count} items");

            // ------
            /*  StandardPurchasingModule.Instance().useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
              StandardPurchasingModule.Instance().useFakeStoreAlways = true;
    */
#if UNITY_ANDROID
            ConfigurationBuilder builder = ConfigurationBuilder.Instance(
                StandardPurchasingModule.Instance(AppStore.GooglePlay)
            );
#elif UNITY_IOS
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.AppleAppStore)
        );
#else
            ConfigurationBuilder builder = ConfigurationBuilder.Instance(
                StandardPurchasingModule.Instance(AppStore.NotSpecified)
            );
#endif
            foreach (ProductCatalogItem item in catalog.allProducts)
            {
                Debug.Log("ID IAP=> " + item.id + "" + item.type);
                builder.AddProduct(item.id, item.type);
            }

            Debug.Log($"Initializing Unity IAP with {builder.products.Count} products");
            UnityPurchasing.Initialize(this, builder);

        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            StoreController = controller;
            ExtensionProvider = extensions;
            Debug.Log($"Successfully Initialized Unity IAP. Store Controller has {StoreController.products.all.Length} products");
            /* PAStoreIconProvider.Initialize(StoreController.products);
             PAStoreIconProvider.OnLoadComplete += HandleAllIconsLoaded;*/
            HandleAllIconsLoaded();
        }

        private void HandleAllIconsLoaded()
        {
            Debug.Log("ON call");
            StartCoroutine(CreateUI());
        }

        [SerializeField] private List<PAProduct> getProducts = new List<PAProduct>();

        private IEnumerator CreateUI()
        {
            List<Product> sortedProducts = StoreController.products.all.ToList();
            Debug.Log("sortedProducts => " + sortedProducts.Count);
            Debug.Log("sortedProducts => " + chipsStoreHandler.getChipsStore.data.Count);

            for (int i = 0; i < chipsStoreHandler.getChipsStore.data.Count; i++)
            {
                Product product = sortedProducts[i];
                PAProduct uiProduct = getProducts[i];
                uiProduct.OnPurchase += HandlePurchase;
                uiProduct.Setup(product, chipsStoreHandler.getChipsStore.data[i].coins);
                yield return null;
            }
        }

        private void HandlePurchase(Product Product, Action OnPurchaseCompleted)
        {
            this.OnPurchaseCompleted = OnPurchaseCompleted;
            StoreController.InitiatePurchase(Product);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log($"Failed to purchase {product.definition.id} because {failureReason}");
            OnPurchaseCompleted?.Invoke();
            OnPurchaseCompleted = null;
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            Debug.Log($"Successfully purchased {purchaseEvent.purchasedProduct.definition.id}");
            Debug.Log($"Successfully purchased {purchaseEvent.purchasedProduct.metadata.localizedPrice}");
            OnPurchaseCompleted?.Invoke();
            OnPurchaseCompleted = null;

            int buyChips = chipsStoreHandler.getChipsStore.data.Find(purchase => purchase.packageId == purchaseEvent.purchasedProduct.definition.id).coins;
            Debug.Log($"Plus buy chips {buyChips}");
            string url = socketHandler.serverUrl[(int)socketHandler.serverType];
            url += HT_StaticData.AddCoins;
            if (dashboardManager.IsAccessTokenAvailable())
                AddCoinsHandle(url);
            else
                userRegistration.UserRegister(PlayerPrefs.GetString("UserName"), (success) => AddCoinsHandle(url));
            //SlotMachineGameManager.instance.walletAmount += buyChips;
            // UiManager.Instance.ChipsStoreClose();
            /*  for (int i = 0; i < Authentification.response.data.purchaseCoinAmount.Count; i++)
              {
                  if (purchaseEvent.purchasedProduct.definition.id == Authentification.response.data.purchaseCoinAmount[i]._id)
                  {
                      JSONObject data = new JSONObject();

                      data.AddField("chips", Authentification.response.data.purchaseCoinAmount[i].chips);
                      data.AddField("packageId", purchaseEvent.purchasedProduct.definition.id);
                      data.AddField("price", (float)purchaseEvent.purchasedProduct.metadata.localizedPrice);
                      SocketConnection.instance.SendDataToSocket(data, OnUpdateUserWallet, PRN_CustomEvents.UPDATE_USER_WALLET.ToString());
                  }
              }

              ChipsStoreManager.instance.chipsStorePanel.SetActive(false);*/
            return PurchaseProcessingResult.Complete;
        }

        void AddCoinsHandle(string url)
        {
            StartCoroutine(HT_APIManager.RequestWithPostData(url, HT_APIEventManager.AddCoins("65c0b2e4ecadc2a25b7c98cc"), (data) =>
            {
                addCoinsResponse = JsonConvert.DeserializeObject<AddCoinsResponse>(data);
                dashboardManager.UserDataSetting(userRegistration.userName, addCoinsResponse.data.coins, userRegistration.profilePic, false);
            }, (error) => uiManager.ApiError(error)));
        }

        /* private void OnUpdateUserWallet(string obj)
         {
             Debug.Log($"[------ ACK Update_User_Wallet -----]  " + obj);
             ClaimedReward response = JsonConvert.DeserializeObject<ClaimedReward>(obj.ToString());
             PRN_StandloneManager.instance.SetPlayerChipsAmount(response.data.chips);

         }*/

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.LogError($"Error initializing IAP because of {error}." +
                 $"\r\nShow a message to the player depending on the error.");
        }


        IEnumerator OpenPurchasePopup(float Balance)
        {
            /// string Amount = PASocketEventReceiver.instance.AmountInString(Balance);
            ///  discriptionText.text = "Congratulations, you have successfully purchased " + Amount;
            //iapPopup.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            ///PASocketConnection.instance.SendData(PASocketEventManger.instance.STORE_HISTROY(PASocketEventReceiver.instance.playerInfo.userId, Balance, "Purchase"), PAEvents.STORE_HISTROY);
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError($"Error initializing IAP because of {error}." +
                 $"\r\nShow a message to the player depending on the error.");
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            throw new NotImplementedException();
        }
    }
    [Serializable]
    public class PurchaseData
    {
        public string storeId;
    }

    [Serializable]
    public class AddCoinsResponseData
    {
        public int coins;
    }

    [Serializable]
    public class AddCoinsResponse
    {
        public string message;
        public string status;
        public int statusCode;
        public bool success;
        public AddCoinsResponseData data;
    }
}