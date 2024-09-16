using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;
namespace FGSOfflineCallBreak
{
    public class RewardCoinAnimation : MonoBehaviour
    {
        public static Action<Transform, int> CoinAnimationEvent;

        public GameObject coinPrefab;
        public Transform destinationPoint;

        public List<GameObject> coinSpawnObject = new List<GameObject>();

        private void OnEnable()
        {
            CoinAnimationEvent += CoinAnimationStart;
        }

        private void OnDisable()
        {
            CoinAnimationEvent -= CoinAnimationStart;
        }

        public void CoinAnimationStart(Transform coinSpawnParent, int coinsCount)
        {
            coinSpawnObject.Clear();
            StartCoroutine(SpawnCoin(coinSpawnParent, coinsCount));
        }

        public IEnumerator SpawnCoin(Transform coinSpawnParent, int coinsCount)
        {
            for (int i = 0; i < 20; i++)
            {
                GameObject coin = Instantiate(coinPrefab);

                float x = coinSpawnParent.transform.position.x + UnityEngine.Random.Range(20, -20);
                float y = coinSpawnParent.transform.position.y + UnityEngine.Random.Range(-70, -100);

                coin.transform.position = new Vector2(x, y);
                coin.transform.SetParent(coinSpawnParent);
                coinSpawnObject.Add(coin);
            }

            for (int i = 0; i < coinSpawnObject.Count; i++)
            {
                coinSpawnObject[i].transform.DOMove(destinationPoint.transform.position, .5f).SetEase(Ease.Linear);
                yield return new WaitForSeconds(0.03f);
            }

            yield return new WaitForSeconds(0.8f);

            //Update the Coin TEXT

            foreach (var item in coinSpawnObject)
            {
                Destroy(item);
            }
        }

    }
}
