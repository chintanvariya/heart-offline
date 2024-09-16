using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace FGSOfflineCallBreak
{
    public class CallBreakSoundManager : MonoBehaviour
    {
        public static System.Action<SoundEffects> PlaySoundEvent;
        public static System.Action PlayVibrationEvent;

        public AudioClip[] audioClipsOfGameplay;

        [Space(10)]
        // public AudioSource bgSoundSource;
        public AudioSource soundSource;
        [Space(5)]

        public Image soundImage;
        //public Image musicImage;
        public Image vibrationImage;
        public Sprite onBtnSprite, offBtnSprite;

        private void OnEnable()
        {
            PlaySoundEvent += PlaySoundEffect;
            PlayVibrationEvent += PlayVibration;
        }
        private void OnDisable()
        {
            PlaySoundEvent -= PlaySoundEffect;
            PlayVibrationEvent -= PlayVibration;
        }
        private void Start() => ChangeSprite();

        public void PlaySoundEffect(SoundEffects soundEffects)
        {
            soundSource.PlayOneShot(audioClipsOfGameplay[(int)soundEffects]);
        }

        public void SoundBtnClick()
        {
            BtnClickSound();

            CallBreakConstants.IsSound = !CallBreakConstants.IsSound;
            if (CallBreakConstants.IsSound)
            {
                soundImage.sprite = onBtnSprite;
                soundSource.mute = false;
            }
            else
            {
                soundImage.sprite = offBtnSprite;
                soundSource.mute = true;
            }

            ChangeSprite();
        }

        public void PlayVibration()
        {
#if !UNITY_WEBGL
            if (CallBreakConstants.IsVibration)
            {
                Handheld.Vibrate();
            }
#endif
        }

        public void MusicBtnClick()
        {
            BtnClickSound();
            CallBreakConstants.IsMusic = !CallBreakConstants.IsMusic;
            
            //if (CallBreakConstants.IsMusic)
            //    musicImage.sprite = onBtnSprite;
            //else
            //    musicImage.sprite = offBtnSprite;

            ChangeSprite();
        }

        public void VibrationBtnClick()
        {
            BtnClickSound();

            CallBreakConstants.IsVibration = !CallBreakConstants.IsVibration;

            if (CallBreakConstants.IsVibration)
                vibrationImage.sprite = onBtnSprite;
            else
                vibrationImage.sprite = offBtnSprite;

            ChangeSprite();
        }

        internal void ChangeSprite()
        {
            if (CallBreakConstants.IsSound)
            {
                soundImage.sprite = onBtnSprite;
                soundSource.mute = false;
            }
            else
            {
                soundImage.sprite = offBtnSprite;
                soundSource.mute = true;
            }

            //if (CallBreakConstants.IsMusic)
            //    musicImage.sprite = onBtnSprite;
            //else
            //    musicImage.sprite = offBtnSprite;

            if (CallBreakConstants.IsVibration)
                vibrationImage.sprite = onBtnSprite;
            else
                vibrationImage.sprite = offBtnSprite;
        }

        public void BtnClickSound()
        {
            PlaySoundEvent(SoundEffects.Click);
        }
    }

    public enum SoundEffects
    {
        Deal, ThrowCard, Click, YourTurn, Win, Lose
    }
}
