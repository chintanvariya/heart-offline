using System;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace HeartCardGame
{
    public class PAProduct : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI quantityText, amountText;
        [SerializeField]
        private Button purchaseBtn;

        public delegate void PurchaseEvent(Product Model, Action OnComplete);
        public event PurchaseEvent OnPurchase;




        private Product Model;
        public void Setup(Product Product, float chipsQty)
        {
            Debug.Log($"Product {Product}");
            Model = Product;
            quantityText.SetText($"{chipsQty} Coins");
            // +$"{Product.metadata.isoCurrencyCode}");
            //  string amount = Product.metadata.localizedDescription;
            amountText.text = $"₹ {Product.metadata.localizedPriceString}";

            Debug.Log("AM=>  " + Product.metadata.localizedPriceString);
            /* Texture2D texture = PAStoreIconProvider.GetIcon(Product.definition.id);
             if (texture != null)
             {
                 Sprite sprite = Sprite.Create(texture,
                     new Rect(0, 0, texture.width, texture.height),
                     Vector2.one / 2f
                 );

                 Icon.sprite = sprite;
             }
             else
             {
                 Debug.LogError($"No Sprite found for {Product.definition.id}!");
             }*/
        }

        public void Purchase()
        {
            Debug.Log("Click On Purchase");
            //StartCoroutine(ChipsStoreController.instance.CheckNetwork(false, Purchase));
            purchaseBtn.enabled = false;
            Debug.Log($"purchase {purchaseBtn.gameObject.name}", gameObject);
            Purchase(true);
        }

        private void Purchase(bool isNetworkOn)
        {
            if (isNetworkOn) OnPurchase?.Invoke(Model, HandlePurchaseComplete);
        }

        private void HandlePurchaseComplete()
        {
            purchaseBtn.enabled = true;
        }
    }
}