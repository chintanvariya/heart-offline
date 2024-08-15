using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HeartCardGame
{
    public class HT_ProfilePicHandler : MonoBehaviour
    {
        [Header("===== Profile Pic Data =====")]
        [SerializeField] private Image profileImg;
        [SerializeField] private GameObject loader;
        [SerializeField] private TextMeshProUGUI btnTxt;
        public GameObject lockObj, selectedObj;
        public Button avatarBtn;

        Coroutine profileCoroutine;

        public void SetProfileImage(string profilePic, bool isLock, bool isCanBuy, bool isFree, bool isUsed, int coins)
        {
            if (isFree)
                btnTxt.SetText($"Free");

            if (isLock)
            {
                lockObj.SetActive(true);
                btnTxt.SetText($"<sprite=0>  {coins}");
            }

            if (!isLock && !isFree)
                btnTxt.SetText($"Purchased");

            selectedObj.SetActive(isUsed);

            avatarBtn.interactable = isCanBuy || !isLock;
            profileCoroutine = StartCoroutine(HT_GameManager.instance.uiManager.GetTexture(profilePic, loader, (sprite) =>
            {
                profileImg.sprite = sprite;
                if (profileCoroutine != null)
                    StopCoroutine(profileCoroutine);
            }));
        }

        public void SetProfileAfterPurchase()
        {
            lockObj.SetActive(false);
            btnTxt.SetText($"Purchased");
        }
    }
}