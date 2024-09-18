using UnityEngine;

namespace FGSOfflineHeart
{
    public class HT_CollectBootHandler : MonoBehaviour
    {
        [Header("===== Boot Object Data =====")]
        public RectTransform rectTransform;
        public UnityEngine.UI.Image bootImg;
        [SerializeField] public TMPro.TextMeshProUGUI bootTxt;

        public void SetPosition(Vector3 cardPos)
        {
            rectTransform.anchoredPosition = cardPos;            
        }

        public void SetBootValue(int entreeFee)
        {
            bootTxt.SetText(entreeFee.ToString());
        }
    }
}