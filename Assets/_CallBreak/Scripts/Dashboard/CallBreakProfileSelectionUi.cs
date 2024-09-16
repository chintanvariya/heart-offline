using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FGSOfflineCallBreak
{
    public class CallBreakProfileSelectionUi : MonoBehaviour
    {
        [Header("Button Label")]
        public TMPro.TextMeshProUGUI buttonLabel;
        public Image profilePicture;

        [Header("AvatarDetails")]
        public AvatarDetails avatarDetails;

        [Header("GameObject")]
        public GameObject coinImage;
        public GameObject buttonObject;

        [Header("Avatar Value And Index")]
        public float avatarValue;
        public int indexOfAvatar;

        public RectTransform parent;

        [Header("ProfileSelection Controller")]
        public CallBreakProfileSelectionController profileSelectionController;

        public void OnButtonClicked()
        {

            switch (avatarDetails)
            {
                case AvatarDetails.Free:
                    profileSelectionController.FreeAvatarSelected(this);
                    break;
                case AvatarDetails.Coins:
                    profileSelectionController.PurchaseAvatarSelected(this);
                    break;
                case AvatarDetails.Video:
                    profileSelectionController.AdsSelectedProfile(this);
                    break;
                default:
                    break;
            }

        }
    }
}