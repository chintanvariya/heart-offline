using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

namespace FGSOfflineCallBreak
{
    public class CallBreakDailyRewardManager : MonoBehaviour
    {
        public DailyRewardBoolsDetails dailyRewardBools;

        public List<CallBreakDailyRewardUI> allDailyRewardUIs;

        public List<int> sevenDaysCoinsReward;
        public List<int> sevenDaysKeysReward;

        public Sprite currentDayHud;
        public Sprite notACurrentDayHud;

        public int currentDay;

        public int claimStartDay, currentTodaysDate;

        public string ClaimStartDayKey;

        public Button claimButton;

        //private void Awake()
        //{
        //    //PlayerPrefs.DeleteKey("DailyRewardDetails");
        //    Debug.Log($"<color><b> DateTimeOffset.Now.ToUnixTimeMilliseconds {DateTimeOffset.Now.ToUnixTimeMilliseconds() }</b></color>");
        //    Debug.LogError($"<color><b> Date Now {UnixTimeStampToDateTime(DateTimeOffset.Now.ToUnixTimeMilliseconds()).ToString("hh:mm tt dd MMMM, yyyy")}</b></color>");
        //    Debug.LogError($"<color><b> Date Now {UnixTimeStampToDateTime(DateTimeOffset.Now.ToUnixTimeMilliseconds()).ToString("dd")}</b></color>");
        //    //currentTodaysDate = int.Parse(UnixTimeStampToDateTime(DateTimeOffset.Now.ToUnixTimeMilliseconds()).ToString("dd"));
        //    //PlayerPrefs.SetInt("ClaimStartDayKey", claimStartDay);
        //    //PlayerPrefs.SetInt("CurrentDay", currentDay);
        //    //Debug.LogError($"<color><b> Current Day {currentDay}</b></color>");
        //    OpenScreen();
        //}

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }

        public void ResetFlags()
        {
            Debug.Log($"<color><b> RESET </b></color>");

            PlayerPrefs.SetInt("CurrentDay", 0);

            dailyRewardBools.LockedOrNot = new List<bool>(new bool[7]);
            dailyRewardBools.ClaimOrNot = new List<bool>(new bool[7]);
            dailyRewardBools.LockedOrNot[0] = true;

            claimStartDay = int.Parse(UnixTimeStampToDateTime(DateTimeOffset.Now.ToUnixTimeMilliseconds()).ToString("dd"));

            PlayerPrefs.SetInt("ClaimStartDayKey", claimStartDay);

            CallBreakConstants.DailyRewardJsonString = CallBreakUtilities.ReturnJsonOfDailyRewardDetails(dailyRewardBools);
        }

        public void OpenScreen()
        {
            if (!CallBreakConstants.ItHasDailyRewardDataOrNot())
                ResetFlags();

            dailyRewardBools = CallBreakUtilities.ReturnDailyRewardDetails(CallBreakConstants.DailyRewardJsonString);

            currentTodaysDate = int.Parse(UnixTimeStampToDateTime(DateTimeOffset.Now.ToUnixTimeMilliseconds()).ToString("dd"));

            claimStartDay = PlayerPrefs.GetInt("ClaimStartDayKey");

            Debug.Log($"<color><b>{currentTodaysDate - currentDay <= claimStartDay}</b></color>");
            Debug.Log($"<color><b>claimStartDay {claimStartDay}</b></color>");
            Debug.Log($"<color><b>currentDay {currentDay}</b></color>");
            Debug.Log($"<color><b>currentTodaysDate {currentTodaysDate}</b></color>");

            if (currentTodaysDate - currentDay == claimStartDay)
            {
                Debug.Log($"<color=green><b> Today </b></color>");
                //claimButton.interactable = dailyRewardBools.LockedOrNot[currentDay];
            }
            else if (currentTodaysDate - currentDay > claimStartDay)
            {
                Debug.Log($"<color=green><b> Next Day </b></color>");
                ResetFlags();
            }
            else if (currentTodaysDate - currentDay < claimStartDay)
            {
                Debug.Log($"<color=green><b> Today </b></color>");
                //claimButton.interactable = false;
            }


            for (int i = 0; i < allDailyRewardUIs.Count; i++)
            {
                allDailyRewardUIs[i].UpdateTheValue(notACurrentDayHud, (i + 1), sevenDaysCoinsReward[i], sevenDaysKeysReward[i]);

                allDailyRewardUIs[i].UpdateAndHideClaimed(dailyRewardBools.ClaimOrNot[i]);
                allDailyRewardUIs[i].UpdateAndHideLocked(dailyRewardBools.LockedOrNot[i]);
            }

            allDailyRewardUIs[currentDay].UpdateDayHud(currentDayHud);

            gameObject.SetActive(true);
        }

        public void GetClaimed()
        {
            if (currentTodaysDate - currentDay == claimStartDay)
            {
                Debug.Log($"<color=green><b> Today </b></color>");
            }
            else if (currentTodaysDate - currentDay > claimStartDay)
            {
                Debug.Log($"<color=green><b> Next Day </b></color>");
            }
            else if (currentTodaysDate - currentDay < claimStartDay)
            {
                Debug.Log($"<color=green><b> Today </b></color>");
                return;
            }

            CallBreakGameManager.instance.selfUserDetails.userChips += sevenDaysCoinsReward[currentDay];
            CallBreakGameManager.instance.selfUserDetails.userKeys += sevenDaysKeysReward[currentDay];
            CallBreakConstants.UserDetialsJsonString = CallBreakUtilities.ReturnJsonString(CallBreakGameManager.instance.selfUserDetails);
            CallBreakUIManager.Instance.dashboardController.profileUiController.UpdateUserChips();
            CallBreakUIManager.Instance.dashboardController.profileUiController.UpdateUserKeys();

            dailyRewardBools.ClaimOrNot[currentDay] = true;

            for (int i = 0; i < dailyRewardBools.LockedOrNot.Count; i++)
                dailyRewardBools.LockedOrNot[i] = false;

            currentDay += 1;
            if (currentDay != 7)
                dailyRewardBools.LockedOrNot[currentDay] = true;
            else
                currentDay = 0;


            PlayerPrefs.SetInt("CurrentDay", currentDay);
            CallBreakConstants.DailyRewardJsonString = CallBreakUtilities.ReturnJsonOfDailyRewardDetails(dailyRewardBools);
            Debug.Log($"<color><b> CallBreakConstants.DailyRewardJsonString {CallBreakConstants.DailyRewardJsonString}</b></color>");

            for (int i = 0; i < allDailyRewardUIs.Count; i++)
            {
                allDailyRewardUIs[i].UpdateTheValue(notACurrentDayHud, (i + 1), sevenDaysCoinsReward[i], sevenDaysKeysReward[i]);

                allDailyRewardUIs[i].UpdateAndHideClaimed(dailyRewardBools.ClaimOrNot[i]);
                allDailyRewardUIs[i].UpdateAndHideLocked(dailyRewardBools.LockedOrNot[i]);
            }
        }

        public void CloseScreen() => gameObject.SetActive(false);
    }

    [System.Serializable]
    public class DailyRewardBoolsDetails
    {
        public List<bool> LockedOrNot = new List<bool>();
        public List<bool> ClaimOrNot = new List<bool>();
    }
}