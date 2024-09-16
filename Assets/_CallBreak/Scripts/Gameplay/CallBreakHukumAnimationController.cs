using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CallBreakHukumAnimationController : MonoBehaviour
{
    public Transform table;

    private void Start()
    {
        //Invoke(nameof(StartAnimationOfCard), 2f);
        //Invoke(nameof(Animation2), 2);
        Debug.Log("HERE I AM START ANIMATION");
        //StartAnimationOfCard(this() =>
        //{
        //    Debug.Log("CAME BACK HERE AFTER ANIMATION");
        //});
    }

    public void StartAnimationOfCard(Transform table, Transform card, Transform target, System.Action callBack)
    {
        card.DOScale(Vector3.one * 4, 0.5f).SetEase(Ease.Linear).
        OnStart(() =>
        {
            table.DOScale(Vector3.one * .75f, 0.5f).SetEase(Ease.Linear);
        }).OnUpdate(() =>
        {
            card.DORotate(new Vector3(0, 100, 0), 0.2f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
        }).OnComplete(() =>
        {
            card.DORotate(Vector3.zero, 0.0f).SetEase(Ease.Linear);
            DOVirtual.DelayedCall(1f, () =>
            {
                card.DOScale(Vector3.one, 0.2f).SetEase(Ease.Linear);
                card.DOJump(target.position, 1f, 1, 0.2f).SetEase(Ease.Linear).
                OnStart(() =>
                {
                    table.DOScale(Vector3.one, 0.5f).SetEase(Ease.Linear);
                    card.DORotate(new Vector3(0, 0, 0), 0.2f);

                }).OnComplete(() =>
                {
                    table.transform.DOShakePosition(duration, strength, vibrato, randomness).OnComplete(() =>
                    {
                        Debug.Log("Animation compeleted here");
                        callBack();
                    });
                });
            });
        });
    }

    [Range(0, 50f)]
    public float duration;

    [Range(-500f, 500f)]
    public float strength;

    [Range(-500, 500)]
    public int vibrato;

    [Range(-180f, 180f)]
    public int randomness;

    void Animation2()
    {
        table.transform.DOShakePosition(duration, strength, vibrato, randomness).OnComplete(() =>
        {
            Invoke(nameof(Animation2), 2);
        });
    }
}
