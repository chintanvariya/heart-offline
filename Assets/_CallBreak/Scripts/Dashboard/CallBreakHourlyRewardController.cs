using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGSOfflineCallBreak
{
    public class CallBreakHourlyRewardController : MonoBehaviour
    {
        public string saveDataWithKey;

        public float miliSecondsToWait;//14400000

        public ulong lastTimeClicked;

        public UnityEngine.UI.Button rewaredButton;

        public TMPro.TextMeshProUGUI timerText;
        public GameObject timerHud;

        private void Start()
        {
            if (PlayerPrefs.HasKey(saveDataWithKey))
                lastTimeClicked = ulong.Parse(PlayerPrefs.GetString(saveDataWithKey));

            if (rewaredButton != null)
                rewaredButton.interactable = Ready();
        }

        public void OnButtonClicked()
        {
            if (Ready())
            {
                lastTimeClicked = (ulong)DateTime.Now.Ticks;
                PlayerPrefs.SetString(saveDataWithKey, lastTimeClicked.ToString());
            }
            else
                Debug.Log("");
        }

        private void Update()
        {
            if (Ready())
            {
                if (rewaredButton != null)
                    rewaredButton.interactable = Ready();
                return;
            }
            else
            {
                timerHud.SetActive(true);
            }

            ulong diff = ((ulong)DateTime.Now.Ticks - lastTimeClicked);
            ulong m = diff / TimeSpan.TicksPerMillisecond;
            float secondsLeft = (float)(miliSecondsToWait - m) / 1000.0f;

            string timerString = "";

            //HOURS
            timerString += ((int)secondsLeft / 3600).ToString() + " : ";

            secondsLeft -= ((int)secondsLeft / 3600) * 3600;
            //MINUTES
            timerString += ((int)secondsLeft / 60).ToString("00") + " : ";


            //SECONDS
            timerString += (secondsLeft % 60).ToString("00");

            //  Time.text = r;
            //Debug.Log($"TIMER LEFT  {timerString}");
            timerText.text = timerString;
            // Time.transform.parent.gameObject.SetActive(true);
        }

        public bool Ready()
        {
            ulong diff = ((ulong)DateTime.Now.Ticks - lastTimeClicked);
            ulong m = diff / TimeSpan.TicksPerMillisecond;

            float secondsLeft = (float)(miliSecondsToWait - m) / 1000.0f;

            if (secondsLeft < 0)
            {
                //DO SOMETHING WHEN TIMER IS FINISHED
                timerHud.SetActive(false);
                return true;
            }

            return false;
        }
    }
}
