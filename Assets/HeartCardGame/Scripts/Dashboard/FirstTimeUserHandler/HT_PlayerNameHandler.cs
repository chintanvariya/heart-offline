using UnityEngine;
using TMPro;

namespace HeartCardGame
{
    public class HT_PlayerNameHandler : MonoBehaviour
    {
        [Header("===== Object Scripts =====")]
        [SerializeField] private HT_UserRegistration userRegistration;
        [SerializeField] private HT_ProfileHandler profileHandler;
        [SerializeField] private TextMeshProUGUI warningTxt;

        [Header("===== Player Name Data Handler =====")]
        [SerializeField] TMP_InputField nameInputField;

        public void UserRegister()
        {
            if (profileHandler.IsUserNameValid(nameInputField.text, warningTxt))
            {
                PlayerPrefs.SetString("UserName", nameInputField.text);
                warningTxt.SetText($"");
                userRegistration.UserRegister(nameInputField.text);
            }
        }
    }
}