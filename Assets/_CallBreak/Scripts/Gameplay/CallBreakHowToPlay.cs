using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace FGSOfflineCallBreak
{
    public class CallBreakHowToPlay : MonoBehaviour
    {
        public RectTransform content;

        public void OpenScreen()
        {
            gameObject.SetActive(true);
            content.anchoredPosition = Vector2.zero;
        }
        public void CloseScreen()
        {
            gameObject.SetActive(false);
        }

    }
}
