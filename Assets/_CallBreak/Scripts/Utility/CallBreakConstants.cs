using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FGSOfflineCallBreak.CallBreakRemoteConfigClass;

public static class CallBreakConstants
{
    public const string TurnMsgDes = "It's not your turn yet , please wait.";
    public const string CardMsgDes = "You should play     follow the first player";
    public const string StandardRoundInfoDes = "Enjoy a standard 5-round game.";
    public const string QuickRoundInfoDes = "Enjoy a quick 3-round game.";

    public const string ExitFromGamePlayMessage = "Are you sure to quit this round ? Your coin will be lost!";
    public const string ExitFromDashboardPlayMessage = "Are you sure to quit ?";

    public static List<int> coinsToClearLevel = new List<int> { 1000, 5000, 15000, 27500, 40000, 50000, 62500, 75000, 87500, 100000 };
    public static List<int> allLobbyAmount = new List<int> { 0, 10, 20, 30, 40, 50, 100, 200, 300, };

    private const string UserDetialsJsonStringKey = "UserDetialsJsonString";
    private const string AvatarPurchaseDetailsKey = "AvatarPurchaseDetails";
    private const string DailyRewardDetailsKey = "DailyRewardDetails";

    public static CallBreakRemoteConfig callBreakRemoteConfig;

    public static bool RegisterOrNot()
    {
        return PlayerPrefs.HasKey(UserDetialsJsonStringKey);
    }

    public static string UserDetialsJsonString
    {
        get => PlayerPrefs.GetString(UserDetialsJsonStringKey);
        set => PlayerPrefs.SetString(UserDetialsJsonStringKey, value);
    }

    public static bool ItHasPurchaseDataOrNot()
    {
        return PlayerPrefs.HasKey(AvatarPurchaseDetailsKey);
    }

    public static string AvatarPurchaseJsonString
    {
        get => PlayerPrefs.GetString(AvatarPurchaseDetailsKey);
        set => PlayerPrefs.SetString(AvatarPurchaseDetailsKey, value);
    }

    public static bool ItHasDailyRewardDataOrNot()
    {
        return PlayerPrefs.HasKey(DailyRewardDetailsKey);
    }

    public static string DailyRewardJsonString
    {
        get => PlayerPrefs.GetString(DailyRewardDetailsKey);
        set => PlayerPrefs.SetString(DailyRewardDetailsKey, value);
    }

    public static bool IsSound
    {
        get => PlayerPrefs.GetInt("SOUND", 1) == 1 ? true : false;
        set => PlayerPrefs.SetInt("SOUND", value == true ? 1 : 0);
    }

    public static bool IsMusic
    {
        get => PlayerPrefs.GetInt("MUSIC", 1) == 1 ? true : false;
        set => PlayerPrefs.SetInt("MUSIC", value == true ? 1 : 0);
    }

    public static bool IsVibration
    {
        get => PlayerPrefs.GetInt("VIBRATION", 1) == 1 ? true : false;
        set => PlayerPrefs.SetInt("VIBRATION", value == true ? 1 : 0);
    }


    internal static List<string> BotNames = new List<string> {
    "Raj",
    "Priya",
    "Rahul",
    "Aisha",
    "Arjun",
    "Neha",
    "Vikram",
    "Anjali",
    "Sanjay",
    "Pooja",
    "Karthik",
    "Deepika",
    "Amit",
    "Nisha",
    "Sunil",
    "Ritu",
    "Rohan",
    "Meera",
    "Akash",
    "Swati",
    "Ajay",
    "Shweta",
    "Vishal",
    "Kavita",
    "Manoj",
    "James",
    "Emily",
    "William",
    "Sophie",
    "Alexander",
    "Charlotte",
    "Thomas",
    "Olivia",
    "Daniel",
    "Amelia",
    "Jack",
    "Jessica",
    "Harry",
    "Grace",
    "George",
    "Chloe",
    "Edward",
    "Ella",
    "Michael",
    "Mia",
    "David",
    "Lily",
    "Matthew",
    "Isabella",
    "John",
    "John",
    "Emily",
    "Michael",
    "Jessica",
    "William",
    "Ashley",
    "David",
    "Sarah",
    "James",
    "Amanda",
    "Robert",
    "Jennifer",
    "Joseph",
    "Brittany",
    "Daniel",
    "Megan",
    "Christopher",
    "Lauren",
    "Matthew",
    "Samantha",
    "Andrew",
    "Nicole",
    "Ryan",
    "Elizabeth",
    "Brandon",
    "Ivan",
    "Svetlana",
    "Dmitry",
    "Natalia",
    "Sergei",
    "Yelena",
    "Alexei",
    "Olga",
    "Vladimir",
    "Tatiana",
    "Andrei",
    "Maria",
    "Nikolai",
    "Anna",
    "Mikhail",
    "Elena",
    "Alexander",
    "Irina",
    "Pavel",
    "Yulia",
    "Oleg",
    "Marina",
    "Anatoly",
    "Galina",
    "Yuri"
};
}