using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace FGSOfflineCallBreak
{
    public class CallBreakUtilities
    {
        public static int ReturnMyCardValue(string cardTypeValue)
        {
            return int.Parse(cardTypeValue);
        }

        public static CardType ReturnMyCardType(string cardTypeName)
        {
            if (cardTypeName == "H")
                return CardType.Heart;
            else if (cardTypeName == "D")
                return CardType.Diamond;
            else if (cardTypeName == "C")
                return CardType.Club;
            else
                return CardType.Spade;
        }

        public static string AbbreviateNumber(float number)
        {
            string[] suffixes = { "", "k", "M", "B", "T" };
            int suffixIndex = 0;
            while (number >= 1000f && suffixIndex < suffixes.Length - 1)
            {
                number /= 1000f;
                suffixIndex++;
            }
            return number.ToString("0.#") + suffixes[suffixIndex];
        }

        public static int ReturnCurrentLevel(float levelProgress)
        {
            int currentLevel = 1;

            Debug.LogError($"{levelProgress}  => {CallBreakConstants.coinsToClearLevel[0]} => {CallBreakConstants.coinsToClearLevel[1]}");
            if (levelProgress > 0 && levelProgress < CallBreakConstants.coinsToClearLevel[0])
            {
                Debug.LogError($"{1}");
                currentLevel = 1;
            }
            else if (levelProgress > CallBreakConstants.coinsToClearLevel[0] && levelProgress < CallBreakConstants.coinsToClearLevel[1])
            {
                Debug.LogError($"{2}");
                currentLevel = 2;
            }
            else if (levelProgress > CallBreakConstants.coinsToClearLevel[1] && levelProgress < CallBreakConstants.coinsToClearLevel[2])
            {
                Debug.LogError($"{3}");
                currentLevel = 3;
            }
            else if (levelProgress > CallBreakConstants.coinsToClearLevel[2] && levelProgress < CallBreakConstants.coinsToClearLevel[3])
            {
                Debug.LogError($"{4}");
                currentLevel = 4;
            }
            return currentLevel;
        }

        public static string ReturnJsonString(UserDetails userDetails)
        {
            return JsonConvert.SerializeObject(userDetails);
        }

        public static UserDetails ReturnUserDetails(string userDetails)
        {
            return JsonConvert.DeserializeObject<UserDetails>(userDetails);
        }

        public static string ReturnJsonOfAvatarPurchaseDetails(AvatarPurchaseDetails avatarPurchaseDetails)
        {
            return JsonConvert.SerializeObject(avatarPurchaseDetails);
        }

        public static AvatarPurchaseDetails ReturnAvatarDetails(string userDetails)
        {
            return JsonConvert.DeserializeObject<AvatarPurchaseDetails>(userDetails);
        }

        public static string ReturnJsonOfDailyRewardDetails(DailyRewardBoolsDetails dailyRewardBools)
        {
            return JsonConvert.SerializeObject(dailyRewardBools);
        }

        public static DailyRewardBoolsDetails ReturnDailyRewardDetails(string dailyRewardBoolDetails)
        {
            return JsonConvert.DeserializeObject<DailyRewardBoolsDetails>(dailyRewardBoolDetails);
        }

    }
}