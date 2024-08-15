using System;
using UnityEngine;
using UnityEngine.UI;

namespace HeartCardGame
{
    public class HT_SettingPanelController : MonoBehaviour
    {
        [Header("===== Object Scripts =====")]
        [SerializeField] private HT_UiManager uiManager;
        [SerializeField] private HT_AudioManager audioManager;
        [SerializeField] private HT_DashboardManager dashboardManager;

        [Header("===== Sound Data =====")]
        [SerializeField] private Button soundBtn;
        [SerializeField] private Image soundBtnImage;
        [SerializeField] private Sprite soundBtnOnSprite, soundBtnOffSprite;

        [Header("===== Music Data =====")]
        [SerializeField] private Button musicBtn;
        [SerializeField] private Image musicBtnImage;
        [SerializeField] private Sprite musicBtnOnSprite, musicBtnOffSprite;

        [Header("===== Vibrate Data =====")]
        [SerializeField] private Button vibrateBtn;
        [SerializeField] private Image vibrateBtnImage;
        [SerializeField] private Sprite vibrateOnSprite, vibrateOffSprite;

        private Action SettingPanelVerify;

        private void Start()
        {
            Debug.Log($"Sound {sound}");
            Debug.Log($"vibrate {vibrate}");
            Debug.Log($"music {music}");
        }

        private void OnEnable()
        {
            SettingPanelVerify += CheckPlayerPrefsSound;
            SettingPanelVerify += CheckPlayerPrefsMusic;
            SettingPanelVerify += CheckPlayerPrefsVibration;

            SettingPanelVerify?.Invoke();
        }

        private void OnDisable()
        {
            SettingPanelVerify -= CheckPlayerPrefsSound;
            SettingPanelVerify -= CheckPlayerPrefsMusic;
            SettingPanelVerify -= CheckPlayerPrefsVibration;
        }

        public bool sound
        {
            get { return (PlayerPrefs.GetString("Sound") == "True"); }
            set { PlayerPrefs.SetString("Sound", value.ToString()); }
        }

        public bool vibrate
        {
            get { return (PlayerPrefs.GetString("Vibrate") == "True"); }
            set { PlayerPrefs.SetString("Vibrate", value.ToString()); }
        }

        public bool music
        {
            get { return (PlayerPrefs.GetString("Music") == "True"); }
            set { PlayerPrefs.SetString("Music", value.ToString()); }
        }

        public void CheckPlayerPrefsSound()
        {
            if (PlayerPrefs.HasKey("Sound"))
                SetValueForSound(sound, sound ? soundBtnOnSprite : soundBtnOffSprite);
            else
                SetValueForSound(true, soundBtnOnSprite);
        }

        public void CheckPlayerPrefsVibration()
        {
            if (PlayerPrefs.HasKey("Vibrate"))
                SetValueForVibration(vibrate, vibrate ? vibrateOnSprite : vibrateOffSprite);
            else
                SetValueForVibration(true, vibrateOnSprite);
        }

        public void CheckPlayerPrefsMusic()
        {
            if (PlayerPrefs.HasKey("Music"))
            {
                Debug.Log("[" + DateTime.Now.ToString("hh:mm:ss fff") + "] CheckPlayerPrefsMusic || True");
                SetValueForMusic(music, music ? musicBtnOnSprite : musicBtnOffSprite);
            }
            else
            {
                Debug.Log("[" + DateTime.Now.ToString("hh:mm:ss fff") + "] CheckPlayerPrefsMusic || False");
                SetValueForMusic(true, musicBtnOnSprite);
            }
        }

        public void SetValueForSound(bool onOffText, Sprite onOffIconSprite)
        {
            soundBtnImage.sprite = onOffIconSprite;
            sound = onOffText;
        }

        public void SetValueForVibration(bool onOffText, Sprite onOffIconSprite)
        {
            vibrateBtnImage.sprite = onOffIconSprite;
            vibrate = onOffText;
        }

        public void SetValueForMusic(bool onOffText, Sprite onOffIconSprite)
        {
            Debug.Log("[" + DateTime.Now.ToString("hh:mm:ss fff") + "] SetValueForMusic : " + onOffText);
            musicBtnImage.sprite = onOffIconSprite;
            music = onOffText;
            if (uiManager.gamePlayPanel.activeInHierarchy)
                audioManager.BackgroundMusicOnOff();
        }

        public void OnClickSoundButton() => SetValueForSound(!sound, sound ? soundBtnOffSprite : soundBtnOnSprite);

        public void OnClickVibrateButton() => SetValueForVibration(!vibrate, vibrate ? vibrateOffSprite : vibrateOnSprite);

        public void OnClickMusicButton() => SetValueForMusic(!music, music ? musicBtnOffSprite : musicBtnOnSprite);

        public void ClickOnHowToPlayBtn()
        {
            dashboardManager.PanelOnOff(dashboardManager.howToPlayPanel, true);
            uiManager.OtherPanelOpen(uiManager.settingPanel, false);
        }
    }
}