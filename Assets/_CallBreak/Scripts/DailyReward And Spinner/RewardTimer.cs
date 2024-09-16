using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace FGSOfflineCallBreak
{
    public class RewardTimer : MonoBehaviour
    {
        [SerializeField] private Button uiSpinButton;
        [SerializeField] private TextMeshProUGUI uiSpinButtonText;

        internal static ulong lastTimeClicked;
        public float msToWait;
        public float msToWaitfor4Hr;

        public GameObject adTextObj, spinTextObj;

        private void OnEnable()
        {

        }
        private void Start()
        {
            if (PlayerPrefs.HasKey("LastTimeClicked"))
                lastTimeClicked = ulong.Parse(PlayerPrefs.GetString("LastTimeClicked"));

            if (!Ready())
            {
                uiSpinButton.interactable = false;
            }

            uiSpinButton.onClick.AddListener(() =>
            {
                uiSpinButton.interactable = false;
                if (adTextObj.activeInHierarchy)
                {
                    //AdmobManager.rewardState = RewardState.FreeSpin;
                    //AdmobManager.instance.ShowRewardedAd();
                }
                else
                {
                    lastTimeClicked = (ulong)DateTime.Now.Ticks;
                    PlayerPrefs.SetString("LastTimeClicked", lastTimeClicked.ToString());
                }


            });

        }

        private void Update()
        {
            if (!uiSpinButton.IsInteractable())
            {
                if (true)
                {
                    return;
                }
                //else if (Ready())
                //{
                //    spinTextObj.SetActive(true);
                //    adTextObj.SetActive(false);
                //    uiSpinButton.interactable = true;
                //    //Time.transform.parent.gameObject.SetActive(false);
                //    return;
                //}
                //else
                //{
                //    adTextObj.SetActive(true);
                //    spinTextObj.SetActive(false);

                //    //if (AdmobManager.isRewardLoad)
                //    //{
                //    //    uiSpinButton.interactable = true;
                //    //}
                //}

                //ulong diff = ((ulong)DateTime.Now.Ticks - lastTimeClicked);
                //ulong m = diff / TimeSpan.TicksPerMillisecond;
                //float secondsLeft = (float)(msToWait - m) / 1000.0f;

                //string r = "";
                ////HOURS
                //r += ((int)secondsLeft / 3600).ToString() + " : ";
                //secondsLeft -= ((int)secondsLeft / 3600) * 3600;
                ////MINUTES
                //r += ((int)secondsLeft / 60).ToString("00") + " : ";
                ////SECONDS
                //r += (secondsLeft % 60).ToString("00");
                ////  Time.text = r;
                //Debug.LogError($"{r}");
                //// Time.transform.parent.gameObject.SetActive(true);
            }
        }

        private bool Ready()
        {
            ulong diff = ((ulong)DateTime.Now.Ticks - lastTimeClicked);
            ulong m = diff / TimeSpan.TicksPerMillisecond;

            float secondsLeft = (float)(msToWait - m) / 1000.0f;

            if (secondsLeft < 0)
            {
                //DO SOMETHING WHEN TIMER IS FINISHED
                return true;
            }

            return false;
        }

    }
}
