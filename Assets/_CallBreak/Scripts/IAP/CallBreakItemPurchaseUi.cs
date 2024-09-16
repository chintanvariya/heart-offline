using UnityEngine;
using UnityEngine.Purchasing;

namespace FGSOfflineCallBreak
{
    public class CallBreakItemPurchaseUi : MonoBehaviour
    {
        public UnityEngine.UI.Text coinText;
        public UnityEngine.UI.Text coinDescriptionText;

        public CallBreakIAPManager iapManager;

        public Product product;

        public void UpdateTheValue(Product _product)
        {
            product = _product;
            coinText.text = _product.metadata.localizedDescription;
            coinDescriptionText.text = _product.metadata.localizedPriceString;
        }

        public void OnButtonClicked()
        {
            iapManager.GoingToPurchase(product);
        }
    }
}
