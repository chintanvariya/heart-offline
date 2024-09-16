using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace FGSOfflineCallBreak
{
    public class CallBreakGamePlayController : MonoBehaviour
    {
        public List<CallBreakUserController> allPlayer = new List<CallBreakUserController>();

        public List<BotDetails> botDetails = new List<BotDetails>();

        public TMPro.TextMeshProUGUI gamePlayLobbyAmount;
        public TMPro.TextMeshProUGUI gamePlayRoundText;

        public void OpenScreen()
        {
            botDetails = new List<BotDetails>();
            for (int i = 0; i < 3; i++)
            {
                botDetails.Add(CallBreakGameManager.instance.generateTheBots.allBotDetails[Random.Range(0, CallBreakGameManager.instance.generateTheBots.allBotDetails.Count)]);
            }
            for (int i = 0; i < allPlayer.Count; i++)
            {
                if (i != 0)
                    allPlayer[i].botDetails = botDetails[i - 1];

                allPlayer[i].ProfileAndNameDataSet();
                allPlayer[i].ResetUserTimer();
            }
            string lobbyAmount = $"{CallBreakGameManager.instance.lobbyAmount}";
            if (CallBreakGameManager.instance.lobbyAmount == 0)
                lobbyAmount = "Free";

            gamePlayLobbyAmount.text = lobbyAmount;

            CallBreakGameManager.instance.currentRound = 0;

            UpdateTheRoundText();

            CallBreakCardAnimation.instance.StartGamePlay(1f);

            CallBreakGameManager.isInGamePlay = true;
            FirebaseController.instance.FirelogEvent("isInGamePlay", "GamePlayController", "CallBreak");

            gameObject.SetActive(true);
            //Card Deal Animation
        }

        public void UpdateTheRoundText() => gamePlayRoundText.text = CallBreakGameManager.instance.currentRound + "/" + CallBreakGameManager.instance.totalRound;

        public void OnButtonClicked(string buttonName)
        {
            switch (buttonName)
            {
                case "GamePlaydMenu":
                    CallBreakUIManager.Instance.menuController.OpenScreen("GamePlaydMenu");
                    break;
                default:
                    break;
            }
        }

        public void CloseScreen()
        {
            gameObject.SetActive(false);
            CallBreakCardAnimation.instance.StopAllCoroutinesOnGamePlay();
        }

        //internal IEnumerator NextUserTurn()
        //{
        //    currentPlayerIndex = (currentPlayerIndex + 1) % 4;

        //    Debug.Log(" Next PlayerTurn ====> " + currentPlayerIndex);

        //    if (gamePlayController.allPlayer.TrueForAll(i => i.isMyTurnComplete == true))
        //    {
        //        gamePlayController.allPlayer[0].ActiveAllSelfPlayerCard(false);

        //        List<CallBreakCardController> spadeCards = new();
        //        List<CallBreakCardController> otherTypeCards = new();

        //        for (int i = 0; i < allTableCards.Count; i++)
        //        {
        //            if (allTableCards[i].cardDetail.cardType == CardType.Spade)
        //            {
        //                spadeCards.Add(allTableCards[i]);
        //                Debug.Log(" Spade Add " + allTableCards[i].name);
        //            }
        //        }

        //        allTableCards.Sort((a, b) => b.cardDetail.value.CompareTo(a.cardDetail.value));
        //        spadeCards.Sort((a, b) => b.cardDetail.value.CompareTo(a.cardDetail.value));

        //        for (int i = 0; i < allTableCards.Count; i++)
        //        {
        //            Debug.LogWarning(allTableCards[i].cardDetail.cardType + " ===== " + currentCard.cardDetail.cardType);

        //            if (allTableCards[i].cardDetail.cardType != currentCard.cardDetail.cardType)
        //            {
        //                otherTypeCards.Add(allTableCards[i]);
        //                Debug.Log(allTableCards[i].name);
        //            }
        //        }

        //        for (int i = 0; i < otherTypeCards.Count; i++)
        //        {
        //            if (allTableCards.Contains(otherTypeCards[i])) allTableCards.Remove(otherTypeCards[i]);
        //        }

        //        CallBreakCardController winCard = (spadeCards.Count > 0) ? spadeCards[0] : allTableCards[0];
        //        yield return new WaitForSeconds(0.2f);

        //        Debug.Log(" WINCARD == " + winCard.name);
        //        Tweener scaleAnimation = winCard.transform.DOScale(new Vector3(WinCardScale, WinCardScale, WinCardScale), 0.2f).SetEase(Ease.Linear).SetAutoKill(false);
        //        Transform playerData = winCard.cardThrowParent;


        //        yield return new WaitForSeconds(0.3f);

        //        scaleAnimation.PlayBackwards();

        //        yield return new WaitForSeconds(0.5f);

        //        for (int i = 0; i < otherTypeCards.Count; i++)
        //        {
        //            allTableCards.Add(otherTypeCards[i]);
        //        }

        //        foreach (var item in particles)
        //        {
        //            item.SetActive(true);
        //            item.transform.localPosition = Vector3.zero;
        //        }

        //        foreach (var item in allTableCards)
        //        {
        //            item.transform.DOScale(Vector3.zero, .3f).SetEase(Ease.Linear);
        //            item.transform.DOMove(winCard.cardThrowParent.position, .3f).SetEase(Ease.Linear);
        //        }

        //        foreach (var item in particles)
        //        {
        //            item.transform.DOMove(winCard.cardThrowParent.position, .3f).SetEase(Ease.Linear);
        //        }


        //        yield return new WaitForSeconds(0.5f);

        //        foreach (var item in particles)
        //        {
        //            item.SetActive(false);
        //        }

        //        winCard.myPlayerData.currentBidScore++;
        //        winCard.myPlayerData.SetMyBid();
        //        TurnDataReset();
        //        currentPlayerIndex = gamePlayController.allPlayer.IndexOf(winCard.myPlayerData);

        //        WinnerTurn();

        //        if (gamePlayController.allPlayer.TrueForAll(i => i.myCards.Count == 0))
        //        {
        //            ScoreBoardDataEvent();
        //            Invoke(nameof(RestartGame), 1.5f);
        //        }
        //    }
        //    else
        //    {
        //        //float delayTime = UnityEngine.Random.Range(0.1f, 0.4f);
        //        if (currentPlayerIndex == 0) delayTime = 0;
        //        else delayTime = 0.3f;
        //        yield return new WaitForSeconds(delayTime);

        //        if (currentPlayerIndex == 0)
        //        {
        //            gamePlayController.allPlayer[0].HighLightCard();
        //            ThrowSelfPlayerCardAuto();
        //        }
        //        else
        //        {
        //            gamePlayController.allPlayer[currentPlayerIndex].turnTimer.SelfUserTimer();//ThrowCard()
        //        }
        //    }
        //}
    }
}
