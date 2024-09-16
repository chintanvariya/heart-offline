using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FGSOfflineCallBreak
{
    public class CallBreakRulesController : MonoBehaviour
    {
        public List<GameObject> rulesGameObject;

        public int imageCounter;

        public GameObject nextBtn;
        public GameObject backBtn;

        public void OpenScreen()
        {
            imageCounter = -1;
            Debug.Log(" ===  " + imageCounter);

            nextBtn.SetActive(true);
            backBtn.SetActive(false);
            gameObject.SetActive(true);
            NextSlideImage();
        }

        public void CloseScreen()
        {
            gameObject.SetActive(false);
        }

        public void NextSlideImage()
        {
            imageCounter++;
            if (imageCounter == rulesGameObject.Count)
            {
                nextBtn.SetActive(false);
                return;
            }

            CallBreakSoundManager.PlaySoundEvent(SoundEffects.Click);

            foreach (var item in rulesGameObject)
                item.SetActive(false);

            rulesGameObject[imageCounter].SetActive(true);

            if (imageCounter == 0)
                backBtn.SetActive(false);

            if (imageCounter > 0)
                backBtn.SetActive(true);

            if (imageCounter == rulesGameObject.Count - 1)
                nextBtn.SetActive(false);
        }

        public void BackSlideImage()
        {
            imageCounter--;
            CallBreakSoundManager.PlaySoundEvent(SoundEffects.Click);

            foreach (var item in rulesGameObject)
                item.SetActive(false);
            rulesGameObject[imageCounter].SetActive(true);


            if (imageCounter == 0)
            {
                backBtn.SetActive(false);
            }
            else
            {
                backBtn.SetActive(true);
            }

            if (imageCounter > 0)
            {
                nextBtn.SetActive(true);
            }
        }

    }

}
