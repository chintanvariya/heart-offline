using UnityEngine;

namespace HeartCardGame
{
    public class HT_AudioManager : MonoBehaviour
    {
        [Header("===== Audio Source =====")]
        public AudioSource backgroundAudioSource, gameAudioSource;

        [Header("===== Audio Clips =====")]
        public AudioClip cardDistributionClip;
        public AudioClip userTurnClip, collectBootClip, cardPassClip, winClip, lossClip, spinClip, spinResultCLip, heartBrokenClip, spadeQClip;

        [Header("===== Object Scripts =====")]
        [SerializeField] private HT_SettingPanelController settingPanelController;

        public void BackgroundMusicOnOff() => backgroundAudioSource.mute = !settingPanelController.music;

        public void GamePlayAudioSetting(AudioClip audioClip)
        {
            if (settingPanelController.sound)
            {
                gameAudioSource.clip = audioClip;
                gameAudioSource.Play();
            }
        }
    }
}