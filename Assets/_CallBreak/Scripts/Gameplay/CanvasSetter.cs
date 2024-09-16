using UnityEngine;
using UnityEngine.UI;

namespace FGSOfflineCallBreak
{
    public class CanvasSetter : MonoBehaviour
    {
        private CanvasScaler canvasScaler;
        public bool isPopup;
        private float ratio = 1;
        private void Awake()
        {
            canvasScaler = GetComponent<CanvasScaler>();
            if (isPopup) ratio = 0.5f;
            SetMatchRatio();
        }
        private void SetMatchRatio()
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            float scaleFactor = screenWidth / screenHeight;
            canvasScaler.matchWidthOrHeight = (scaleFactor < 1.4f) ? 0f : ratio;
        }
    }
}