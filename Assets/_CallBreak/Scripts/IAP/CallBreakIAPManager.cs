using System;
using UnityEngine;
using UnityEngine.Purchasing;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine.Purchasing.Extension;
using System.Collections.Generic;

namespace FGSOfflineCallBreak
{
    public class CallBreakIAPManager : MonoBehaviour, IStoreListener
    {
        private static IStoreController m_StoreController;          // The Unity Purchasing system.
        private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

        public List<Product> allProduct = new List<Product>();

        public static CallBreakIAPManager Instance;

        public List<string> allCoinPackValue;
        public List<string> allCoinValue;

        private string environment = "production";

        [Obsolete]
        async void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            try
            {
                var options = new InitializationOptions().SetEnvironmentName(environment);

                await UnityServices.InitializeAsync(options);

                if (m_StoreController == null)
                {
                    // Begin to configure our connection to Purchasing
                    InitializePurchasing();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                // An error occurred during initialization.
            }
            // If we haven't set up the Unity Purchasing reference
        }

        [Obsolete]
        public void InitializePurchasing()
        {
            // If we have already connected to Purchasing ...
            if (IsInitialized())
            {
                // ... we are done here.
                return;
            }

            // Create a builder, first passing in a suite of Unity provided stores.
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            Debug.Log(" ==== IAP INIT ====");

            //builder.AddProduct(allCoinPackValue[], ProductType.Consumable);
            for (int i = 0; i < allCoinPackValue.Count; i++)
            {
                builder.AddProduct(allCoinPackValue[i], ProductType.Consumable);
            }

            UnityPurchasing.Initialize(this, builder);
            Debug.Log(" ==== IAP INIT ====");
        }

        private bool IsInitialized()
        {
            // Only say we are initialized if both the Purchasing references are set.         
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }


        public void GoingToPurchase(Product product)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                CallBreakUIManager.Instance.noInternetController.OpenScreen();
            }
        }

        void BuyProductID()
        {
            // If Purchasing has been initialized ...
            if (IsInitialized())
            {
                // system's products collection.
                for (int i = 0; i < allCoinPackValue.Count; i++)
                {
                    Product product = m_StoreController.products.WithID(allCoinPackValue[i]);
                    allProduct.Add(product);
                }

                //if (product1 != null && product1.availableToPurchase)
                //{
                //    PlayerPrefs.SetString("price1", product1.metadata.localizedPriceString);
                //    PlayerPrefs.SetString("descriptioncoinpack1", product1.metadata.localizedDescription);
                //}
            }
            else
            {
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }

        //  
        // --- IStoreListener
        //

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            // Purchasing has succeeded initializing. Collect our Purchasing references.
            //			Debug.Log("OnInitialized: PASS");

            // Overall Purchasing system, configured with products for this application.
            m_StoreController = controller;
            // Store specific subsystem, for accessing device-specific store features.
            m_StoreExtensionProvider = extensions;

            BuyProductID();
        }

        public Product ReturnTheProduct(string packName)
        {
            // Loop through all the products in the list
            foreach (Product product in allProduct)
            {
                Debug.Log($"Coin Pack Id {product.definition.id}");
                // Check if the product ID matches the given packName
                if (product.definition.id == packName)
                {
                    return product;
                }
            }

            // Return null if no matching product is found
            return null;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error.ToString());
        }

        public int coinStore;
        public int keys;
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            // A consumable product has been purchased by this user.
            if (String.Equals(args.purchasedProduct.definition.id, allCoinPackValue[0], StringComparison.Ordinal))
            {
                coinStore =int.Parse(allCoinValue[0]);
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            }
            else if (String.Equals(args.purchasedProduct.definition.id, allCoinPackValue[1], StringComparison.Ordinal))
            {
                coinStore =int.Parse(allCoinValue[1]);
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            }
            else if (String.Equals(args.purchasedProduct.definition.id, allCoinPackValue[2], StringComparison.Ordinal))
            {
                coinStore =int.Parse(allCoinValue[2]);
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            }
            else if (String.Equals(args.purchasedProduct.definition.id, allCoinPackValue[3], StringComparison.Ordinal))
            {
                coinStore =int.Parse(allCoinValue[3]);
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            }
            else if (String.Equals(args.purchasedProduct.definition.id, allCoinPackValue[4], StringComparison.Ordinal))
            {
                coinStore =int.Parse(allCoinValue[4]);
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            }
            else if (String.Equals(args.purchasedProduct.definition.id, allCoinPackValue[5], StringComparison.Ordinal))
            {
                coinStore = int.Parse(allCoinValue[5]);
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            }
            else if (String.Equals(args.purchasedProduct.definition.id, allCoinPackValue[6], StringComparison.Ordinal))
            {
                keys = int.Parse(allCoinValue[6]);
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            }

            CallBreakUIManager.Instance.rewardCoinAnimation.CollectCoinAnimation("CoinStore");

            return PurchaseProcessingResult.Complete;
        }


        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
            // this reason with the user to guide their troubleshooting actions.
            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));

        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.Log($"OnInitializeFailed InitializationFailureReason: 212121212 {error.ToString()} || {message}");
        }

    }
}
