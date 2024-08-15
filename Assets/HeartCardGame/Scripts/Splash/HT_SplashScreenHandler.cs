using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class HT_SplashScreenHandler : MonoBehaviour
{
    [SerializeField] private Image loadingFillImg;
    [SerializeField] private float loadingTime;


    private void Start()
    {
        loadingFillImg.fillAmount = 0;
        StartCoroutine(LoaderFill());
    }

    private IEnumerator LoaderFill()
    {
        int totalRandom = 0;

        for (int i = 0; i < (int)loadingTime; i++)
        {
            Debug.Log("int i index || " + i + "  loading time || " + loadingTime);

            if (i == (int)loadingTime - 1) totalRandom = 100;
            else
            {
                int refere = (int)(100 / loadingTime);
                totalRandom += Random.Range(refere / 2, refere + (i > 0 ? 0 : refere / 2));
            }

            yield return new WaitForSeconds(1);
            var loadAnim = loadingFillImg.DOFillAmount((float)totalRandom / 100, 0.5f).SetEase(Ease.InOutCirc);

            loadAnim.OnUpdate(() =>
            {
                float progress = loadingFillImg.fillAmount * 100;
                //percentageText.text = $"{progress:F0}%";
            });

            if (totalRandom == 100)
            {
                yield return new WaitForSeconds(0.5f);

                Debug.Log("isSplashLoadSuccess");
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                //SplashCanvasMakeOff();
            }
        }
    }
}
