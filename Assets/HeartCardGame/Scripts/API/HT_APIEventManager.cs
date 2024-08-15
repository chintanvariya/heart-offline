using UnityEngine;
using Newtonsoft.Json;

namespace HeartCardGame
{
    public class HT_APIEventManager
    {
        public static string UserRegister(string userName)
        {
            UserRegisterData data = new UserRegisterData();

            data.deviceId = SystemInfo.deviceUniqueIdentifier;
            data.deviceType = SystemInfo.deviceType.ToString();
            data.token = PlayerPrefs.HasKey("Token") ? PlayerPrefs.GetString("Token") : "";
            data.userName = userName;

            return JsonConvert.SerializeObject(data);
        }

        public static string ProfileEdit(string userName)
        {
            ProfileEditData profileEditData = new ProfileEditData();
            profileEditData.userName = userName;

            return JsonConvert.SerializeObject(profileEditData);
        }

        public static string GetLobbys()
        {
            GetLobbyData data = new GetLobbyData();

            data.isCash = false;
            data.maxPlayer = 4;

            return JsonConvert.SerializeObject(data);
        }

        public static string AvatarSet(string avatarId)
        {
            AvatarBuyOrSet data = new AvatarBuyOrSet();

            data.avatarId = avatarId;

            return JsonConvert.SerializeObject(data);
        }

        public static string AddCoins(string id)
        {
            PurchaseData data = new PurchaseData();

            data.storeId = id;

            return JsonConvert.SerializeObject(data);
        }

        public static string ClaimDailyWheelBonus(int day, int index)
        {
            ClaimDailyWheelBonus data = new ClaimDailyWheelBonus();

            data.day = day;
            data.index = index;

            return JsonConvert.SerializeObject(data);
        }
    }
}