using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGSOfflineCallBreak
{
    public class CallBreakNoInternetController : MonoBehaviour
    {
        public void OpenScreen() => gameObject.SetActive(true);
        public void CloseScreen() => gameObject.SetActive(false);

        public void OnButtonClicked()
        {
            //ReStartApp Or Else Restart all function those requiered internet ads inapp firebase
            CloseScreen();
        }
    }
}
