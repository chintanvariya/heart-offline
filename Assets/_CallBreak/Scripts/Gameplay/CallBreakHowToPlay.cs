using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace FGSOfflineCallBreak
{
    public class CallBreakHowToPlay : MonoBehaviour
    {
        public List<GameObject> howToPlaySprites;

        public GameObject welcomePanel, backGround;

        public GameObject BackBtn;

        private int imageCounter;

        public static string howToPlay01 = "";

        public string temp;
        public void Speak()
        {
            for (int i = 0; i < howToPlay01.Length; i++)
            {

            }
        }

        //public void OnEnable()
        //{
        //    welcomePanel.SetActive(false);
        //    imageCounter = -1;
        //    NextSlideImage();
        //}

        public void DisebleAllPanel()
        {
            foreach (var item in howToPlaySprites)
            {
                item.SetActive(false);
            }
        }

        public void OnButtonClicked(string buttonName)
        {
            switch (buttonName)
            {
                case "RePlay":
                    OpenScreen();
                    break;
                case "PlayNow":
                    gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }


        public void OpenScreen()
        {
            backGround.SetActive(true);
            welcomePanel.SetActive(false);
            imageCounter = -1;
            NextSlideImage();
            gameObject.SetActive(true);
        }



        public void NextSlideImage()
        {
            if (imageCounter == howToPlaySprites.Count - 1)
            {
                welcomePanel.SetActive(true);
                return;
            }

            DisebleAllPanel();

            CallBreakSoundManager.PlaySoundEvent(SoundEffects.Click);
            howToPlaySprites[++imageCounter].SetActive(true);

            if (imageCounter > 0)
            {
                BackBtn.SetActive(true);
            }
            else
            {
                BackBtn.SetActive(false);
            }

        }

        public void BackSlideImage()
        {
            CallBreakSoundManager.PlaySoundEvent(SoundEffects.Click);

            DisebleAllPanel();
            howToPlaySprites[--imageCounter].SetActive(true);

            if (imageCounter == 0)
            {
                BackBtn.SetActive(false);
            }
            else
            {
                BackBtn.SetActive(true);
            }
        }



    }
}
