using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json;
using System.Linq;
using System.Collections;

namespace FGSOfflineHeart
{
    public class HT_ShootingAnimation : MonoBehaviour
    {
        [Header("===== Object Script =====")]
        [SerializeField] private HT_EventManager eventManager;
        [SerializeField] private HT_JoinTableHandler joinTableHandler;
        [SerializeField] private HT_UiManager uiManager;
        [SerializeField] private HT_GameManager gameManager;

        [Header("===== Animation Object =====")]
        [SerializeField] private Image rocketImg;
        [SerializeField] private RectTransform rocketRectTransform;
        int spriteCount = 0;

        //[Header("===== Rocket Sprites =====")]
        //[SerializeField] private List<Sprite> rocketSprites;

        [Header("===== Model Class =====")]
        [SerializeField] private ShootingResponse shootingResponse;

        private void OnEnable() => eventManager.RegisterEvent(SocketEvents.SHOOTING_MOON, ShootingMoonAnimation);

        private void OnDisable() => eventManager.UnregisterEvent(SocketEvents.SHOOTING_MOON, ShootingMoonAnimation);

        //private void Start() => ShootingMoonAnimation();

        private void ShootingMoonAnimation(string data)
        {
            shootingResponse = JsonConvert.DeserializeObject<ShootingResponse>(data);
            ShootingMoonAnimation();
        }

        public void ShootingMoonAnimation()
        {
            uiManager.shootingMoonPanel.SetActive(true);
            InvokeRepeating(nameof(SpriteAnimation), 0f, 0.3f);
            rocketRectTransform.anchoredPosition = new Vector2(0, -250f);

            rocketRectTransform.DOShakePosition(1.5f, 20, 50, 200).OnComplete(() =>
            {
                CancelInvoke(nameof(SpriteAnimation));
                rocketRectTransform.DOLocalMove(new Vector3(0, 1400, 0), 3f).OnComplete(() =>
                {
                    if (gameManager.isOffline)
                        HeartSpadeSpreadAnimation("", joinTableHandler.playerData.Find(x => x.roundHeartPoint + x.roundSpadePoint == 26).mySeatIndex);
                    else
                        HeartSpadeSpreadAnimation(shootingResponse.data.Find(x => x.isShootingMoon).userId);
                    //HeartSpadeSpreadAnimation("");
                });
            });
            //DOVirtual.DelayedCall(2.7f, () =>
            //{
            //});
        }

        void HeartSpadeSpreadAnimation(string userId = "", int seatIndex = 0)
        {
            List<HT_PlayerController> playerControllers = new();
            HT_PlayerController player = null;
            if (gameManager.isOffline)
            {
                playerControllers = joinTableHandler.playerData.Where((player) => player.mySeatIndex != seatIndex).ToList();
                player = joinTableHandler.playerData.Find(player => player.mySeatIndex == seatIndex);
                playerControllers.ForEach(x =>
                {
                    x.roundHeartPoint = 13;
                    x.roundSpadePoint = 13;
                });
                player.roundHeartPoint = 0;
                player.roundSpadePoint = 0;
            }
            else
            {
                playerControllers = joinTableHandler.playerData.Where((player) => player.userId != userId).ToList();
                player = joinTableHandler.playerData.Find(player => player.userId == userId);
            }
            //List<HT_PlayerController> playerControllers = joinTableHandler.playerData.Where((player) => player.mySeatIndex != 1).ToList();
            //HT_PlayerController player = joinTableHandler.playerData.Find(player => player.mySeatIndex == 1);
            player.HeartSpadeInfoSet(13, 13);
            player.HeartSpadeObjectGenerate(playerControllers);
            for (int i = 0; i < playerControllers.Count; i++)
            {
                player.ShootingMoonAnimation(playerControllers[i].spadeRects, playerControllers[i].spadeInfoObj, false, playerControllers[i]);
            }

            for (int i = 0; i < playerControllers.Count; i++)
            {
                player.ShootingMoonAnimation(playerControllers[i].heartRects, playerControllers[i].heartInfoObj, true, playerControllers[i]);
            }
        }

        IEnumerator HeartSpadeSpreadAnimation()
        {
            yield return new WaitForSeconds(0.1f);
        }

        public void SpriteAnimation()
        {
            //rocketImg.sprite = rocketSprites[spriteCount];
            spriteCount++;
            if (spriteCount > 11)
                spriteCount = 0;
        }
    }

    [System.Serializable]
    public class ShootingResponseData
    {
        public bool isShootingMoon;
        public string userId;
        public int roundPoint;
        public int heartPoint;
        public int spadePoint;
    }

    [System.Serializable]
    public class ShootingResponse
    {
        public string en;
        public List<ShootingResponseData> data;
    }
}