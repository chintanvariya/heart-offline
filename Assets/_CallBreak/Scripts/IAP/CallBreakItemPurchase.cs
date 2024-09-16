using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace FGSOfflineCallBreak
{
    public class CallBreakItemPurchase : MonoBehaviour
    {
        public List<CallBreakItemPurchaseUi> allCoinPack;
        public List<string> allCoinPackString;
        public List<string> allCoinPackValue;

        public void OpenScreen()
        {
            for (int i = 0; i < allCoinPack.Count; i++)
            {
                allCoinPack[i].UpdateTheValue(CallBreakIAPManager.Instance.ReturnTheProduct(allCoinPackString[i]));
            }
            gameObject.SetActive(true);
        }

        public void CloseScreen() => gameObject.SetActive(false);
    }
}